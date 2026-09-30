using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// PORT: automatic updates from the GitHub releases of the port.
// FeerSceneManager starts the check when the game opens and, after the logo (instead of going straight to the
// theme selection), calls OfferUpdate: if a newer release exists, a Yes/No dialog asks whether to update.
// Yes downloads the release zip and closes the game; a PowerShell script waits for the game to exit, copies
// the new files over the game folder and starts the game again. Saves live elsewhere (LocalLow) and are kept.
// The installed version is StreamingAssets/port_version.txt; releases are tagged "v<version>" and carry c_AssetName.
public static class PortUpdater
{
	private const string c_ApiUrl = "https://api.github.com/repos/vrivrixd/FEER-PC/releases/latest";

	private const string c_AssetName = "FEER-PC-win64.zip";

	// Seconds OfferUpdate waits for a check that is still running
	private const float c_WaitForCheck = 5f;

	[Serializable]
	private class ReleaseAsset
	{
		public string name;

		public string browser_download_url;
	}

	[Serializable]
	private class Release
	{
		public string tag_name;

		public ReleaseAsset[] assets;
	}

	private static bool s_Checking;

	private static string s_NewVersion;

	private static string s_DownloadUrl;

	private static GameObject s_Dialog;

	private static Text s_DialogText;

	private static GameObject s_Buttons;

	private static AccessibleLabel s_DialogLabel;

	private static int s_Answer; // 0 = waiting, 1 = yes, 2 = no

	public static bool DialogOpen => s_Dialog != null && s_Answer == 0;

