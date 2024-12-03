using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistTools.Tweaks.Core.Modules.SavePatch
{
    internal static class SavePaths
    {
        public static string OriginalProfilePath => AppDomain.CurrentDomain.BaseDirectory + "/ATLYSS_Data/profileCollections";
        public static string DocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Atlyss/profileCollections";
    }
}
