using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Wang.GauntletUI
{
    public class InformationVM : ViewModel
    {
        private readonly Action _onRefresh;
        private bool _isSelected;
        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public InformationVM(Action onRefresh)
        {
            _onRefresh = onRefresh;


            RefreshValues();


            RefreshList();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            if (!IsSelected)
            {
                return;
            }

            


        }

        public void RefreshList()
        {
        }

        public void OnRefresh()
        {
            _onRefresh?.Invoke();
        }
    }
}
