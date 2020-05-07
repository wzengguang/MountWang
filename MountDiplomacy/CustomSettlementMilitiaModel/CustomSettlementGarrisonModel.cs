using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Wang
{
    [HarmonyPatch(typeof(SettlementGarrisonModel))]
    public class CustomSettlementGarrisonModel
    {
        private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage");

        private static readonly TextObject _paymentIsLess = GameTexts.FindText("str_payment_is_less");

        [HarmonyPostfix]
        [HarmonyPatch("CalculateGarrisonChangeInternal")]
        private static int CalculateGarrisonChangeInternal(Settlement settlement, StatExplainer explanation = null)
        {

            ExplainedNumber result = new ExplainedNumber(0f, explanation);
            if (settlement.IsTown || settlement.IsCastle)
            {
                //修改驻军减少算法
                if (settlement.Town.GarrisonParty != null && settlement.IsStarving)
                {
                    float foodChange = settlement.Town.FoodChange;
                    float num = (settlement.Town.Owner.IsStarving && foodChange < 0f) ? (int)foodChange : 0;
                    num = num * 10f / (settlement.Town.Loyalty + 10f);

                    if (settlement.Town.Governor == null || settlement.Town.Governor.Culture != settlement.Town.Culture)
                    {
                        num *= 2f;
                    }
                    else
                    {
                        num /= 2f;
                    }

                    result.Add(num, _foodShortageText);
                }

                if (settlement.Town.GarrisonParty != null && ((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + result.ResultNumber) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit > settlement.Town.GarrisonParty.PaymentRatio)
                {
                    int num2 = 0;
                    do
                    {
                        num2++;
                    }
                    while (!(((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + result.ResultNumber - (float)num2) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit < settlement.Town.GarrisonParty.PaymentRatio) && !((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + result.ResultNumber - (float)num2 <= 0f) && num2 < 20);
                    result.Add(-num2, _paymentIsLess);
                }
            }
            CustomSettlementGarrisonModelHelp.GetSettlementGarrisonChangeDueToIssues(settlement, ref result);
            return (int)result.ResultNumber;
        }



        [HarmonyPostfix]
        [HarmonyPatch("FindNumberOfTroopsToTakeFromGarrison")]
        private static void FindNumberOfTroopsToTakeFromGarrison(ref int __result, MobileParty mobileParty, Settlement settlement, float defaultIdealGarrisonStrengthPerWalledCenter = 0f)
        {
            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            int num9 = 75;
            num9 *= (settlement.IsTown ? 2 : 1);
            if (__result > garrisonParty.Party.MemberRoster.TotalRegulars - num9)
            {
                __result = garrisonParty.Party.MemberRoster.TotalRegulars - num9;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch("FindNumberOfTroopsToLeaveToGarrison")]
        private static bool FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
        {
            return !settlement.IsStarving;
        }
    }

    public static class CustomSettlementGarrisonModelHelp
    {
        private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues");
        public static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, out float totalChange))
            {
                result.Add(totalChange, _issues);
            }
        }
    }
}
