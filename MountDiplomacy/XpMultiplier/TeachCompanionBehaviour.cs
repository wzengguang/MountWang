using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Wang
{
    public class TeachCompanionBehaviour : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, TeachCompanion);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void TeachCompanion()
        {

            if (XpMultiplierConfig.TeachCompanionXPNumber < 1)
            {
                return;
            }

            var companions = Hero.MainHero.CompanionsInParty.ToList();



            foreach (var companion in companions)
            {
                if (companion.IsWounded)
                {
                    return;
                }
                foreach (var skill in DefaultSkills.GetAllSkills())
                {
                    var xp = 0f;
                    var companionSkill = companion.GetSkillValue(skill);
                    var playerSkill = Hero.MainHero.GetSkillValue(skill);

                    if (playerSkill > 100 && playerSkill > companionSkill)
                    {

                        xp = MBRandom.RandomFloat * XpMultiplierConfig.TeachCompanionXPNumber * playerSkill / (companionSkill * 3 + playerSkill);

                        companion.AddSkillXp(skill, xp);
                    }

                }
            }

        }

    }
}
