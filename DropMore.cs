using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DropMore
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class DropMoreMain : BaseUnityPlugin
    {
        public const string MODNAME = "ModDrops";
        public const string AUTHOR = "TastyChickenLegs";
        public const string GUID = MODNAME+"."+AUTHOR;
        public const string VERSION = "1.0.8";
        public static ManualLogSource logger;
        internal Harmony harmony;
        internal Assembly assembly;
        public string modFolder;
        public static ConfigEntry<int> materialMultiplier;
        public static ConfigEntry<int> lootMultiplier;
        public static ConfigEntry<int> pickupMultiplier;
        public static ConfigEntry<bool> enableWhitelist;
        private static Assembly ass = typeof(DropMoreMain).Assembly;
        public static List<string> whitelist;

        private void Awake()
        {
            materialMultiplier = Config.Bind<int>("General", "Multiplier for resources", 1, "Material Multiplier");
            lootMultiplier = Config.Bind<int>("General", "Multiplier for monster drops", 1, " Monster Drop Multiplier");
            pickupMultiplier = Config.Bind<int>("General", "Multiplier for pickable objects", 1, "Pickup Multiplier");
            enableWhitelist = Config.Bind<bool>("Whitelist", "Enable whitelist filter", false, "Whitelist");
            whitelist = Enumerable.Distinct<string>(File.ReadAllLines(Path.GetDirectoryName(ass.Location) + "\\whitelist.txt")).ToList<string>();
            logger = Logger;

            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(this.assembly.Location);
            harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            //Dbgl("Destroying plugin");
            harmony.UnpatchSelf();
        }
      
    }
}