using UnityEngine;
using UnityEngine.Audio;

// PORT: countdown before the active power-up ends, with the same sounds as the clock shown when the
// game starts or resumes (ClockAnim1: a tick at 3, 2 and 1 seconds left, the final sound at the end).
// Power-ups last a distance, not a time: TrackManager converts the remaining distance into seconds
// and calls Tick every frame while the track is running.
// Option in the settings menu (PlayerData_v_1_1_3.portPowerUpCountdown).
public static class PortPowerUpCountdown
{
	// ClockAnim1 enables TickFinishedAudio at 2.917 s of 3 s
	private const float c_FinishedAt = 0.083f;

	private static int s_NextTick = -1;

	private static AudioSource s_TickAudio;

	private static AudioSource s_FinishedAudio;

	// timeLeft < 0: no power-up active
	public static void Tick(float timeLeft)
	{
		if (timeLeft < 0f)
		{
			s_NextTick = -1;
			return;
		}
		if (s_NextTick == -1)
		{
			// New power-up: count only whole seconds still ahead
			s_NextTick = Mathf.Min(3, Mathf.FloorToInt(timeLeft));
			if (s_NextTick < 1)
			{
				s_NextTick = 0;
			}
		}
		if (s_NextTick >= 1 && timeLeft <= s_NextTick)
		{
			while (s_NextTick >= 1 && timeLeft <= s_NextTick)
			{
				s_NextTick--;
			}
			Play(false);
		}
		else if (s_NextTick == 0 && timeLeft <= c_FinishedAt)
		{
			s_NextTick = -2;
			Play(true);
		}
	}

	private static void Play(bool finished)
	{
		DataManager dm = DataManager.Instance;
		if (dm == null || !dm.playerData.portPowerUpCountdown || !FindSources())
		{
			return;
		}
		AudioSource source = finished ? s_FinishedAudio : s_TickAudio;
		source.PlayOneShot(source.clip);
	}

	// Copies clip and mixer group from the clock's audio objects (Game Manager/PlayingGame/ResumePanel/ClockPanel)
	private static bool FindSources()
	{
		if (s_TickAudio != null && s_FinishedAudio != null)
		{
			return true;
		}
		AudioClip tick = null;
		AudioClip finished = null;
		AudioMixerGroup tickGroup = null;
		AudioMixerGroup finishedGroup = null;
		foreach (AudioSource source in Resources.FindObjectsOfTypeAll<AudioSource>())
		{
			if (source.gameObject.scene.name == null)
			{
				continue;
			}
			if (source.name == "TickAudio")
			{
				tick = source.clip;
				tickGroup = source.outputAudioMixerGroup;
			}
			else if (source.name == "TickFinishedAudio")
			{
				finished = source.clip;
				finishedGroup = source.outputAudioMixerGroup;
			}
		}
		if (tick == null || finished == null)
		{
			Debug.Log("[PortPowerUpCountdown] Clock sounds not found");
			return false;
		}
		GameObject go = new GameObject("PortPowerUpCountdown");
		Object.DontDestroyOnLoad(go);
		s_TickAudio = CreateSource(go, tick, tickGroup);
		s_FinishedAudio = CreateSource(go, finished, finishedGroup);
		return true;
	}

	private static AudioSource CreateSource(GameObject go, AudioClip clip, AudioMixerGroup group)
	{
		AudioSource source = go.AddComponent<AudioSource>();
		source.playOnAwake = false;
		source.spatialBlend = 0f;
		source.clip = clip;
		source.outputAudioMixerGroup = group;
		return source;
	}
}
