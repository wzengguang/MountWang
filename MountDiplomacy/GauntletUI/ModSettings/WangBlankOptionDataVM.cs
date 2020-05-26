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
    public class WangBlankOptionDataVM : WangGenericOptionDataVM
    {


        public WangBlankOptionDataVM(SettingVM settingVM, ISetting setting, PropertyInfo property) : base(settingVM, setting, property)
        {

        }


        public override void UpdateValue()
        {
        }
    }
}
