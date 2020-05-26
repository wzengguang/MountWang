using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang
{

    [HarmonyPatch(typeof(KingdomDecisionProposalBehavior))]
    public class KingdomDecisionProposalBehaviorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetRandomWarDecision")]
        private static bool GetRandomWarDecision(Clan clan, ref KingdomDecision __result)
        {
            if (!DiplomacySetting.Instance.EnableSmartChoseFactionToDeclareWar)
            {
                return true;
            }

            __result = null;
            Kingdom kingdom = clan.Kingdom;
            if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is DeclareWarDecision) != null)
            {
                return false;
            }
            IFaction DeclaredWarFaction = KingdomDecisionProposalBehaviorHelp.GetRandomPossibleDeclarWar(kingdom);
            if (DeclaredWarFaction != null && KingdomDecisionProposalBehaviorHelp.ConsiderWar(clan, kingdom, DeclaredWarFaction))
            {
                //  InformationManager.DisplayMessage(new InformationMessage(kingdom.Name.ToString() + ":" + DeclaredWarFaction.Name.ToString()));
                __result = new DeclareWarDecision(clan, DeclaredWarFaction);
            }
            return false;
        }
    }


    public class KingdomDecisionProposalBehaviorHelp
    {

        public static IFaction GetRandomPossibleDeclarWar(Kingdom kingdom)
        {
            var atWars = Kingdom.All.Where(a => a != kingdom && a.IsAtWarWith(kingdom)).ToList();

            var strength = kingdom.MapFaction.TotalStrength;


            if (atWars.Count > 1)
            {
                return null;
            }

            if (atWars.Count > 0)
            {
                var avgStrength = atWars.Sum(a => a.MapFaction.TotalStrength) / atWars.Count;

                var avgSettlement = 1f * atWars.Sum(a => a.Settlements.Where(s => s.IsTown || s.IsCastle).Count()) / atWars.Count;

                if (strength < avgStrength * 1.2f)
                {
                    return null;
                }
            }

            var nears = Help.GetNearFactions(kingdom, Kingdom.All);
            float num = 0f;
            IFaction faction = null;
            List<IFaction> occupy = Help.CheckOwnSettlementOccupyedByFaction(kingdom);
            foreach (IFaction item in nears)
            {
                var atWars2 = Kingdom.All.Where(a => a != item && a.IsAtWarWith(item)).Count();

                if (atWars2 > 2)
                {
                    continue;
                }

                float scoreOfDeclaringWar = strength / item.MapFaction.TotalStrength;

                if (item.MapFaction.TotalStrength > strength * 1.2f && atWars2 > 0)
                {
                    scoreOfDeclaringWar = 1f;
                }

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

                    scoreOfDeclaringWar *= 2;
                }
                else
                {
                    if (Help.AtTruce(kingdom, item))
                    {
                        continue;
                    }
                }

                if (kingdom.Culture == item.Culture)
                {
                    var factor = Math.Max(0.4f, 1 - Kingdom.All.Where(a => a.Settlements != null && a.Settlements.Where(t => t.IsTown).Count() > 3 && (a.Culture != kingdom.Culture || a.Ruler == Hero.MainHero)).Count() * 0.15f);
                    scoreOfDeclaringWar *= factor;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }

            if (faction != null && MBRandom.RandomFloat < num * 0.2f)
            {
                if (occupy.Count > 0 && !occupy.Contains(faction))
                {
                    return null;
                }

                return faction;
            }
            return null;
        }


        public static bool ConsiderWar(Clan clan, Kingdom kingdom, IFaction otherFaction)
        {
            int num = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(kingdom) / 2;
            if (clan.Influence < (float)num)
            {
                return false;
            }
            DeclareWarDecision declareWarDecision = new DeclareWarDecision(clan, otherFaction);
            if (declareWarDecision.CalculateSupport(clan) > 50f)
            {
                float kingdomSupportForDecision = GetKingdomSupportForDecision(declareWarDecision);
                if ((double)MBRandom.RandomFloat < (double)kingdomSupportForDecision - 0.55)
                {
                    return true;
                }
            }
            return false;
        }

        private static float GetKingdomSupportForDecision(KingdomDecision decision)
        {
            return new KingdomElection(decision).GetLikelihoodForOutcome(0);
        }
    }
}
