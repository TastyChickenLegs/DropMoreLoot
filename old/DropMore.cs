using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;
using System.Linq;

namespace DropMore
{
    [BepInPlugin(ModName, Name, Version)]
    public class DropMoreMain : BaseUnityPlugin
    {
        public const string Name = "DropMore";
        public const string ModName = "TastyChickenLegs." + Name;
        public const string Version = "1.0.0";
        public static DropMoreMain context;
        public static readonly ManualLogSource TastyLogger =
               BepInEx.Logging.Logger.CreateLogSource(ModName);
        //public static Assembly ass = typeof(DropMoreMain).Assembly;
        //public static ConfigEntry<float> wood;
        //public static ConfigEntry<float> stone;
        //public static ConfigEntry<float> fineWood;
        //public static ConfigEntry<float> coreWood;
        //public static ConfigEntry<float> elderBark;
        //public static ConfigEntry<float> ironScrap;
        //public static ConfigEntry<float> tinOre;
        //public static ConfigEntry<float> copperOre;
        //public static ConfigEntry<float> silverOre;
        //public static ConfigEntry<float> chitin;
        //public static ConfigEntry<float> flint;
        //internal static ConfigEntry<float> dropChance;
        internal static ConfigEntry<int> materialMultiplier;
        internal static ConfigEntry<int> lootMultiplier;
        internal static ConfigEntry<int> pickupMultiplier;
        internal static ConfigEntry<bool> enableWhitelist;
        internal static List<string> whitelist;
        internal static Assembly ass = typeof(DropMoreMain).Assembly;
        internal static Assembly assembly;

        public void Awake()
        {
            //wood = Config.Bind<float>("Wood", "Wood", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //stone = Config.Bind<float>("Stone", "Stone", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //fineWood = Config.Bind<float>("fineWood", "FineWood", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //coreWood = Config.Bind<float>("coreWood", "CoreWood", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //elderBark = Config.Bind<float>("elderBark", "ElderBark", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //ironScrap = Config.Bind<float>("ironScrap", "IronScrap", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //tinOre = Config.Bind<float>("tinOre", "TinOre", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //copperOre = Config.Bind<float>("copperOre", "CopperOre", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //silverOre = Config.Bind<float>("silverOre", "silverOre", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //chitin = Config.Bind<float>("chitin", "Chitin", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //dropChance = Config.Bind<float>("dropChance", "DropChance", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            //flint = Config.Bind<float>("Flint", "Flint", 100f, new ConfigDescription("The value 50 will increase the dropped wood from trees from 10 to 15. The value -50 will reduce the amount of dropped wood from 10 to 5", new AcceptableValueRange<float>(1f, 100f)));
            materialMultiplier = Config.Bind<int>("General", "Multiplier for resources", 1, "Material Multiplier");
            lootMultiplier = Config.Bind<int>("General", "Multiplier for monster drops", 1, " Monster Drop Multiplier");
            pickupMultiplier = Config.Bind<int>("General", "Multiplier for pickable objects", 1, "Pickup Multiplier");
            enableWhitelist = Config.Bind<bool>("Whitelist", "Enable whitelist filter", false, "Whitelist");
            whitelist = Enumerable.Distinct<string>(File.ReadAllLines(Path.GetDirectoryName(DropMoreMain.ass.Location) + "\\whitelist.txt")).ToList<string>();
            

            Harmony harmony = new Harmony(ModName);
            assembly = Assembly.GetExecutingAssembly();

            harmony.PatchAll();

        }
        private void Update()
        {
            Config.Save();
        }

