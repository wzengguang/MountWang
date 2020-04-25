using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultPregnancyModel))]
    public class DefaultPregnancyModelPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("GetDailyChanceOfPregnancyForHero")]
        private static void GetDailyChanceOfPregnancyForHeroFix(Hero hero, ref float __result)
        {

            var isHeroAgeSuitableForPregnancy = hero.Age > 18f && hero.Age <= 45f;

            float result = 0f;
            if (hero.Spouse != null && hero.IsFertile && isHeroAgeSuitableForPregnancy)
            {
                ExplainedNumber bonuses = new ExplainedNumber(1f);
                PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.PerfectHealth, hero.Clan.Leader.CharacterObject, ref bonuses);
                result = (6.5f - (hero.Age - 18f) * 0.23f) * 0.95f * 0.01f * bonuses.ResultNumber * 0.5714286f;

                if (hero.Spouse == Hero.MainHero && hero.Children != null && hero.Children.Count > 0)
                {
                    var dif = CampaignTime.Now.ToDays - hero.Children.Max(a => a.BirthDay.ToDays);
                    if (dif < 36 * Math.Pow(2, hero.Children.Count))
                    {
                        __result = 0f;
                        return;
                    }
                }
            }
            __result = result;
        }

    }

}
