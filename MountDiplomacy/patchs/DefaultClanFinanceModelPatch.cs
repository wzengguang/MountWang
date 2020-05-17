using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Wang.patchs
{
    [HarmonyPatch(typeof(DefaultClanFinanceModel))]
    public class DefaultClanFinanceModelPatch
    {
        //[HarmonyPostfix]
        //[HarmonyPatch("CalculatePartyWage")]
        //private static void CalculatePartyWage(ref int __result, MobileParty mobileParty, bool applyWithdrawals)
        //{
        //    if (mobileParty.IsActive && mobileParty.IsGarrison && mobileParty.Party.Owner != null && mobileParty.Party.Owner.Clan.Leader != Hero.MainHero)
        //    {
        //        __result = (int)(__result * 0.7f);
        //    }
        //}


        [HarmonyPostfix]
        [HarmonyPatch("CalculateOwnerIncomeFromWorkshop")]
        private static void CalculateOwnerIncomeFromWorkshop(DefaultClanFinanceModel __instance, ref int __result, Workshop workshop)
        {
            if (workshop.Owner != null &&
                (workshop.Owner.Clan == Clan.PlayerClan &&
                workshop.Owner.Clan.MapFaction.IsAtWarWith(workshop.Settlement.MapFaction)) &&
                !workshop.Owner.Clan.Leader.GetPerkValue(DefaultPerks.Trade.RapidDevelopment))
            {
                workshop.ChangeGold(-__result);
                __result = 0;
            }
        }
    }
}
