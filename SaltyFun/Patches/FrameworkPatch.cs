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
    class FrameworkPatch
    {
        public static void Patch(Harmony harmonyInstance) // Framework methods to patch with the harmony instance
        {
            var AwakeMethod = AccessTools.Method(typeof(Framework), "Awake");
            var AwakeMethodPostfix = new HarmonyMethod(typeof(FrameworkPatch).GetMethod(nameof(FrameworkPatch.AwakeMethodPostfix))); // Patches Awake with postfix method
            harmonyInstance.Patch(AwakeMethod, postfix: AwakeMethodPostfix);
        }

        public static void AwakeMethodPostfix(Framework __instance) // Postfix method for OnDeath()
        {
            __instance.devMode = true;
            Debug.Log("dev mode? : " + Framework.instance.devMode);

            //AddCustomItems();
        }
    }
}
