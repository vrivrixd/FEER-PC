using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PortBuild
{
    public static void BuildWindows()
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        var report = BuildPipeline.BuildPlayer(scenes, "../Build/Feer.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log("PORTBUILD RESULT: " + report.summary.result + " errors=" + report.summary.totalErrors);
    }
}
