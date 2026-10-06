using System.Text;
using UnityEngine;

// PORT: run status spoken on request (keys and buttons in PortBindings: score, lights, missions).
// During the run the accessibility plugin is paused, so the text goes straight to the screen reader -
// Tolk or SAPI on Windows, VoiceOver on the Mac - interrupting what it was saying.
public static class PortStatus
{
	public static void SayScore()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		LocalizationManager lm = LocalizationManager.Instance;
		if (cgm == null || lm == null)
		{
			return;
		}
		Say(lm.GetLocalizedValue("tts_you_have_scored") + " " + NumberFormatter.FormatToLocale(cgm.score) + " " + lm.GetLocalizedValue("tts_points_this_run") + ".");
	}

	public static void SayLights()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		LocalizationManager lm = LocalizationManager.Instance;
		DataManager dm = DataManager.Instance;
		if (cgm == null || lm == null || dm == null)
		{
			return;
		}
		// The run's lights are only added to the saved total at the end of the run
		int total = dm.playerData.coins + cgm.sumCollectedGhosts;
		Say(lm.GetLocalizedValue("tts_you_have_collected") + " " + NumberFormatter.FormatToLocale(cgm.sumCollectedGhosts) + " " + lm.GetLocalizedValue("tts_lights_this_run") + ". " + string.Format(lm.GetLocalizedValue("port_lights_total"), NumberFormatter.FormatToLocale(total)));
	}

	// Current missions: "Quest 1: title description. 12 left" (or "completed"), as the missions screen reads them
	public static void SayMissions()
	{
		MissionManager mm = MissionManager.Instance;
		LocalizationManager lm = LocalizationManager.Instance;
		if (mm == null || lm == null || mm.currentMissions == null)
		{
			return;
		}
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < mm.currentMissions.Length; i++)
		{
			Mission mission = mm.currentMissions[i];
			if (mission == null)
			{
				continue;
			}
			string progress = mission.PortLiveProgress() ?? lm.GetLocalizedValue("COMPLETED");
			sb.Append(lm.GetLocalizedValue("Quest") + " " + (i + 1) + ": " + mission.GetMissionTitle(true) + " " + mission.GetMissionDesc(true) + ". " + progress + ". ");
		}
		Say(sb.ToString().Trim());
	}

	// The run status is spoken outside the accessibility plugin's queue, so it goes straight to the
	// platform's screen reader and interrupts whatever was being said.
	private static void Say(string text)
	{
		Debug.Log("[PortStatus] " + text);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		// Tolk (NVDA, JAWS, ...), or the plugin's SAPI voice when no screen reader is running.
		WindowsTTS.Stop();
		WindowsTTS.Speak(text);
#else
		// PORT: the Mac has no Tolk; the same text goes to VoiceOver, or to the system voice when
		// VoiceOver is not running. MacOSTTS is the plugin's own Mac component, so when it exists the
		// queue stays in step with the rest of the game.
		if (MacOSTTS.instance != null)
		{
			MacOSTTS.instance.Stop();
			MacOSTTS.instance.Speak(text);
		}
		else
		{
			PortVoiceOver.Stop();
			PortVoiceOver.Speak(text);
		}
#endif
	}
}
