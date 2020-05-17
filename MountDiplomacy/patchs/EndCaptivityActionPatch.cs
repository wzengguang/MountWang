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
    /// 战后释放领主，领主10天后才能复出
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

    }
}
