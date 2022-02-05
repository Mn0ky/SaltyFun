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
    class ChatUIPatch
    {
        public static void Patch(Harmony harmonyInstance) // Framework methods to patch with the harmony instance
        {
            var SendChatMessageMethod = AccessTools.Method(typeof(ChatUI), "SendChatMessage");
            var SendChatMessageMethodPrefix = new HarmonyMethod(typeof(ChatUIPatch).GetMethod(nameof(ChatUIPatch.SendChatMessageMethodPrefix))); // Patches SendChatMessage() with prefix method
            harmonyInstance.Patch(SendChatMessageMethod, prefix: SendChatMessageMethodPrefix);
        }

        public static void SendChatMessageMethodPrefix(ChatUI __instance) // Prefix method for SendChatMessage()
        {
            if (__instance.input.text[0] != '!') return;
            ChatUIPatch.Commands(__instance.input.text.TrimStart('!'));
        }

        public static void Commands(string command)
        {
            Debug.Log("Got command: " + command);
            if (command == "spawn")
            {
                Mesh testMesh = ObjImporter.ImportFile("D:\\downloads\\mp5k-obj\\mp5k.obj");
                Debug.Log(testMesh.subMeshCount);
            }
            else if (command == "itemtest")
            {
                Texture2D testImage = FileImporting.ImportImage("D:\\downloads\\god.jpg");
                foreach (Item item in UnityEngine.Object.FindObjectsOfType<Item>())
                {
                    Debug.Log(item.title);
                    if (item.title == "Marlin")
                    {
                        Debug.Log("FOUND AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
                        item.icon = testImage;
                        item.title = "Pocket God";
                        item.rarity = ItemRarity.Legendary;
                        // item.description = "The abrahamic legend fitted snuggly in your pocket!";
                        item.flareText = "The abrahamic lad fitted snuggly in your pocket!";
                        item.price = 9999999;
                        Debug.Log(item.currentItemContainer.ToString());
                        return;
                    }
                }
            }
            else if (command == "pocket")
            {
                Texture2D testImage = FileImporting.ImportImage("D:\\downloads\\Fahada_in_Black_Bikini.png");
                GameObject ItemObj = new GameObject("GodItem");

                Item item = ItemObj.AddComponent(typeof(Item)) as Item;
                item.icon = testImage;
                item.title = "Pocket God";
                item.rarity = ItemRarity.Legendary;
                item.flareText = "The abrahamic lad fitted snuggly in your pocket!\n\nPlease don't try to eat him, it's impossible anyways :P";
                item.price = 9999999;
                item.hasClickEffect = true;
                // item.isstackable (need to do something about this)
                item.stackSize = 3;
                item.id = 80085;

                StaminaOnClick staminaModifier = ItemObj.AddComponent(typeof(StaminaOnClick)) as StaminaOnClick;
                staminaModifier.normalStamina = 200;
                //staminaModifier.failureMessage = "You just <i>had</i> to try >:(";
                //staminaModifier.successMessage = "Did you really just... oh my <i>god</i>";
                staminaModifier.successMessage = "success!";
                staminaModifier.failureMessage = "fail!";

                ItemObj.AddComponent(typeof(ApplyEffectsOnEat));

                GameObject buffObj = new GameObject("myBuff");
                buffObj.transform.SetParent(ItemObj.transform);

                BuffStats statModifier = buffObj.AddComponent(typeof(BuffStats)) as BuffStats;
                var statsWanted = new UnitStatValue[]
                {
                    new(UnitStat.CritMultiplier, 10f)
                };
                statModifier.statBuffs = statsWanted;
                Debug.Log("stat description: " + statsWanted[0].description + " " + statModifier.statBuffs.GetType());
                statModifier.tickLength = 10f;
                statModifier.duration = 600f;

                EffectIconDetails statIcon = buffObj.AddComponent(typeof(EffectIconDetails)) as EffectIconDetails;
                statIcon.iconTexture = testImage;
                statIcon.title = "Godly Power";
                statIcon.effectDescription = "You feel the power!";

                GameObject InstItemObj = UnityEngine.Object.Instantiate(ItemObj);
                Inventory.instance.AddItem(InstItemObj.GetComponent<Item>());

                GUIMaster.ShowInfoText("Added item: " + item.title);
                Debug.Log("Created item!");
            }
            else if (command == "active")
            {
                Debug.Log(Inventory.instance.activeItemContainer.ToString());
                foreach (ItemContainer container in Inventory.instance.inactiveContainers)
                {
                    Debug.Log($"{container.ToString()} | {container.name}");
                    foreach (Item item in container.items)
                    {
                        Debug.Log(item.title);
                    }
                }
            }
            else if (command.StartsWith("Spawn"))
            {
                Debug.Log("running pirate command");
                string objWanted = command.Substring(6);
                foreach (GameObject gameObject4 in (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject)))
                {
                    if (gameObject4.name == objWanted)
                    {
                        Debug.Log("Attempting to spawn:" + gameObject4.name);
                        UnityEngine.Object.Instantiate<GameObject>(gameObject4, Player.instance.transform.position, default).SetActive(true);
                        return;
                    }
                }
            }
            else if (command == "custom")
            {
                AddCustomItems();
            }
            else
            { 
                Debug.LogError($"No '{command}' found ");
            }
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
            return (UnitStat)Enum.Parse(typeof(UnitStat), jsonStatusEffect);
        }
    }
}