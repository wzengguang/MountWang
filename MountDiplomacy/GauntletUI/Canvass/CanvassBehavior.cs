using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using Wang.Saveable;

namespace Wang.GauntletUI.Canvass
{
    public class CanvassBehavior : CampaignBehaviorBase
    {

        private List<CanvassSave> _canvassSaves = new List<CanvassSave>();

        public CanvassBehavior()
        {

        }

        public override void RegisterEvents()
        {
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, WeeklyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("WangCanvassSave", ref _canvassSaves);
        }


        private void WeeklyTick()
        {
            foreach (var item in _canvassSaves)
            {
                if (item.IsCurrent())
                {
                    var daysToNow = (int)CampaignTime.DaysFromNow(item.DayTime).ToDays;

                    var hero = item.Hero;
                    var clan = item.Clan;

                    if (hero != null && clan != null && clan.Leader != null && daysToNow > 0)
                    {
                        ApplyAddRelation(hero, clan, daysToNow);
                        CalculateBonus(clan, item);
                    }
                    else if (clan != null)
                    {
                        CalculateBonus(clan, item);
                    }
                }
            }
        }

        private void CalculateBonus(Clan clan, CanvassSave save)
        {
            if (clan.IsKingdomFaction)
            {
                return;
            }

            var daysToNow = (int)CampaignTime.DaysFromNow(save.DayTime).ToDays;

            if (save.IsCurrent())
            {
                save.Bonus = Math.Min(1, save.Bonus + daysToNow / 30f);
            }
            else
            {
                save.Bonus = Math.Max(0, save.Bonus - daysToNow / 30f);
            }

        }


        private void ApplyAddRelation(Hero hero, Clan clan, int daysToNow)
        {
            var relation = GetExpectRelation(hero, clan, daysToNow);
            var cost = GetExpectGoldCostOfRelation(clan, relation);

            if (Hero.MainHero.Gold > cost * 1.2 && CharacterRelationManager.GetHeroRelation(Hero.MainHero, clan.Leader) < 100)
            {
                ChangeRelationAction.ApplyPlayerRelation(clan.Leader, GetExpectRelation(hero, clan, daysToNow, false));
                GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, clan.Leader, cost);
            }

        }

        public static int GetExpectGoldCostOfRelation(Clan clan, int relation)
        {
            var now = CharacterRelationManager.GetHeroRelation(Hero.MainHero, clan.Leader);
            var end = (now + relation) > 100 ? 100 : now + relation;

            var cost = (end < 0 ? -1 : 1) * end * end * 10 - (now < 0 ? -1 : 1) * now * now * 10;

            return cost;
        }

        public static int GetExpectRelation(Hero hero, Clan clan, int daysToNow, bool hasFactor = true)
        {
            var bonus = GetTraitBonus(hero, clan.Leader) + GetTraitBonus(Hero.MainHero, clan.Leader);

            var addRelation = bonus * 10 * (daysToNow > 7 ? 1 : daysToNow / 7f);

            if (!hasFactor || addRelation == 0)
            {
                return (int)addRelation;
            }

            ExplainedNumber explainedNumber = new ExplainedNumber((float)addRelation, new StatExplainer(), null);
            Campaign.Current.Models.DiplomacyModel.GetRelationIncreaseFactor(Hero.MainHero, clan.Leader, ref explainedNumber);
            addRelation = MBRandom.RoundRandomized(explainedNumber.ResultNumber);
            return (int)addRelation;
        }

        private static float GetTraitBonus(Hero hero, Hero clanLeader)
        {
            var bonus = 0f;
            var mercy = hero.GetTraitLevel(DefaultTraits.Mercy) * clanLeader.GetTraitLevel(DefaultTraits.Mercy);
            bonus += mercy > 0 ? 1 : (mercy == 0 ? 0 : -1);

            var honor = hero.GetTraitLevel(DefaultTraits.Honor) * clanLeader.GetTraitLevel(DefaultTraits.Honor);
            bonus += honor > 0 ? 1 : (honor == 0 ? 0 : -1);

            var valor = hero.GetTraitLevel(DefaultTraits.Valor) * clanLeader.GetTraitLevel(DefaultTraits.Valor);
            bonus += valor > 0 ? 1 : (valor == 0 ? 0 : -1);

            var generosity = hero.GetTraitLevel(DefaultTraits.Generosity) * clanLeader.GetTraitLevel(DefaultTraits.Generosity);
            bonus += generosity > 0 ? 1 : (generosity == 0 ? 0 : -1);

            var cal = hero.GetTraitLevel(DefaultTraits.Calculating) * clanLeader.GetTraitLevel(DefaultTraits.Calculating);
            bonus += cal > 0 ? 1 : (cal == 0 ? 0 : -1);

            var charm = Hero.MainHero.GetSkillValue(DefaultSkills.Charm);

            return Math.Max(0, bonus * 0.2f * charm / 100);
        }

        public Hero GetCurrent(Clan clan)
        {
            var find = _canvassSaves.FirstOrDefault(a => a.Hero != null);

            if (find != null)
            {
                var findClan = Clan.All.FirstOrDefault(a => a.Id.GetHashCode() == find.ClanId);

                if (findClan != clan)
                {
                    return null;
                }

                var hero = Clan.PlayerClan.Companions.FirstOrDefault(a => a.Id.GetHashCode() == find.HeroId);
                return hero;
            }
            return null;
        }

        public Clan GetCurrentClan()
        {
            var find = _canvassSaves.FirstOrDefault(a => a.Hero != null);

            if (find != null)
            {
                return find.Clan;
            }
            return null;
        }


        public float GetCanvassBonus(Clan clan)
        {
            var find = _canvassSaves.FirstOrDefault(a => a.ClanId == clan.Id.GetHashCode());
            if (find != null)
            {
                return find.Bonus;
            }
            return 0;
        }

        public void UpdateCanvass(Hero hero, Clan clan)
        {
            var find = _canvassSaves.FirstOrDefault(a => a.Hero != null);
            if (find != null)
            {
                find.Hero = null;
                find.DayTime = (float)CampaignTime.Now.ToDays;
            }

            var heroId = hero?.Id.GetHashCode().ToString();
            var clanId = clan.Id.GetHashCode().ToString();

            find = _canvassSaves.FirstOrDefault(a => a.ClanId == clan.Id.GetHashCode());
            if (find != null)
            {
                find.Hero = hero;
                find.DayTime = (float)CampaignTime.Now.ToDays;
            }
            else
            {
                _canvassSaves.Add(new CanvassSave
                {
                    Hero = hero,
                    Clan = clan,
                    DayTime = (float)CampaignTime.Now.ToDays,
                    Bonus = 0
                }); ;
            }

        }
    }
}
