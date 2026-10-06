using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
// System.Diagnostics also has a Debug, and this file wants both namespaces for Process and for the log.
using Debug = UnityEngine.Debug;

// PORT: VoiceOver output on the Mac - the C# version of what the Papa Sangre port does in
// papasangre/accessibility/voiceover.py, through /usr/bin/osascript instead of pyobjc.
//
// The Mac needs no native plugin for this: osascript ships with the system, it is the same NSAppleScript
// the Python port uses through pyobjc, and nothing has to be built or committed. Two things can carry the
// speech:
//
//  1. "if application VoiceOver is running then tell application VoiceOver to output ..." - the
//     lowest-latency path and the one that interrupts cleanly. The "is running" test matters: without it
//     AppleScript *launches* VoiceOver, so a player who never asked for a screen reader would be given one.
//  2. /usr/bin/say, when VoiceOver is not running or the Apple Event was refused (osascript error -1743,
//     "the process has never been granted automation"). The accessibility plugin's own MacOSTTS already
//     spoke through say, so this is behaviour that existed before, not something new.
//
//  3. IsSpeaking is an estimate of how long the utterance takes, because VoiceOver does not report one any
//     more than NVDA does - which is why WindowsTTS guesses the same way.
//
// PORT: nothing here may block the caller, and this file got that wrong first and the game froze on its
// first spoken line. Two separate mistakes, both worth not repeating:
//
//   - Speak used to wait for the osascript round trip so it could tell whether VoiceOver took the line and
//     fall back to say if not. That put a process spawn, a subprocess and an Apple Event in the middle of
//     Unity's Update. The accessibility plugin expects the opposite: MacOSTTS.Speak is non-blocking and
//     UAP_AudioQueue polls IsSpeaking() to know when the line is done. The original plugin went to some
//     trouble to be that way, with a thread and a coroutine, and it was replaced with a blocking call.
//   - Worse, the very first Apple Event a process sends raises the macOS Automation prompt, and osascript
//     does not return until the player answers it. That put a system dialog in front of the main thread, so
//     the game sat frozen for as long as the dialog was on screen, with no way to skip past it.
//
// So the Apple Event is fire-and-forget on a worker thread with a hard timeout, IsSpeaking answers from the
// estimate immediately, and the choice between VoiceOver and say is made on that worker - where a late or
// refused answer costs nothing but a wrong voice for one line. Once an Apple Event has worked, that is
// remembered, so the common case does not pay for the probe.
public static class PortVoiceOver
{
	// VoiceOver reads at roughly three words a second at its default rate.
	private const float c_WordsPerSecond = 3.0f;

	private const float c_MinSeconds = 0.4f;

	private const float c_DetectInterval = 2f;

	// How long osascript may take before it is killed and treated as a failure. Generous enough for a cold
	// Apple Event, bounded so a wedged osascript can never hold the game.
	private const int c_AppleEventTimeoutMs = 2500;

	// Say is left to run to completion rather than given a timeout of its own: it is the last voice a
	// player has, it is stopped by Stop() instead, and its exit time is not something anything waits on.

	// Only one line of speech at a time. The plugin already waits for IsSpeaking, so this is belt and
	// braces against two overlapping lines talking over each other.
	private static readonly object s_SpeechLock = new object();

	private static float s_SpeakingUntil;

	private static float s_NextDetect;

	private static bool s_Probing;

	// True once an Apple Event has actually been accepted, and remembered so later lines go straight to
	// VoiceOver instead of paying for a probe each time.
	private static volatile bool s_VoiceOverGood;

	private static volatile bool s_Running;

	// The subprocess currently running, so Stop can kill it. This is the only reason speech can be
	// interrupted, and it is what the empty utterance used to be for.
	private static Process s_Current;

	// VoiceOver can take the speech. Asked, not guessed. Only used for the startup log.
	public static bool Available
	{
		get
		{
			Probe();
			return s_Running;
		}
	}

