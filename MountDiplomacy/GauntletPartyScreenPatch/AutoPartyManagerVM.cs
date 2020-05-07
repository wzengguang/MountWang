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
                    OnPropertyChanged("OrderTroop");
                }
            }
        }




        public AutoPartyManagerVM(PartyVM partyVM)
        {
            _partyVM = partyVM;
            InitOrderTroop();
        }

        private void InitOrderTroop()
        {
            OrderTroop = new SelectorVM<OrderTroopSelectorItemVM>(0, OnOrderTroopChange);
            OrderTroop.SetOnChangeAction(null);
            foreach (TroopSortType sort in (TroopSortType[])Enum.GetValues(typeof(TroopSortType)))
            {
                OrderTroop.AddItem(new OrderTroopSelectorItemVM(sort));
            }
            OrderTroop.SetOnChangeAction(OnOrderTroopChange);
        }

        private void OnOrderTroopChange(SelectorVM<OrderTroopSelectorItemVM> obj)
        {
            if (obj.SelectedItem != null)
            {

                InformationManager.DisplayMessage(new InformationMessage(obj.SelectedItem.SortType.ToString()));

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
