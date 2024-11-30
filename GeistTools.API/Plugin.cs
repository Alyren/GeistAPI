using BepInEx;
using BepInEx.Logging;

namespace GeistTools.API;

[BepInPlugin("com.alyren.geist.api", "GeistTools.API", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
