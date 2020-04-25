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
    [HarmonyPatch(typeof(PoliticalStagnationAndBorderIncidentCampaignBehavior))]
    public class PoliticalStagnationAndBorderIncidentCampaignBehaviorPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ThinkAboutDeclaringWar")]
        private static void Prefix(PoliticalStagnationAndBorderIncidentCampaignBehavior __instance, Kingdom kingdom)
        {
            List<IFaction> possibleKingdomsToDeclareWar = FactionHelper.GetPossibleKingdomsToDeclareWar(kingdom);

            var nears = Help.GetNearFactions(kingdom, Kingdom.All);

            var results = nears.Intersect(possibleKingdomsToDeclareWar).ToList();

            float num = 0f;
            IFaction faction = null;
            foreach (IFaction item in results)
            {
                float scoreOfDeclaringWar = Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(kingdom, item);

                if (kingdom.Culture == item.Culture)
                {
                    scoreOfDeclaringWar *= 0.5f;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }
            if (faction != null && MBRandom.RandomFloat < Math.Min(0.35f, num / 100000f) && Help.CanDeclareWar(kingdom, faction))
            {
                DeclareWarAction.ApplyDeclareWarOverProvocation(kingdom, faction);
            }

        }
    }

}
