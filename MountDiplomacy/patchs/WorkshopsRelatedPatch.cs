using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch]
    public class WorkshopsRelatedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "OnWarDeclared")]
        private static bool OnWarDeclared(IFaction faction1, IFaction faction2)
        {
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateOwnerIncomeFromWorkshop")]
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
