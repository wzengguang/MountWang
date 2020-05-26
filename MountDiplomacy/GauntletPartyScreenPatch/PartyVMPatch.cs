using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Wang.Setting;

namespace Wang
{

    [HarmonyPatch(typeof(PartyVM))]
    class PartyVMPatch
    {
        /// <summary>
        /// 同伴的阵型fixed
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("ExecuteDone")]
        private static void ExecuteDone()
        {
            var behavior = Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>();

            foreach (var item in MobileParty.MainParty.MemberRoster.Where(a => a.Character.IsHero))
            {
                behavior.SetHeroFormation(item.Character.HeroObject, (int)item.Character.CurrentFormationClass);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch("TransferAllCharacters")]
        private static bool TransferAllCharacters(PartyVM __instance, ref PartyScreenLogic ____partyScreenLogic, ref PartyCharacterVM ____currentCharacter, PartyScreenLogic.PartyRosterSide rosterSide, PartyScreenLogic.TroopType type)
        {


            MBBindingList<PartyCharacterVM> list;

            if (type == PartyScreenLogic.TroopType.Member)
            {
                if (rosterSide == PartyScreenLogic.PartyRosterSide.Left)
                {
                    list = __instance.OtherPartyTroops;
                }
                else
                {
                    list = __instance.MainPartyTroops;
                }
            }
            else
            {
                if (rosterSide == PartyScreenLogic.PartyRosterSide.Left)
                {
                    list = __instance.OtherPartyPrisoners;
                }
                else
                {
                    list = __instance.MainPartyPrisoners;
                }
            }


            var newList = list;
            var lack = 10000;

            var prisionerSetting = PrisonerSetting.Instance;

            if (prisionerSetting.IsEnabled && rosterSide == PartyScreenLogic.PartyRosterSide.Left && PartyScreenLogic.TroopType.Prisoner == type)
            {
                var limit = MobileParty.MainParty.Party.PrisonerSizeLimit;
                lack = limit - __instance.MainPartyPrisoners.Sum(a => a.Number);



                var fixeds = new List<PartyCharacterVM>();
                var order1 = new List<PartyCharacterVM>();
                var order2 = new List<PartyCharacterVM>();
                var order3 = new List<PartyCharacterVM>();
                var order4 = new List<PartyCharacterVM>();
                var other = new List<PartyCharacterVM>();

                foreach (var item in newList)
                {
                    if (prisionerSetting.IsFixTroop(item.Character) > 0)
                    {
                        fixeds.Add(item);
                    }
                    else if (prisionerSetting.OrderTroop(item.Character, prisionerSetting.Order1, prisionerSetting.Order1MinTier) > 0)
                    {
                        order1.Add(item);
                    }
                    else if (prisionerSetting.OrderTroop(item.Character, prisionerSetting.Order2, prisionerSetting.Order2MinTier) > 0)
                    {
                        order2.Add(item);
                    }
                    else if (prisionerSetting.OrderTroop(item.Character, prisionerSetting.Order3, prisionerSetting.Order3MinTier) > 0)
                    {
                        order3.Add(item);
                    }
                    else if (prisionerSetting.OrderTroop(item.Character, prisionerSetting.Order4, prisionerSetting.Order4MinTier) > 0)
                    {
                        order4.Add(item);
                    }
                    else if (!prisionerSetting.EnableFilter)
                    {
                        other.Add(item);
                    }
                }

                //var orderList = newList
                //    .OrderBy(a =>
                //    {
                //        return prisionerSetting.IsFixTroop(a.Character);

                //    })
                //    .ThenBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order1, prisionerSetting.Order1MinTier); })
                //    .ThenBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order2, prisionerSetting.Order2MinTier); })
                //    .ThenBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order3, prisionerSetting.Order3MinTier); })
                //    .ThenBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order4, prisionerSetting.Order4MinTier); })
                //    .ToList();

                newList = new MBBindingList<PartyCharacterVM>();
                foreach (var item in other.OrderBy(a => a.Character.Tier))
                {
                    newList.Add(item);
                }
                foreach (var item in order4.OrderBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order4, prisionerSetting.Order4MinTier); }))
                {
                    newList.Add(item);
                }
                foreach (var item in order3.OrderBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order3, prisionerSetting.Order3MinTier); }))
                {
                    newList.Add(item);
                }
                foreach (var item in order2.OrderBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order2, prisionerSetting.Order2MinTier); }))
                {
                    newList.Add(item);
                }
                foreach (var item in order1.OrderBy(a => { return prisionerSetting.OrderTroop(a.Character, prisionerSetting.Order1, prisionerSetting.Order1MinTier); }))
                {
                    newList.Add(item);
                }
                foreach (var item in fixeds.OrderBy(a => { return prisionerSetting.IsFixTroop(a.Character); }))
                {
                    newList.Add(item);
                }
            }

            int tranfer = 0;
            for (int i = newList.Count - 1; i >= 0; i--)
            {
                if (lack <= tranfer)
                {
                    break;
                }

                if (____partyScreenLogic.IsTroopTransferrable(type, newList[i].Character, (int)newList[i].Side))
                {
                    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
                    PartyCharacterVM partyCharacterVM = newList[i];
                    ____currentCharacter = partyCharacterVM;

                    if (partyCharacterVM.Type == PartyScreenLogic.TroopType.AttachedGroups)
                    {
                        partyCommand.FillForTransferPartyLeaderTroop(partyCharacterVM.Side, __instance.CurrentCharacter.Type, partyCharacterVM.Character, partyCharacterVM.Troop.Number);

                    }
                    else if (partyCharacterVM.Side == PartyScreenLogic.PartyRosterSide.Right)
                    {

                        if (PartyScreenLogic.TroopType.Prisoner == type && prisionerSetting.IsEnabled && prisionerSetting.IsFixTroop(partyCharacterVM.Character) > 0)
                        {
                            continue;
                        }

                        partyCommand.FillForTransferTroop(partyCharacterVM.Side, partyCharacterVM.Type, partyCharacterVM.Character, partyCharacterVM.Troop.Number, partyCharacterVM.Troop.WoundedNumber, -1);
                    }
                    else
                    {
                        var tranferNumber = partyCharacterVM.Troop.Number;
                        var tranferWoundedNumber = partyCharacterVM.Troop.WoundedNumber;
                        if (rosterSide == PartyScreenLogic.PartyRosterSide.Left && PartyScreenLogic.TroopType.Prisoner == type)
                        {
                            if (lack - tranfer <= partyCharacterVM.Troop.Number)
                            {
                                tranferNumber = lack - tranfer;

                                var heath = partyCharacterVM.Troop.Number - partyCharacterVM.Troop.WoundedNumber;
                                if (tranferNumber < heath)
                                {
                                    tranferWoundedNumber = 0;
                                }
                                else
                                {
                                    tranferWoundedNumber = tranferNumber - heath;
                                }


                            }
                            tranfer += tranferNumber;
                        }
                        partyCommand.FillForTransferTroop(partyCharacterVM.Side, partyCharacterVM.Type, partyCharacterVM.Character, tranferNumber, tranferWoundedNumber, -1);
                    }
                    ____partyScreenLogic.AddCommand(partyCommand);
                }
            }

            return false;
        }

    }
}
