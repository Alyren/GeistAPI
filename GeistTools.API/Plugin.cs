using BepInEx;
using BepInEx.Logging;

namespace GeistTools.API
{
    [BepInPlugin("com.arylen.geist.api", "GeistTools API", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"API is loaded!");
        }
    }
}
