using HarmonyLib;
using DropMoreLoot;
using DropMore;
using System;

namespace DropMoreLoot.Patches
{
    internal class GamePatches
    {
        [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Awake))]
        public static class ItemDrop_Awake_Patch
        {
            private static void Prefix(ref ItemDrop __instance)
            {

                if (__instance.m_itemData.m_shared.m_maxStackSize > 1)
                {

                    if (DropMoreLootMain.itemStackMultiplier.Value >= 1 || DropMoreLootMain.enableStacking.Value)
                    {
                        float itemStackFloat = Convert.ToSingle(DropMoreLootMain.itemStackMultiplier.Value);
                        __instance.m_itemData.m_shared.m_maxStackSize = (int)TastyUtils.applyModifierValue(__instance.m_itemData.m_shared.m_maxStackSize, itemStackFloat);
                    }
                }

                // Add floating property to all dropped items.
                if (!__instance.gameObject.GetComponent<Floating>() && DropMoreLootMain.itemsFloatInWater.Value)
                {
                    __instance.gameObject.AddComponent<Floating>();
                    __instance.gameObject.GetComponent<Floating>().m_waterLevelOffset = 0.5f;
                }
            }
        }
    }
}
