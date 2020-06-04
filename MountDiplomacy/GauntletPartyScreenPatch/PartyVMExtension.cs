using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
using Wang.Setting;

namespace Wang
{

    public enum TroopOrderEnum
    {
        rideRangeInfantry
    }

    public static class PartyVMExtension
    {

        private static TroopOrderEnum TroopOrderEnum = TroopOrderEnum.rideRangeInfantry;

        public static void RefreshPartyScreen(this PartyVM partyVM)
        {
            Traverse.Create(partyVM).Method("RefreshTopInformation").GetValue();
            Traverse.Create(partyVM).Method("RefreshPartyInformation").GetValue();
            Traverse.Create(partyVM).Method("RefreshPrisonersRecruitable").GetValue();
        }

        public static void ProcessCommand(this PartyScreenLogic instance, PartyScreenLogic.PartyCommand command)
        {
            Traverse.Create(instance).Method("ProcessCommand", new Type[1]
            {
            typeof(PartyScreenLogic.PartyCommand)
            }).GetValue(command);
        }

        public static PartyScreenLogic GetPartyScreenLogic(this PartyVM instance)
        {
            return Traverse.Create(instance).Field<PartyScreenLogic>("_partyScreenLogic").Value;
        }

        public static void OrderParty(this PartyVM partyVM)
        {
            PartyScreenLogic partyScreenLogic = GetPartyScreenLogic(partyVM);

            var own = partyScreenLogic.MemberRosters[1];

            List<TroopRosterElement> list = own.ToList();
            List<FlattenedTroopRosterElement> elementList = CreateFlattenedRoster(own, partyVM);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Character.IsHero)
                {
                    own.RemoveTroop(list[i].Character, list[i].Number);
                }
            }
            own.Add(elementList);
        }

        public static List<FlattenedTroopRosterElement> CreateFlattenedRoster(TroopRoster roster, PartyVM partyVM)
        {
            IEnumerable<FlattenedTroopRosterElement> source = roster.ToFlattenedRoster().Where(a => !a.Troop.IsHero);
            switch (TroopOrderEnum)
            {
                case TroopOrderEnum.rideRangeInfantry:
                    source = source.OrderBy(a => a.Troop.IsMounted).ThenBy(a => a.Troop.IsArcher).ThenByDescending(a => a.Troop.Tier);

                    break;
            }

            TroopOrderEnum = (TroopOrderEnum)(((int)TroopOrderEnum + 1) % 1);

            return source.ToList();
        }







        public static void ExecuteRecruitAll(this PartyVM partyVM)
        {

            PartyScreenLogic partyScreenLogic = GetPartyScreenLogic(partyVM);
            int num = partyScreenLogic.RightOwnerParty.PartySizeLimit - partyScreenLogic.MemberRosters[1].TotalManCount;
            int num2 = 0;
            int num3 = 0;
            foreach (PartyCharacterVM item in partyVM.MainPartyPrisoners.OrderByDescending((PartyCharacterVM o) => o.Character.Tier).ToList())
            {
                if (num <= 0)
                {
                    break;
                }
                if (!item.IsHero && item.NumOfRecruitablePrisoners > 0)
                {
                    int num4 = Math.Min(item.NumOfRecruitablePrisoners, num);
                    num2++;
                    num -= num4;
                    num3 += num4;
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForRecruitTroop(item.Side, item.Type, item.Character, num4);
                    partyVM.CurrentCharacter = item;
                    ProcessCommand(partyScreenLogic, partyCommand);
                }
            }
            RefreshPartyScreen(partyVM);
            if (num2 > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage($"招募 {num3} 兵 ({num2})"));
            }
            if (num <= 0)
            {
                InformationManager.DisplayMessage(new InformationMessage("队伍成员达到上限."));
            }
        }

        public static void ExecuteUpgradeAll(this PartyVM partyVM)
        {
            var hasTwoUpgrade = new List<PartyCharacterVM>();

            PartyScreenLogic partyScreenLogic = GetPartyScreenLogic(partyVM);
            int troopNum = 0;
            int troopNumTotal = 0;

            var party = partyVM.MainPartyTroops.OrderByDescending((PartyCharacterVM o) => o.Character.Tier).ToList();
            foreach (PartyCharacterVM partyCharacterVM in party)
            {
                if (!partyCharacterVM.IsHero && partyCharacterVM.IsUpgrade1Available && !partyCharacterVM.IsUpgrade2Exists && partyCharacterVM.NumOfTarget1UpgradesAvailable > 0 && !partyCharacterVM.IsUpgrade1Insufficient)
                {
                    int numOfTarget1UpgradesAvailable = partyCharacterVM.NumOfTarget1UpgradesAvailable;
                    troopNum++;
                    troopNumTotal += numOfTarget1UpgradesAvailable;
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForUpgradeTroop(partyCharacterVM.Side, partyCharacterVM.Type, partyCharacterVM.Character, numOfTarget1UpgradesAvailable, PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1);
                    partyVM.CurrentCharacter = partyCharacterVM;
                    ProcessCommand(partyScreenLogic, partyCommand);
                }

                if (!partyCharacterVM.IsHero && partyCharacterVM.IsUpgrade1Exists && partyCharacterVM.IsUpgrade1Available && partyCharacterVM.IsUpgrade2Exists && partyCharacterVM.IsUpgrade2Available)
                {
                    hasTwoUpgrade.Add(partyCharacterVM);
                }
            }

            foreach (var partyCharacterVM in hasTwoUpgrade)
            {


                if (UpgradeSetting.Instance.IsEnabled && partyCharacterVM.IsUpgrade1Exists && partyCharacterVM.IsUpgrade2Exists && partyCharacterVM.IsUpgrade1Available && partyCharacterVM.IsUpgrade2Available && !partyCharacterVM.IsUpgrade1Insufficient && !partyCharacterVM.IsUpgrade2Insufficient)
                {
                    var upgradeSide = UpgradeSetting.Instance.FindUpgradeTopInSetting(partyCharacterVM.Character);
                    if (upgradeSide > -1 && upgradeSide < 2)
                    {
                        var upgradeNum = upgradeSide == 0 ? partyCharacterVM.NumOfTarget1UpgradesAvailable : partyCharacterVM.NumOfTarget2UpgradesAvailable;
                        troopNum++;
                        troopNumTotal += upgradeNum;

                        PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                        partyCommand.FillForUpgradeTroop(partyCharacterVM.Side, partyCharacterVM.Type, partyCharacterVM.Character, upgradeNum, (PartyScreenLogic.PartyCommand.UpgradeTargetType)upgradeSide);

                        partyVM.CurrentCharacter = partyCharacterVM;
                        ProcessCommand(partyScreenLogic, partyCommand);
                        continue;
                    }
                }

                partyVM.MainPartyTroops.Remove(partyCharacterVM);
                partyVM.MainPartyTroops.Insert(1, partyCharacterVM);
            }




            RefreshPartyScreen(partyVM);
            if (troopNum > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage($"升级 {troopNumTotal} 部队 ({troopNum})"));
            }
        }
    }
}
