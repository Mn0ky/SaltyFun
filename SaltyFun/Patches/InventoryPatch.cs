using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace SaltyFun
{
    class InventoryPatch
    {
        public static void Patch(Harmony harmonyInstance) // Framework methods to patch with the harmony instance
        {
            var AwakeMethod = AccessTools.Method(typeof(Inventory), "Awake");
            var AwakeMethodPostfix = new HarmonyMethod(typeof(InventoryPatch).GetMethod(nameof(InventoryPatch.AwakeMethodPostfix))); // Patches Awake with postfix method
            harmonyInstance.Patch(AwakeMethod, postfix: AwakeMethodPostfix);
        }

        public static void AwakeMethodPostfix()
        {
            AddCustomItems();
        }

        public static void AddCustomItems()
        {
            Debug.Log("custom items method called");
            var firstInactiveCont = Inventory.instance.inactiveContainers[0];

            Debug.Log($"plugin path: {Paths.PluginPath}");
            var itemFileList = Directory.GetFiles($"{Paths.PluginPath}\\SaltyFun\\Items");

            if (itemFileList.Length == 0)
            {
                Debug.Log("No item files found, stopping...");
                return;
            }

            var idCounter = 9999;
            foreach (var itemFile in itemFileList)
            {
                ItemSaveable itemSaved = JsonConvert.DeserializeObject<ItemSaveable>(File.ReadAllText(itemFile));

                GameObject ItemObj = new GameObject($"{itemSaved.ItemTitle}Item");

                Item item = ItemObj.AddComponent(typeof(Item)) as Item;
                item.icon = itemSaved.ItemIconTextureImage;
                item.title = itemSaved.ItemTitle;
                item.rarity = GetProperRarity(itemSaved.ItemRarityName);
                item.flareText = itemSaved.ItemFlareText;
                item.price = int.Parse(itemSaved.ItemPrice);

                // item.isstackable (need to do something about this)
                item.stackSize = 3; // TODO: Add option for this
                item.id = idCounter + 1;

                if (itemSaved.ItemType == "Consumable")
                {
                    item.hasClickEffect = true;

                    StaminaOnClick staminaModifier = ItemObj.AddComponent(typeof(StaminaOnClick)) as StaminaOnClick; // TODO: This is for food!!
                    staminaModifier.normalStamina = 10;
                    staminaModifier.failureMessage = "You just <i>had</i> to try >:(";
                    staminaModifier.successMessage = "Did you really just... oh my <i>god</i>";

                    ItemObj.AddComponent(typeof(ApplyEffectsOnEat));

                    GameObject buffObj = new GameObject("myBuff");
                    buffObj.transform.SetParent(ItemObj.transform);

                    BuffStats statModifier = buffObj.AddComponent(typeof(BuffStats)) as BuffStats;
                    var statsWanted = new UnitStatValue[]
                    {
                        new(GetProperStat(itemSaved.ItemStatusEffect), int.Parse(itemSaved.ItemEffectAmount))
                    };
                    statModifier.statBuffs = statsWanted;
                    Debug.Log("stat description: " + statsWanted[0].description + " " + statModifier.statBuffs.GetType());
                    statModifier.tickLength = 10f; // TODO: Effect tick length
                    statModifier.duration = 600f; // TODO: Effect duration

                    EffectIconDetails statIcon = buffObj.AddComponent(typeof(EffectIconDetails)) as EffectIconDetails;
                    statIcon.iconTexture = itemSaved.ItemIconTextureImage; // TODO: Custom icon support.
                    statIcon.title = "Godly Power"; // TODO: Custom icon description.
                    statIcon.effectDescription = "You feel the power!"; // TODO: Custom icon description.
                }
                
                GameObject InstItemObj = UnityEngine.Object.Instantiate(ItemObj);
                Inventory.instance.AddItem(InstItemObj.GetComponent<Item>());

                GUIMaster.ShowInfoText("Added item: " + item.title);
                Debug.Log("Created item!");

                idCounter++;
            }
        }

        public static ItemRarity GetProperRarity(string jsonRarity)
        {
            return (ItemRarity)Enum.Parse(typeof(ItemRarity), jsonRarity); // TODO: Add custom rarity support.
        }

        public static UnitStat GetProperStat(string jsonStatusEffect)
        {
            return (UnitStat) Enum.Parse(typeof(UnitStat), jsonStatusEffect);
        }
    }
}
