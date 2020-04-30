using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(MobileParty), "DailyTick")]
    public static class MobilePartyPatch
    {
        /// <summary>
        ///  统御加经验技能
        /// </summary>
        private static bool Prefix(MobileParty __instance)
        {
            //2-3 474 3-4:719   4-5 1000 5-6 1300

            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.CombatTips))
            {
                foreach (CharacterObject troop in __instance.MemberRoster.Troops)
                {
                    int troopPerksXp = (int)Math.Pow(__instance.MemberRoster.GetTroopCount(troop), 0.5) * Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);

                    __instance.Party.MemberRoster.AddXpToTroop(troopPerksXp, troop);
                }
            }
            else if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.RaiseTheMeek))
            {
                foreach (CharacterObject item in __instance.MemberRoster.Troops.Where((CharacterObject x) => x.Tier < 4))
                {
                    int troopPerksXp2 = (int)Math.Pow(__instance.MemberRoster.GetTroopCount(item), 0.5) * Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                    __instance.Party.MemberRoster.AddXpToTroop(troopPerksXp2, item);
                }
            }

            __instance.RecentEventsMorale -= __instance.RecentEventsMorale * 0.1f;
            if (__instance.LeaderHero != null)
            {
                __instance.LeaderHero.PassedTimeAtHomeSettlement *= 0.9f;
            }
            if (__instance.IsActive && __instance.MapEvent == null && __instance != MobileParty.MainParty)
            {
                Campaign.Current.PartyUpgrader.UpgradeReadyTroops(__instance.Party);
            }

            return false;
        }

    }
}
