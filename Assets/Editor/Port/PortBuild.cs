using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// PORT: the two editor entry points, one per platform.
//
//	PortBuild.BuildWindows   ->  ../Build/Feer.exe
//	PortBuild.BuildMacOS     ->  ../Build/Feer.app
//
// Both are -executeMethod targets so the game can be built without opening the editor by hand:
//
//	Unity.exe  -quit -projectPath <folder> -executeMethod PortBuild.BuildWindows -logFile build.log
//	/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -buildTarget StandaloneOSX \
//	    -projectPath <folder> -executeMethod PortBuild.BuildMacOS -logFile build.log
//
// -buildTarget matters: it switches the active target before the method runs, which is both what the
// architecture setting needs and what an -executeMethod script has to do anyway - calling
// EditorUserBuildSettings.SwitchActiveBuildTarget from inside the method would reload the domain and abort
// the method halfway.
//
// The macOS build is Apple Silicon (arm64) and the editor is arm64. Unity 2020.3 could only produce x86_64
// players and had no arm64 editor at all, so the project is on 2022.3 LTS for this; a macOS 26/27/28 machine
// with no Rosetta will not run an x86_64 Unity player.
public static class PortBuild
{
	// The folder the Windows native plugins live in. The name says what is in it.
	private const string c_WindowsPlugins = "Assets/Plugins/x86_64";

	// PORT: macOS architecture.
	//
	// Unity 2022.3 does not let the build settings restrict a macOS player to one architecture. The manual
	// is explicit - "Architecture: Select the CPU to build for (only applies to Build And Run)" - and the
	// three experiments agree: platformArchitecture 0, 1 and 2, and EditorUserBuildSettings' "Architecture"
	// string, all produce a byte-identical universal binary. The setting only chooses which slice Build And
	// Run launches. Unity 6 added a real single-architecture choice; getting there means 2022.3 -> 6000.x on
	// a project AssetRipper recovered from a 2020.3 Android build, which is a far larger risk than it is
	// worth for the memory of a game.
	//
	// So the bundle is thinned after the build instead. That is deterministic and checkable, which is the
	// whole point: the architecture is read back off the built binary with lipo and logged, rather than
	// believed. A universal result here is a bug, not a variation.
	//
	// Note the earlier attempt at this which "verified" itself by reading the setting straight back with
	// EditorUserBuildSettings.GetPlatformSettings. Nothing validates that string - it returns whatever was
	// written - so it proved only that the write happened, and the log cheerfully printed ARM64 for three
	// consecutive universal builds.
	private const string c_ARM64 = "arm64";

	private const string c_Platform = "Standalone";

	private const string c_ArchitectureKey = "Architecture";

	public static void BuildWindows()
	{
		Build(BuildTarget.StandaloneWindows64, "../Build/Feer.exe");
	}

