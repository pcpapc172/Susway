using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CIBuilder
{
    public static void Build()
    {
        string targetName = GetArgument("-ciTarget");
        BuildTarget target;
        BuildTargetGroup group;
        string output;

        switch (targetName)
        {
            case "StandaloneWindows64":
                target = BuildTarget.StandaloneWindows64;
                group = BuildTargetGroup.Standalone;
                output = "build/StandaloneWindows64/Subway-Surfers-Resurferd.exe";
                break;
            case "StandaloneLinux64":
                target = BuildTarget.StandaloneLinux64;
                group = BuildTargetGroup.Standalone;
                output = "build/StandaloneLinux64/Subway-Surfers-Resurferd.x86_64";
                break;
            case "StandaloneOSX":
                target = BuildTarget.StandaloneOSX;
                group = BuildTargetGroup.Standalone;
                output = "build/StandaloneOSX/Subway-Surfers-Resurferd.app";
                break;
            case "Android":
                target = BuildTarget.Android;
                group = BuildTargetGroup.Android;
                output = "build/Android/Subway-Surfers-Resurferd.apk";
                break;
            default:
                throw new ArgumentException("Unsupported -ciTarget: " + targetName);
        }

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
            throw new InvalidOperationException("No enabled scenes were found in EditorBuildSettings.");

        string outputDirectory = Path.GetDirectoryName(output);
        if (!string.IsNullOrEmpty(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(group, target))
            throw new InvalidOperationException("Could not switch to build target " + targetName + ". Is its Unity module installed?");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException(
                "Build failed: " + report.summary.result + " (" + report.summary.totalErrors + " errors)");

        Console.WriteLine("Built {0} to {1} ({2} bytes)", targetName, output, report.summary.totalSize);
    }

    private static string GetArgument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        }

        throw new ArgumentException("Missing required command-line argument " + name);
    }
}
