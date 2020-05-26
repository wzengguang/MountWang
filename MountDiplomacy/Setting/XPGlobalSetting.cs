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
    public class XPGlobalSetting : SettingBase
    {
        public static XPGlobalSetting Instance { get; private set; }

        public XPGlobalSetting()
        {
            Instance = this;
        }
        public override string Name { get; set; } = "{=setting_xp_global}Xp Setting";


        [SettingBoolean("{=setting_lead_skill_change}Tweak Lead skill", "{=setting_lead_skill_change_desc}Description: Tweak CombatTips And RaiseTheMeek perk of lead skill. enhance the xp of troop gaining.", true)]
        public bool EnableCombatTipsAndRaiseTheMeekSkillRework { get; set; } = true;

        [SettingNumeric("{=setting_xp_global_player}Player XP Multiple", "{=setting_xp_global_player_desc}Description: the multiple of  player xp, if you don`t want to apply this tweak, set to 1.", 1f, 1f, 10f)]
        public float PlayerXPMultiple { get; set; } = 2;

        [SettingNumeric("{=setting_xp_global_companion}Companion XP Multiple", "{=setting_xp_global_companion_desc}Description: the multiple of companion xp, if you don`t want to apply this tweak, set to 1.", 1f, 1f, 10f)]
        public float CompanionXPMultiple { get; set; } = 3;

        [SettingNumeric("{=setting_xp_learning}Learning XP Multiple", "{=setting_xp_learning_desc}Description: the multiple of learning skill xp.", 1f, 1f, 10f)]
        public float LearningXPMultipier { get; set; } = 1;




    }
}
