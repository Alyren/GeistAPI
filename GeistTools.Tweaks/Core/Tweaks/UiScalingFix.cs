using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using MonoMod.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeistTools.Tweaks.Core.Tweaks
{
    internal class UiScalingFix : ITweak
    {
        public string Name => "UI Scaling Fix";

        private ConfigFile Config { get; set; }

        private ConfigEntry<bool> fixEnabled;
        private ConfigEntry<float> widthToHeightRatio;
        private ConfigEntry<int> referenceResolutionWidth;
        private ConfigEntry<int> referenceResolutionHeight;
        private ConfigEntry<float> escapeMenuScaleOverride;
        private ConfigEntry<int> tabMenuResWidth;
        private ConfigEntry<int> tabMenuResHeight;

        private List<CanvasScaler> scalersInScene { get; set; } = new();

        private Dictionary<string, ConfigEntry<float>> customScaleFactors = new();
        private Dictionary<string, (ConfigEntry<int>, ConfigEntry<int>)> customResolutions = new();

        public void Awake(TweakLoadContainer container)
        {
            Config = container.Config;
            LoadConfig();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LoadConfig()
        {
            fixEnabled = Config.Bind(
                "Tweaks.UiScalingFix",
                "Enabled",
                true,
                "Enable the UI Scaling Fix."
            );

            widthToHeightRatio = Config.Bind(
                "Tweaks.UiScalingFix",
                "ScaleRatio",
                1f,
                "Ratio of scaling from screen width to screen height. (Keep 1 if ultrawide)"
            );

            referenceResolutionWidth = Config.Bind(
                "Tweaks.UiScalingFix",
                "UIResolutionWidth",
                1600,
                "Reference Resolution X"
            );

            referenceResolutionHeight = Config.Bind(
                "Tweaks.UiScalingFix",
                "UIResolutionHeight",
                900,
                "Reference Resolution Y"
            );

            escapeMenuScaleOverride = Config.Bind(
                "Tweaks.UiScalingFix",
                "EscapeMenuOverride",
                1.5f,
                "Scale value for escape menu. (-1 to disable)"
            );

            tabMenuResWidth = Config.Bind(
                "Tweaks.UiScalingFix",
                "InventoryResX",
                1280,
                "Reference resolution width for the inventory menu."
            );

            tabMenuResHeight = Config.Bind(
                "Tweaks.UiScalingFix",
                "InventoryResY",
                720,
                "Reference resolution height for the inventory menu."
            );

            customScaleFactors.AddRange(new Dictionary<string, ConfigEntry<float>>{
                { "Canvas_EscapeMenu", escapeMenuScaleOverride },
            });

            customResolutions.AddRange(new Dictionary<string, (ConfigEntry<int>, ConfigEntry<int>)>
            {
                { "Canvas_InGameMenu", (tabMenuResWidth, tabMenuResHeight) },
            });
        }

        private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
        {
            if (!fixEnabled.Value)
            {
                Plugin.Logger.LogDebug($"Skipping past scene load due to tweak being disabled in config.");
                return;
            }

            Plugin.Logger.LogInfo($"Scene loaded into [{newScene.buildIndex}] \"{newScene.name}\"");
            RefreshScalersInScene();
            ApplyFixes();
        }

        private void RefreshScalersInScene()
        {
            scalersInScene = GameObject.FindObjectsOfType<Canvas>(true)
                .Where(canvas =>
                       canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    || canvas.renderMode == RenderMode.ScreenSpaceCamera
                )
                .Select(canvas => canvas.GetComponent<CanvasScaler>())
                .Cast<CanvasScaler>()
                .ToList();
        }

        private void ApplyFixes()
        {
            Plugin.Logger.LogInfo($"Applying global adjustments to {scalersInScene.Count} CanvasScalers");
            var referenceResolution = new Vector2(referenceResolutionWidth.Value, referenceResolutionHeight.Value);
            foreach (var scaler in scalersInScene)
            {
                var oldRatio = scaler.matchWidthOrHeight;
                var oldRes = scaler.referenceResolution;
                
                scaler.matchWidthOrHeight = widthToHeightRatio.Value;
                scaler.referenceResolution = referenceResolution;

                if (customResolutions.TryGetValue(scaler.name, out var customResolution))
                {
                    scaler.referenceResolution = new Vector2(customResolution.Item1.Value, customResolution.Item2.Value);
                }

                Plugin.Logger.LogDebug($"  Applied: [{scaler.name}]");
                Plugin.Logger.LogDebug($"    - matchWidthOrHeight: {oldRatio} -> {scaler.matchWidthOrHeight}");
                Plugin.Logger.LogDebug($"    - referenceResolution: {oldRes} -> {scaler.referenceResolution}");
                if (customScaleFactors.TryGetValue(scaler.name, out var newScaleFactor))
                {
                    var oldScaleFactor = scaler.scaleFactor;
                    scaler.scaleFactor = newScaleFactor.Value;
                    Plugin.Logger.LogDebug($"    - scaleFactor: {oldScaleFactor} -> {scaler.scaleFactor}");
                }
            }
        }
    }
}
