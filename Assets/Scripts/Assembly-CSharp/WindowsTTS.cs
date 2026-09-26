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

	public static WindowsTTS instance = null;
	private static bool m_UseNVDA = false;
	private static float m_NVDAIsSpeakingTimer = -1.0f;

	private const float c_NVDACharsPerSecond = 20.0f;

	//////////////////////////////////////////////////////////////////////////

	void Awake()
	{
		// Test every game start whether NVDA is present
		m_UseNVDA = nvdaController_testIfRunning() == 0;
	}

	//////////////////////////////////////////////////////////////////////////

	public static bool IsScreenReaderDetected()
	{
		m_UseNVDA = nvdaController_testIfRunning() == 0;
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
			nvdaController_speakText(msg);
			// PORT: o NVDA nao informa quando termina de falar; a duracao e estimada pelo tamanho do texto.
			// Original do plugin: 16 caracteres por segundo. Ajustado para 20 (pausas ~20% menores).
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
			nvdaController_cancelSpeech();
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
			instance = null;
		}
	}
#endif
}
