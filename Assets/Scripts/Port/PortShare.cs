using UnityEngine;

// PORT: the share buttons (game over screen and profile) copy the results to the clipboard instead of opening
// the Android share dialog. The text is built from the game's own share strings, in the current language;
// the store links of the original are replaced by the port's page.
public static class PortShare
{
	private const string c_Link = "https://github.com/vrivrixd/FEER-PC";

	private const string c_GameName = "FEER - The Game of Running Blind";

	// Game over: "My new highscore is N! Can you beat me?" or "I scored N! Can you beat me?"
	public static void CopyRunResult(int score, bool newHighscore)
	{
		LocalizationManager lm = LocalizationManager.Instance;
		string first = lm.GetLocalizedValue(newHighscore ? "My new highscore is" : "I scored");
		Copy(first + " " + NumberFormatter.FormatToLocale(score) + CanYouBeatMe(lm) + "\n" + c_GameName + "\n" + c_Link);
	}

	// Profile: "My highscore is N. Try to beat me in FEER!"
	public static void CopyProfile(int highscore)
	{
		LocalizationManager lm = LocalizationManager.Instance;
		string first = lm.GetLocalizedValue("My highscore is ").TrimEnd();
		Copy(first + " " + NumberFormatter.FormatToLocale(highscore) + ". " + lm.GetLocalizedValue("Try to beat me in FEER!") + "\n" + c_GameName + "\n" + c_Link);
	}

	// "! Can you beat me?\nFEER - The Game of Running Blind. Available on the App Store (" -> "! Can you beat me?"
	private static string CanYouBeatMe(LocalizationManager lm)
	{
		string text = lm.GetLocalizedValue("! Can you beat me?\nFEER - The Game of Running Blind. Available on the App Store (");
		int end = text.IndexOf('\n');
		return (end >= 0) ? text.Substring(0, end) : "! " + lm.GetLocalizedValue("Can you beat me?");
	}

	private static void Copy(string text)
	{
		GUIUtility.systemCopyBuffer = text;
		Debug.Log("[PortShare] Copied: " + text.Replace("\n", " | "));
		UAP_AccessibilityManager.Say(LocalizationManager.Instance.GetLocalizedValue("port_copied_to_clipboard"), false, true, UAP_AudioQueue.EInterrupt.All);
	}
}
