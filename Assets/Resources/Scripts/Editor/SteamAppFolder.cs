using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SteamAppFolder : IPreprocessBuildWithReport
{

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string appId = "480";
        string buildDir = Path.GetDirectoryName(report.summary.outputPath);
        string filePath = Path.Combine(buildDir, "steam_appid.txt");

        if (!File.Exists(filePath))
            File.WriteAllText(filePath, appId);
    }
}