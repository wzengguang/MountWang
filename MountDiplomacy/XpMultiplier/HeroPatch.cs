using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "CalculateLearningRate", new Type[] { typeof(Hero), typeof(SkillObject), typeof(StatExplainer) })]
    public class HeroXPRatePatch
    {
        public static void Postfix(ref float __result, Hero hero, SkillObject skill, StatExplainer explainer = null)
        {
            float plus = 1f;

            var setting = XPGlobalSetting.Instance;

            if (hero == Hero.MainHero)
            {
                plus += setting.PlayerXPMultiple;
            }
            else if (hero.Clan == Hero.MainHero.Clan)
            {
                plus += setting.CompanionXPMultiple;
            }

            plus += skill == DefaultSkills.Medicine ? setting.MedicineXPMultiple : 0;
            plus += skill == DefaultSkills.Roguery ? setting.RogueryXPMultiple : 0;
            plus += skill == DefaultSkills.Engineering ? setting.EngineeringXPMultiple : 0;
            __result *= plus;
        }
    }

    // [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class HeroPatch
    {
        public static void Prefix(Hero __instance, SkillObject skill, float xpAmount)
        {

            if (__instance.HeroDeveloper != null)
            {
                if (__instance.HeroDeveloper.Hero == Hero.MainHero)
                {
                    xpAmount = xpAmount * XPGlobalSetting.Instance.PlayerXPMultiple;
                }
                else if (__instance.HeroDeveloper.Hero.Clan == Hero.MainHero.Clan)
                {
                    xpAmount = xpAmount * XPGlobalSetting.Instance.CompanionXPMultiple;
                }

                //
                __instance.HeroDeveloper.AddSkillXp(skill, xpAmount);
            }
        }

    }
}