	public static string InstalledVersion
	{
		get
		{
			try
			{
				return File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "port_version.txt")).Trim();
			}
			catch (Exception)
			{
				return "0";
			}
		}
	}

	public static void StartCheck(MonoBehaviour host)
	{
		host.StartCoroutine(Check());
	}

	private static IEnumerator Check()
	{
		s_Checking = true;
		using (UnityWebRequest request = UnityWebRequest.Get(c_ApiUrl))
		{
			request.timeout = 10;
			request.SetRequestHeader("User-Agent", "FEER-PC");
			request.SetRequestHeader("Accept", "application/vnd.github+json");
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("[PortUpdater] Check failed: " + request.error);
			}
			else
			{
				try
				{
					Release release = JsonUtility.FromJson<Release>(request.downloadHandler.text);
					string latest = (release.tag_name ?? "").TrimStart('v', 'V');
					string url = null;
					if (release.assets != null)
					{
						foreach (ReleaseAsset asset in release.assets)
						{
							if (asset.name == c_AssetName)
							{
								url = asset.browser_download_url;
							}
						}
					}
					Debug.Log("[PortUpdater] Installed " + InstalledVersion + ", latest release " + latest);
					if (url != null && IsNewer(latest, InstalledVersion))
					{
						s_NewVersion = latest;
						s_DownloadUrl = url;
					}
				}
				catch (Exception e)
				{
					Debug.Log("[PortUpdater] Check failed: " + e.Message);
				}
			}
		}
		s_Checking = false;
	}

	private static bool IsNewer(string latest, string installed)
	{
		Version a;
		Version b;
		if (!Version.TryParse(Normalize(latest), out a) || !Version.TryParse(Normalize(installed), out b))
		{
			return false;
		}
		return a > b;
	}

	private static string Normalize(string version)
	{
		return version.Contains(".") ? version : version + ".0";
	}

	// Called after the logo. Returns when the game should continue (no update, No, or a failed download).
	public static IEnumerator OfferUpdate()
	{
		float waited = 0f;
		while (s_Checking && waited < c_WaitForCheck)
		{
			waited += Time.unscaledDeltaTime;
			yield return null;
		}
		if (s_NewVersion == null)
		{
			yield break;
		}
		LocalizationManager lm = LocalizationManager.Instance;
		s_Answer = 0;
		CreateDialog(string.Format(lm.GetLocalizedValue("port_update_available"), s_NewVersion, InstalledVersion), lm.GetLocalizedValue("YES"), lm.GetLocalizedValue("NO"));
		while (s_Answer == 0)
		{
			yield return null;
		}
		if (s_Answer == 2)
		{
			CloseDialog();
			yield break;
		}
		// Without the buttons the accessibility plugin would move to the message and read it again
		// (after Say below changed it): the progress messages are only spoken once, by Say.
		UnityEngine.Object.Destroy(s_DialogLabel);
		s_Buttons.SetActive(false);
		Say(lm.GetLocalizedValue("port_update_downloading"));
		string zip = Path.Combine(Path.GetTempPath(), "FEER-PC-update.zip");
		bool ok;
		using (UnityWebRequest request = UnityWebRequest.Get(s_DownloadUrl))
		{
			request.downloadHandler = new DownloadHandlerFile(zip) { removeFileOnAbort = true };
			request.SetRequestHeader("User-Agent", "FEER-PC");
			yield return request.SendWebRequest();
			ok = request.result == UnityWebRequest.Result.Success;
			if (!ok)
			{
				Debug.Log("[PortUpdater] Download failed: " + request.error);
			}
		}
		if (ok)
		{
			try
			{
				StartInstaller(zip);
			}
			catch (Exception e)
			{
				Debug.Log("[PortUpdater] Installer failed: " + e.Message);
				ok = false;
			}
		}
		if (!ok)
		{
			Say(lm.GetLocalizedValue("port_update_failed"));
			yield return new WaitForSeconds(4f);
			CloseDialog();
			yield break;
		}
		Say(lm.GetLocalizedValue("port_update_installing"));
		yield return new WaitForSeconds(3f);
		Application.Quit();
		// Keeps the theme selection from appearing while the game closes
		while (true)
		{
			yield return null;
		}
	}

	// Esc / Circle / B in the dialog = No
	public static void Decline()
	{
		if (DialogOpen)
		{
			s_Answer = 2;
		}
	}

	private static void StartInstaller(string zip)
	{
		string gameDir = Path.GetDirectoryName(Application.dataPath);
		string exe = Path.Combine(gameDir, Path.GetFileNameWithoutExtension(Application.dataPath).Replace("_Data", "") + ".exe");
		string temp = Path.Combine(Path.GetTempPath(), "FEER-PC-update");
		string log = Path.Combine(Path.GetTempPath(), "FEER-PC-update.log");
		string script = Path.Combine(Path.GetTempPath(), "FEER-PC-update.ps1");
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("$ErrorActionPreference = 'Stop'");
		sb.AppendLine("$zip = " + Quote(zip));
		sb.AppendLine("$temp = " + Quote(temp));
		sb.AppendLine("$game = " + Quote(gameDir));
		sb.AppendLine("$exe = " + Quote(exe));
		sb.AppendLine("$log = " + Quote(log));
		sb.AppendLine("Start-Transcript -Path $log -Force | Out-Null");
		sb.AppendLine("try {");
		sb.AppendLine("  Wait-Process -Id " + Process.GetCurrentProcess().Id + " -Timeout 60 -ErrorAction SilentlyContinue");
		sb.AppendLine("  if (Test-Path $temp) { Remove-Item $temp -Recurse -Force }");
		sb.AppendLine("  Expand-Archive -LiteralPath $zip -DestinationPath $temp -Force");
		sb.AppendLine("  $src = Join-Path $temp 'FEER-PC'");
		sb.AppendLine("  if (-not (Test-Path $src)) { $src = $temp }");
		// Files can stay locked for a moment after the game exits (crash handler): retry
		sb.AppendLine("  for ($i = 0; $i -lt 10; $i++) {");
		sb.AppendLine("    try { Copy-Item -Path (Join-Path $src '*') -Destination $game -Recurse -Force; break }");
		sb.AppendLine("    catch { if ($i -eq 9) { throw }; Start-Sleep -Seconds 1 }");
		sb.AppendLine("  }");
		sb.AppendLine("  Write-Output 'Update installed'");
		sb.AppendLine("} catch {");
		sb.AppendLine("  Write-Output ('Update failed: ' + $_)");
		sb.AppendLine("} finally {");
		sb.AppendLine("  Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue");
		sb.AppendLine("  Remove-Item $zip -Force -ErrorAction SilentlyContinue");
		sb.AppendLine("  Stop-Transcript | Out-Null");
		sb.AppendLine("  Start-Process -FilePath $exe -WorkingDirectory $game");
		sb.AppendLine("}");
		// UTF-8 with BOM: Windows PowerShell reads BOM-less scripts in the ANSI code page
		File.WriteAllText(script, sb.ToString(), new UTF8Encoding(true));
		ProcessStartInfo info = new ProcessStartInfo("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"" + script + "\"");
		info.UseShellExecute = false;
		info.CreateNoWindow = true;
		Process.Start(info);
		Debug.Log("[PortUpdater] Installer started: " + script);
	}

	private static string Quote(string path)
	{
		return "'" + path.Replace("'", "''") + "'";
	}

	private static void Say(string text)
	{
		Debug.Log("[PortUpdater] " + text);
		if (s_DialogText != null)
		{
			s_DialogText.text = text;
		}
		UAP_AccessibilityManager.Say(text, false, true, UAP_AudioQueue.EInterrupt.All);
	}

	// Modal accessible dialog built at runtime: message + Yes/No buttons
	private static void CreateDialog(string message, string yes, string no)
	{
		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		s_Dialog = new GameObject("PortUpdateDialog");
		s_Dialog.SetActive(false);
		UnityEngine.Object.DontDestroyOnLoad(s_Dialog);
		Canvas canvas = s_Dialog.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 1000;
		CanvasScaler scaler = s_Dialog.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1280f, 720f);
		s_Dialog.AddComponent<GraphicRaycaster>();

		GameObject panel = CreateRect("Panel", s_Dialog.transform, new Vector2(0.15f, 0.25f), new Vector2(0.85f, 0.75f));
		panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.92f);
		AccessibleUIGroupRoot root = panel.AddComponent<AccessibleUIGroupRoot>();
		root.m_PopUp = true;
		root.m_AutoRead = true;

		GameObject label = CreateRect("Message", panel.transform, new Vector2(0.05f, 0.4f), new Vector2(0.95f, 0.95f));
		s_DialogText = label.AddComponent<Text>();
		s_DialogText.font = font;
		s_DialogText.fontSize = 34;
		s_DialogText.alignment = TextAnchor.MiddleCenter;
		s_DialogText.color = Color.white;
		s_DialogText.text = message;
		s_DialogLabel = label.AddComponent<AccessibleLabel>();
		s_DialogLabel.m_NameLabel = label;
		s_DialogLabel.m_ManualPositionOrder = 0;

		s_Buttons = CreateRect("Buttons", panel.transform, new Vector2(0f, 0f), new Vector2(1f, 0.4f));
		CreateButton("Yes", s_Buttons.transform, new Vector2(0.1f, 0.2f), new Vector2(0.45f, 0.8f), yes, font, 1, () => s_Answer = 1);
		CreateButton("No", s_Buttons.transform, new Vector2(0.55f, 0.2f), new Vector2(0.9f, 0.8f), no, font, 2, () => s_Answer = 2);

		s_Dialog.SetActive(true);
	}

	private static void CreateButton(string name, Transform parent, Vector2 min, Vector2 max, string text, Font font, int order, UnityEngine.Events.UnityAction onClick)
	{
		GameObject go = CreateRect(name, parent, min, max);
		Image image = go.AddComponent<Image>();
		image.color = new Color(0.55f, 0f, 0f, 1f);
		Button button = go.AddComponent<Button>();
		button.targetGraphic = image;
		button.onClick.AddListener(onClick);
		GameObject label = CreateRect("Text", go.transform, Vector2.zero, Vector2.one);
		Text t = label.AddComponent<Text>();
		t.font = font;
		t.fontSize = 34;
		t.alignment = TextAnchor.MiddleCenter;
		t.color = Color.white;
		t.text = text;
		AccessibleButton accessible = go.AddComponent<AccessibleButton>();
		accessible.m_NameLabel = label;
		accessible.m_ManualPositionOrder = order;
	}

	private static GameObject CreateRect(string name, Transform parent, Vector2 min, Vector2 max)
	{
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.transform.SetParent(parent, false);
		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = min;
		rect.anchorMax = max;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		return go;
	}

	private static void CloseDialog()
	{
		if (s_Dialog != null)
		{
			UnityEngine.Object.Destroy(s_Dialog);
		}
		s_Dialog = null;
		s_DialogText = null;
		s_Buttons = null;
		s_DialogLabel = null;
	}
}
