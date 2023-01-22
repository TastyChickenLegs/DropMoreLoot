using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;

namespace DropMore
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInIncompatibility("org.bepinex.plugins.creaturelevelcontrol")]
    public class DropMoreLootMain : BaseUnityPlugin
    {
        public const string MODNAME = "DropMoreLoot";
        public const string AUTHOR = "TastyChickenLegs";
        public const string GUID = AUTHOR+"."+MODNAME;
        public const string VERSION = "1.0.3";
        public static ManualLogSource logger;
        internal Harmony harmony;
        internal Assembly assembly;
        public string modFolder = null;
        public static ConfigEntry<int> materialMultiplier = null;
        public static ConfigEntry<int> lootMultiplier = null;
        public static ConfigEntry<int> pickupMultiplier = null;
        public static ConfigEntry<bool> enableWhitelist = null;
        private static Assembly ass = typeof(DropMoreLootMain).Assembly;
        public static List<string> whitelist = null;
        public static string whitelistFile = null;
        public static ConfigEntry<int> pickUpRange = null;
        public static ConfigEntry<int> itemStackMultiplier = null;
        public static ConfigEntry<bool> itemsFloatInWater = null;
        public static ConfigEntry<bool> enableStacking = null;
        public static ConfigEntry<bool> enablePickUpRange = null;
        





        public static readonly ManualLogSource TastyLogger =
            BepInEx.Logging.Logger.CreateLogSource(MODNAME);

        private void Awake()
        {

            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            if (!File.Exists(whitelistFile))
                File.Create(whitelistFile).Dispose();

            materialMultiplier = Config.Bind<int>("Loot", "Multiplier for resources", 1, new ConfigDescription( "Material Multiplier", new AcceptableValueRange<int>(1, 5)));
            lootMultiplier = Config.Bind<int>("Loot", "Multiplier for monster drops", 1, new ConfigDescription( " Monster Drop Multiplier", new AcceptableValueRange<int>(1,5)));
            pickupMultiplier = Config.Bind<int>("Loot", "Multiplier for pickable objects", 1, new ConfigDescription("Pickup Multiplier", new AcceptableValueRange<int>(1,5)));
            enableWhitelist = Config.Bind<bool>("Whitelist", "Enable whitelist filter", false, "Whitelist");
            enablePickUpRange = Config.Bind<bool>("Pickup", "Enable Pickup Range", false, "Enable Pickup Range");
            pickUpRange = Config.Bind<int>("Pickup", "Pickup Range", 1, new ConfigDescription("Auto Pickup Distance from Player", new AcceptableValueRange<int>(1, 5)));
            itemStackMultiplier = Config.Bind<int>("Stacking", "Item Stack Multiplier", 1, new ConfigDescription("Item Stacking Multiplier", new AcceptableValueRange<int>(1, 5)));
            itemsFloatInWater = Config.Bind<bool>("Pickup", "Items Float in Water", false, "Items Always Float in Water");
            enableStacking = Config.Bind<bool>("Stacking", "Enable Stacking", false, "Use Stacking Feature");
            whitelist = Enumerable.Distinct<string>(File.ReadAllLines(Path.GetDirectoryName(ass.Location) + "\\whitelist.txt")).ToList<string>();
            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            
            



            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(this.assembly.Location);
            harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
            TastyLogger.LogInfo(whitelist);
        }

        private void OnDestroy()
        {
            //Dbgl("Destroying plugin");
            harmony.UnpatchSelf();
        }


       
      
    }
}