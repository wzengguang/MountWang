using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Wang.Setting;

namespace Wang.GameComponents
{
    public class WangSettlementMilitiaModel : DefaultSettlementMilitiaModel
    {
        private static readonly TextObject _baseText = new TextObject("{=qTFKXGSg}Base", null);

        private static readonly TextObject _fromHearthsText = new TextObject("{=cTmiNAlI}From Hearths", null);

        private static readonly TextObject _fromProsperityText = new TextObject("{=cTmiNAlI}From Prosperity", null);

        private static readonly TextObject _retiredText = new TextObject("{=gHnfFi1s}Retired", null);

        private static readonly TextObject _cultureText = new TextObject("{=PUjDWe5j}Culture", null);

        private static readonly TextObject _militiaFromMarketText = new TextObject("{=7ve3bQxg}Weapons From Market", null);

        private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues", null);

        private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);
        public override float CalculateMilitiaChange(Settlement settlement, StatExplainer explanation = null)
        {
            return CustomCalculateMilitiaChangeInternal(settlement, explanation);
        }



        private float CustomCalculateMilitiaChangeInternal(Settlement settlement, StatExplainer explanation = null)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation);
            if (settlement.Party.MapEvent == null)
            {
                var militia = settlement.Militia;
                if (settlement.IsFortification)
                {
                    explainedNumber.Add(1f, _baseText);
                }
                float value = (float)(-militia) * 0.033f * SettlementSetting.Instance.MillitiaRetireMultiple;
                explainedNumber.Add(value, _retiredText);
                if (settlement.IsVillage)
                {
                    float value2 = settlement.Village.Hearth / 250f;
                    explainedNumber.Add(value2, _fromHearthsText);
                }
                else if (settlement.IsFortification)
                {
                    #region MyRegion
                    //增加忠诚度对民兵影响
                    float value3 = settlement.Town.Loyalty * settlement.Town.Prosperity / (settlement.IsCastle ? 25000f : 50000f);
                    //添加文化因素
                    if (settlement.OwnerClan == null || settlement.OwnerClan.Culture != settlement.Town.Culture)
                    {
                        value3 /= 5f;
                    }

                    if (settlement.OwnerClan != null && settlement.OwnerClan.Culture == settlement.Town.Culture && settlement.Town.MilitiaParty != null && settlement.Town.MilitiaParty.Party.MemberRoster.TotalRegulars < 200)
                    {
                        value3 *= 2f;
                    }

                    if (settlement.IsStarving)
                    {
                        value3 /= 2f;
                    }

                    explainedNumber.Add(value3, _fromProsperityText);
                    #endregion
                }
                if (settlement.Culture.MilitiaBonus > 0)
                {
                    float value4 = settlement.Culture.MilitiaBonus;
                    if (Math.Abs(value4) > 0.01f)
                    {
                        explainedNumber.Add(value4, _cultureText);
                    }
                }
                if (settlement.IsTown)
                {
                    int num = settlement.Town.SoldItems.Sum(delegate (Town.SellLog x)
                    {
                        if (x.Category.Properties != ItemCategory.Property.BonusToMilitia)
                        {
                            return 0;
                        }
                        return x.Number;
                    });
                    if (num > 0)
                    {
                        explainedNumber.Add(1f * (float)num, WangSettlementMilitiaModel._militiaFromMarketText);
                    }
                    if (settlement.OwnerClan.Kingdom != null)
                    {
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom))
                        {
                            explainedNumber.Add(-1f, DefaultPolicies.Serfdom.Name);
                        }
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.GrazingRights))
                        {
                            explainedNumber.Add(1f, DefaultPolicies.GrazingRights.Name);
                        }
                        if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Cantons))
                        {
                            explainedNumber.Add(1f, DefaultPolicies.Cantons.Name);
                        }
                    }
                }
                if (settlement.IsCastle || settlement.IsTown)
                {
                    #region MyRegion
                    //修改民兵减少算法
                    if (settlement.Town.MilitiaParty != null && settlement.IsStarving && settlement.Town.Loyalty <= 50f)
                    {
                        float foodChange = settlement.Town.FoodChange;
                        int num = (settlement.Town.Owner.IsStarving && foodChange < 0f) ? (int)foodChange : 0;
                        num = (int)(num * 10f / (settlement.Town.Loyalty + 10f));
                        explainedNumber.Add(num, _foodShortageText);
                    }
                    #endregion

                    if (settlement.Town.CurrentBuilding.BuildingType == DefaultBuildingTypes.TrainMilitiaDaily)
                    {
                        AddDefaultDailyBonus(settlement.Town, ref explainedNumber);
                    }
                    foreach (Building building in settlement.Town.Buildings)
                    {
                        if (!building.BuildingType.IsDefaultProject)
                        {
                            float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Militia);
                            if (buildingEffectAmount > 0f)
                            {
                                explainedNumber.Add(buildingEffectAmount, building.Name);
                            }
                        }
                    }
                    if (settlement.IsCastle && settlement.Town.IsRebeling)
                    {
                        float resultNumber = explainedNumber.ResultNumber;
                        float num3 = 0f;
                        foreach (Building building2 in settlement.Town.Buildings)
                        {
                            if (num3 < 1f && (!building2.BuildingType.IsDefaultProject || settlement.Town.CurrentBuilding == building2))
                            {
                                float buildingEffectAmount2 = building2.GetBuildingEffectAmount(BuildingEffectEnum.ReduceMilitia);
                                if (buildingEffectAmount2 > 0f)
                                {
                                    float num4 = buildingEffectAmount2 * 0.01f;
                                    num3 += num4;
                                    if (num3 > 1f)
                                    {
                                        num4 -= num3 - 1f;
                                    }
                                    float value5 = resultNumber * -num4;
                                    explainedNumber.Add(value5, building2.Name);
                                }
                            }
                        }
                    }

                    GetSettlementMilitiaChangeDueToPolicies(settlement, ref explainedNumber);
                    GetSettlementMilitiaChangeDueToPerks(settlement, ref explainedNumber);
                    GetSettlementMilitiaChangeDueToIssues(settlement, ref explainedNumber);
                }
            }
            float num6 = explainedNumber.ResultNumber;
            if (settlement.Town != null && settlement.Town.IsRebeling)
            {
                num6 = num6 * (100f - settlement.Town.Security) * 0.01f;
            }


            return num6;
        }
        internal static void AddDefaultDailyBonus(Town fortification, ref ExplainedNumber result)
        {
            float value = fortification.Construction * (float)fortification.CurrentBuilding.BuildingType.Effects[0].Level1Effect * 0.01f;
            result.Add(value / 4, fortification.CurrentBuilding.BuildingType.Name);
        }

        public override void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate, out float meleeEliteTroopRate, out float rangedEliteTroopRate)
        {
            meleeTroopRate = 0.5f;
            rangedTroopRate = 1f - meleeTroopRate;
            rangedEliteTroopRate = 0.1f;
            meleeEliteTroopRate = 0.1f;
            if (settlement.IsTown && settlement.Town.Governor != null && settlement.Town.Governor.GetPerkValue(DefaultPerks.Leadership.CitizenMilitia) && settlement.Town.Governor.CurrentSettlement != null && settlement.Town.Governor.CurrentSettlement.IsTown && settlement.Town.Governor.CurrentSettlement.Town == settlement.Town)
            {
                rangedEliteTroopRate += DefaultPerks.Leadership.CitizenMilitia.PrimaryBonus / 100f;
                meleeEliteTroopRate += DefaultPerks.Leadership.CitizenMilitia.PrimaryBonus / 100f;

            }
            if (settlement.IsTown && settlement.Town.Governor != null)
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(0f, null);
                PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.StrongArms, settlement.Town, ref explainedNumber);
                rangedEliteTroopRate += explainedNumber.ResultNumber;
                meleeEliteTroopRate += explainedNumber.ResultNumber;
            }

            rangedEliteTroopRate *= SettlementSetting.Instance.EliteTroopRate;
            meleeEliteTroopRate *= SettlementSetting.Instance.EliteTroopRate;

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
            float value;
            if (IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementMilitia, settlement, out value))
            {
                result.Add(value, _issues);
            }
        }
    }
}
