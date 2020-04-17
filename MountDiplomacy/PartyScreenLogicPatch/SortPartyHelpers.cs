using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang
{
    public class SortPartyHelpers
    {
        public static void SortPartyScreen(PartyScreenLogic partyScreen)
        {
            SortPartyScreen(partyScreen, right: true, left: true, troops: true, prisoners: true);
        }

        public static void SortPartyScreen(PartyScreenLogic partyScreen, bool right, bool left, bool troops, bool prisoners)
        {
            try
            {
                if (left)
                {
                    if (troops)
                    {
                        SortUnits(partyScreen.MemberRosters[0]);
                    }
                    if (prisoners)
                    {
                        SortUnits(partyScreen.PrisonerRosters[0]);
                    }
                }
                if (right)
                {
                    if (troops)
                    {
                        SortUnits(partyScreen.MemberRosters[1]);
                    }
                    if (prisoners)
                    {
                        SortUnits(partyScreen.PrisonerRosters[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage("Error in SortParty: " + ex.Message));
            }
        }

        public static void SortUnits(TroopRoster input)
        {
            if (SortPartyConfig.SortOrder == SortType.None)
            {
                return;
            }

            List<TroopRosterElement> list = input.ToList();
            List<FlattenedTroopRosterElement> elementList = CreateFlattenedRoster(input);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Character.IsHero)
                {
                    input.RemoveTroop(list[i].Character, list[i].Number);
                }
            }
            input.Add(elementList);
        }

        public static List<FlattenedTroopRosterElement> CreateFlattenedRoster(TroopRoster roster)
        {
            IEnumerable<FlattenedTroopRosterElement> source = from x in roster.ToFlattenedRoster()
                                                              where !x.Troop.IsHero
                                                              select x;
            switch (SortPartyConfig.SortOrder)
            {
                case SortType.TierDesc:
                    return (from x in source
                            orderby x.Troop.Tier descending, x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.TierAsc:
                    return (from x in source
                            orderby x.Troop.Tier, x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.TierDescType:
                    return (from x in source
                            orderby x.Troop.Tier descending, IsMountedUnit(x.Troop), IsRangedUnit(x.Troop), x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.TierAscType:
                    return (from x in source
                            orderby x.Troop.Tier, IsMountedUnit(x.Troop), IsRangedUnit(x.Troop), x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.MountRangeTierDesc:
                    return (from x in source
                            orderby IsMountedUnit(x.Troop) descending, IsRangedUnit(x.Troop), x.Troop.Tier descending, x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.MountRangeTierAsc:
                    return (from x in source
                            orderby IsMountedUnit(x.Troop) descending, IsRangedUnit(x.Troop), x.Troop.Tier, x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.CultureTierDesc:
                    return (from x in source
                            orderby x.Troop.Culture.Name.ToString(), x.Troop.Tier descending, x.Troop.Name.ToString()
                            select x).ToList();
                case SortType.CultureTierAsc:
                    return (from x in source
                            orderby x.Troop.Culture.Name.ToString(), x.Troop.Tier, x.Troop.Name.ToString()
                            select x).ToList();
                default:
                    return (from x in source
                            orderby x.Troop.Tier descending, x.Troop.Name.ToString()
                            select x).ToList();
            }
        }

        public static MBBindingList<PartyCharacterVM> SortVMTroops(MBBindingList<PartyCharacterVM> input, bool sortRecruitUpgrade = false)
        {
            List<PartyCharacterVM> list = null;
            if (sortRecruitUpgrade)
            {
                list = (from x in input
                        where !x.IsHero
                        orderby x.IsTroopRecruitable || (x.IsUpgrade1Available && !x.IsUpgrade1Insufficient) || (x.IsUpgrade2Available && !x.IsUpgrade2Insufficient) descending, (x.IsUpgrade1Available && x.IsUpgrade1Insufficient) || (x.IsUpgrade2Available && x.IsUpgrade2Insufficient) descending, x.Character.Tier descending, x.Character.Name.ToString()
                        select x).ToList();
            }
            else
            {
                switch (SortPartyConfig.SortOrder)
                {
                    case SortType.TierDesc:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Tier descending, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.TierAsc:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Tier, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.TierDescType:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Tier descending, IsMountedUnit(x.Character), IsRangedUnit(x.Character), x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.TierAscType:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Tier, IsMountedUnit(x.Character), IsRangedUnit(x.Character), x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.MountRangeTierDesc:
                        list = (from x in input
                                where !x.IsHero
                                orderby IsMountedUnit(x.Character) descending, IsRangedUnit(x.Character), x.Character.Tier descending, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.MountRangeTierAsc:
                        list = (from x in input
                                where !x.IsHero
                                orderby IsMountedUnit(x.Character) descending, IsRangedUnit(x.Character), x.Character.Tier, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.CultureTierDesc:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Culture.Name.ToString(), x.Character.Tier descending, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                    case SortType.CultureTierAsc:
                        list = (from x in input
                                where !x.IsHero
                                orderby x.Character.Culture.Name.ToString(), x.Character.Tier, x.Character.Name.ToString()
                                select x).ToList();
                        break;
                }
            }
            if (list != null)
            {
                MBBindingList<PartyCharacterVM> mBBindingList = new MBBindingList<PartyCharacterVM>();
                foreach (PartyCharacterVM item in input.Where((PartyCharacterVM x) => x.IsHero))
                {
                    mBBindingList.Add(item);
                }
                foreach (PartyCharacterVM item2 in list)
                {
                    mBBindingList.Add(item2);
                }
                return mBBindingList;
            }
            return input;
        }

        public static bool IsRangedUnit(CharacterObject troop)
        {
            bool result = false;
            List<Equipment> list = troop.BattleEquipments.ToList();
            if (list.Count > 0)
            {
                ItemObject.ItemTypeEnum itemType = list[0].GetEquipmentFromSlot(EquipmentIndex.WeaponItemBeginSlot).Item.ItemType;
                result = (itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Crossbow || itemType == ItemObject.ItemTypeEnum.Thrown);
            }
            return result;
        }

        public static bool IsMountedUnit(CharacterObject troop)
        {
            bool result = false;
            List<Equipment> list = troop.BattleEquipments.ToList();
            if (list.Count > 0)
            {
                result = !list[0].Horse.IsEmpty;
            }
            return result;
        }

        public static void LogException(string method, Exception ex)
        {
            InformationManager.DisplayMessage(new InformationMessage("SortParty " + method + " exception: " + ex.Message));
        }
    }

}
