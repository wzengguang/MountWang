using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
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

        [SettingNumeric("{=setting_xp_global_player}Player XP Multiple", "{=setting_xp_global_player_desc}Description: the adding multiple of  player xp, if you don`t want to apply this tweak, set to 0.", 0f, 0f, 10f)]
        public float PlayerXPMultiple { get; set; } = 1;

        [SettingNumeric("{=setting_xp_global_companion}Companion XP Multiple", "{=setting_xp_global_companion_desc}Description: the adding multiple of companion xp, if you don`t want to apply this tweak, set to 0.", 0f, 0f, 10f)]
        public float CompanionXPMultiple { get; set; } = 2;

        [SettingNumeric("{=setting_xp_roguery}Roguery XP Multiple", "{=setting_xp_roguery_desc}Description: Roguery XP Multiple.Set 0 disable this function.", 0f, 0f, 10f)]
        public float RogueryXPMultiple { get; set; } = 1;

        [SettingNumeric("{=setting_xp_engineering}Engineering XP Multiple", "{=setting_xp_engineering_desc}Description: Engineering XP Multiple.Set 0 disable this function.", 0f, 0f, 10f)]
        public float EngineeringXPMultiple { get; set; } = 1;

        [SettingNumeric("{=setting_xp_medicine}Medicine XP Multiple", "{=setting_xp_medicine_desc}Description: Medicine XP Multiple.Set 0 disable this function.", 0f, 0f, 10f)]
        public float MedicineXPMultiple { get; set; } = 1;


        [SettingNumeric("{=setting_xp_learning}Learning XP Multiple", "{=setting_xp_learning_desc}Description: the adding multiple of learning skill xp.", 0f, 0f, 10f)]
        public float LearningXPMultipier { get; set; } = 1;




    }
}
