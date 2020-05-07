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

namespace Wang
{
    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "CalculateLearningRate", new Type[] { typeof(Hero), typeof(SkillObject), typeof(StatExplainer) })]
    public class HeroXPRatePatch
    {
        public static void Postfix(ref float __result, Hero hero, SkillObject skill, StatExplainer explainer = null)
        {
            if (hero == Hero.MainHero && XpMultiplierConfig.PlayerEnabled)
            {
                __result *= XpMultiplierConfig.PlayerMultipier;
            }
            else if (hero.Clan == Hero.MainHero.Clan && XpMultiplierConfig.PlayerEnabled)
            {
                __result *= XpMultiplierConfig.TeammateMultipier;
            }
        }
    }

    // [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class HeroPatch
    {
        public static void Prefix(Hero __instance, SkillObject skill, float xpAmount)
        {

            if (__instance.HeroDeveloper != null)
            {
                if (__instance.HeroDeveloper.Hero == Hero.MainHero && XpMultiplierConfig.PlayerEnabled)
                {
                    xpAmount = xpAmount * XpMultiplierConfig.PlayerMultipier;
                }
                else if (__instance.HeroDeveloper.Hero.Clan == Hero.MainHero.Clan && XpMultiplierConfig.PlayerEnabled)
                {
                    xpAmount = xpAmount * XpMultiplierConfig.TeammateMultipier;
                }

                //
                __instance.HeroDeveloper.AddSkillXp(skill, xpAmount);
            }
        }

    }
}
