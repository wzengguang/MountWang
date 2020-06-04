using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace Wang.Perks
{
    [HarmonyPatch(typeof(DefaultPartySizeLimitModel), "CalculateMobilePartyMemberSizeLimit")]
    public class PartySizeRelatedPatch
    {

        private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation = null)
        {
            if (party.LeaderHero == null)
            {
                return;
            }
            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Riding.Squires))
            {
                explainedNumber.Add(DefaultPerks.Riding.Squires.PrimaryBonus, DefaultPerks.Riding.Squires.Name);
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Riding.Conroi))
            {
                explainedNumber.Add(DefaultPerks.Riding.Conroi.PrimaryBonus, DefaultPerks.Riding.Conroi.Name);
            }

            if (party.LeaderHero.GetPerkValue(DefaultPerks.Bow.MerryMen))
            {
                explainedNumber.Add(DefaultPerks.Bow.MerryMen.PrimaryBonus, DefaultPerks.Bow.MerryMen.Name);
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Throwing.Skirmishers))
            {
                explainedNumber.Add(DefaultPerks.Throwing.Skirmishers.PrimaryBonus, DefaultPerks.Throwing.Skirmishers.Name);
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Trade.Extra2))
            {
                explainedNumber.Add(DefaultPerks.Trade.Extra2.PrimaryBonus, DefaultPerks.Trade.Extra2.Name);
            }

            if (party.MapFaction.IsKingdomFaction && party.LeaderHero.IsFactionLeader && party.LeaderHero.GetPerkValue(DefaultPerks.Steward.SwordsAsTribute))
            {
                explainedNumber.Add(DefaultPerks.Steward.SwordsAsTribute.PrimaryBonus, DefaultPerks.Steward.SwordsAsTribute.Name);
            }
            //封地+5
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Steward.ManAtArms) && party.LeaderHero.Clan.Settlements != null)
            {
                var add = 5 * party.LeaderHero.Clan.Settlements.Where(a => a.IsTown || a.IsCastle).Count();
                if (add > 0)
                {
                    explainedNumber.Add(add, DefaultPerks.Steward.ManAtArms.Name);
                }
            }

            __result += (int)explainedNumber.ResultNumber;
        }
    }
}
