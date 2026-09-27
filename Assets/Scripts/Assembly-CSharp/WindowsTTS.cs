using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WindowsTTS : MonoBehaviour
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	[DllImport("WindowsTTS")]
	public static extern void Initialize();
	[DllImport("WindowsTTS")]
	public static extern void DestroySpeech();
	[DllImport("WindowsTTS")]
	public static extern void StopSpeech();
	[DllImport("WindowsTTS")]
	public static extern void AddToSpeechQueue(string s);
	//[DllImport("WindowsTTS")]
	//public static extern void SetVolume(int volume);
	//[DllImport("WindowsTTS")]
	//public static extern void SetRate(int rate);
	[DllImport("WindowsTTS")]
	public static extern bool IsVoiceSpeaking();

	[DllImport("nvdaControllerClient")]
	internal static extern int nvdaController_testIfRunning();

	[DllImport("nvdaControllerClient", CharSet = CharSet.Auto)]
	internal static extern int nvdaController_speakText(string text);

	[DllImport("nvdaControllerClient")]
	internal static extern int nvdaController_cancelSpeech();

	//[DllImport("nvdaControllerClient")]
	//internal static extern int nvdaController_isSpeaking();

	// PORT: the original plugin only recognized NVDA. It now uses Tolk, which speaks through the screen
	// reader in use (NVDA, JAWS, System Access, ZoomText, Dolphin...). Without a screen reader it
	// keeps using the plugin's SAPI voice (WindowsTTS.dll). If Tolk is missing, it falls back to NVDA directly.
	[DllImport("Tolk")]
	private static extern void Tolk_Load();

	[DllImport("Tolk")]
	private static extern void Tolk_Unload();

	[DllImport("Tolk")]
	private static extern IntPtr Tolk_DetectScreenReader();

	[DllImport("Tolk", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool Tolk_Speak(string str, [MarshalAs(UnmanagedType.I1)] bool interrupt);

	[DllImport("Tolk")]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool Tolk_Silence();

	[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr LoadLibrary(string path);

	private static int s_TolkState = 0; // 0 = not loaded, 1 = ok, -1 = unavailable

	private static string s_ScreenReaderName = null;

	private float m_NextDetect = 0f;

	private static bool EnsureTolk()
	{
		if (s_TolkState == 0)
		{
			try
			{
				// The screen reader DLLs live in Plugins; Tolk loads them by name, so preload them by full path.
				string dir = System.IO.Path.Combine(Application.dataPath, "Plugins/x86_64");
				foreach (string dll in new[] { "nvdaControllerClient64.dll", "SAAPI64.dll" })
				{
					string path = System.IO.Path.Combine(dir, dll);
					if (!System.IO.File.Exists(path))
					{
						path = System.IO.Path.Combine(Application.dataPath, "Plugins/" + dll);
					}
					if (System.IO.File.Exists(path))
					{
						LoadLibrary(path);
					}
				}
				Tolk_Load();
				s_TolkState = 1;
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Accessibility] Tolk unavailable, using NVDA directly: " + e.Message);
				s_TolkState = -1;
			}
		}
		return s_TolkState == 1;
	}

	// Screen reader currently active (null if none)
	private static bool DetectScreenReader()
	{
		if (EnsureTolk())
		{
			IntPtr name = Tolk_DetectScreenReader();
			string sr = (name == IntPtr.Zero) ? null : Marshal.PtrToStringUni(name);
			if (sr != s_ScreenReaderName)
			{
				s_ScreenReaderName = sr;
				Debug.Log("[Accessibility] Screen reader: " + (sr ?? "none (using SAPI)"));
			}
			return sr != null;
		}
		try
		{
			return nvdaController_testIfRunning() == 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static WindowsTTS instance = null;
	private static bool m_UseNVDA = false;
	private static float m_NVDAIsSpeakingTimer = -1.0f;

	private const float c_NVDACharsPerSecond = 20.0f;

	//////////////////////////////////////////////////////////////////////////

	void Awake()
	{
		// Test every game start whether a screen reader is present
		m_UseNVDA = DetectScreenReader();
	}

	//////////////////////////////////////////////////////////////////////////

	public static bool IsScreenReaderDetected()
	{
		m_UseNVDA = DetectScreenReader();
		return m_UseNVDA;
	}

	//////////////////////////////////////////////////////////////////////////

	void Start()
	{
		if (instance == null)
		{
			instance = this;
			Initialize();

			// No longer needed, because this is now a child of the Accessibility Manager, which is already set to DontDestroyOnLoad
			//DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.LogError("[Accessibility] Trying to create another Windows TTS instance, when there already is one.");
			DestroyImmediate(gameObject);
			return;
		}

		//Debug.Log("[Accessibility] TTS: NVDA " + (m_UseNVDA ? "detected." : "not detected"));

		//bool isWindows = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
		//Debug.Log("[Accessibility] TTS: Window SAPI " + (isWindows ? "detected." : "not detected"));
	}

	//////////////////////////////////////////////////////////////////////////
	
	public static void Speak(string msg)
	{
		if (m_UseNVDA)
		{
			if (s_TolkState == 1)
			{
				Tolk_Speak(msg, false);
			}
			else
			{
				nvdaController_speakText(msg);
			}
			// PORT: NVDA does not report when it finishes speaking; the duration is estimated from the text length.
			// Plugin original: 16 characters per second. Tuned to 20 (pauses ~20% shorter).
			m_NVDAIsSpeakingTimer += (msg.Length / c_NVDACharsPerSecond);
		}
		else
		{
			AddToSpeechQueue(msg);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public static void Stop()
	{
		if (m_UseNVDA)
		{
			if (s_TolkState == 1)
			{
				Tolk_Silence();
			}
			else
			{
				nvdaController_cancelSpeech();
			}
			m_NVDAIsSpeakingTimer = 0.0f;
		}
		else
		{
			StopSpeech();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public static bool IsSpeaking()
	{
		if (!Application.isPlaying)
			return false;

		if (m_UseNVDA)
		{
			return m_NVDAIsSpeakingTimer > 0.0f;
			//return nvdaController_isSpeaking() > 0;
		}
		else
		{
			return IsVoiceSpeaking();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	void Update()
	{
		if (m_NVDAIsSpeakingTimer > 0.0f)
			m_NVDAIsSpeakingTimer -= Time.unscaledDeltaTime;

		// PORT: follows screen reader changes while the game is open (e.g. closing NVDA and starting JAWS)
		if (Time.unscaledTime >= m_NextDetect)
		{
			m_NextDetect = Time.unscaledTime + 2f;
			bool use = DetectScreenReader();
			if (use != m_UseNVDA)
			{
				m_UseNVDA = use;
				m_NVDAIsSpeakingTimer = 0.0f;
			}
		}
	}

/*
	//////////////////////////////////////////////////////////////////////////

	public static void SetSpeechVolume(int volume)
	{
		SetVolume(volume);
	}

	//////////////////////////////////////////////////////////////////////////

	public static void SetSpeechRate(int rate)
	{
		SetRate(rate);
	}
*/
	
	//////////////////////////////////////////////////////////////////////////

	void OnDestroy()
	{
		if (instance == this)
		{
			DestroySpeech();
			if (s_TolkState == 1)
			{
				Tolk_Unload();
				s_TolkState = 0;
			}
			instance = null;
		}
	}
#endif
}
