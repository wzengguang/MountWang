using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Wang.Setting;

namespace Wang.GameComponents
{
    public class WangSettlementGarrisonModel : SettlementGarrisonModel
    {

        private static readonly TextObject _townWallsText = new TextObject("{=SlmhqqH8}Town Walls", null);

        private static readonly TextObject _moraleText = new TextObject("{=UjL7jVYF}Morale", null);

        private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

        private static readonly TextObject _surplusFoodText = GameTexts.FindText("str_surplus_food", null);

        private static readonly TextObject _recruitFromCenterNotablesText = GameTexts.FindText("str_center_notables", null);

        private static readonly TextObject _recruitFromVillageNotablesText = GameTexts.FindText("str_village_notables", null);

        private static readonly TextObject _villageBeingRaided = GameTexts.FindText("str_village_being_raided", null);

        private static readonly TextObject _villageLooted = GameTexts.FindText("str_village_looted", null);

        private static readonly TextObject _townIsUnderSiege = GameTexts.FindText("str_villages_under_siege", null);

        private static readonly TextObject _retiredText = GameTexts.FindText("str_retired", null);

        private static readonly TextObject _paymentIsLess = GameTexts.FindText("str_payment_is_less", null);

        private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues", null);

        public override int CalculateGarrisonChange(Settlement settlement, StatExplainer explanation = null)
        {
            return WangSettlementGarrisonModel.CalculateGarrisonChangeInternal(settlement, explanation);
        }

        private static int CalculateGarrisonChangeInternal(Settlement settlement, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            if (settlement.IsTown || settlement.IsCastle)
            {
                float loyalty = settlement.Town.Loyalty;
                if (settlement.IsStarving)
                {
                    float foodChange = settlement.Town.FoodChange;
                    int num = (settlement.Town.Owner.IsStarving && foodChange < -19f) ? ((int)((foodChange + 10f) / 10f)) : 0;
                    explainedNumber.Add((float)num, WangSettlementGarrisonModel._foodShortageText, null);
                }
                if (settlement.Town.GarrisonParty != null && ((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit > settlement.Town.GarrisonParty.PaymentRatio)
                {
                    int num2 = 0;
                    do
                    {
                        num2++;
                    }
                    while (((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num2) / (float)settlement.Town.GarrisonParty.Party.PartySizeLimit >= settlement.Town.GarrisonParty.PaymentRatio && (float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + explainedNumber.ResultNumber - (float)num2 > 0f && num2 < 20);
                    explainedNumber.Add((float)(-(float)num2), WangSettlementGarrisonModel._paymentIsLess, null);
                }
            }
            WangSettlementGarrisonModel.GetSettlementGarrisonChangeDueToIssues(settlement, ref explainedNumber);
            return (int)explainedNumber.ResultNumber;
        }

        private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            float value;
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, out value))
            {
                result.Add(value, WangSettlementGarrisonModel._issues, null);
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
            if (SettlementGarrisonSetting.Instance.DisablePlayerClanLeaveTroopToGarrison && mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan == Clan.PlayerClan)
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
            float num4 = FactionHelper.SettlementProsperityEffectOnGarrisonSizeConstant(settlement);
            num2 *= num3;
            num2 *= num4;
            if ((settlement.OwnerClan.Leader != Hero.MainHero || (mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan == Clan.PlayerClan)) && num < num2)
            {
                int numberOfRegularMembers = mobileParty.Party.NumberOfRegularMembers;
                float num5 = 1f + (float)mobileParty.Party.NumberOfWoundedRegularMembers / (float)mobileParty.Party.NumberOfRegularMembers;
                float num6 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
                float num7 = (float)(Math.Pow((double)Math.Min(2f, (float)numberOfRegularMembers / num6), 1.2000000476837158) * 0.75);
                float num8 = (1f - num / num2) * (1f - num / num2);
                if (mobileParty.Army != null)
                {
                    num8 = Math.Min(num8, 0.5f);
                }
                float num9 = 0.5f;
                if (settlement.OwnerClan == mobileParty.Leader.HeroObject.Clan || settlement.OwnerClan == mobileParty.Party.Owner.MapFaction.Leader.Clan)
                {
                    num9 = 1f;
                }
                float num10 = (mobileParty.Army != null) ? 1.25f : 1f;
                float num11 = 1f;
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
                            float num12 = settlement2.Position2D.DistanceSquared(mobileParty.Position2D);
                            for (int j = 0; j < 5; j++)
                            {
                                if (num12 < list[j])
                                {
                                    for (int k = 4; k >= j + 1; k--)
                                    {
                                        list[k] = list[k - 1];
                                    }
                                    list[j] = num12;
                                    break;
                                }
                            }
                        }
                    }
                }
                float num13 = 0f;
                for (int l = 0; l < 5; l++)
                {
                    num13 += (float)Math.Sqrt((double)list[l]);
                }
                num13 /= 5f;
                float num14 = Math.Max(0f, Math.Min(Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f, num13 - Campaign.MapDiagonal / 30f));
                float num15 = Campaign.MapDiagonal / 15f - Campaign.MapDiagonal / 30f;
                float num16 = num14 / num15;
                float num17 = Math.Min(0.7f, num11 * num7 * num8 * num9 * num10 * num5);
                return MBRandom.RoundRandomized((float)numberOfRegularMembers * num17);
            }
            return 0;
        }
    }


}
