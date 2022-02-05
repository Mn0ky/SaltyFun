using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace SaltyFun
{
    class AddToInventoryPatches
    {
        public static void Patch(Harmony harmonyInstance) // Framework methods to patch with the harmony instance
        {
            var StartMethod = AccessTools.Method(typeof(AddToInventory), "Start");
            var StartMethodPostfix = new HarmonyMethod(typeof(AddToInventoryPatches).GetMethod(nameof(AddToInventoryPatches.StartMethodPostfix))); // Patches SendChatMessage() with prefix method
            harmonyInstance.Patch(StartMethod, postfix: StartMethodPostfix);
        }

        public static void StartMethodPostfix()
        {
            var firstInactiveCont = Inventory.instance.inactiveContainers[0];

            Debug.Log($"plugin path: {Paths.PluginPath}");
            var itemFileList = Directory.GetFiles($"{Paths.PluginPath}\\Items");

            if (itemFileList.Length == 0)
            {
                Debug.Log("No item files found, stopping...");
                return;
            }

            foreach (var itemFile in itemFileList)
            {
                ItemSaveable itemSaved = JsonConvert.DeserializeObject<ItemSaveable>(File.ReadAllText(itemFile));

                Debug.Log("hello world: " + itemSaved.ItemTitle);
            }
        }
    }
}
