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
            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.CombatTips))
            {
                foreach (CharacterObject troop in __instance.MemberRoster.Troops)
                {
                    int troopPerksXp = (int)Math.Pow(__instance.MemberRoster.GetTroopCount(troop), 0.5) * Campaign.Current.Models.PartyTrainingModel.GetPerkExperiencesForTroops(DefaultPerks.Leadership.CombatTips);

                    __instance.Party.MemberRoster.AddXpToTroop(troopPerksXp, troop);
                }
            }
            else if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.RaiseTheMeek))
            {
                foreach (CharacterObject item in __instance.MemberRoster.Troops)
                {
                    if (item.Tier > 3)
                    {
                        continue;
                    }

                    int troopPerksXp2 = (int)Math.Pow(__instance.MemberRoster.GetTroopCount(item), 0.5) * Campaign.Current.Models.PartyTrainingModel.GetPerkExperiencesForTroops(DefaultPerks.Leadership.RaiseTheMeek);
                    __instance.Party.MemberRoster.AddXpToTroop(troopPerksXp2, item);
                }
            }

            return true;
        }

    }
}
