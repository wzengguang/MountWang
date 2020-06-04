using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Wang.Saveable;
using Wang.Setting;

namespace Wang
{
    public class HeroLearningSkillBehaviour : CampaignBehaviorBase
    {
        private List<CompanionHeroSave> _companionHeroSaves = new List<CompanionHeroSave>();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
        }

        public void RefreshHeroFormationOnGameLoaded()
        {
            foreach (var item in _companionHeroSaves)
            {
                item.Hero.CharacterObject.CurrentFormationClass = (FormationClass)item.Formation;
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("WangCcompanionHeroSaves", ref _companionHeroSaves);
        }

        public void SetHeroFormation(Hero hero, int formation)
        {
            var find = _companionHeroSaves.FirstOrDefault(a => a.Hero == hero);
            if (find == null)
            {
                AddNew(hero, formation: formation);
            }
            else
            {
                find.Formation = formation;
            }
        }

        private void AddNew(Hero hero, SkillObject skill = null, int formation = 0, int perkTime = 0)
        {
            _companionHeroSaves.Add(new CompanionHeroSave
            {
                Hero = hero,
                SkillObject = skill,
                Formation = formation,
                WashPerkTime = perkTime
            });
        }

        public int GetWishPerkTime(Hero hero)
        {
            var find = _companionHeroSaves.FirstOrDefault(a => a.Hero == hero);
            if (find == null)
            {
                return 0;
            }
            return find.WashPerkTime;
        }

        public int SetWishPerkTime(Hero hero)
        {
            var find = _companionHeroSaves.FirstOrDefault(a => a.Hero == hero);
            if (find == null)
            {
                AddNew(hero, perkTime: 1);
                return 1;
            }
            return ++find.WashPerkTime;
        }
        public void SetHeroLearningSkill(Hero hero, SkillObject skill)
        {

            var find = _companionHeroSaves.FirstOrDefault(a => a.Hero == hero);
            if (find == null)
            {
                AddNew(hero, skill);
            }
            else
            {
                find.SkillObject = skill;
            }
        }

        public SkillObject getHeroLearningSkill(Hero hero)
        {
            var find = _companionHeroSaves.FirstOrDefault(a => a.Hero == hero);

            return find?.SkillObject;
        }

        private void DailyTick()
        {
            if (this._companionHeroSaves.Count == 0)
            {
                return;
            }

            Dictionary<SkillObject, int> max = new Dictionary<SkillObject, int>();

            var heroes = new HashSet<Hero>() { Hero.MainHero };
            foreach (var item in Hero.MainHero.CompanionsInParty)
            {
                heroes.Add(item);
            }
            foreach (var item in Hero.MainHero.Clan.Heroes)
            {
                if (item.PartyBelongedTo == MobileParty.MainParty)
                {
                    heroes.Add(item);
                }
            }

            foreach (var hero in heroes)
            {
                if (hero.IsWounded)
                {
                    continue;
                }
                foreach (var skill in DefaultSkills.GetAllSkills())
                {
                    var value = hero.GetSkillValue(skill);
                    if (max.ContainsKey(skill))
                    {
                        max[skill] = max[skill] > value ? max[skill] : value;
                    }
                    else
                    {
                        max.Add(skill, value);
                    }
                }
            }


            foreach (var hero in heroes)
            {
                if (hero.IsWounded)
                {
                    continue;
                }
                var learningSkill = getHeroLearningSkill(hero);
                if (learningSkill == null || !max.ContainsKey(learningSkill) || max[learningSkill] < 50)
                {
                    continue;
                }
                var companionSkillValue = hero.GetSkillValue(learningSkill);

                if (max[learningSkill] > (companionSkillValue + 3))
                {
                    //升到下一级的经验
                    var baseXp = 0.01f * (Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(companionSkillValue)
                        - Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(companionSkillValue - 1));

                    var xp = (1 + XPGlobalSetting.Instance.LearningXPMultipier) * baseXp * Math.Min(10, Math.Sqrt(max[learningSkill] - companionSkillValue));

                    xp /= (Math.Max(30, companionSkillValue) / 30);

                    //xp += companionSkillValue;
                    hero.AddSkillXp(learningSkill, (float)xp);
                }
            }
        }
    }
}
