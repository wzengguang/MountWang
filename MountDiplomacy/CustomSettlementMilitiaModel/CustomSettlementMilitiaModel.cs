using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Wang
{
    public class CustomSettlementMilitiaModel : DefaultSettlementMilitiaModel
    {
        private static readonly TextObject _baseText = new TextObject("{=qTFKXGSg}Base");

        private static readonly TextObject _fromHearthsText = new TextObject("{=cTmiNAlI}From Hearths");

        private static readonly TextObject _fromProsperityText = new TextObject("{=cTmiNAlI}From Prosperity");

        private static readonly TextObject _retiredText = new TextObject("{=gHnfFi1s}Retired");

        private static readonly TextObject _cultureText = new TextObject("{=PUjDWe5j}Culture");

        private static readonly TextObject _militiaFromMarketText = new TextObject("{=7ve3bQxg}Weapons From Market");

        private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues");

        private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage");
        private readonly TextObject _loyaltyText = GameTexts.FindText("str_loyalty");
        public override float CalculateMilitiaChange(Settlement settlement, StatExplainer explanation = null)
        {
            return CustomCalculateMilitiaChangeInternal(settlement, explanation);
        }


        private float CustomCalculateMilitiaChangeInternal(Settlement settlement, StatExplainer explanation = null)
        {
            ExplainedNumber result = new ExplainedNumber(0f, explanation);
            if (settlement.Party.MapEvent == null)
            {
                int militia = settlement.Militia;
                if (settlement.IsFortification)
                {
                    result.Add(1f, _baseText);
                }
                float value = (float)(-militia) * 0.05f;
                result.Add(value, _retiredText);
                if (settlement.IsVillage)
                {
                    float value2 = settlement.Village.Hearth / 250f;
                    result.Add(value2, _fromHearthsText);
                }
                else if (settlement.IsFortification)
                {
                    //增加忠诚度对民兵影响
                    float value3 = settlement.Town.Loyalty * settlement.Town.Prosperity / (settlement.IsCastle ? 15000f : 40000f);
                    //添加文化因素
                    if (settlement.Town.Governor == null || settlement.Town.Governor.Culture != settlement.Town.Culture)
                    {
                        value3 /= 10f;
                    }

                    if (settlement.Town.Governor != null && settlement.Town.Governor.Culture == settlement.Town.Culture && settlement.Town.MilitiaParty != null && settlement.Town.MilitiaParty.Party.MemberRoster.TotalRegulars < 200)
                    {
                        value3 *= 2f;
                    }

                    if (settlement.IsStarving)
                    {
                        value3 /= 2f;
                    }

                    result.Add(value3, _fromProsperityText);

                }
                if (settlement.Culture.MilitiaBonus > 0)
                {
                    float value4 = settlement.Culture.MilitiaBonus;
                    if (Math.Abs(value4) > 0.01f)
                    {
                        result.Add(value4, _cultureText);
                    }
                }
                if (settlement.IsTown)
                {
                    int num = settlement.Town.SoldItems.Sum((Town.SellLog x) => (x.Category.Properties == ItemCategory.Property.BonusToMilitia) ? x.Number : 0);
                    if (num > 0)
                    {
                        //乘5倍
                        result.Add(1f * (float)num, _militiaFromMarketText);
                    }
                    if (settlement.OwnerClan.Kingdom != null)
                    {
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom))
                        {
                            result.Add(-1f, DefaultPolicies.Serfdom.Name);
                        }
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
                        {
                            result.Add(settlement.Notables.Count, DefaultPolicies.CouncilOfTheCommons.Name);
                        }
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.GrazingRights))
                        {
                            result.Add(1f, DefaultPolicies.GrazingRights.Name);
                        }
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Cantons))
                        {
                            result.Add(1f, DefaultPolicies.Cantons.Name);
                        }
                    }
                }
                if (settlement.IsCastle || settlement.IsTown)
                {
                    //if (settlement.IsStarving)
                    //{
                    //    float foodChange = settlement.Town.FoodChange;
                    //    int num2 = (settlement.IsTown && settlement.Town.Owner.IsStarving && foodChange < 0f) ? ((int)(foodChange / 4f)) : 0;
                    //    result.Add(num2, _foodShortageText);
                    //}


                    //修改民兵减少算法
                    if (settlement.Town.MilitiaParty != null && settlement.IsStarving && settlement.Town.Loyalty <= 50f)
                    {
                        float foodChange = settlement.Town.FoodChange;
                        int num = (settlement.Town.Owner.IsStarving && foodChange < 0f) ? (int)foodChange : 0;
                        num = (int)(num * 10f / (settlement.Town.Loyalty + 10f));
                        result.Add(num, _foodShortageText);
                    }


                    foreach (Building building in settlement.Town.Buildings)
                    {
                        if (!building.BuildingType.IsDefaultProject || settlement.Town.CurrentBuilding == building)
                        {
                            float num3 = building.GetBuildingEffectAmount(DefaultBuildingEffects.Militia);
                            if (num3 > 0f)
                            {
                                result.Add(num3, building.Name);
                            }
                        }
                    }
                    if (settlement.IsCastle && settlement.Town.IsRebeling)
                    {
                        float resultNumber = result.ResultNumber;
                        float num4 = 0f;
                        foreach (Building building2 in settlement.Town.Buildings)
                        {
                            if (num4 < 1f && (!building2.BuildingType.IsDefaultProject || settlement.Town.CurrentBuilding == building2))
                            {
                                int buildingEffectAmount = building2.GetBuildingEffectAmount(DefaultBuildingEffects.ReduceMilitia);
                                if (buildingEffectAmount > 0)
                                {
                                    float num5 = (float)buildingEffectAmount * 0.01f;
                                    num4 += num5;
                                    if (num4 > 1f)
                                    {
                                        num5 -= num4 - 1f;
                                    }
                                    float value5 = resultNumber * (0f - num5);
                                    result.Add(value5, building2.Name);
                                }
                            }
                        }
                    }
                    GetSettlementMilitiaChangeDueToPolicies(settlement, ref result);
                    GetSettlementMilitiaChangeDueToPerks(settlement, ref result);
                    GetSettlementMilitiaChangeDueToIssues(settlement, ref result);
                }
            }
            float num6 = result.ResultNumber;
            if (settlement.Town != null && settlement.Town.IsRebeling)
            {
                num6 = num6 * (100f - settlement.Town.Security) * 0.01f;
            }
            return num6;
        }


        public override void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate, out float meleeEliteTroopRate, out float rangedEliteTroopRate)
        {
            meleeTroopRate = 0.5f;
            rangedTroopRate = 1f - meleeTroopRate;
            rangedEliteTroopRate = 0.1f;
            meleeEliteTroopRate = 0.1f;
            if (settlement.IsTown && settlement.Town.Governor != null && settlement.Town.Governor.GetPerkValue(DefaultPerks.Leadership.CitizenMilitia) && settlement.Town.Governor.CurrentSettlement != null && settlement.Town.Governor.CurrentSettlement.IsTown && settlement.Town.Governor.CurrentSettlement.Town == settlement.Town)
            {
                //PrimaryBonus 默认20   PrimaryBonus / 100f
                rangedEliteTroopRate += DefaultPerks.Leadership.CitizenMilitia.PrimaryBonus / SettlementMillitiaConfig.EliteTroopRate;
                meleeEliteTroopRate += DefaultPerks.Leadership.CitizenMilitia.PrimaryBonus / SettlementMillitiaConfig.EliteTroopRate;
            }
        }

        private static void GetSettlementMilitiaChangeDueToPerks(Settlement settlement, ref ExplainedNumber result)
        {
        }

        private static void GetSettlementMilitiaChangeDueToPolicies(Settlement settlement, ref ExplainedNumber result)
        {
            Kingdom kingdom = settlement.OwnerClan.Kingdom;
            if (kingdom != null && kingdom.ActivePolicies.Contains(DefaultPolicies.Citizenship))
            {
                result.Add(1f, DefaultPolicies.Citizenship.Name);
            }
        }

        private static void GetSettlementMilitiaChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
        {
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementMilitia, settlement, out float totalChange))
            {
                result.Add(totalChange, _issues);
            }
        }
    }
}
