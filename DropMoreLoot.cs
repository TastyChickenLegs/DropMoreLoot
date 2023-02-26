using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LongerDays;
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
   
    public class DropMoreLootMain : BaseUnityPlugin
    {
        public const string MODNAME = "DropMoreLoot";
        public const string AUTHOR = "TastyChickenLegs";
        public const string GUID = AUTHOR+"."+MODNAME;
        public const string VERSION = "1.0.6";
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
        public static ConfigEntry<float> itemWeightReduction = null;





        public static readonly ManualLogSource TastyLogger =
            BepInEx.Logging.Logger.CreateLogSource(MODNAME);

        private void Awake()
        {

            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            if (!File.Exists(whitelistFile))
                File.Create(whitelistFile).Dispose();

            materialMultiplier = Config.Bind<int>("Loot", "Multiplier for resources", 3, 
                new ConfigDescription( "Material Multiplier", 
                new AcceptableValueRange<int>(1, 5)));
            lootMultiplier = Config.Bind<int>("Loot", "Multiplier for monster drops", 3, 
                new ConfigDescription( " Monster Drop Multiplier", 
                new AcceptableValueRange<int>(1,5)));
            pickupMultiplier = Config.Bind<int>("Loot", "Multiplier for pickable objects", 3, 
                new ConfigDescription("Pickup Multiplier", 
                new AcceptableValueRange<int>(1,5)));
            enableWhitelist = Config.Bind<bool>("Whitelist", "Enable whitelist filter", false, "Whitelist");
            enablePickUpRange = Config.Bind<bool>("Pickup", "Enable Pickup Range", true, "Enable Pickup Range");
            pickUpRange = Config.Bind<int>("Pickup", "Pickup Range", 3, 
                new ConfigDescription("Auto Pickup Distance from Player", 
                new AcceptableValueRange<int>(1, 10)));
            itemStackMultiplier = Config.Bind<int>("Stacking", "Item Stack Multiplier", 3, 
                new ConfigDescription("Item Stacking Multiplier", 
                new AcceptableValueRange<int>(1, 5)));
            itemsFloatInWater = Config.Bind<bool>("Pickup", "Items Float in Water", true, "Items Always Float in Water");
            enableStacking = Config.Bind<bool>("Stacking", "Enable Stacking", true, "Use Stacking Feature");
            whitelist = Enumerable.Distinct<string>(File.ReadAllLines(Path.GetDirectoryName(ass.Location) + "\\whitelist.txt")).ToList<string>();
            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            itemWeightReduction = Config.Bind<float>("Loot", "percentofweight", 0.5f, 
                new ConfigDescription("Item Weight Reduction Multiplier - Lower number means less weight per item", 
                new AcceptableValueRange<float>(0.0f, 1.0f), null, new ConfigurationManagerAttributes { ShowRangeAsPercent = 
                true, DispName = "Weight Multiplyer" }));




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