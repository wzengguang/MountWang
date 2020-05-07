using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Wang
{
    public class HeroLearningSkillBehaviour : CampaignBehaviorBase
    {

        private List<Hero> heroes = new List<Hero>();
        private List<SkillObject> LearningSkills = new List<SkillObject>();
        private List<int> washPerkTime = new List<int>();

        private List<int> heroFormation = new List<int>();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
        }

        public void RefreshHeroFormationOnGameLoaded()
        {
            if (heroFormation.Count < heroes.Count)
            {
                heroFormation.AddRange(new int[heroes.Count - heroFormation.Count]);
            }

            for (int i = 0; i < heroes.Count; i++)
            {
                heroes[i].CharacterObject.CurrentFormationClass = (FormationClass)heroFormation[i];
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("LearningSkillHero", ref heroes);
            dataStore.SyncData("LearningSkills", ref LearningSkills);
            dataStore.SyncData("washPerkTime", ref washPerkTime);
            dataStore.SyncData("heroFormation", ref heroFormation);
        }

        public void SetHeroFormation(Hero hero, int formation)
        {
            if (heroFormation.Count < heroes.Count)
            {
                heroFormation.AddRange(new int[heroes.Count - heroFormation.Count]);
            }
            var index = heroes.FindIndex(a => a == hero);
            if (index > -1)
            {
                heroFormation[index] = formation;
            }

            AddNew(hero, formation: formation);
        }

        private void AddNew(Hero hero, SkillObject skill = null, int formation = 0, int perkTime = 0)
        {
            heroes.Add(hero);
            LearningSkills.Add(null);
            washPerkTime.Add(perkTime);
            heroFormation.Add(formation);
        }

        public int GetWishPerkTime(Hero hero)
        {
            //兼容旧档？
            if (washPerkTime.Count < heroes.Count)
            {
                washPerkTime.AddRange(new int[heroes.Count - washPerkTime.Count]);
            }

            var index = heroes.FindIndex(a => a == hero);
            if (index > -1)
            {
                return washPerkTime[index];
            }
            return 0;
        }

        public int SetWishPerkTime(Hero hero)
        {
            var index = heroes.FindIndex(a => a == hero);
            if (index > -1)
            {
                washPerkTime[index]++;
                return washPerkTime[index];
            }

            AddNew(hero, perkTime: 1);
            return 1;
        }
        public void SetHeroLearningSkill(Hero hero, SkillObject skill)
        {
            var index = heroes.FindIndex(a => a == hero);
            if (index > -1)
            {
                LearningSkills[index] = skill;
            }
            else
            {
                AddNew(hero, skill);
            }
        }

        public SkillObject getHeroLearningSkill(Hero hero)
        {
            var index = heroes.FindIndex(a => a == hero);
            if (index > -1)
            {
                return LearningSkills[index];
            }
            return null;
        }


        private void DailyTick()
        {
            if (this.heroes.Count == 0 || XpMultiplierConfig.LearningXPMultipier < 1 || XpMultiplierConfig.LearningXPMultipier == 0)
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

                    var xp = XpMultiplierConfig.LearningXPMultipier * baseXp * Math.Min(10, Math.Sqrt(max[learningSkill] - companionSkillValue));

                    xp /= (Math.Max(30, companionSkillValue) / 30);

                    //InformationManager.DisplayMessage(new InformationMessage(baseXp.ToString() + ":" + xp.ToString()));

                    if (learningSkill == DefaultSkills.Trade)
                    {
                        xp *= 10;
                    }

                    //xp += companionSkillValue;
                    hero.AddSkillXp(learningSkill, (float)xp);
                    //HeroPatch.Prefix(hero, learningSkill, (float)xp);
                }
            }
        }
    }
}
