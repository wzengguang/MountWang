using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using Wang.Setting;
using Wang.Setting.Attributes;

namespace Wang.GauntletUI.ModSettings
{
    public class SettingVM : ViewModel
    {
        private ISetting _setting;
        private readonly Action<SettingVM> _onSelected;

        private string _name;

        private bool _isSelected;
        private MBBindingList<WangGenericOptionDataVM> _options;

        private string _onHoverDescription;

        private bool _isHover;

        [DataSourceProperty]
        public bool IsHover
        {
            get => _isHover;
            set
            {
                if (_isHover != value)
                {
                    _isHover = value;
                    OnPropertyChanged(nameof(IsHover));

                }
            }
        }

        [DataSourceProperty]
        public string OnHoverDescription
        {
            get => _onHoverDescription;
            set
            {
                if (_onHoverDescription != value)
                {
                    _onHoverDescription = value;
                    OnPropertyChanged(nameof(OnHoverDescription));
                }
            }
        }

        [DataSourceProperty]
        public String Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataSourceProperty]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<WangGenericOptionDataVM> Options
        {
            get
            {
                return this._options;
            }
            set
            {
                if (value != this._options)
                {
                    this._options = value;
                    base.OnPropertyChanged(nameof(Options));
                }
            }
        }

        public SettingVM(Action<SettingVM> onSelected, ISetting setting)
        {
            Name = new TextObject(setting.Name).ToString();
            _onSelected = onSelected;
            _setting = setting;

            Options = new MBBindingList<WangGenericOptionDataVM>();

            var properties = setting.GetType().GetProperties();

            foreach (var item in properties)
            {
                var attrs = item.GetCustomAttributes(true);

                var sAttr = attrs.FirstOrDefault(a => a as SettingBaseAttribute != null);

                if (sAttr != null)
                {
                    if (item.PropertyType == typeof(bool) && sAttr is SettingBooleanAttribute)
                    {
                        Options.Add(new WangBooleanOptionDataVM(this, setting, item));
                    }
                    else if (item.PropertyType == typeof(float) && sAttr is SettingNumericAttribute)
                    {
                        Options.Add(new WangNumericOptionDataVM(this, setting, item));
                    }
                    else if (item.PropertyType == typeof(DateTime) && sAttr is SettingBlankAttribute)
                    {
                        Options.Add(new WangBlankOptionDataVM(this, setting, item));
                    }
                    else if (item.PropertyType == typeof(string) && sAttr is SettingStringAttribute)
                    {
                        var selector = ((SettingStringAttribute)sAttr).SelectorProperty;

                        Options.Add(new WangStringOptionDataVM(this, setting, item, setting.GetType().GetProperty(selector)));
                    }
                }
            }
        }


        public override void RefreshValues()
        {
            base.RefreshValues();
            this.Options.ApplyActionOnAllItems(delegate (WangGenericOptionDataVM x)
            {
                x.RefreshValues();
            });
        }

        public void OnSelected()
        {
            IsHover = false;
            OnHoverDescription = _setting.Description == null ? Name : new TextObject(_setting.Description).ToString();
            _onSelected.Invoke(this);

        }
    }
}
