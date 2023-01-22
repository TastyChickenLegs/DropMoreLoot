using HarmonyLib;
using DropMoreLoot;
using DropMore;
using System;
using UnityEngine;

namespace DropMoreLoot.Patches
{
    internal class GamePatches
    {
        [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Awake))]
        public static class ItemDrop_Awake_Patch
        {
            private static void Prefix(ref ItemDrop __instance)
            {
                // Add stacking for items
                if (__instance.m_itemData.m_shared.m_maxStackSize > 1)
                {

                    if (DropMoreLootMain.itemStackMultiplier.Value >= 1 || DropMoreLootMain.enableStacking.Value)
                    {
                        int itemStackInt = DropMoreLootMain.itemStackMultiplier.Value;
                        int value = __instance.m_itemData.m_shared.m_maxStackSize * itemStackInt;
                        __instance.m_itemData.m_shared.m_maxStackSize = value;
                    }
                }

                // Add floating to dropped items.
                if (!__instance.gameObject.GetComponent<Floating>() && DropMoreLootMain.itemsFloatInWater.Value)
                {
                    __instance.gameObject.AddComponent<Floating>();
                    __instance.gameObject.GetComponent<Floating>().m_waterLevelOffset = 0.5f;
                }
            }
        }
    }
}
