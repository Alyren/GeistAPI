using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using GeistTools.Tweaks.Core;
using GeistTools.Tweaks.Core.Patches;
using GeistTools.Tweaks.Core.Tweaks;
using HarmonyLib;

namespace GeistTools.Tweaks;

[BepInPlugin("com.alyren.geist.tweaks", "GeistTools.Tweaks", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private List<ITweak> Tweaks { get; } = new();

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Tweaks loaded!");

        LoadTweaks();
        InitializeTweaks();
        ApplyPatches();
    }

    private void LoadTweaks()
    {
        if (Tweaks.Count != 0)
        {
            Logger.LogError("Tweaks already loaded?");
            return;
        }
        try
        {
            var assembly = typeof(Plugin).Assembly;
            var tweakInterface = typeof(ITweak);
            var tweakImplementations = assembly
                .GetTypes()
                .Where(type => !type.IsInterface && !type.IsAbstract)
                .Where(type => tweakInterface.IsAssignableFrom(type))
                .ToArray();

            Logger.LogInfo($"Tweaks found: {tweakImplementations.Length}");
            foreach (var tweak in tweakImplementations)
            {
                var instance = TryLoadTweak(tweak);
                if (instance != null)
                {
                    Logger.LogInfo($"  - Loaded: [{instance.Name}]");
                    Tweaks.Add(instance);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to load tweaks. No tweaks will be initialized. Reason: {e.Message}");
        }
    }

    private ITweak TryLoadTweak(System.Type tweakType)
    {
        ITweak instance = null;

        try
        {
            Logger.LogDebug($"Loading tweak from type [{tweakType.Name}]");
            instance = (ITweak)Activator.CreateInstance(tweakType);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to load tweak [{tweakType.Name}]: {ex.Message}");
        }

        return instance;
    }

    private void InitializeTweaks()
    {
        var container = new TweakLoadContainer
        {
            Config = Config,
        };
        foreach (var tweak in Tweaks)
        {
            try
            {
                tweak.Awake(container);
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed calling Awake on Tweak [{tweak.Name}]. Reason: {e.Message}");
            }
        }
    }

    private void ApplyPatches()
    {
        Logger.LogInfo("Applying patches:");
        Logger.LogInfo("  - Borderless Window Patch");
        Harmony.CreateAndPatchAll(typeof(BorderlessFullscreenPatch));
    }
}
