using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(MakePeaceKingdomDecision))]
    public class MakePeaceKingdomDecisionPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("DetermineSupport")]
        public static void DetermineSupport(MakePeaceKingdomDecision __instance, ref float __result, Clan clan, DecisionOutcome possibleOutcome)
        {
            var test1 = new PeaceBarterable(__instance.Kingdom, __instance.FactionToMakePeaceWith, CampaignTime.Years(1f)).GetValueForFaction(clan);

            //InformationManager.DisplayMessage(new InformationMessage(__instance.FactionToMakePeaceWith.Name.ToString() + ":" + test1.ToString()));

            var shouldPeaceBeDeclared = (bool)possibleOutcome.GetType().GetField("ShouldPeaceBeDeclared", BindingFlags.Instance | BindingFlags.Public).GetValue(possibleOutcome);

            if (DiplomacySetting.Instance.EnableMakePeaceStrategyPlus)
            {

                var atWars = (from x in clan.Kingdom.Stances
                              where x.IsAtWar && x.Faction1.IsKingdomFaction && x.Faction2.IsKingdomFaction
                              select x).ToArray<StanceLink>();

                //var plus = 2 * atWars.Length * (float)(new PeaceBarterable(__instance.Kingdom, __instance.FactionToMakePeaceWith, CampaignTime.Years(1f)).GetValueForFaction(clan)) * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence();

                var plus = 1f;

                var settlementsOccupyed = DiplomacySetting.GetFactionSettlementOccupyedByFaction(clan.MapFaction, __instance.FactionToMakePeaceWith).Sum(a => a.IsCastle ? 2 : 3);
                plus *= settlementsOccupyed == 0 ? 0f : (float)Math.Sqrt(settlementsOccupyed);

                StanceLink stanceWith = clan.MapFaction.GetStanceWith(__instance.FactionToMakePeaceWith);

                var toDays = stanceWith.WarStartDate.ElapsedDaysUntilNow;
                //兼容旧档
                if (toDays > 2000)
                {
                    Traverse.Create(stanceWith).Property("WarStartDate").SetValue(CampaignTime.Now);
                    toDays = stanceWith.WarStartDate.ElapsedDaysUntilNow;
                }
                var daysFactor = Math.Min(9, toDays < 20 ? 1 : toDays / 20f);
                var factor = Math.Max(0, daysFactor - plus);

                var clanMercy = clan.Leader.GetTraitLevel(DefaultTraits.Mercy) * 20;

                if (shouldPeaceBeDeclared)
                {
                    __result += Math.Abs(__result) / 10 * (factor) + clanMercy;
                }
                else
                {
                    //  __result -= Math.Abs(__result) / 10 * (resultFactor) - clanMercy;
                }
            }

            if (__instance.ProposerClan == Clan.PlayerClan && DiplomacySetting.Instance.RelationEffectOfMakePeaceDecision > 0 && __result >= 0)
            {
                var relation = CharacterRelationManager.GetHeroRelation(Hero.MainHero, clan.Leader);
                relation = relation > 0 ? relation : 0;

                if (shouldPeaceBeDeclared)
                {
                    __result += Math.Abs(__result) * DiplomacySetting.Instance.RelationEffectOfMakePeaceDecision * relation / 100f;
                }
                else
                {
                    __result -= Math.Abs(__result) * DiplomacySetting.Instance.RelationEffectOfMakePeaceDecision / 10 * relation / 100f;
                }
            }
        }

    }
}
