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
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultPregnancyModel))]
    public class DefaultPregnancyModelPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("GetDailyChanceOfPregnancyForHero")]
        private static void GetDailyChanceOfPregnancyForHeroFix(Hero hero, ref float __result)
        {
            if (!CommonSetting.Instance.LessChildren)
            {
                return;
            }

            if (
                hero.Children != null &&
                hero.Children.Count > 0 &&
                (CampaignTime.Now.ToDays - hero.Children.Max(a => a.BirthDay.ToDays)) < 36 * Math.Pow(2, hero.Children.Count))
            {
                __result = 0f;
            }
        }

    }

}
