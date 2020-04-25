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
        private static void Prefix(MobileParty __instance)
        {
            //2-3 474 3-4:719   4-5 1000 5-6 1300

            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.CombatTips))
            {
                foreach (CharacterObject troop in __instance.MemberRoster.Troops)
                {
                    int troopPerksXp = Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.CombatTips) * XpMultiplierConfig.CombatTips;
                    __instance.Party.MemberRoster.AddXpToTroop(troopPerksXp, troop);
                }
            }
            else if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.RaiseTheMeek))
            {
                foreach (CharacterObject item in __instance.MemberRoster.Troops.Where((CharacterObject x) => x.Tier < 4))
                {
                    int troopPerksXp2 = Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek) * XpMultiplierConfig.RaiseTheMeek;
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
        }

        private static void AddXpToTroop(MobileParty __instance)
        {
            if (XpMultiplierConfig.AddTroopXpEnabled && __instance.IsActive && __instance.LeaderHero != null && !__instance.LeaderHero.MapFaction.IsBanditFaction && __instance != MobileParty.MainParty)
            {
                float[] ratio = XpMultiplierConfig.PartyTroopRatio;
                var xps = XpMultiplierConfig.TierXps;

                var total = 0f + __instance.MemberRoster.Where(a => !a.Character.IsHero).Select(a => a.Number).Sum();
                List<CharacterObject>[] troopOrder = new List<CharacterObject>[7] { new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>() };

                var troopsCount = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
                foreach (var troop in __instance.MemberRoster.Troops)
                {
                    if (troop.IsHero)
                    {
                        continue;
                    }

                    troopOrder[troop.Tier].Add(troop);
                    troopsCount[troop.Tier] += __instance.MemberRoster.GetTroopCount(troop);
                }

                for (int i = troopOrder.Length - 1; i > 0; i--)
                {
                    if (troopsCount[i] / total < ratio[i])
                    {
                        var scale = Math.Cos(1.47 * troopsCount[i] / (ratio[i] * total));

                        foreach (var troop in troopOrder[i - 1])
                        {
                            if (troop.UpgradeTargets != null && troop.UpgradeTargets.Length > 0)
                            {
                                var max = Math.Min(troop.UpgradeXpCost * 0.7f, scale * xps[i - 1]);

                                var xp = (int)(max * __instance.MemberRoster.GetTroopCount(troop));

                                __instance.Party.MemberRoster.AddXpToTroop(xp, troop);
                            }
                        }
                    }
                }
            }
        }
    }
}
