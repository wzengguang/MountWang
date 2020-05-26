using System;
using System.Configuration;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Wang.GauntletUI;
using Wang.Setting;
using Wang.Setting.Attributes;

namespace Wang.GauntletUI.ModSettings
{
    public class WangNumericOptionDataVM : WangGenericOptionDataVM
    {
        private float _min;

        private float _max;

        private float _optionValue;
        private bool _isDiscrete;

        public WangNumericOptionDataVM(SettingVM settingVM, ISetting setting, PropertyInfo property) : base(settingVM, setting, property)
        {
            this.OptionValue = (float)property.GetValue(setting);
            foreach (var item in property.GetCustomAttributes(true))
            {
                if (item as SettingNumericAttribute != null)
                {
                    var b = item as SettingNumericAttribute;
                    Max = b.Max;
                    Min = b.Min;
                    IsDiscrete = b.IsDiscrete;
                }
            }
            RefreshValues();
        }

        public override void UpdateValue()
        {
            Property.SetValue(Setting, this.OptionValue);
        }

        [DataSourceProperty]
        public float Min
        {
            get
            {
                return this._min;
            }
            set
            {
                if (value != this._min)
                {
                    this._min = value;
                    base.OnPropertyChanged(nameof(Min));
                }
            }
        }

        [DataSourceProperty]
        public float Max
        {
            get
            {
                return this._max;
            }
            set
            {
                if (value != this._max)
                {
                    this._max = value;
                    base.OnPropertyChanged(nameof(Max));
                }
            }
        }

        [DataSourceProperty]
        public float OptionValue
        {
            get
            {
                return this._optionValue;
            }
            set
            {
                if (value != this._optionValue)
                {
                    this._optionValue = value;
                    base.OnPropertyChanged(nameof(OptionValue));
                    base.OnPropertyChanged(nameof(OptionValueAsString));
                    this.UpdateValue();
                }
            }
        }

        [DataSourceProperty]
        public string OptionValueAsString
        {
            get
            {
                if (!this.IsDiscrete)
                {
                    return this._optionValue.ToString("F");
                }
                return ((int)this._optionValue).ToString();
            }
        }

        [DataSourceProperty]
        public bool IsDiscrete
        {
            get
            {
                return this._isDiscrete;
            }
            set
            {
                if (value != this._isDiscrete)
                {
                    this._isDiscrete = value;
                    base.OnPropertyChanged(nameof(IsDiscrete));
                }
            }
        }


    }
}
