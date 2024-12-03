using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace GeistTools.Tweaks.Core.Modules.SavePatch
{
    internal static class SavePatchSettings
    {
        public static ConfigEntry<bool> enabled;
        public static ConfigEntry<string> saveLocation;

        static SavePatchSettings()
        {
            enabled = Plugin.FileConfig.Bind(
                "Tweaks.ExternalSaves",
                "Enabled",
                false,
                "Makes the game use a folder stored in your Documents folder as its save and settings location."
            );

            saveLocation = Plugin.FileConfig.Bind(
                "Tweaks.ExternalSaves",
                "SaveLocation",
                SavePaths.DocumentsPath,
                "Sets the path to store save data in."
            );
        }
    }
}
