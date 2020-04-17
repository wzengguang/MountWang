using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace Wang
{
    public static class PartyVMExtension
    {
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

        public static void ExecuteRecruitAll(this PartyVM partyVM)
        {
            PartyScreenLogic partyScreenLogic = GetPartyScreenLogic(partyVM);
            int num = partyScreenLogic.RightOwnerParty.PartySizeLimit - partyScreenLogic.MemberRosters[1].TotalManCount;
            int num2 = 0;
            int num3 = 0;
            foreach (PartyCharacterVM item in partyVM.MainPartyPrisoners.OrderBy((PartyCharacterVM o) => o.Character.Tier).ToList())
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
            PartyScreenLogic partyScreenLogic = GetPartyScreenLogic(partyVM);
            int num = 0;
            int num2 = 0;
            foreach (PartyCharacterVM item in partyVM.MainPartyTroops.OrderByDescending((PartyCharacterVM o) => o.Character.Tier).ToList())
            {
                if (!item.IsHero && item.IsUpgrade1Available && !item.IsUpgrade2Exists && item.NumOfTarget1UpgradesAvailable > 0 && !item.IsUpgrade1Insufficient)
                {
                    int numOfTarget1UpgradesAvailable = item.NumOfTarget1UpgradesAvailable;
                    num++;
                    num2 += numOfTarget1UpgradesAvailable;
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    partyCommand.FillForUpgradeTroop(item.Side, item.Type, item.Character, numOfTarget1UpgradesAvailable, PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1);
                    partyVM.CurrentCharacter = item;
                    ProcessCommand(partyScreenLogic, partyCommand);
                }
            }
            RefreshPartyScreen(partyVM);
            if (num > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage($"升级 {num2} 部队 ({num})"));
            }
        }
    }
}
