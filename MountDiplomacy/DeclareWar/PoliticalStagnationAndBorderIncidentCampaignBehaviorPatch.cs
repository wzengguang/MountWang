using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang
{
    //[HarmonyPatch(typeof(PoliticalStagnationAndBorderIncidentCampaignBehavior))]
    public class PoliticalStagnationAndBorderIncidentCampaignBehaviorPatch
    {
        // [HarmonyPrefix]
        // [HarmonyPatch("ThinkAboutDeclaringWar")]
        private static bool ThinkAboutDeclaringWar(PoliticalStagnationAndBorderIncidentCampaignBehavior __instance, Kingdom kingdom)
        {
            var atWars = Kingdom.All.Where(a => a != kingdom && a.IsAtWarWith(kingdom)).ToList();

            if (atWars.Count > 1)
            {
                return false;
            }

            List<IFaction> possibleKingdomsToDeclareWar = FactionHelper.GetPossibleKingdomsToDeclareWar(kingdom);


            var nears = DiplomacySetting.GetNearFactionsWithFaction(kingdom, Kingdom.All);
            var results = nears.Intersect(possibleKingdomsToDeclareWar).ToList();
            float num = 0f;
            IFaction faction = null;
            List<IFaction> occupy = DiplomacySetting.GetAllFactionOccupyFactionSettlement(kingdom);
            foreach (IFaction item in results)
            {
                var factor1 = 1f;

                var atWars2 = Kingdom.All.Where(a => a != item && a.IsAtWarWith(item)).Count();
                if (occupy.Contains(item))
                {
                    if (DiplomacySetting.InTruce(kingdom, item))
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
                    if (atWars2 > 2)
                    {
                        continue;
                    }
                    if (DiplomacySetting.InTruce(kingdom, item))
                    {
                        continue;
                    }
                }

                //float scoreOfDeclaringWar = factor1 * Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(kingdom, item);
                float scoreOfDeclaringWar = factor1 * 1f;

                if (kingdom.Culture == item.Culture)
                {
                    var factor = Math.Max(0.1f, 1 - Kingdom.All.Where(a => a.MapFaction != null && a.MapFaction.Settlements.Where(t => t.IsTown).Count() > 3 && (a.Culture != kingdom.Culture || a.Ruler == Hero.MainHero)).Count() * 0.15f);
                    scoreOfDeclaringWar *= factor * 0.5f;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }

            if (faction != null && MBRandom.RandomFloat < 0.2f)
            {
                DeclareWarAction.ApplyDeclareWarOverProvocation(kingdom, faction);
            }
            return false;
        }
    }
}