	public static void BuildMacOS()
	{
		// Still set both, so that Build And Run from the editor picks the arm64 slice, and so that the value
		// is right if this is ever run on a Unity version that honours it.
		PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1 /* ARM64 */);
		EditorUserBuildSettings.SetPlatformSettings(c_Platform, c_ArchitectureKey, c_ARM64);
		ExcludeWindowsPlugins(BuildTarget.StandaloneOSX);
		Build(BuildTarget.StandaloneOSX, "../Build/Feer.app");
		ThinToArm64("../Build/Feer.app");
	}

	// Reduces every Mach-O in the bundle to the arm64 slice. Idempotent: a binary that is already arm64
	// only is left alone, so a future Unity that honours the setting costs nothing here.
	private static void ThinToArm64(string appPath)
	{
		if (!Directory.Exists(appPath))
		{
			Debug.LogError("[PortBuild] No bundle at " + appPath + " to thin.");
			return;
		}
		int thinned = 0;
		int skipped = 0;
		int found = 0;
		foreach (string file in Directory.GetFiles(appPath, "*", SearchOption.AllDirectories))
		{
			if (!IsMachO(file))
			{
				continue;
			}
			found++;
			string archs = Lipo(file, "-archs");
			if (archs == c_ARM64)
			{
				skipped++;
				continue;
			}
			// The commands are lipo's own, which are not what you would guess: the subcommand is "-verify_arch",
			// not "-verify", and lipo prints its usage to stderr and exits non-zero on a command it does not
			// know. Getting that wrong made the first version report "Could not thin" for every binary while
			// never actually attempting a thin, because the bad verify short-circuited the real work.
			if (Lipo(file, "-verify_arch", c_ARM64).Length > 0)
			{
				Debug.LogError("[PortBuild] " + file.Substring(appPath.Length) + " has no " + c_ARM64 + " slice.");
				continue;
			}
			if (Lipo(file, "-thin", c_ARM64, "-o", file + ".arm64").Length > 0)
			{
				Debug.LogError("[PortBuild] Could not thin " + file.Substring(appPath.Length) + " to " + c_ARM64 + ".");
				continue;
			}
			// lipo carries the input file's mode over to its output, so the executable bit survives the swap.
			File.Delete(file);
			File.Move(file + ".arm64", file);
			Debug.Log("[PortBuild] Thinned to arm64: " + file.Substring(appPath.Length) + " (was " + archs + ")");
			thinned++;
		}
		// Read the result back off the disk. This is the check that matters.
		int checkedCount = 0;
		var remaining = new List<string>();
		foreach (string file in Directory.GetFiles(appPath, "*", SearchOption.AllDirectories))
		{
			if (!IsMachO(file))
			{
				continue;
			}
			checkedCount++;
			string archs = Lipo(file, "-archs");
			if (archs != c_ARM64)
			{
				remaining.Add(file.Substring(appPath.Length) + " = " + archs);
			}
		}
		// Zero Mach-O files is not a pass. It is the exact shape of the bug above - a recognition test that
		// matched nothing and then reported that everything was fine - so it is refused here rather than
		// allowed to print the reassuring line.
		if (checkedCount == 0)
		{
			Debug.LogError("[PortBuild] Found no Mach-O binaries in " + appPath +
				"; the architecture could not be verified at all.");
			return;
		}
		if (remaining.Count == 0)
		{
			Debug.Log("[PortBuild] macOS architecture: arm64 only, verified with lipo on " + checkedCount +
				" binaries (" + thinned + " thinned, " + skipped + " already arm64).");
		}
		else
		{
			Debug.LogError("[PortBuild] The bundle is not arm64 only: " + string.Join(", ", remaining.ToArray()));
		}
	}

	// Is this a Mach-O binary worth thinning? Two shapes answer yes, and getting this wrong is how the first
	// attempt at thinning passed without doing anything: a *thin* binary starts with the Mach-O magic
	// (CF FA ED FE little-endian on disk), but a *universal* one starts with the fat header (CA FE BA BE) and
	// the Mach-O headers are inside the slices. Checking only for CF FA ED FE therefore matched nothing at
	// all in a universal bundle, found zero files, and the "verified" line that followed was true of no files.
	private static bool IsMachO(string path)
	{
		try
		{
			using (var stream = File.OpenRead(path))
			{
				var magic = new byte[4];
				if (stream.Read(magic, 0, 4) != 4)
				{
					return false;
				}
				// Fat (universal) and fat-64, then thin Mach-O 64 and 32.
				return Matches(magic, 0xCA, 0xFE, 0xBA, 0xBE)
					|| Matches(magic, 0xCA, 0xFE, 0xBA, 0xBF)
					|| Matches(magic, 0xCF, 0xFA, 0xED, 0xFE)
					|| Matches(magic, 0xCE, 0xFA, 0xED, 0xFE);
			}
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static bool Matches(byte[] magic, byte a, byte b, byte c, byte d)
	{
		return magic[0] == a && magic[1] == b && magic[2] == c && magic[3] == d;
	}

	// Runs /usr/bin/lipo over one file. Returns stdout on success and stderr on failure; lipo prints its
	// complaint to stderr and exits non-zero, so the caller tests for text rather than for an exit code it
	// cannot see.
	private static string Lipo(string file, params string[] args)
	{
		try
		{
			var start = new System.Diagnostics.ProcessStartInfo("/usr/bin/lipo")
			{
				Arguments = Quote(file) + " " + string.Join(" ", args),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};
			using (var process = System.Diagnostics.Process.Start(start))
			{
				string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
				process.WaitForExit();
				return output.Trim();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("[PortBuild] lipo failed: " + e.Message);
			return e.Message;
		}
	}

	private static string Quote(string path)
	{
		return "\"" + path.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
	}

	private static void Build(BuildTarget target, string locationPathName)
	{
		var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		var report = BuildPipeline.BuildPlayer(scenes, locationPathName, target, BuildOptions.None);
		Debug.Log("PORTBUILD RESULT: " + report.summary.result + " errors=" + report.summary.totalErrors +
			" size=" + report.summary.totalSize + " path=" + locationPathName);
	}

	// PORT: the screen reader DLLs in Assets/Plugins/x86_64 are Windows PE binaries - Tolk, the NVDA
	// controller client, SAPI and the plugin's own voice. None of them can be loaded on a Mac, and shipping
	// them is at best dead weight.
	//
	// Their .meta files disable the macOS target, but that is not something to rely on: Unity 2020 wrote the
	// platform key as "OSXUniversal" and 2021 renamed it to "OSX", so a record for the old key no longer
	// excludes anything, and "Any" being off is not by itself a guarantee. The import names were recovered
	// by AssetRipper and are not ours to trust either. So before every Mac build the importers are asked
	// directly and made to say no, and the result is checked rather than assumed.
	private static void ExcludeWindowsPlugins(BuildTarget target)
	{
		var paths = new List<string>();
		foreach (string guid in AssetDatabase.FindAssets(string.Empty, new[] { c_WindowsPlugins }))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			var importer = AssetImporter.GetAtPath(path) as PluginImporter;
			if (importer != null && importer.GetCompatibleWithPlatform(target))
			{
				paths.Add(path);
			}
		}
		foreach (string path in paths)
		{
			var importer = (PluginImporter)AssetImporter.GetAtPath(path);
			importer.SetCompatibleWithPlatform(target, false);
			importer.SaveAndReimport();
			Debug.Log("[PortBuild] Excluded the Windows plugin from " + target + ": " + path);
		}
		// Say so either way, so the log shows what the build actually used rather than what it assumed.
		int left = AssetDatabase.FindAssets(string.Empty, new[] { c_WindowsPlugins })
			.Select(AssetDatabase.GUIDToAssetPath)
			.Select(AssetImporter.GetAtPath)
			.OfType<PluginImporter>()
			.Count(i => i.GetCompatibleWithPlatform(target));
		Debug.Log("[PortBuild] Windows plugins enabled for " + target + ": " + left +
			(left == 0 ? " (correct)" : " (UNEXPECTED - Windows binaries would ship in the Mac build)"));
	}
}