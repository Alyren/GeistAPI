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
    private List<IPatch> Patchets { get; } = new();

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Tweaks loaded!");

        LoadTweaks();
        InitializeTweaks();
        ApplyPatches();
    }

    private System.Type[] GetTypesImplementing(System.Type interfaceType)
    {
        if (!interfaceType.IsInterface)
            throw new Exception($"Must pass an interface type to GetTypesImplementing!");
        return typeof(Plugin)
            .Assembly
            .GetTypes()
            .Where(type => !type.IsInterface && !type.IsAbstract)
            .Where(type => interfaceType.IsAssignableFrom(type))
            .ToArray();
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
            var tweakImplementations = GetTypesImplementing(typeof(ITweak));

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
                Logger.LogError($"Failed calling Awake on Tweak [{tweak.Name}].\n    Reason: {e.Message}\n    Stack Trace:\n{e.StackTrace}");
            }
        }
    }

    private void ApplyPatches()
    {
        var patches = GetTypesImplementing(typeof(IPatch));

        Logger.LogInfo($"Patches loading: {patches.Length}");
        foreach (var patch in patches)
        {
            try
            {
                Logger.LogInfo($"  - {patch.Name}");
                Harmony.CreateAndPatchAll(patch);
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to load {patch.Name}!\nReason: {e.Message}\nStacktrace: {e.StackTrace}");
            }
        }
    }
}
