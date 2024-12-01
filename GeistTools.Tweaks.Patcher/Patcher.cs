using System;
using System.Collections.Generic;
using AssetsTools.NET.Extra;
using AssetsTools.NET;
using BepInEx.Configuration;
using BepInEx.Logging;
using Mono.Cecil;
using System.IO;

/*
 * The code here is kinda bad, was tired and got sent on multiple wild goose chases.
 * This should just get turned into a universal mod at some point.
 */

public static class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = new string[0];

    private static string DataPath = AppDomain.CurrentDomain.BaseDirectory + "/ATLYSS_Data";

    private static string GlobalGameManagersPath = $"{DataPath}/globalgamemanagers";

    private static string PatcherPath =
        AppDomain.CurrentDomain.BaseDirectory +
        "/BepInEx" +
        "/patchers" +
        "/GeistTools.Tweaks.Patcher";

    private static string TpkPath = $"{PatcherPath}/classdata.tpk";
    private static string DisablePath = $"{PatcherPath}/disable.txt";
    private static string OffPath = $"{PatcherPath}/off.txt";

    private static void Log(string message) => Console.WriteLine($"[GeistTools.Tweaks] {message}");

    public static void Patch(AssemblyDefinition assembly) { }

    public static void Initialize()
    {
        if (File.Exists(DisablePath))
        {
            Log($"  [!] Disable file exists. Not doing anything!");
            return;
        }

        Log("Patching globalgamemanagers");
        DoFirstRun();
        PatchUnity();
    }

    private static void DoFirstRun()
    {
        var path = $"{GlobalGameManagersPath}.original";
        if (File.Exists(path))
        {
            Log($".original file exists already, skipping first-time patch.");
            return;
        }
        File.Copy(GlobalGameManagersPath, $"{GlobalGameManagersPath}.original", true);
    }

    private static void PatchUnity()
    {
        var manager = new AssetsManager();
        manager.LoadClassPackage(TpkPath);

        var fileInstance = manager.LoadAssetsFile(GlobalGameManagersPath, false);
        var file = fileInstance.file;
        manager.LoadClassDatabaseFromPackage(file.Metadata.UnityVersion);

        var playerSettings = file.GetAssetsOfType(AssetClassID.PlayerSettings);
        if (playerSettings.Count != 1)
        {
            Log($"Incorrect PlayerSettings assets? {playerSettings.Count}");
            return;
        }
        var playerSettingsInfo = playerSettings[0];

        var baseField = manager.GetBaseField(fileInstance, playerSettingsInfo);
        var visibleInBackground = baseField.Get("visibleInBackground");
        if (visibleInBackground == null)
        {
            Log($"Failed to grab visibleInBackground from baseField");
            return;
        }

        var oldValue = visibleInBackground.Value.AsBool;
        Log($"Current visibleInBackground is {oldValue}");

        var newValue = !File.Exists(OffPath);
        if (!newValue)
        {
            Log($"  [!] off.txt was found, patching to false.");
        }

        visibleInBackground.Value.AsBool = newValue;

        playerSettingsInfo.SetNewData(baseField);

        Log($"Writing to [{GlobalGameManagersPath}.tmp]");
        using var writer = new AssetsFileWriter($"{GlobalGameManagersPath}.tmp");
        file.Write(writer);
        writer.Flush();
        writer.Close();
        file.Close();
        file.Reader.Close();

        Log($"Swapping files.");
        File.Copy($"{GlobalGameManagersPath}.tmp", GlobalGameManagersPath, true);

        Log($"Done!");
    }
}