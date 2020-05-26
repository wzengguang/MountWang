using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Options.ManagedOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using Wang.Setting.Attributes;

namespace Wang.Setting
{
    public class PrisonerEscapeSetting : SettingBase
    {
        public static PrisonerEscapeSetting Instance { get; private set; }

        public PrisonerEscapeSetting()
        {
            Instance = this;
        }
        public override string Name { get; set; } = "{=setting_prisorner_escape}Hero Prisoner Escape Chance";

        [SettingBoolean("{=setting_prisorner_escape_enabled}enable this tweak", "{=setting_prisorner_escape_enabled_desc}Description: Enable this tweak.", true)]
        public bool IsEnabled { get; set; } = true;


        [SettingNumeric("{=setting_prisorner_escape_days}Day Factor", "{=setting_prisorner_escape_days_desc}Description: Along with the days of hero captured increasing, the escaping chance become more bigger, the max value not more bigger than vanilla value. change this value more bigger can make hero prisoner more harder to escape.", 2f, 1f, 100f, true)]
        public float Basedays { get; set; } = 10;


    }
}
