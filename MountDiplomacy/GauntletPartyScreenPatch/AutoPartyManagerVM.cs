using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Wang.GauntletUI;

namespace Wang
{
    public class AutoPartyManagerVM : ViewModel
    {

        private PartyVM _partyVM;


        private string _orderText;

        [DataSourceProperty]
        public string OrderText
        {
            get { return _orderText; }
            set
            {
                if (value != _orderText)
                {
                    _orderText = value;
                    OnPropertyChanged(nameof(OrderText));
                }
            }
        }

        private SelectorVM<OrderTroopSelectorItemVM> _orderTroop;

        [DataSourceProperty]
        public SelectorVM<OrderTroopSelectorItemVM> OrderTroop
        {
            get
            {
                return _orderTroop;
            }
            set
            {
                if (value != _orderTroop)
                {
                    _orderTroop = value;
                    OnPropertyChanged(nameof(OrderTroop));
                }
            }
        }

        public AutoPartyManagerVM(PartyVM partyVM)
        {
            OrderText = "order";
            _partyVM = partyVM;
            InitOrderTroop();
        }

        private void InitOrderTroop()
        {
            //OrderTroop = new SelectorVM<OrderTroopSelectorItemVM>(0, OnOrderTroopChange);
            //OrderTroop.SetOnChangeAction(null);
            //foreach (TroopSortType sort in (TroopSortType[])Enum.GetValues(typeof(TroopSortType)))
            //{
            //    OrderTroop.AddItem(new OrderTroopSelectorItemVM(sort));
            //}
            //OrderTroop.SetOnChangeAction(OnOrderTroopChange);
        }

        private void OnOrderTroopChange(SelectorVM<OrderTroopSelectorItemVM> obj)
        {
            if (obj.SelectedItem != null)
            {
                SortPartyConfig.SortOrder = obj.SelectedItem.SortType;
                // obj.SelectedItem.SortType=
                var newOrders = new MBBindingList<PartyCharacterVM>();
                switch (obj.SelectedItem.SortType)
                {
                    case TroopSortType.None:
                        foreach (var item in _partyVM.MainPartyTroops.OrderBy(a => a.IsHero).ThenBy(a => a.Troop.Character.Tier).ToList())
                        {
                            newOrders.Add(item);
                        }
                        _partyVM.MainPartyTroops = newOrders;
                        break;
                }
                _partyVM.RefreshValues();

            }
        }

        public void ExecuteRecruitAll()
        {
            // InformationManager.DisplayMessage(new InformationMessage("ExecuteRecruitAll"));
            _partyVM.ExecuteRecruitAll();

        }

        public void OrderParty()
        {
            //  _partyVM.OrderParty();

            //Func<CharacterObject, CharacterObject, bool> func = (a, b) =>
            //{
            //    return a.Tier > b.Tier;
            //};

            //PartyScreenLogic partyScreenLogic = _partyVM.GetPartyScreenLogic();

            //List<PartyCharacterVM> partyCharacterVMs = new List<PartyCharacterVM>();

            //var heroCount = 0;

            //for (int i = 0; i < _partyVM.MainPartyTroops.Count; i++)
            //{
            //    var item = _partyVM.MainPartyTroops[i];
            //    if (!item.IsHero)
            //    {
            //        var newIndex = 0;
            //        for (int n = 0; n < partyCharacterVMs.Count; n++)
            //        {
            //            if (func.Invoke(item.Character, partyCharacterVMs[n].Character))
            //            {
            //                newIndex = n;
            //            }
            //        }
            //        partyCharacterVMs.Insert(newIndex, item);
            //    }
            //    else
            //    {
            //        heroCount++;
            //    }
            //}

            //for (int i = 0; i < partyCharacterVMs.Count; i++)
            //{
            //    var troop = partyCharacterVMs[i];

            //    PartyScreenLogic.PartyCommand partyCommand = new PartyScreenLogic.PartyCommand();
            //    partyCommand.FillForShiftTroop(troop.Side, troop.Type, troop.Character, i + heroCount);
            //    partyScreenLogic.AddCommand(partyCommand);
            //}
        }

        public void ExecuteUpgradeAll()
        {
            _partyVM.ExecuteUpgradeAll();

        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
        }
    }
}
