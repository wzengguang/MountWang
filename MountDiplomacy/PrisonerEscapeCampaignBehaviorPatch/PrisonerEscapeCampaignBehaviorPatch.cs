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
        private static bool Prefix(Hero hero)
        {
            if (hero.IsPrisoner && hero.PartyBelongedToAsPrisoner != null && hero != Hero.MainHero)
            {
                var time = (CampaignTime.Now - hero.CaptivityStartTime).ToDays;

                float num = Math.Min((float)(time * time / PrisonerEscapeConfig.BaseLine), 0.075f);

                if (hero.PartyBelongedToAsPrisoner.IsMobile)
                {
                    num *= 6f - (float)Math.Pow(Math.Min(81, hero.PartyBelongedToAsPrisoner.NumberOfHealthyMembers), 0.25);

                    if (hero.PartyBelongedToAsPrisoner.MapFaction.IsBanditFaction)
                    {
                        num *= 100f;
                    }
                }
                if (hero.PartyBelongedToAsPrisoner == PartyBase.MainParty || (hero.PartyBelongedToAsPrisoner.IsSettlement && hero.PartyBelongedToAsPrisoner.Settlement.OwnerClan == Clan.PlayerClan))
                {
                    num *= (hero.PartyBelongedToAsPrisoner.IsSettlement ? 0.5f : 0.33f);
                }

                if (MBRandom.RandomFloat < num)
                {
                    EndCaptivityAction.ApplyByEscape(hero, null);
                }
            }
            return false;
        }
    }
}