        [HarmonyPatch(typeof(DropTable), "GetDropList", new Type[]
        {

        })]
        [HarmonyPrefix]
        private static bool MultiplyResources(DropTable __instance, ref List<GameObject> __result)
        {
            List<DropTable.DropData> list = new List<DropTable.DropData>(__instance.m_drops);
            int amount;
            if (DropMoreMain.enableWhitelist.Value)
            {
                foreach (DropTable.DropData dropData in __instance.m_drops)
                {
                    if (dropData.m_item != null)
                    {
                        foreach (string value in DropMoreMain.whitelist)
                        {
                            if (dropData.m_item.name.Equals(value))
                            {
                                amount = UnityEngine.Random.Range(__instance.m_dropMin, __instance.m_dropMax + 1) * DropMoreMain.materialMultiplier.Value;
                                List<GameObject> dropList = __instance.GetDropList(amount);
                                foreach (DropTable.DropData dropData2 in list)
                                {
                                    int num = UnityEngine.Random.Range(dropData2.m_stackMin, dropData2.m_stackMax) * DropMoreMain.materialMultiplier.Value;
                                    if (dropData2.m_item.name.Equals("Honey") || dropData2.m_item.name.Equals("QueenBee"))
                                    {
                                        for (int i = 0; i < num; i++)
                                        {
                                            dropList.Add(dropData2.m_item);
                                        }
                                    }
                                }
                                __result = dropList;
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            amount = UnityEngine.Random.Range(__instance.m_dropMin, __instance.m_dropMax + 1) * DropMoreMain.materialMultiplier.Value;
            List<GameObject> dropList2 = __instance.GetDropList(amount);
            foreach (DropTable.DropData dropData3 in list)
            {
                int num2 = UnityEngine.Random.Range(dropData3.m_stackMin, dropData3.m_stackMax) * DropMoreMain.materialMultiplier.Value;
                if (dropData3.m_item.name.Equals("Honey") || dropData3.m_item.name.Equals("QueenBee"))
                {
                    for (int j = 0; j < num2; j++)
                    {
                        dropList2.Add(dropData3.m_item);
                    }
                }
            }
            __result = dropList2;
            return false;
        }


        [HarmonyPatch(typeof(CharacterDrop), "GenerateDropList")]
        [HarmonyPrefix]
        private static bool MultiplyLoot(CharacterDrop __instance, ref List<KeyValuePair<GameObject, int>> __result)
        {
            List<KeyValuePair<GameObject, int>> list = new List<KeyValuePair<GameObject, int>>();
            int num = __instance.m_character ? Mathf.Max(1, (int)Mathf.Pow(2f, (float)(__instance.m_character.GetLevel() - 1))) : 1;
            foreach (CharacterDrop.Drop drop in __instance.m_drops)
            {
                if (!(drop.m_prefab == null))
                {
                    float num2 = drop.m_chance;
                    if (drop.m_levelMultiplier)
                    {
                        num2 *= (float)num;
                    }
                    if (UnityEngine.Random.value <= num2)
                    {
                        int num3 = UnityEngine.Random.Range(drop.m_amountMin, drop.m_amountMax);
                        if (drop.m_levelMultiplier)
                        {
                            num3 *= num;
                        }
                        if (drop.m_onePerPlayer)
                        {
                            num3 = ZNet.instance.GetNrOfPlayers();
                        }
                        if (num3 > 0)
                        {
                            if (DropMoreMain.enableWhitelist.Value)
                            {
                                if (drop.m_prefab != null)
                                {
                                    foreach (string value in DropMoreMain.whitelist)
                                    {
                                        if (drop.m_prefab.name.Equals(value))
                                        {
                                            list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3 * DropMoreMain.lootMultiplier.Value - num3));
                                        }
                                    }
                                    list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3));
                                }
                            }
                            else
                            {
                                list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3 * DropMoreMain.lootMultiplier.Value));
                            }
                        }
                    }
                }
            }
            __result = list;
            return false;
        }


        [HarmonyPatch(typeof(Pickable), "RPC_Pick")]
        [HarmonyPrefix]
        private static bool MultiplyPickables(Pickable __instance)
        {
            if (!__instance.m_nview.IsOwner())
            {
                return true;
            }
            if (__instance.m_picked)
            {
                return true;
            }
            int num = 0;
            if (DropMoreMain.pickupMultiplier.Value >= 2)
            {
                int i = 0;
                while (i < __instance.m_amount)
                {
                    if (!DropMoreMain.enableWhitelist.Value)
                    {
                        goto IL_AA;
                    }
                    if (__instance.m_itemPrefab != null)
                    {
                        using (List<string>.Enumerator enumerator = DropMoreMain.whitelist.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                string value = enumerator.Current;
                                if (__instance.m_itemPrefab.name.Equals(value))
                                {
                                    __instance.Drop(__instance.m_itemPrefab, num++, DropMoreMain.pickupMultiplier.Value - 1);
                                }
                            }
                            goto IL_C7;
                        }
                        goto IL_AA;
                    }
                IL_C7:
                    i++;
                    continue;
                IL_AA:
                    __instance.Drop(__instance.m_itemPrefab, num++, DropMoreMain.pickupMultiplier.Value - 1);
                    goto IL_C7;
                }
            }
            return true;
        }


        [HarmonyPatch(typeof(PickableItem), "Drop")]
        [HarmonyPrefix]
        private static bool GetValuable(PickableItem __instance)
        {
            Vector3 position = __instance.gameObject.transform.position + Vector3.up * 0.2f;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.m_itemPrefab.gameObject, position, __instance.gameObject.transform.rotation);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * 4f;
            ItemDrop component = gameObject.GetComponent<ItemDrop>();
            if (DropMoreMain.enableWhitelist.Value)
            {
                using (List<string>.Enumerator enumerator = DropMoreMain.whitelist.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        string value = enumerator.Current;
                        if (component.m_itemData.m_dropPrefab.name.Equals(value))
                        {
                            component.m_itemData.m_stack = __instance.GetStackSize() * DropMoreMain.pickupMultiplier.Value;
                            return false;
                        }
                        component.m_itemData.m_stack = __instance.GetStackSize();
                        return false;
                    }
                }
            }
            component.m_itemData.m_stack = __instance.GetStackSize() * DropMoreMain.pickupMultiplier.Value;
            return false;
        }


        [HarmonyPatch(typeof(Fish), "RPC_Pickup")]
        [HarmonyPrefix]
        private static bool FishStack(Fish __instance)
        {
            if (DropMoreMain.enableWhitelist.Value)
            {
                foreach (string value in DropMoreMain.whitelist)
                {
                    if (__instance.m_pickupItem.name.Equals(value))
                    {
                        __instance.m_pickupItemStackSize *= DropMoreMain.lootMultiplier.Value;
                    }
                }
                return true;
            }
            __instance.m_pickupItemStackSize *= DropMoreMain.lootMultiplier.Value;
            return true;
        }


        [HarmonyPatch(typeof(Humanoid), "PickupPrefab")]
        [HarmonyPrefix]
        private static void MaxStack(Humanoid __instance, ref GameObject prefab)
        {
            prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize *= DropMoreMain.lootMultiplier.Value;
        }

    }
}

