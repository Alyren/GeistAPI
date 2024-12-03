using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeistTools.Tweaks.Core.Extensions;
using HarmonyLib;
using UnityEngine;

namespace GeistTools.Tweaks.Core.Modules.SavePatch
{
    internal class SavePathPatch : IPatch
    {
        private static bool IsEnabled => SavePatchSettings.enabled.Value;
        private static string SaveLocation => SavePatchSettings.saveLocation.Value.TrimEnd('/') + "/";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Awake))]
        static void SettingsManager_Awake_Postfix(SettingsManager __instance)
        {
            if (!IsEnabled)
            {
                Plugin.Logger.LogInfo("External Save Disabled!");
                return;
            }

            Plugin.Logger.LogWarning($"Postfix: {nameof(SettingsManager)}.Awake()\n_dataPath -> [{SaveLocation.CensorWindowsUsername()}]");

            __instance._dataPath = SaveLocation;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Load_SettingsData))]
        static bool SettingsManager_LoadSettingsData_Prefix(SettingsManager __instance)
        {
            if (!IsEnabled)
            {
                Plugin.Logger.LogInfo("External Save Disabled!");
                return true;
            }

            Plugin.Logger.LogWarning($"Prefix: {nameof(SettingsManager)}.Load_SettingsData()\n_dataPath -> [{SaveLocation.CensorWindowsUsername()}]");

            __instance._dataPath = SaveLocation;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Save_SettingsData))]
        static bool SettingsManager_SaveSettingsData_Prefix(SettingsManager __instance)
        {
            if (!IsEnabled)
            {
                Plugin.Logger.LogInfo("External Save Disabled!");
                return true;
            }

            Plugin.Logger.LogWarning($"Prefix: {nameof(SettingsManager)}.Save_SettingsData()\n_dataPath -> [{SaveLocation.CensorWindowsUsername()}]");

            __instance._dataPath = SaveLocation;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Save_SettingsData))]
        static void SettingsManager_SaveSettingsData_Postfix(SettingsManager __instance)
        {
            Plugin.Logger.LogInfo($"Postfix: {nameof(SettingsManager)}.Save_SettingsData()\n_dataPath == [{__instance._dataPath}]");
        }

            [HarmonyPostfix]
        [HarmonyPatch(typeof(ProfileDataManager), nameof(ProfileDataManager.Awake))]
        static void ProfileDataManager_Awake_Postfix(ProfileDataManager __instance)
        {
            if (!IsEnabled)
            {
                Plugin.Logger.LogInfo("External Save Disabled!");
                return;
            }

            __instance._dataPath = SaveLocation;
            Plugin.Logger.LogInfo($"Postfix: {nameof(ProfileDataManager)}.Awake()\n_dataPath -> [{SaveLocation.CensorWindowsUsername()}]");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputControlManager), nameof(InputControlManager.Awake))]
        static void InputControlManager_Awake_Postfix(InputControlManager __instance)
        {
            if (!IsEnabled)
            {
                Plugin.Logger.LogInfo("External Save Disabled!");
                return;
            }

            Plugin.Logger.LogInfo($"Postfix: {nameof(InputControlManager)}.Awake()\n_dataPath -> [{SaveLocation.CensorWindowsUsername()}]");

            __instance.dataPath = SaveLocation;
        }
    }
}
