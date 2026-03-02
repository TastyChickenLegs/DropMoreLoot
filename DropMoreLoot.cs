using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DropMoreLoot.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data;
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
        public const string GUID = AUTHOR + "." + MODNAME;
        public const string VERSION = "1.0.9";
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
        public static ConfigEntry<bool> useWeightReduction;
        public static ConfigEntry<string> AutoPickupBlockList;
        public static ConfigEntry<string> IncludedCategories;
        internal static IEnumerable<ItemDrop.ItemData.ItemType> categories;
        private static string ConfigFileName = GUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;



        public static readonly ManualLogSource TastyLogger =
            BepInEx.Logging.Logger.CreateLogSource(MODNAME);

        private void Awake()
        {

            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            if (!File.Exists(whitelistFile))
                File.Create(whitelistFile).Dispose();

            materialMultiplier = Config.Bind<int>("Loot", "Multiplier for resources", 3,
                new ConfigDescription("Material Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            lootMultiplier = Config.Bind<int>("Loot", "Multiplier for monster drops", 3,
                new ConfigDescription(" Monster Drop Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            pickupMultiplier = Config.Bind<int>("Loot", "Multiplier for pickable objects", 3,
                new ConfigDescription("Pickup Multiplier",
                new AcceptableValueRange<int>(1, 5)));
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
                new AcceptableValueRange<float>(0.0f, 1.0f), null, new ConfigurationManagerAttributes
                {
                    ShowRangeAsPercent =
                true,
                    DispName = "Weight Multiplyer"
                }));
            useWeightReduction = Config.Bind<bool>("Loot", "useWeightReduction", true,
                new ConfigDescription("Use the Weight Reduction Feature", null,
                new ConfigurationManagerAttributes { DispName = "Ues Weight Reduction Settings" }));

            AutoPickupBlockList = Config.Bind("General", "AutoPickupBlockList", string.Empty);
            IncludedCategories = Config.Bind("General", "IncludedCategories", "Material;Trophie;Consumable;Torch;Tool", "Semicolon separated list of item types to include. Possible types are:None;Material;Consumable;OneHandedWeapon;Bow;Shield;Helmet;Chest;Ammo;Customization;Legs;Hands;Trophie;TwoHandedWeapon;Torch;Misc;Shoulder;Utility;Tool;Attach_Atgeir");
            categories = BuildCategoryList();




            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(this.assembly.Location);
            harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
            TastyLogger.LogInfo(whitelist);
            SetupWatcher();
        }
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                TastyLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                TastyLogger.LogError($"There was an issue loading your {ConfigFileName}");
                TastyLogger.LogError("Please check your config entries for spelling and format!");
            }
        }
            private void OnDestroy()
        {
            //Dbgl("Destroying plugin");
            //harmony.UnpatchSelf();
            Config.Save();

        }
        
  
        private static IEnumerable<ItemDrop.ItemData.ItemType> BuildCategoryList()
        {
            var fromConfig = DropMoreLootMain.IncludedCategories.Value.Trim().Replace(" ", "").Split(';');
            foreach (var c in fromConfig)
            {
                if (Enum.TryParse<ItemDrop.ItemData.ItemType>(c, out var result))
                {
                    yield return result;
                }
            }
        }

    }
}