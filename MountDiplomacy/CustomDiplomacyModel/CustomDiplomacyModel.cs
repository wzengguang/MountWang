using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace Wang
{

    public class CustomDiplomacyModel : DefaultDiplomacyModel
    {



        //public override float GetScoreOfClanToJoinKingdom(Clan clan, Kingdom kingdom)
        //{
        //    if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
        //    {
        //        return -1E+08f;
        //    }
        //    int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
        //    float num = (float)Math.Min(2.0, Math.Max(0.5, 1.0 + Math.Sqrt(Math.Abs(relationBetweenClans)) * (double)((relationBetweenClans < 0) ? (-0.06f) : 0.04f)));
        //    float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 0.15f : (-0.15f));
        //    float num3 = clan.CalculateSettlementValue();
        //    float num4 = clan.CalculateSettlementValue(kingdom);
        //    int num5 = clan.CommanderHeroes.Count();
        //    float num6 = 0f;
        //    float num7 = 0f;
        //    if (!clan.IsMinorFaction)
        //    {
        //        float num8 = 0f;
        //        foreach (Settlement item in Settlement.All)
        //        {
        //            if (item.IsFortification && item.MapFaction == kingdom)
        //            {
        //                num8 += item.GetSettlementValueForFaction(kingdom);
        //            }
        //        }
        //        int num9 = 0;
        //        foreach (Clan clan2 in kingdom.Clans)
        //        {
        //            if (!clan2.IsMinorFaction || clan2 == Clan.PlayerClan)
        //            {
        //                num9 += clan2.CommanderHeroes.Count;
        //            }
        //        }
        //        num6 = num8 / (float)(num9 + num5);
        //        num7 = 0f - (float)(num9 * num9) * 50f;
        //    }
        //    return num6 * (float)Math.Sqrt(num5) * 0.3f * (num * num2) + (num4 - num3) + num7;
        //}

        /// <summary>
        /// 封地越多，越容易离开。
        /// 加入时间越长，越容易离开。原版时间因素100天保底。因子，相乘领主关系和文化一致性。
        /// 兵力越强，越不容易叛变
        /// 人品越差，城越多越容易叛变。反之，人品越好，城越多越不容易叛变。
        /// </summary>
        /// <param name="clan"></param>
        /// <param name="kingdom"></param>
        /// <returns></returns>
        public override float GetScoreOfClanToLeaveKingdom(Clan clan, Kingdom kingdom)
        {
            int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
            //0.5-2
            float num = (float)Math.Min(2.0, Math.Max(0.5, 1.0 + Math.Sqrt(Math.Abs(relationBetweenClans)) * (double)((relationBetweenClans < 0) ? (-0.06f) : 0.04f)));
            //1.15-0.85
            float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 0.15f : (-0.15f));

            //一个Town在10万左右，所有的Town
            float townValue = clan.CalculateSettlementValue();
            //国家城越多，该值越小。
            float num4 = clan.CalculateSettlementValue(kingdom);
            int num5 = clan.CommanderHeroes.Count();
            float num6 = 0f;
            if (!clan.IsMinorFaction)
            {
                float num7 = 0f;
                foreach (Town fortification in kingdom.Fortifications)
                {
                    num7 += fortification.Owner.Settlement.GetSettlementValueForFaction(kingdom);
                }
                int num8 = 0;
                foreach (Clan clan2 in kingdom.Clans)
                {
                    if (!clan2.IsMinorFaction || clan2 == Clan.PlayerClan)
                    {
                        num8 += clan2.CommanderHeroes.Count;
                    }
                }
                num6 = num7 / (float)(num8 + num5);
            }
            float num9 = (clan.TotalStrength + 150f * (float)num5) * 10f;
            float num10 = HeroHelper.CalculateReliabilityConstant(clan.Leader);
            double toDays = (CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
            //(0,20000)
            float num11 = 2000f * (float)(20.0 - Math.Sqrt(Math.Min(400.0, toDays)));
            int num12 = 40000 + ((clan.Fortifications != null) ? clan.Fortifications.Count() : 0) * 20000;

            return ((0f - num6) * (float)Math.Sqrt(num5) * 0.3f - (float)num12 * num10 + (0f - num9) * num10 + (0f - num11)) * (num * num2) + (townValue - num4) + (float)((kingdom.Ruler == Hero.MainHero) ? (-70000) : 0);
        }

        //public override float GetScoreOfKingdomToGetClan(Kingdom kingdom, Clan clan)
        //{
        //    float num = Math.Min(2f, Math.Max(0.33f, 1f + 0.02f * (float)FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan)));
        //    float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 1f : 0f);
        //    int num3 = (clan.CommanderHeroes != null) ? clan.CommanderHeroes.Count() : 0;
        //    float num4 = (clan.TotalStrength + 150f * (float)num3) * 10f;
        //    float powerRatioToEnemies = FactionHelper.GetPowerRatioToEnemies(kingdom);
        //    float num5 = HeroHelper.CalculateReliabilityConstant(clan.Leader);
        //    float num6 = 1f / Math.Max(0.4f, Math.Min(2.5f, (float)Math.Sqrt(powerRatioToEnemies)));
        //    num4 *= num6;
        //    return (clan.CalculateSettlementValue(kingdom) * 0.1f + num4) * num * num2 * num5;
        //}





    }

}
