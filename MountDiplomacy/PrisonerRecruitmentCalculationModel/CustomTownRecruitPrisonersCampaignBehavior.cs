using Helpers;
using MountAndBlade.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
namespace Wang
{
    public class CustomTownRecruitPrisonersCampaignBehavior : CampaignBehaviorBase, ICampaignBehavior
    {
        public override void SyncData(IDataStore dataStore)
        {
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, recruit);
        }

        public void recruit(Town town)
        {
            if ((!town.IsCastle && !town.IsTown) || town.GarrisonParty == null || town.GarrisonParty.PrisonRoster == null || town.Settlement.IsStarving)
            {
                return;
            }

            TroopRoster prisonRoster = town.GarrisonParty.PrisonRoster;
            float[] dailyRecruitedPrisoners = { };
            PrisonerRecruitmentCalculationModelPatch.GetDailyRecruitedPrisoners(ref dailyRecruitedPrisoners, MobileParty.MainParty);
            int num = MBRandom.RandomInt(prisonRoster.Count);

            for (int i = 0; i < prisonRoster.Count; i++)
            {
                int index = (i + num) % prisonRoster.Count;
                CharacterObject characterAtIndex = prisonRoster.GetCharacterAtIndex(index);
                if (characterAtIndex.Tier > 6 || characterAtIndex.IsHero)
                {
                    continue;
                }


                int tier = characterAtIndex.Tier;
                if (tier < dailyRecruitedPrisoners.Length && dailyRecruitedPrisoners[tier] > 0f && MBRandom.RandomFloat < dailyRecruitedPrisoners[tier])//
                {

                    dailyRecruitedPrisoners[tier] -= 1f;

                    if (MBRandom.RandomFloat < (town.Owner.LeaderHero == Hero.MainHero ? 0.8 : 0.5))
                    {
                        town.GarrisonParty.MemberRoster.AddToCounts(characterAtIndex, 1);
                    }
                    prisonRoster.AddToCounts(characterAtIndex, -1);

                }
            }




        }

    }
}
