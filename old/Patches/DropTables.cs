//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using DropMore;

//namespace DropMore.Patches;

//internal class DropTablesMain
//{
//    [HarmonyPatch(typeof(DropTable), "GetDropList", new Type[] { typeof(int) })]
//    public static class DropTable_GetDropList_Patch
//    {
//        private static float originalDropChance = 0;

//        private static void Prefix(ref DropTable __instance, ref List<GameObject> __result, int amount)
//        {
//            originalDropChance = __instance.m_dropChance; // we have to save the original to change it back after the function
//                                                          //if (Configuration.Current.Gathering.IsEnabled && Configuration.Current.Gathering.dropChance != 0)
//                                                          //{
//            float newDropChance = TastyUtils.applyModifierValue(__instance.m_dropChance, DropMoreMain.dropChance.Value);
//            if (newDropChance >= 1)
//                newDropChance = 1;
//            if (newDropChance <= 0)
//                newDropChance = 0;

//            if (__instance.m_dropChance != 1)
//                __instance.m_dropChance = newDropChance;
//            //}
//        }

//        private static void Postfix(ref DropTable __instance, ref List<GameObject> __result, int amount)
//        {
//            __instance.m_dropChance = originalDropChance; // Apply the original drop chance in case modified

//            //if (!Configuration.Current.Gathering.IsEnabled)
//            //    return;
//            int flint = 0;
//            GameObject flintObject = null;

//            int wood = 0;
//            GameObject woodObject = null;

//            int coreWood = 0;
//            GameObject coreWoodObject = null;

//            int stone = 0;
//            GameObject stoneObject = null;

//            int scrapIron = 0;
//            GameObject scrapIronObject = null;

//            int tinOre = 0;
//            GameObject tinOreObject = null;

//            int copperOre = 0;
//            GameObject copperOreObject = null;

//            int silverOre = 0;
//            GameObject silverOreObject = null;

//            int elderBark = 0;
//            GameObject elderBarkObject = null;

//            int fineWood = 0;
//            GameObject fineWoodObject = null;

//            int chitin = 0;
//            GameObject chitinObject = null;

//            List<GameObject> defaultDrops = new List<GameObject>();
//            foreach (GameObject toDrop in __result)
//            {
//                switch (toDrop.name)
//                {
//                    case "Flint":
//                        flint += 1;
//                        flintObject= toDrop;
//                        break;

//                    case "Wood": // Wood
//                        wood += 1;
//                        woodObject = toDrop;
//                        break;

//                    case "RoundLog": // Corewood
//                        coreWood += 1;
//                        coreWoodObject = toDrop;
//                        break;

//                    case "Stone": // Stone
//                        stone += 1;
//                        stoneObject = toDrop;
//                        break;

//                    case "IronScrap": // Iron
//                        scrapIron += 1;
//                        scrapIronObject = toDrop;
//                        break;

//                    case "TinOre": // Tin
//                        tinOre += 1;
//                        tinOreObject = toDrop;
//                        break;

//                    case "CopperOre": // Copper
//                        copperOre += 1;
//                        copperOreObject = toDrop;
//                        break;

//                    case "SilverOre": // Silver
//                        silverOre += 1;
//                        silverOreObject = toDrop;
//                        break;

//                    case "ElderBark": // ElderBark
//                        elderBark += 1;
//                        elderBarkObject = toDrop;
//                        break;

//                    case "FineWood": // Finewood
//                        fineWood += 1;
//                        fineWoodObject = toDrop;
//                        break;

//                    case "Chitin": // Chitin
//                        chitin += 1;
//                        chitinObject = toDrop;
//                        break;

//                    default:
//                        defaultDrops.Add(toDrop);
//                        break;
//                }
//            }
//            //Add Flint
//            for (int i = 0; i < TastyUtils.applyModifierValue(flint, DropMoreMain.flint.Value); i++)
//            {
//                defaultDrops.Add(flintObject);
//            }
//            // Add Wood
//            for (int i = 0; i < TastyUtils.applyModifierValue(wood, DropMoreMain.wood.Value); i++)
//            {
//                defaultDrops.Add(woodObject);
//            }

//            // Add CoreWood
//            for (int i = 0; i < TastyUtils.applyModifierValue(coreWood, DropMoreMain.coreWood.Value); i++)
//            {
//                defaultDrops.Add(coreWoodObject);
//            }

//            //// Add Stone
//            for (int i = 0; i < TastyUtils.applyModifierValue(stone, DropMoreMain.stone.Value); i++)
//            {
//                defaultDrops.Add(stoneObject);
//            }

//            //// ScrapIron
//            for (int i = 0; i < TastyUtils.applyModifierValue(scrapIron, DropMoreMain.ironScrap.Value); i++)
//            {
//                defaultDrops.Add(scrapIronObject);
//            }

//            //// TinOre
//            for (int i = 0; i < TastyUtils.applyModifierValue(tinOre, DropMoreMain.tinOre.Value); i++)
//            {
//                defaultDrops.Add(tinOreObject);
//            }

//            //// CopperOre
//            for (int i = 0; i < TastyUtils.applyModifierValue(copperOre, DropMoreMain.copperOre.Value); i++)
//            {
//                defaultDrops.Add(copperOreObject);
//            }

//            //// silverOre
//            for (int i = 0; i < TastyUtils.applyModifierValue(silverOre, DropMoreMain.silverOre.Value); i++)
//            {
//                defaultDrops.Add(silverOreObject);
//            }

//            // ElderBark
//            for (int i = 0; i < TastyUtils.applyModifierValue(elderBark, DropMoreMain.elderBark.Value); i++)
//            {
//                defaultDrops.Add(elderBarkObject);
//            }

//            // FineWood
//            for (int i = 0; i < TastyUtils.applyModifierValue(fineWood, DropMoreMain.fineWood.Value); i++)
//            {
//                defaultDrops.Add(fineWoodObject);
//            }

//            // Chitin
//            for (int i = 0; i < TastyUtils.applyModifierValue(chitin, DropMoreMain.chitin.Value); i++)
//            {
//                defaultDrops.Add(chitinObject);
//            }

//            __result = defaultDrops;
//        }
//    }

    
//}


