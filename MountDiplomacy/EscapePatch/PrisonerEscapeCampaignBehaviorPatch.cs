using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(PrisonerEscapeCampaignBehavior), "DailyHeroTick")]
    public class PrisonerEscapeCampaignBehaviorPatch
    {
        private static void Postfix(Hero hero)
        {
            if (hero.IsPrisoner && hero.PartyBelongedToAsPrisoner != null && hero != Hero.MainHero)
            {
                var time = (CampaignTime.Now - hero.CaptivityStartTime).ToDays;

                float num = (float)(time * time / PrisonerEscapeConfig.BaseLine);

                if (hero.PartyBelongedToAsPrisoner.IsMobile)
                {
                    num *= 6f - (float)Math.Pow(Math.Min(81, hero.PartyBelongedToAsPrisoner.NumberOfHealthyMembers), 0.25);

                    if (!hero.PartyBelongedToAsPrisoner.MapFaction.Culture.CanHaveSettlement || hero.PartyBelongedToAsPrisoner.MapFaction.IsBanditFaction)
                    {
                        num *= 100f;
                    }
                }
                else if (hero.PartyBelongedToAsPrisoner.Settlement != null && hero.PartyBelongedToAsPrisoner.Settlement.IsTown)
                {
                    num = num > 0.1f ? 0.1f : num;
                }

                if (hero.PartyBelongedToAsPrisoner == PartyBase.MainParty)
                {
                    num *= 0.33f;
                }
                if (MBRandom.RandomFloat < num)
                {
                    EndCaptivityAction.ApplyByEscape(hero);
                }
            }
        }
    }
}
