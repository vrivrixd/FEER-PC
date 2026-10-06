using UnityEngine;

public class MacOSTTS : MonoBehaviour
{
	public static MacOSTTS instance = null;
	bool m_IsSpeaking = false;

	//////////////////////////////////////////////////////////////////////////

	void Start()
	{
		if (instance == null)
		{
			instance = this;

			// No longer needed, because this is now a child of the Accessibility Manager, which is already set to DontDestroyOnLoad
			//DontDestroyOnLoad(gameObject);
		}
		else
		{
			UnityEngine.Debug.LogError("[Accessibility] Trying to create another MacOS TTS instance, when there already is one.");
			DestroyImmediate(gameObject);
			return;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public void Speak(string msg)
	{
		if (msg.Length == 0)
			return;

		Stop();

#if (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		// PORT: the plugin spoke through the "say" command, which is a speaker, not a screen reader: a
		// VoiceOver user heard the game in the room instead of in their reader. The Mac port speaks through
		// VoiceOver first and falls back to "say" only when VoiceOver is not running or has refused the
		// Apple Event, so nothing that used to be heard is lost. See PortVoiceOver.
		m_IsSpeaking = PortVoiceOver.Speak(msg);
#endif
	}

	//////////////////////////////////////////////////////////////////////////

	public void Stop()
	{
#if (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		// PORT: was: kill the "say" process. Now: stop VoiceOver's queue as well, and kill say if that is
		// what is talking.
		PortVoiceOver.Stop();
		m_IsSpeaking = false;
#endif
	}

	//////////////////////////////////////////////////////////////////////////

	public bool IsSpeaking()
	{
		if (!Application.isPlaying)
			return false;

#if (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		// PORT: VoiceOver does not report when it finishes, so this is the estimate PortVoiceOver keeps,
		// which is what the plugin's own flag was for.
		return m_IsSpeaking && PortVoiceOver.IsSpeaking();
#else
		return m_IsSpeaking;
#endif
	}

	//////////////////////////////////////////////////////////////////////////

	void OnDestroy()
	{
		Stop();
	}

	//////////////////////////////////////////////////////////////////////////

}
