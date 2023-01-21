using HarmonyLib;
using DropMoreLoot;
using DropMore;
using System;

namespace DropMoreLoot.Patches
{
    internal class PlayerPatches
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        public static class Player_Awake_Patch
        {
            private static void Postfix(ref Player __instance)
            {
                if (DropMoreLootMain.enablePickUpRange.Value)
                { 
                float autoPickupFloat = Convert.ToSingle(DropMoreLootMain.pickUpRange.Value);
                __instance.m_autoPickupRange = autoPickupFloat;
                }

            }
        }
    }
}
