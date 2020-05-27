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
            var atWars = Kingdom.All.Where(a => a != kingdom && a.IsAtWarWith(kingdom)).ToList();

            if (atWars.Count > 1)
            {
                return false;
            }

            List<IFaction> possibleKingdomsToDeclareWar = FactionHelper.GetPossibleKingdomsToDeclareWar(kingdom);


            var nears = Help.GetNearFactions(kingdom, Kingdom.All);
            var results = nears.Intersect(possibleKingdomsToDeclareWar).ToList();
            float num = 0f;
            IFaction faction = null;
            List<IFaction> occupy = Help.CheckOwnSettlementOccupyedByFaction(kingdom);
            foreach (IFaction item in results)
            {
                var factor1 = 1f;

                var atWars2 = Kingdom.All.Where(a => a != item && a.IsAtWarWith(item)).Count();
                if (occupy.Contains(item))
                {
                    if (Help.AtTruce(kingdom, item))
                    {
                        if (atWars2 < 1)
                        {
                            continue;
                        }
                        if (atWars2 < 2 && MBRandom.RandomFloat < 0.5f)
                        {
                            continue;
                        }
                    }

                    factor1 = 1.5f;
                }
                else
                {
                    if (Help.AtTruce(kingdom, item))
                    {
                        continue;
                    }
                }

                float scoreOfDeclaringWar = factor1 * Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(kingdom, item);

                if (kingdom.Culture == item.Culture)
                {
                    var factor = Math.Max(0.2f, 1 - Kingdom.All.Where(a => a.Settlements != null && a.Settlements.Where(t => t.IsTown).Count() > 3 && (a.Culture != kingdom.Culture || a.Ruler == Hero.MainHero)).Count() * 0.15f);
                    scoreOfDeclaringWar *= factor;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }

            if (faction != null && MBRandom.RandomFloat < Math.Min(0.3f, num / 400000f))
            {
                DeclareWarAction.ApplyDeclareWarOverProvocation(kingdom, faction);
            }
            return false;
        }
    }
}
