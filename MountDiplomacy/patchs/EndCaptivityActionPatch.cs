using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Wang.patchs
{
    /// <summary>
    /// 战后释放领主，领主10天后才能复出，复出前，钱少，家族内给其钱。
    /// </summary>
    [HarmonyPatch]
    class EndCaptivityActionPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndCaptivityAction), "ApplyInternal")]
        private static void Postfix(Hero prisoner, EndCaptivityDetail detail, Hero facilitatior = null)
        {
            if (prisoner != Hero.MainHero && !prisoner.IsPlayerCompanion && detail == EndCaptivityDetail.ReleasedAfterBattle)
            {
                prisoner.DaysLeftToRespawn = Settings.PrisonerDaysLeftToRespawn;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HeroSpawnCampaignBehavior), "SpawnLord")]
        private static bool Prefix(Hero hero, bool spawnAsParty)
        {
            var clanGold = hero.Clan.Heroes.Sum(a => a.Gold);
            var average = clanGold / hero.Clan.Heroes.Count();
            if (spawnAsParty && hero.Gold < average / 2 && hero.Clan.Heroes.Count() > 1)
            {
                foreach (var clanHero in hero.Clan.Heroes.OrderByDescending(a => a.Gold))
                {
                    if (hero != clanHero)
                    {
                        var give = Math.Min(average, clanHero.Gold > average ? clanHero.Gold - average : 0);

                        if (give > 0)
                        {
                            GiveGoldAction.ApplyBetweenCharacters(clanHero, hero, give);
                        }
                    }
                    if (hero.Gold >= average)
                    {
                        break;
                    }
                }
            }

            if (spawnAsParty && hero.Gold < 100)
            {
                return false;
            }

            return true;
        }
    }
}
