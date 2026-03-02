using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;

namespace DropMoreLoot
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class DropMoreLootMain : BaseUnityPlugin
    {
        internal const string ModName = "DropMoreLoot";
        internal const string ModVersion = "1.2.5";
        internal const string Author = "TastyChickenLegs";
        private const string ModGUID = Author + "." + ModName;
        public static ManualLogSource logger;
        internal Harmony harmony;
        internal Assembly assembly;
        public string modFolder = null;
        public static ConfigEntry<int> materialMultiplier = null;
        public static ConfigEntry<int> lootMultiplier = null;
        public static ConfigEntry<int> pickupMultiplier = null;
        public static ConfigEntry<bool> enableWhitelist = null;
        public static ConfigEntry<string> whitelistitems = null;
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


        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static string ConnectionError = "";

        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource DropMoreLootLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            // Make sure to populate the English.yml file in the translation folder with your keys to be localized and the values associated before uncommenting!.
            //Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            if (!File.Exists(whitelistFile))
                File.Create(whitelistFile).Dispose();

            materialMultiplier = config<int>("Loot", "Multiplier for resources", 3,
                new ConfigDescription("Material Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            lootMultiplier = config<int>("Loot", "Multiplier for monster drops", 3,
                new ConfigDescription(" Monster Drop Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            pickupMultiplier = config<int>("Loot", "Multiplier for pickable objects", 3,
                new ConfigDescription("Pickup Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            enableWhitelist = config<bool>("Whitelist", "Enable whitelist filter", false, "Whitelist");
            enablePickUpRange = config<bool>("Pickup", "Enable Pickup Range", true, "Enable Pickup Range");
            pickUpRange = config<int>("Pickup", "Pickup Range", 3,
                new ConfigDescription("Auto Pickup Distance from Player",
                new AcceptableValueRange<int>(1, 10)));
            itemStackMultiplier = config<int>("Stacking", "Item Stack Multiplier", 3,
                new ConfigDescription("Item Stacking Multiplier",
                new AcceptableValueRange<int>(1, 5)));
            itemsFloatInWater = config<bool>("Pickup", "Items Float in Water", true, "Items Always Float in Water");
            enableStacking = config<bool>("Stacking", "Enable Stacking", true, "Use Stacking Feature");
            whitelist = Enumerable.Distinct<string>(File.ReadAllLines(Path.GetDirectoryName(ass.Location) + "\\whitelist.txt")).ToList<string>();
            whitelistFile = Path.GetDirectoryName(ass.Location) + "\\whitelist.txt";
            itemWeightReduction = config<float>("Loot", "percentofweight", 0.5f,
                new ConfigDescription("Item Weight Reduction Multiplier - Lower number means less weight per item",
                new AcceptableValueRange<float>(0.0f, 1.0f), null, new ConfigurationManagerAttributes
                {
                 ShowRangeAsPercent = 
                true,
                    DispName = "Weight Multiplyer"
                }));
            useWeightReduction = config<bool>("Loot", "useWeightReduction", true,
                new ConfigDescription("Use the Weight Reduction Feature", null,
                new ConfigurationManagerAttributes { DispName = "Ues Weight Reduction Settings" }));
            whitelistitems = config<string>("Whitelist", "White List Itmes", "", "White List Items synced with server, comma-seperated.");

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
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
                DropMoreLootLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                DropMoreLootLogger.LogError($"There was an issue loading your {ConfigFileName}");
                DropMoreLootLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }



        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
    
    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }
    }
}