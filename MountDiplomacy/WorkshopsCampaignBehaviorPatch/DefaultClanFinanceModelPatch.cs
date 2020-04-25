using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateOwnerIncomeFromWorkshop")]
    public class DefaultClanFinanceModelPatch
    {

        private static void Postfix(DefaultClanFinanceModel __instance, Workshop workshop, ref int __result)
        {
            __result = (int)((float)Math.Max(0, workshop.ProfitMade) / __instance.RevenueSmoothenFraction());

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
