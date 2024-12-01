using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssetsTools.NET.Extra;
using BepInEx.Configuration;

namespace GeistTools.Tweaks.Core.Tweaks
{
    internal class VisibleInBackgroundTweak : ITweak
    {
        public string Name => "Visible In Background";

        private ConfigEntry<bool> enabledInConfig;
        private ConfigEntry<bool> restoreMode;
        private ConfigEntry<bool> firstRun;

        private string filepath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ATLYSS_Data",
                    "globalgamemanagers"
                );

        public void Awake(TweakLoadContainer container)
        {
            enabledInConfig = container.Config.Bind(
                "Tweaks.BorderlessWindow",
                "Enabled",
                true,
                "Enabled the BorderlessWindow tweaks"
            );

            restoreMode = container.Config.Bind(
                "Tweaks.BorderlessWindow",
                "RestoreMode",
                false,
                "Sets the tweak to start in restore mode, unpatching the game entirely."
            );

            firstRun = container.Config.Bind(
                "Tweaks.BorderlessWindow",
                "FirstRun",
                true,
                "Sets FirstRun mode for this tweak. Don't move unless you know what you're doing."
            );

            if (restoreMode.Value)
            {
                Plugin.Logger.LogWarning("Starting in Restore Mode!");
                DoRestore();
                return;
            }

            DoFirstRun();
            PatchUnity();
        }

        private void DoFirstRun()
        {
            Plugin.Logger.LogInfo("Performing first-run tasks.");
            Plugin.Logger.LogInfo($"FirstRun state: {firstRun}");
            if (!firstRun.Value)
            {
                Plugin.Logger.LogInfo("Skipping first-run patches.");
                return;
            }

            Plugin.Logger.LogInfo("Backing up globalgamemanagers.");
            File.Copy(filepath, $"{filepath}.original");

            firstRun.Value = false;
        }

        private void DoRestore()
        {
            var originalFilePath = $"{filepath}.original";
            if (!File.Exists(originalFilePath))
            {
                Plugin.Logger.LogError($"No original file at [{originalFilePath}]!");
                return;
            }
            
            File.Copy(originalFilePath, filepath, true);
            Plugin.Logger.LogInfo("Restored file!");
        }

        private void PatchUnity()
        {
            var classData = Assembly.GetExecutingAssembly().GetManifestResourceStream("TweakedData.classdata.tpk");

            var assetsManager = new AssetsManager();
            assetsManager.LoadClassPackage(classData);

            var globalGameManager = assetsManager.LoadAssetsFile(filepath, false);
            var file = globalGameManager.file;
            var table = globalGameManager.table;

            assetsManager.LoadClassDatabaseFromPackage(file.typeTree.unityVersion);

            var playerSettings = table.GetAssetInfo(1);
            var valueField = assetsManager.GetTypeInstance(file, playerSettings).GetBaseField();
            var visibilityField = valueField.Get("visibleInBackground").GetValue();

            if (visibilityField.AsBool() == enabledInConfig.Value)
            {
                Plugin.Logger.LogInfo($"Patch already is correct. Skipping.");
                assetsManager.UnloadAllAssetsFiles();
                return;
            }

            //BackupGameManager(file.);
        }

    }
}
