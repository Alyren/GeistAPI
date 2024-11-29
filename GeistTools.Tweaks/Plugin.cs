using BepInEx;
using BepInEx.Logging;

namespace GeistTools.Tweaks
{
    [BepInPlugin("com.arylen.geist.tweaks", "GeistTools Tweaks", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Tweaks!");
        }
    }
}
