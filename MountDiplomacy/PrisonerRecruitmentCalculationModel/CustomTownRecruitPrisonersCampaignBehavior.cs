using Helpers;
using MountAndBlade.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, Recruit);
        }

        public void Recruit(Settlement settlement)
        {
            if (!settlement.IsTown && !settlement.IsCastle)
            {
                return;
            }

            var town = settlement.Town;

            if (town.Owner == null || town.Owner.PrisonRoster == null)
            {
                return;
            }

            if (town.GarrisonParty == null)
            {
                town.Settlement.AddGarrisonParty(false);
            }

            TroopRoster prisonRoster = town.Owner.PrisonRoster;
            float[] dailyRecruitedPrisoners = Array.Empty<float>();
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

                    if (MBRandom.RandomFloat < (town.Settlement.OwnerClan == Clan.PlayerClan ? 0.8 : 1))
                    {
                        town.GarrisonParty.MemberRoster.AddToCounts(characterAtIndex, 1);
                    }
                    prisonRoster.AddToCounts(characterAtIndex, -1);

                }
            }
        }







    }

}

