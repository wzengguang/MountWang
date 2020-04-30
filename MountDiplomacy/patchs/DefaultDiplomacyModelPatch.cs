using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultDiplomacyModel))]
    public class DefaultDiplomacyModelPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("GetScoreOfClanToLeaveKingdom")]
        private static void GetScoreOfClanToLeaveKingdom(DefaultDiplomacyModel __instance, Clan clan, Kingdom kingdom, ref float __result)
        {
            int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
            //0.5-2
            float num = (float)Math.Min(2.0, Math.Max(0.5, 1.0 + Math.Sqrt(Math.Abs(relationBetweenClans)) * (double)((relationBetweenClans < 0) ? (-0.06f) : 0.04f)));
            //1.15-0.85
            float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 0.15f : (-0.15f));

            double toDays = (CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
            //(0,20000)
            float num11;
            if (kingdom.Ruler != Hero.MainHero)
            {
                num11 = 2000f * (float)(100.0 - Math.Sqrt(Math.Min(1000.0, toDays)));
            }
            else
            {
                num11 = 2000f * (float)(20.0 - Math.Sqrt(Math.Min(400.0, toDays)));
            }

            int num12 = 40000 + ((clan.Fortifications != null) ? clan.Fortifications.Count() : 0) * 20000;

            __result += (0f - num11) * (num * num2);

        }


    }

}
