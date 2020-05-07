using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Wang
{

    // [HarmonyPatch(typeof(BanditsCampaignBehavior))]
    public class BanditsCampaignBehaviorPatch
    {

        //  [HarmonyPostfix]
        //  [HarmonyPatch("SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement")]
        //private static void PostfixActivate(BanditsCampaignBehavior __instance, int numberOfBanditsWillBeSpawned)
        //{
        //    //(1+(n3*13+200-n2)*0.01,620)
        //    // InformationManager.DisplayMessage(new InformationMessage("SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement."));

        //    List<Clan> list = Clan.BanditFactions.ToList();
        //    Dictionary<Clan, int> dictionary = new Dictionary<Clan, int>(list.Count);
        //    foreach (Clan item in list)
        //    {
        //        dictionary.Add(item, 0);
        //    }
        //    foreach (Settlement item2 in Settlement.All)
        //    {
        //        if (item2.IsHideout() && item2.Hideout.IsInfested)
        //        {
        //            dictionary[item2.OwnerClan]++;
        //        }
        //    }

        //    int num = __instance.NumberOfMaxBanditPartiesAroundEachHideout() + __instance.NumberOfMaximumBanditPartiesInEachHideout() + 1;
        //    int num2 = __instance.NumberOfMaxHideoutsAtEachBanditFaction() * num;
        //    int num3 = 0;
        //    foreach (Clan item3 in list)
        //    {
        //        num3 += item3.Parties.Count();
        //    }
        //    numberOfBanditsWillBeSpawned = Math.Max(0, Math.Min(numberOfBanditsWillBeSpawned, list.Count((Clan f) => !__instance.IsLooterFaction(f)) * num2 + __instance.NumberOfMaximumLooterParties() - num3));
        //    numberOfBanditsWillBeSpawned = (int)Math.Ceiling((float)numberOfBanditsWillBeSpawned * 0.667f) + MBRandom.RandomInt(numberOfBanditsWillBeSpawned / 3);
        //    for (int i = 0; i < numberOfBanditsWillBeSpawned; i++)
        //    {
        //        Clan clan = null;
        //        float num4 = 1f;
        //        for (int j = 0; j < list.Count; j++)
        //        {
        //            float num5 = 1f;
        //            if (__instance.IsLooterFaction(list[j]))
        //            {
        //                num5 = (float)list[j].Parties.Count() / (float)__instance.NumberOfMaximumLooterParties();
        //            }
        //            else
        //            {
        //                int num6 = dictionary[list[j]];
        //                if (num6 > 0)
        //                {
        //                    num5 = (float)list[j].Parties.Count() / (float)(num6 * num);
        //                }
        //            }
        //            if (num5 < 1f && (clan == null || num5 < num4))
        //            {
        //                clan = list[j];
        //                num4 = num5;
        //            }
        //        }
        //        if (clan == null)
        //        {
        //            break;
        //        }



        //        BanditsCampaignBehaviorPatch.SpawnAPartyInFaction(__instance, clan);

        //        // SpawnAPartyInFaction(clan);
        //    }
        //}



    }
}