	// PORT: the one decision that has to be answered now rather than in a moment: whether to turn the
	// accessibility plugin on by itself, the way Windows does when WindowsTTS.IsScreenReaderDetected() finds
	// NVDA or JAWS. This must be synchronous, and Available above cannot be used for it - Available starts a
	// background probe and returns the cached answer, which on the very first call is the initial "no", so
	// the plugin never turned itself on and the whole accessible UI stayed dark on the Mac. That is why the
	// game reached the tutorial with no way out of it: without the plugin there is no keyboard or gamepad
	// navigation and no speech, only a literal mouse click on a button.
	//
	// It runs once, during initialization, so a bounded wait here costs a few tens of milliseconds at
	// startup and nothing thereafter. The Automation prompt is the one case that blocks, and it is bounded
	// by c_AppleEventTimeoutMs like everything else.
	public static bool DetectNow()
	{
		string output;
		s_Running = RunScript(IsRunningScript(), out output, c_AppleEventTimeoutMs);
		s_NextDetect = Time.realtimeSinceStartup + c_DetectInterval;
		return s_Running;
	}

	// Refreshes the cached answer off the main thread, at most once every c_DetectInterval. Callers must
	// not treat this as authoritative: a stale "no" must never decide where a line of speech goes.
	private static void Probe()
	{
		if (s_Probing || Time.realtimeSinceStartup < s_NextDetect)
		{
			return;
		}
		s_NextDetect = Time.realtimeSinceStartup + c_DetectInterval;
		s_Probing = true;
		Thread worker = new Thread(() =>
		{
			try
			{
				string ignored;
				s_Running = RunScript(IsRunningScript(), out ignored, c_AppleEventTimeoutMs);
				s_VoiceOverGood |= s_Running;
			}
			finally
			{
				s_Probing = false;
			}
		})
		{ IsBackground = true, Name = "PortVoiceOver.Detect" };
		worker.Start();
	}

	private static string IsRunningScript()
	{
		return "if application \"VoiceOver\" is running then\n" +
			   "return \"yes\"\n" +
			   "else\n" +
			   "return \"no\"\n" +
			   "end if";
	}

	// Speaks a line. Returns immediately; the work happens on a worker. Returns true when there is a voice
	// to speak it with at all, which is what MacOSTTS records so the plugin keeps its IsSpeaking contract.
	public static bool Speak(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		// Answer straight away from the estimate. The plugin's queue moves on when IsSpeaking goes false, and
		// waiting for the real duration would mean waiting for the Apple Event, which is the thing that was
		// hanging.
		s_SpeakingUntil = Time.realtimeSinceStartup + EstimateSeconds(text);
		Thread worker = new Thread(() => SpeakOnWorker(text))
		{ IsBackground = true, Name = "PortVoiceOver.Speak" };
		worker.Start();
		return true;
	}

	// The whole of the actual speaking. Off the main thread, and inside the lock so two lines cannot talk
	// over each other.
	private static void SpeakOnWorker(string text)
	{
		lock (s_SpeechLock)
		{
			// VoiceOver first, but only when it has been seen to work or has not been disproved. The "is
			// running" test is in the script as well: VoiceOver may have been switched off since the last
			// probe, and the bare Apple Event would start it up.
			if (!s_VoiceOverRefused)
			{
				string output;
				string source = SpeakScript(text);
				if (RunScript(source, out output, c_AppleEventTimeoutMs))
				{
					if (output == "yes")
					{
						s_Running = true;
						s_VoiceOverGood = true;
						LogBackendOnce("VoiceOver");
						return;
					}
					LogBackendOnce("VoiceOver not running");
				}
				else
				{
					// Timed out or refused. If it was refused, stop asking: a blind player who has said no
					// to the Automation prompt should get the system voice, not a prompt on every line.
					if (s_LastErrorWasRefusal)
					{
						s_VoiceOverRefused = true;
					}
					LogBackendOnce("VoiceOver unusable (" + (s_LastErrorWasRefusal ? "refused, -1743" : "no answer") + ")");
				}
			}
			LogBackendOnce("system voice");
			SpeakThroughSay(text);
		}
	}

