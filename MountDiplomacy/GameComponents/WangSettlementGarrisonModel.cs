using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Wang.GameComponents
{
    public class WangSettlementGarrisonModel : SettlementGarrisonModel
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

            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation);
            if (settlement.IsTown || settlement.IsCastle)
            {
                //修改驻军减少算法
                if (settlement.IsStarving)
                {
                    float foodChange = settlement.Town.FoodChange;
                    float num = (settlement.Town.Owner.IsStarving && foodChange < -19f) ? (int)foodChange : 0;
                    num = num * 10f / (settlement.Town.Loyalty + 10f);

                    if (settlement.OwnerClan == null || settlement.OwnerClan.Culture != settlement.Town.Culture)
                    {
                        num *= 2f;
                    }
                    else
                    {
                        num /= 2f;
                    }

                    explainedNumber.Add(num, _foodShortageText);
                }

                if (settlement.Town.GarrisonParty != null && ((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit > settlement.Town.GarrisonParty.PaymentRatio)
                {
                    int num2 = 0;
                    do
                    {
                        num2++;
                    }
                    while (((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num2) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit >= settlement.Town.GarrisonParty.PaymentRatio && (float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num2 > 0f && num2 < 20);
                    explainedNumber.Add((float)(-(float)num2), _paymentIsLess);
                }
            }
            GetSettlementGarrisonChangeDueToIssues(settlement, ref explainedNumber);
            return (int)explainedNumber.ResultNumber;
        }

        private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            float value;
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, out value))
            {
                result.Add(value, _issues);
            }
        }

        public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement, float defaultIdealGarrisonStrengthPerWalledCenter = 0f)
        {
            MobileParty garrisonParty = settlement.Town.GarrisonParty;
            if (garrisonParty != null)
            {
                float totalStrength = garrisonParty.Party.TotalStrength;
                float num = (defaultIdealGarrisonStrengthPerWalledCenter > 0.1f) ? defaultIdealGarrisonStrengthPerWalledCenter : FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
                float num2 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
                num *= num2;
                num *= (settlement.IsTown ? 2f : 1f);
                float num3 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
                int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
                float num4 = num3 / (float)numberOfAllMembers;
                float num5 = (float)Math.Min(11.0, (double)num4 * Math.Sqrt((double)num4)) - 1f;
                float num6 = (float)Math.Pow((double)(totalStrength / num), 1.5);
                float num7 = (mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero) ? 2f : 1f;
                int num8 = MBRandom.RoundRandomized(num5 * num6 * num7);
                int num9 = 25;
                num9 *= (settlement.IsTown ? 2 : 1);
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
            if (settlement.IsStarving && settlement.Town.FoodChange < -19f)
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
                float num6 = (float)(Math.Pow((double)Math.Min(2f, (float)numberOfRegularMembers / num5), 1.2000000476837158) * 0.75);
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
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    if (kingdom.IsKingdomFaction && mobileParty.MapFaction.IsAtWarWith(kingdom))
                    {
                        foreach (Settlement settlement2 in kingdom.Settlements)
                        {
                            float num11 = settlement2.Position2D.DistanceSquared(mobileParty.Position2D);
                            for (int j = 0; j < 5; j++)
                            {
                                if (num11 < list[j])
                                {
                                    for (int k = 4; k >= j + 1; k--)
                                    {
                                        list[k] = list[k - 1];
                                    }
                                    list[j] = num11;
                                    break;
                                }
                            }
                        }
                    }
                }
                float num12 = 0f;
                for (int l = 0; l < 5; l++)
                {
                    num12 += (float)Math.Sqrt((double)list[l]);
                }
                num12 /= 5f;
                float num13 = Math.Max(0f, Math.Min(Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f, num12 - Campaign.MapDiagonal / 30f));
                float num14 = Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f;
                float num15 = num13 / num14;
                float num16 = Math.Min(0.7f, num10 * num6 * num7 * num8 * num9 * num4);
                return MBRandom.RoundRandomized((float)numberOfRegularMembers * num16);
            }
            return 0;
        }
    }
}
