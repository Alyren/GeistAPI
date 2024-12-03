using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using GeistTools.Tweaks.Core.Extensions;

namespace GeistTools.Tweaks.Core.Modules.SavePatch
{
    internal class SaveMigrator : ITweak
    {
        public string Name => "External Save Location";

        public void Awake()
        {
            if (!SavePatchSettings.enabled.Value)
            {
                Plugin.Logger.LogWarning($"External Save Tweak/Patches disabled.");
                return;
            }

            Plugin.Logger.LogInfo($"Save location: {SavePatchSettings.saveLocation.Value.CensorWindowsUsername()}");

            var state = CheckMigrationState();

            Plugin.Logger.LogInfo($"Migration state: {state.ToString()}");

            if (state == MigrationState.Unknown)
            {
                Plugin.Logger.LogError($"Failed to get migration status.");
                return;
            }
            
            if (state == MigrationState.AlreadyMigrated)
            {
                Plugin.Logger.LogInfo("Already migrated! Skipping!");
                return;
            }

            if (state == MigrationState.NeedsFirstMigration)
            {
                PerformFirstTimeMigration();
            }
        }

        private MigrationState CheckMigrationState()
        {
            if (!File.Exists(SavePatchSettings.saveLocation.Value + "/migrated.txt"))
                return MigrationState.NeedsFirstMigration;
            return MigrationState.AlreadyMigrated;
        }

        private void PerformFirstTimeMigration()
        {
            var saveLocation = SavePatchSettings.saveLocation.Value;
            Plugin.Logger.LogInfo($"Performing first time migration of save data.");

            Plugin.Logger.LogInfo($"Creating save folder at [{SavePaths.DocumentsPath.CensorWindowsUsername()}]");
            Directory.CreateDirectory(saveLocation);
            Directory.CreateDirectory($"{saveLocation}/inputControl");

            Plugin.Logger.LogInfo($"Gathering file list to copy.");
            var files = GetFilesRecursively(SavePaths.OriginalProfilePath);

            foreach (var file in files)
            {
                Plugin.Logger.LogInfo($"  - Copying File: {file.CensorWindowsUsername()}");
                var targetPathAddition = file.Replace(SavePaths.OriginalProfilePath, "");
                File.Copy(file, $"{saveLocation}/{targetPathAddition}");
            }

            Plugin.Logger.LogInfo("Creating migration marker");
            File.WriteAllText($"{saveLocation}/migrated.txt", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        }

        private string[] GetFilesRecursively(string path)
        {
            var filesFound = new List<string>();

            foreach (var directory in Directory.GetDirectories(path))
            {
                Plugin.Logger.LogInfo($"  -> Recursing into [{directory.CensorWindowsUsername()}]");
                filesFound.AddRange(GetFilesRecursively(directory));
            }

            foreach (var file in Directory.GetFiles(path))
            {
                filesFound.Add(file);
            }

            return filesFound.ToArray();
        }
    }
}