	// Which voice the game is actually using, once. This is the one line that answers "is VoiceOver really
	// carrying the speech, or is it falling back to the speakers", which is the whole point of the port and
	// otherwise invisible.
	private static bool s_LoggedBackend;

	private static void LogBackendOnce(string backend)
	{
		if (s_LoggedBackend)
		{
			return;
		}
		s_LoggedBackend = true;
		Debug.Log("[PortVoiceOver] First spoken line is going to: " + backend);
	}

	private static volatile bool s_VoiceOverRefused;

	private static volatile bool s_LastErrorWasRefusal;

	private static string SpeakScript(string text)
	{
		return "if application \"VoiceOver\" is running then\n" +
			   "tell application \"VoiceOver\" to output \"" + Escape(text) + "\"\n" +
			   "return \"yes\"\n" +
			   "else\n" +
			   "return \"no\"\n" +
			   "end if";
	}

	// The plugin's own MacOS voice: no screen reader, just the system voice. The speech rate is stored
	// 0..100 and mapped the way MacOSTTS maps it. The text is quoted the way MacOSTTS quotes it, which is
	// the plugin's own code path and is known to work; backslashes are folded to forward slashes first so
	// an odd one cannot escape the quotes.
	private static void SpeakThroughSay(string text)
	{
		try
		{
			int rate = (int)((UAP_AccessibilityManager.GetSpeechRate() / 100.0f) * 175 * 2);
			string quoted = text.Replace('\\', '/').Replace('"', '\'');
			Process process = Start("/usr/bin/say", "-r " + rate + " \"" + quoted + "\"");
			if (process == null)
			{
				return;
			}
			s_Current = process;
			// Left to run to completion: say is the last voice there is, and it is stopped by Stop() instead.
		}
		catch (Exception e)
		{
			Debug.LogWarning("[PortVoiceOver] say failed: " + e.Message);
		}
	}

	// Halts the queue. An empty utterance is what VoiceOver is given to stop talking; killing the process is
	// what stops say. Neither waits for anything, because this is called on the way out of a menu.
	public static void Stop()
	{
		s_SpeakingUntil = 0f;
		KillCurrent();
		Thread worker = new Thread(() =>
		{
			string ignored;
			RunScript("if application \"VoiceOver\" is running then\n" +
					  "tell application \"VoiceOver\" to output \"\"\n" +
					  "end if", out ignored, c_AppleEventTimeoutMs);
		})
		{ IsBackground = true, Name = "PortVoiceOver.Stop" };
		worker.Start();
	}

	private static void KillCurrent()
	{
		Process process = s_Current;
		s_Current = null;
		try
		{
			if (process != null)
			{
				if (!process.HasExited)
				{
					process.Kill();
				}
				process.Dispose();
			}
		}
		catch (Exception)
		{
			// A process that already went away is nothing to interrupt.
		}
	}

	// Estimated, because VoiceOver does not report it. This is what the accessibility plugin's queue runs
	// on, so it answers from the clock and never from a subprocess.
	public static bool IsSpeaking()
	{
		return Time.realtimeSinceStartup < s_SpeakingUntil;
	}

	// Words are counted as the runs of non-space characters between them, which is what a screen reader
	// is timed against.
	private static float EstimateSeconds(string text)
	{
		int words = 1;
		for (int i = 1; i < text.Length; i++)
		{
			if (char.IsWhiteSpace(text[i]) && !char.IsWhiteSpace(text[i - 1]))
			{
				words++;
			}
		}
		return Mathf.Max(c_MinSeconds, words / c_WordsPerSecond);
	}

