using UnityEngine;

// PORT: run status spoken on request (keyboard S/L, gamepad L3/R3; see PortInput).
// During the run the accessibility plugin is paused, so the text goes straight to the
// screen reader (Tolk) or SAPI through WindowsTTS, interrupting what it was saying.
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

	private static void Say(string text)
	{
		Debug.Log("[PortStatus] " + text);
		WindowsTTS.Stop();
		WindowsTTS.Speak(text);
	}
}
