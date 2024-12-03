using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace GeistTools.Tweaks.Core.Patches
{
    internal class SavePathPatch : IPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Awake))]
        static void SettingsManager_Awake_Postfix(SettingsManager __instance)
        {
            Plugin.Logger.LogWarning(nameof(SettingsManager_Awake_Postfix));
            Plugin.Logger.LogInfo($"SettingsManager.dataPath: {__instance._dataPath}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProfileDataManager), nameof(ProfileDataManager.Awake))]
        static void ProfileDataManager_Awake_Postfix(ProfileDataManager __instance)
        {
            Plugin.Logger.LogWarning(nameof(ProfileDataManager_Awake_Postfix));
            Plugin.Logger.LogInfo($"ProfileDataManager.dataPath: {__instance._dataPath}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControlManager), nameof(InputControlManager.Awake))]
        static void InputControlManager_Awake_Postfix(InputControlManager __instance)
        {
            Plugin.Logger.LogWarning(nameof(InputControlManager_Awake_Postfix));
            Plugin.Logger.LogInfo($"InputControlManager.dataPath: {__instance.dataPath}");
        }
    }
}