	// AppleScript string escaping: backslashes first, then the quotes.
	private static string Escape(string text)
	{
		return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}

	// Runs one AppleScript and waits for the answer, but never longer than timeoutMs: the subprocess is
	// killed if it overruns. That bound is the whole reason this can be called from a worker and the game
	// cannot be held by a dialog. osascript reports success by exit code and writes the answer to stdout;
	// execution errors, the -1743 automation refusal among them, go to stderr.
	private static bool RunScript(string source, out string output, int timeoutMs)
	{
		output = null;
		s_LastErrorWasRefusal = false;
		Process process = null;
		try
		{
			process = Start("/usr/bin/osascript", WriteScript(source));
			if (process == null)
			{
				return false;
			}
			s_Current = process;
			// Wait first, and read the streams afterwards. This order is the whole point of the timeout:
			// ReadToEnd() blocks until the stream reaches EOF, which for a hung osascript is never, so
			// reading before waiting would make the timeout unreachable and put the freeze straight back.
			// The pipes are small here - osascript answers with one word, and the Automation prompt is what
			// we are bounding, not a chatty child.
			if (!process.WaitForExit(timeoutMs))
			{
				Debug.LogWarning("[PortVoiceOver] osascript did not answer within " + timeoutMs +
					" ms; killed. The first Apple Event waits on the Automation prompt, so this is expected " +
					"once, on the first line, if the prompt is left unanswered.");
				try
				{
					process.Kill();
				}
				catch (Exception)
				{
				}
				return false;
			}
			string error = process.StandardError.ReadToEnd();
			output = process.StandardOutput.ReadToEnd().Trim();
			if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
			{
				LogRefusedOnce(error);
				s_LastErrorWasRefusal = error.IndexOf("-1743", StringComparison.Ordinal) >= 0;
			}
			return process.ExitCode == 0;
		}
		catch (Exception e)
		{
			Debug.LogWarning("[PortVoiceOver] osascript failed: " + e.Message);
			return false;
		}
		finally
		{
			if (ReferenceEquals(s_Current, process))
			{
				s_Current = null;
			}
			if (process != null)
			{
				process.Dispose();
			}
		}
	}

	// -1743 is the interesting one: the game is not allowed to send Apple Events to VoiceOver, and the
	// player has to grant it in System Preferences > Privacy > Automation or live without a screen reader.
	// Worth saying once, not on every line the game speaks.
	private static bool s_LoggedRefusal;

	private static void LogRefusedOnce(string error)
	{
		if (s_LoggedRefusal || error.IndexOf("-1743", StringComparison.Ordinal) < 0)
		{
			return;
		}
		s_LoggedRefusal = true;
		Debug.LogWarning("[PortVoiceOver] VoiceOver refused the Apple Event (-1743). Falling back to the " +
						 "system voice. To speak through VoiceOver, allow it under System Preferences > " +
						 "Privacy & Security > Automation.");
	}

	private static Process Start(string fileName, string arguments)
	{
		Process process = new Process();
		process.StartInfo.FileName = fileName;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.StandardOutputEncoding = new UTF8Encoding(false);
		// A player bundle has no PATH or HOME worth trusting; say picks no voice at all without one.
		process.StartInfo.EnvironmentVariables["HOME"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		process.StartInfo.EnvironmentVariables["PATH"] = "/usr/bin:/bin:/usr/sbin:/sbin";
		return process.Start() ? process : null;
	}

	// The script goes through a file rather than -e: ProcessStartInfo.Arguments is parsed with Windows
	// quoting rules, and an AppleScript full of quotes and newlines does not survive that. One file per
	// call, so two utterances cannot race each other.
	private static string WriteScript(string source)
	{
		string path = Path.Combine(Path.GetTempPath(), "feer-voiceover-" + Guid.NewGuid().ToString("N") + ".applescript");
		File.WriteAllText(path, source, new UTF8Encoding(false));
		return path;
	}
}