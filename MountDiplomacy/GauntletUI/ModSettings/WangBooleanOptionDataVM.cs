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
    public class WangBooleanOptionDataVM : WangGenericOptionDataVM
    {

        private bool _optionValue;

        public WangBooleanOptionDataVM(SettingVM settingVM, ISetting setting, PropertyInfo property) : base(settingVM, setting, property)
        {
            this.OptionValueAsBoolean = (bool)property.GetValue(setting);
        }


        [DataSourceProperty]
        public bool OptionValueAsBoolean
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
                    base.OnPropertyChanged(nameof(OptionValueAsBoolean));
                    this.UpdateValue();
                }
            }
        }


        public override void UpdateValue()
        {
            Property.SetValue(Setting, this.OptionValueAsBoolean);
        }
    }
}
