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
    public class CustomSettlementGarrisonModel : SettlementGarrisonModel
    {
        private static readonly TextObject _townWallsText = new TextObject("{=SlmhqqH8}Town Walls");

        private static readonly TextObject _moraleText = new TextObject("{=UjL7jVYF}Morale");

        private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage");

        private static readonly TextObject _surplusFoodText = GameTexts.FindText("str_surplus_food");

        private static readonly TextObject _recruitFromCenterNotablesText = GameTexts.FindText("str_center_notables");

        private static readonly TextObject _recruitFromVillageNotablesText = GameTexts.FindText("str_village_notables");

        private static readonly TextObject _villageBeingRaided = GameTexts.FindText("str_village_being_raided");

        private static readonly TextObject _villageLooted = GameTexts.FindText("str_village_looted");

        private static readonly TextObject _townIsUnderSiege = GameTexts.FindText("str_villages_under_siege");

        private static readonly TextObject _retiredText = GameTexts.FindText("str_retired");

        private static readonly TextObject _paymentIsLess = GameTexts.FindText("str_payment_is_less");

        private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues");

        public override int CalculateGarrisonChange(Settlement settlement, StatExplainer explanation = null)
        {
            return CalculateGarrisonChangeInternal(settlement, explanation);
        }

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
            GetSettlementGarrisonChangeDueToIssues(settlement, ref result);
            return (int)result.ResultNumber;
        }

        private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, out float totalChange))
            {
                result.Add(totalChange, _issues);
            }
        }

        public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement)
        {
            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            float num = 0f;
            if (garrisonParty != null)
            {
                num = garrisonParty.Party.TotalStrength;
                float num2 = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
                float num3 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
                num2 *= num3;
                num2 *= (settlement.IsTown ? 2f : 1f);
                float num4 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
                int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
                float num5 = Math.Min(12f, num4 / (float)numberOfAllMembers) - 1f;
                float num6 = (float)Math.Pow(num / num2, 1.5);
                float num7 = (mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero) ? 2f : 1f;
                int num8 = MBRandom.RoundRandomized(num5 * num6 * num7);
                int num9 = 100;
                num9 = ((!settlement.IsTown) ? SettlementMillitiaConfig.CastleRemainMillitiaNumber : SettlementMillitiaConfig.TownRemainMillitiaNumber);
                if (num8 > garrisonParty.Party.MemberRoster.TotalRegulars - num9)
                {
                    num8 = garrisonParty.Party.MemberRoster.TotalRegulars - num9;
                }
                return num8;
            }
            return 0;
        }

        public override int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
        {
            if (settlement.IsStarving)
            {
                return 0;
            }

            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            float num = 0f;
            if (garrisonParty != null)
            {
                num = garrisonParty.Party.TotalStrength;
            }
            float num2 = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
            float num3 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
            num2 *= num3;
            num2 *= (settlement.IsTown ? 2f : 1f);
            if ((settlement.OwnerClan.Leader != Hero.MainHero || (mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan == Clan.PlayerClan)) && num < num2)
            {
                int numberOfRegularMembers = mobileParty.Party.NumberOfRegularMembers;
                float num4 = 1f + (float)mobileParty.Party.NumberOfWoundedRegularMembers / (float)mobileParty.Party.NumberOfRegularMembers;
                float num5 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
                float num6 = (float)(Math.Pow(Math.Min(2f, (float)numberOfRegularMembers / num5), 1.2000000476837158) * 0.75);
                float num7 = (1f - num / num2) * (1f - num / num2);
                if (mobileParty.Army != null)
                {
                    num7 = Math.Min(num7, 0.5f);
                }
                float num8 = 0.5f;
                if (settlement.OwnerClan == mobileParty.Leader.HeroObject.Clan || settlement.OwnerClan == mobileParty.Party.Owner.MapFaction.Leader.Clan)
                {
                    num8 = 1f;
                }
                float num9 = (mobileParty.Army != null) ? 1.25f : 1f;
                float num10 = 1f;
                List<float> list = new List<float>(5);
                for (int i = 0; i < 5; i++)
                {
                    list.Add(Campaign.MapDiagonal * Campaign.MapDiagonal);
                }
                foreach (Kingdom item in Kingdom.All)
                {
                    if (item.IsKingdomFaction && mobileParty.MapFaction.IsAtWarWith(item))
                    {
                        foreach (Settlement settlement2 in item.Settlements)
                        {
                            float num11 = settlement2.Position2D.DistanceSquared(mobileParty.Position2D);
                            for (int j = 0; j < 5; j++)
                            {
                                if (num11 < list[j])
                                {
                                    for (int num12 = 4; num12 >= j + 1; num12--)
                                    {
                                        list[num12] = list[num12 - 1];
                                    }
                                    list[j] = num11;
                                    break;
                                }
                            }
                        }
                    }
                }
                float num13 = 0f;
                for (int k = 0; k < 5; k++)
                {
                    num13 += (float)Math.Sqrt(list[k]);
                }
                num13 /= 5f;
                float num14 = Math.Max(0f, Math.Min(Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f, num13 - Campaign.MapDiagonal / 30f));
                float num15 = Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f;
                _ = num14 / num15;
                float num16 = Math.Min(0.7f, num10 * num6 * num7 * num8 * num9 * num4);
                return MBRandom.RoundRandomized((float)numberOfRegularMembers * num16);
            }
            return 0;
        }
    }
}
