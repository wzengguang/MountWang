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

        private List<Hero> LearningSkillHero = new List<Hero>();
        private List<SkillObject> LearningSkills = new List<SkillObject>();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("LearningSkillHero", ref LearningSkillHero);
            dataStore.SyncData("LearningSkills", ref LearningSkills);
        }

        public void SetHeroLearningSkill(Hero hero, SkillObject skill)
        {
            var index = LearningSkillHero.FindIndex(a => a == hero);
            if (index > -1)
            {
                LearningSkills[index] = skill;
            }
            else
            {
                LearningSkillHero.Add(hero);
                LearningSkills.Add(skill);
            }


        }

        public SkillObject getHeroLearningSkill(Hero hero)
        {
            var index = LearningSkillHero.FindIndex(a => a == hero);
            if (index > -1)
            {
                return LearningSkills[index];
            }
            return null;
        }



        private void DailyTick()
        {
            if (LearningSkillHero.Count == 0 || XpMultiplierConfig.LearningXPMultipier < 1 || XpMultiplierConfig.LearningXPMultipier == 0)
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
                    var xp = companionSkillValue * XpMultiplierConfig.LearningXPMultipier * max[learningSkill] / (companionSkillValue / 2 + max[learningSkill]);

                    // hero.AddSkillXp(learningSkill, xp);
                    HeroPatch.Prefix(hero, learningSkill, xp);
                }
            }
        }
    }
}
