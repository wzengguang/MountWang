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

            if (party.LeaderHero.GetPerkValue(DefaultPerks.Riding.Squires))
            {
                __result += 2;
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Riding.Conroi))
            {
                __result += 4;
            }

            if (party.LeaderHero.GetPerkValue(DefaultPerks.Bow.MerryMen))
            {
                __result += 3;
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Throwing.Skirmishers))
            {
                __result += 5;
            }
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Trade.Extra2))
            {
                __result += 15;
            }

            if (party.MapFaction.IsKingdomFaction && party.LeaderHero.IsFactionLeader && party.LeaderHero.GetPerkValue(DefaultPerks.Steward.SwordsAsTribute))
            {

                __result += 10;


            }
            //封地+5
            if (party.LeaderHero.GetPerkValue(DefaultPerks.Steward.ManAtArms) && party.LeaderHero.Clan.Settlements != null)
            {
                __result += 5 * party.LeaderHero.Clan.Settlements.Where(a => a.IsTown || a.IsCastle).Count();
            }
        }
    }
}
