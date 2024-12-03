using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace GeistTools.Tweaks.Core.Patches
{
    internal class BorderlessFullscreenPatch : IPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Set_Resolution))]
        static bool Set_ResolutionPrefix(SettingsManager __instance, ref int _index)
        {
            Plugin.Logger.LogWarning(nameof(Set_ResolutionPrefix));
            if (_index >= __instance._filteredResolutions.Count)
            {
                Plugin.Logger.LogError($"OOB resolution: {_index} / {__instance._filteredResolutions.Count}");
                return false;
            }

            var resolution = __instance._filteredResolutions[_index];
            Screen.SetResolution(
                resolution.width,
                resolution.height,
                __instance._fullScreenToggle.isOn
                    ? FullScreenMode.FullScreenWindow
                    : FullScreenMode.Windowed
            );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Set_FullScreenToggle))]
        static bool Set_FullscreenTogglePrefix(SettingsManager __instance)
        {
            Plugin.Logger.LogWarning(nameof(Set_FullscreenTogglePrefix));
            
            Screen.fullScreen = __instance._fullScreenToggle.isOn;
            Screen.fullScreenMode = __instance._fullScreenToggle.isOn 
                ? FullScreenMode.FullScreenWindow 
                : FullScreenMode.Windowed;
            
            return false;
        }
    }
}
