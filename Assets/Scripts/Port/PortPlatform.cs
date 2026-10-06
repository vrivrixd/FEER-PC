using UnityEngine;

// PORT: which platform the port is running on, decided once.
//
// The port was Windows-only, and every Windows-specific choice - the screen reader DLLs, SAPI, XInput,
// HID, PowerShell, the .exe shape, the release asset name - now sits behind this class, so gameplay code
// never branches on Application.platform itself. A new platform means touching this file and whatever it
// names, not the game.
//
// Windows                              Mac
// -----------------------------------  -------------------------------------------------------
// Tolk (NVDA, JAWS, Dolphin...), SAPI   VoiceOver through /usr/bin/osascript, "say" as fallback
// XInputSetState (Xbox pads)            GameController framework, via the Input System package
// kernel32 HID (DS4 / DualSense)        GameController framework, via the Input System package
// powershell.exe + generated .ps1       /bin/sh + ditto; the .app is replaced as a bundle
// Feer.exe                             Feer.app
// FEER-PC-win64.zip                    FEER-PC-osx.zip
// %USERPROFILE%\AppData\LocalLow\...   ~/Library/Application Support/...
// Alt+F4                               Cmd+Q
//
// Saves live in Application.persistentDataPath, which Unity resolves per platform; nothing here is needed
// for them. The Windows layout is the one the existing saves were made with and it must not change.
public static class PortPlatform
{
	// The one name the rest of the port tests against.
	public static bool IsWindows { get; private set; }

	public static bool IsMac { get; private set; }

	static PortPlatform()
	{
		IsWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
		IsMac = Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
	}

	// How the port names itself in logs and in the updater's user agent.
	public static string PortName => IsMac ? "FEER-Mac" : "FEER-PC";

	// The release asset each platform updates from. The GitHub release has to carry both zips.
	public static string ReleaseAssetName => IsMac ? "FEER-PC-osx.zip" : "FEER-PC-win64.zip";

	// The saved profile lives here on both platforms. Only used by the error log and the README.
	public static string SavePath => Application.persistentDataPath;

	// Screen reader that carries the speech, for the log line the accessibility plugin writes on startup.
	// VoiceOver is the whole story on the Mac: it is part of the system and always installed.
	public static string ScreenReaderName => IsMac ? "VoiceOver" : "screen reader (Tolk)";

	// Nothing on the keyboard or the gamepad quits the game on either platform.
	public static string QuitHint => IsMac ? "Cmd+Q" : "Alt+F4";

	// The dialogs the port builds at runtime (the update prompt, the controls menu) need a font.
	// "Arial.ttf" is the Windows builtin name and returns null on the Mac, where the builtin is called
	// "LegacyRuntime.ttf"; "LegacyRuntime.ttf" is the same face under both names in 2020.3, so asking for
	// it first gives Windows the identical font and the Mac one that exists.
	public static Font DefaultFont()
	{
		Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
	}
}