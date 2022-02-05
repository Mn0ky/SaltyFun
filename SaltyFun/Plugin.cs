using System;
using BepInEx;
using HarmonyLib;

namespace SaltyFun
{
    [BepInProcess("Salt.exe")]
    [BepInPlugin("monky.plugins.saltyfun", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            try
            {
                Harmony harmony = new Harmony("monky.QOL"); // Creates harmony instance with identifier
                Logger.LogInfo("Applying Framework patches...");
                FrameworkPatch.Patch(harmony);
                Logger.LogInfo("Applying ChatUI patches...");
                ChatUIPatch.Patch(harmony);
                //Logger.LogInfo("Applying AddToInventory patches...");
                //AddToInventoryPatches.Patch(harmony);
                //InventoryPatch.Patch(harmony);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception on applying patches: " + ex.InnerException);
            }
        }
    }
}
