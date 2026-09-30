using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// PORT: the share buttons (game over screen and profile) copy the results shown on screen to the clipboard,
// in the game's language, instead of opening the Android share dialog.
public static class PortShare
{
	// Game over: "Can you beat me?", then what the screen reads: the run's points and lights,
	// the lights owned and the highscore
	public static void CopyRunResult(int score, int lights, int totalLights, int highscore)
	{
		LocalizationManager lm = LocalizationManager.Instance;
		StringBuilder sb = new StringBuilder();
		sb.AppendLine(lm.GetLocalizedValue("Can you beat me?"));
		sb.AppendLine(lm.GetLocalizedValue("tts_you_have_scored") + " " + NumberFormatter.FormatToLocale(score) + " " + lm.GetLocalizedValue("tts_points_this_run"));
		sb.AppendLine(lm.GetLocalizedValue("tts_you_have_collected") + " " + NumberFormatter.FormatToLocale(lights) + " " + lm.GetLocalizedValue("tts_lights_this_run"));
		sb.AppendLine(lm.GetLocalizedValue("You have") + " " + NumberFormatter.FormatToLocale(totalLights) + " " + lm.GetLocalizedValue("LIGHTS"));
		sb.AppendLine(string.Format(lm.GetLocalizedValue("port_copy_highscore"), NumberFormatter.FormatToLocale(highscore)));
		Copy(sb.ToString());
	}

	// Profile: the profile tab (mission level, highscore) and the statistics tab, in screen order
	public static void CopyProfile(Transform profileRows, Transform statsRows)
	{
		StringBuilder sb = new StringBuilder();
		List<Transform> rows = new List<Transform>();
		foreach (Transform row in profileRows)
		{
			rows.Add(row);
		}
		// The profile tab is laid out with anchors: top to bottom
		rows.Sort((a, b) => ((RectTransform)b).anchorMin.y.CompareTo(((RectTransform)a).anchorMin.y));
		AppendRows(sb, rows);
		rows.Clear();
		// The statistics list uses a layout group: sibling order
		foreach (Transform row in statsRows)
		{
			rows.Add(row);
		}
		AppendRows(sb, rows);
		Copy(sb.ToString());
	}

	// Row = label "Text" + value "Text (1)"; a row with only a label is a heading.
	// The leaderboard rank and the nickname need the online server (removed); buttons are not copied.
	private static void AppendRows(StringBuilder sb, List<Transform> rows)
	{
		foreach (Transform row in rows)
		{
			if (!row.gameObject.activeSelf || row.name == "Leaderboard" || row.name == "Nickname" || row.GetComponent<Button>() != null)
			{
				continue;
			}
			Text label = null;
			Text value = null;
			Text single = row.GetComponent<Text>();
			foreach (Transform child in row)
			{
				if (!child.gameObject.activeSelf)
				{
					continue;
				}
				if (child.name == "Text")
				{
					label = child.GetComponent<Text>();
				}
				else if (child.name == "Text (1)")
				{
					value = child.GetComponent<Text>();
				}
			}
			if (label == null)
			{
				label = single;
			}
			if (label == null)
			{
				continue;
			}
			if (value != null)
			{
				AppendRow(sb, LabelText(label), value.text);
			}
			else
			{
				if (sb.Length > 0)
				{
					sb.AppendLine();
				}
				sb.AppendLine(LabelText(label));
			}
		}
	}

	private static void AppendRow(StringBuilder sb, string label, string value)
	{
		sb.AppendLine(label + ": " + value.Trim());
	}

	// Labels are translated by LocalizedTextUI only when first shown; translate here too (no-op if done)
	private static string LabelText(Text label)
	{
		return LocalizationManager.Instance.GetLocalizedValue(label.text.Trim()).Trim().TrimEnd(':');
	}

	private static void Copy(string text)
	{
		text = text.TrimEnd();
		GUIUtility.systemCopyBuffer = text;
		Debug.Log("[PortShare] Copied: " + text.Replace("\n", " | "));
		UAP_AccessibilityManager.Say(LocalizationManager.Instance.GetLocalizedValue("port_copied_to_clipboard"), false, true, UAP_AudioQueue.EInterrupt.All);
	}
}
