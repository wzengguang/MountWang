using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(PoliticalStagnationAndBorderIncidentCampaignBehavior))]
    public class PoliticalStagnationAndBorderIncidentCampaignBehaviorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ThinkAboutDeclaringWar")]
        private static bool ThinkAboutDeclaringWar(PoliticalStagnationAndBorderIncidentCampaignBehavior __instance, Kingdom kingdom)
        {
            List<IFaction> possibleKingdomsToDeclareWar = FactionHelper.GetPossibleKingdomsToDeclareWar(kingdom);
            var nears = Help.GetNearFactions(kingdom, Kingdom.All);
            var results = nears.Intersect(possibleKingdomsToDeclareWar).ToList();
            float num = 0f;
            IFaction faction = null;
            foreach (IFaction item in results)
            {
                //var str = $"{results.Count()}:{kingdom.Name}:{item.Name}:{num / 500000}{Help.CanDeclareWar(kingdom, item)}";
                //InformationManager.DisplayMessage(new InformationMessage(str));

                if (!Help.CanDeclareWar(kingdom, item, false, true))
                {
                    continue;
                }

                float scoreOfDeclaringWar = Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(kingdom, item);

                if (kingdom.Culture == item.Culture)
                {
                    scoreOfDeclaringWar *= 0.5f;
                }
                if (Help.CheckOwnSettlementOccupyedByFaction(item).Contains(item))
                {
                    scoreOfDeclaringWar *= 1.2f;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }

            if (faction != null && MBRandom.RandomFloat < Math.Min(0.3f, num / 500000f))
            {
                DeclareWarAction.ApplyDeclareWarOverProvocation(kingdom, faction);
            }
            return false;

        }
    }

}
