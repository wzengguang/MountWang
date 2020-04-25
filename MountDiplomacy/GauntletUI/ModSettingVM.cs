using System;
using TaleWorlds.Library;

namespace Wang.GauntletUI
{
    public class ModSettingVM : ViewModel
    {

        private readonly Action _onRefresh;

        private bool _isSelected;
        //   private MBBindingList<SettingPropertyGroup> _settingPropertyGroups;
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


        //[DataSourceProperty]
        //public MBBindingList<SettingPropertyGroup> SettingPropertyGroups
        //{
        //    get => _settingPropertyGroups;
        //    set
        //    {
        //        if (_settingPropertyGroups != value)
        //        {
        //            _settingPropertyGroups = value;
        //            OnPropertyChanged("SettingPropertyGroups");
        //        }
        //    }
        //}


        public ModSettingVM(Action onRefresh)
        {
            _onRefresh = onRefresh;


            RefreshValues();


            RefreshList();
        }


        public override void RefreshValues()
        {
            //foreach (var settingGroup in SettingPropertyGroups)
            //    settingGroup.AssignUndoRedoStack(URS);

            //  foreach (var group in SettingPropertyGroups)
            //      group.RefreshValues();
            //  OnPropertyChanged(nameof(IsSelected));
            //   OnPropertyChanged(nameof(ModName));
            //   OnPropertyChanged(nameof(SettingPropertyGroups));
            base.RefreshValues();
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
