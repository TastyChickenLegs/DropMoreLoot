using HarmonyLib;
using DropMore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace ModDrops;

public class DropPatches
{
    [HarmonyPatch(typeof(DropTable), "GetDropList", new Type[] { })]

    public static class MultiplyResources_Patch
    {
        [HarmonyPrefix]
        private static bool MultiplyResources(DropTable __instance, ref List<GameObject> __result)
        {
            List<DropTable.DropData> list = new List<DropTable.DropData>(__instance.m_drops);
            int amount;
            
            if (DropMoreLootMain.enableWhitelist.Value)
            {
                foreach (DropTable.DropData dropData in __instance.m_drops)
                {
                    if (dropData.m_item != null)
                    {
                        foreach (string value in DropMoreLootMain.whitelist)
                        {
                            if (dropData.m_item.name.Equals(value))
                            {
                                amount = UnityEngine.Random.Range(__instance.m_dropMin, __instance.m_dropMax + 1) * DropMoreLootMain.materialMultiplier.Value;
                                List<GameObject> dropList = __instance.GetDropList(amount);
                                foreach (DropTable.DropData dropData2 in list)
                                {
                                    int num = UnityEngine.Random.Range(dropData2.m_stackMin, dropData2.m_stackMax) * DropMoreLootMain.materialMultiplier.Value;
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
            amount = UnityEngine.Random.Range(__instance.m_dropMin, __instance.m_dropMax + 1) * DropMoreLootMain.materialMultiplier.Value;
            List<GameObject> dropList2 = __instance.GetDropList(amount);
            foreach (DropTable.DropData dropData3 in list)
            {
                int num2 = UnityEngine.Random.Range(dropData3.m_stackMin, dropData3.m_stackMax) * DropMoreLootMain.materialMultiplier.Value;
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

        public static class MultiplyLoot_Patch
        {
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
 
                                if (DropMoreLootMain.enableWhitelist.Value)
                                {
                                    if (drop.m_prefab != null)
                                    {
                                        foreach (string value in DropMoreLootMain.whitelist)
                                        {
                                            if (drop.m_prefab.name.Equals(value))
                                            {
                                                list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3 * DropMoreLootMain.lootMultiplier.Value - num3));
                                            }
                                        }
                                        list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3));
                                    }

                                }
                                else
                                {
                                    list.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num3 * DropMoreLootMain.lootMultiplier.Value));
                                }
                            }
                        }
                    }
                }
                __result = list;
                return false;
            }
        }

        [HarmonyPatch(typeof(Pickable), "RPC_Pick")]


        public static class Pickable_Patch
        {
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
                if (DropMoreLootMain.pickupMultiplier.Value >= 2)
                {
                    int i = 0;
                    while (i < __instance.m_amount)
                    {
                        if (!DropMoreLootMain.enableWhitelist.Value)
                        {
                            goto IL_AA;
                        }
                        if (__instance.m_itemPrefab != null)
                        {
                            using (List<string>.Enumerator enumerator = DropMoreLootMain.whitelist.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    string value = enumerator.Current;
                                    if (__instance.m_itemPrefab.name.Equals(value))
                                    {
                                        __instance.Drop(__instance.m_itemPrefab, num++, DropMoreLootMain.pickupMultiplier.Value - 1);
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
                        __instance.Drop(__instance.m_itemPrefab, num++, DropMoreLootMain.pickupMultiplier.Value - 1);
                        goto IL_C7;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PickableItem), "Drop")]


        public static class Getvaluable_Patch
        {
            [HarmonyPrefix]
            private static bool GetValuable(PickableItem __instance)
            {
                Vector3 position = __instance.gameObject.transform.position + Vector3.up * 0.2f;
                GameObject gameObject = DropMoreLootMain.Instantiate(__instance.m_itemPrefab.gameObject, position, __instance.gameObject.transform.rotation);
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * 4f;
                ItemDrop component = gameObject.GetComponent<ItemDrop>();
                if (DropMoreLootMain.enableWhitelist.Value)
                {
                    using (List<string>.Enumerator enumerator = DropMoreLootMain.whitelist.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            string value = enumerator.Current;
                            if (component.m_itemData.m_dropPrefab.name.Equals(value))
                            {
                                component.m_itemData.m_stack = __instance.GetStackSize() * DropMoreLootMain.pickupMultiplier.Value;
                                return false;
                            }
                            component.m_itemData.m_stack = __instance.GetStackSize();
                            return false;
                        }
                    }
                }
                
                component.m_itemData.m_stack = __instance.GetStackSize() * DropMoreLootMain.pickupMultiplier.Value;
                return false;
            }
        }

        [HarmonyPatch(typeof(Fish), "RPC_Pickup")]

        private static class Rpgpickup_Patch
        {
            [HarmonyPrefix]
            private static bool FishStack(Fish __instance)
            {
                if (DropMoreLootMain.enableWhitelist.Value)
                {
                    foreach (string value in DropMoreLootMain.whitelist)
                    {
                        if (__instance.m_pickupItem.name.Equals(value))
                        {
                            __instance.m_pickupItemStackSize *= DropMoreLootMain.lootMultiplier.Value;
                        }
                    }
                    return true;
                }
                __instance.m_pickupItemStackSize *= DropMoreLootMain.lootMultiplier.Value;
                return true;
            }
        }
    }
}
