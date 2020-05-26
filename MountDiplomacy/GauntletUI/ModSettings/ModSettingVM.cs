using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Wang.Setting;

namespace Wang.GauntletUI.ModSettings
{
    public class ModSettingVM : ViewModel
    {

        private readonly Action _onRefresh;

        private bool _isSelected;


        private MBBindingList<SettingVM> _settings = new MBBindingList<SettingVM>();

        private SettingVM _currentSelectedSetting;


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
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }



        [DataSourceProperty]
        public MBBindingList<SettingVM> Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged(nameof(Settings));
                }
            }
        }

        [DataSourceProperty]
        public SettingVM CurrentSelectedSetting
        {
            get => _currentSelectedSetting;
            set
            {
                if (_currentSelectedSetting != value)
                {
                    _currentSelectedSetting = value;
                    OnPropertyChanged(nameof(CurrentSelectedSetting));
                }
            }
        }

        public ModSettingVM(Action onRefresh)
        {
            _onRefresh = onRefresh;
            RefreshValues();
        }

        public void OnSelectedSetting(SettingVM setting)
        {
            if (setting != this.CurrentSelectedSetting)
            {
                if (this.CurrentSelectedSetting != null)
                {
                    this.CurrentSelectedSetting.IsSelected = false;
                }
                this.CurrentSelectedSetting = setting;
                this.CurrentSelectedSetting.IsSelected = true;
            }

        }


        public override void RefreshValues()
        {
            base.RefreshValues();
        }


        public void OnRefresh()
        {
            _onRefresh?.Invoke();
        }

        internal void RefreshSetting()
        {
            this.Settings.Clear();

            foreach (var item in FileData.Settings.OrderBy(a => a.Order))
            {
                Settings.Add(new SettingVM(OnSelectedSetting, item));
            }


            if (Settings.Count > 0)
            {
                OnSelectedSetting(Settings[0]);
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }
    }
}
