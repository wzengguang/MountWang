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
    public class CommonSetting : SettingBase
    {
        public static CommonSetting Instance { get; private set; }

        public CommonSetting()
        {
            Instance = this;
        }
        public override int Order { get; set; } = 1;
        public override string Name { get; set; } = "{=setting_common}Common";

        [SettingBoolean("{=setting_main_feature_lock_no_miss}Lock No miss", "{=setting_main_feature_lock_no_miss_desc}Description: Locked item don`t miss by exhausted.")]
        public bool LockNoMiss { get; set; } = true;

        [SettingBoolean("{=setting_main_feature_sell_out}Sell Out Optimization", "{=setting_main_feature_sell_out_desc}Description: When use sell-out funtion, if the town has not enough money, stop sell free.")]
        public bool SellOut { get; set; } = true;

        [SettingBoolean("{=setting_main_feature_less_children}Less Child", "{=setting_main_feature_less_children_desc}Description: More less child. Only for player. Add a interval time when child is born. The children count is more bigger, the interval time is more bigger.", true)]
        public bool LessChildren { get; set; } = true;

        [SettingBoolean("{=setting_main_feature_workShop_no_confiscate}Workshop No Confiscated", "{=setting_main_feature_workShop_no_confiscate_desc}Description: Workshop will not be confiscated when war.", true)]
        public bool WorkshopNoConfiscate { get; set; } = true;

        [SettingNumeric("{=setting_main_feature_PartyPrisonerSizeLimit}party prisoner size limit", "{=setting_main_feature_PartyPrisonerSizeLimit_desc}Description: If you set value to 5,meaning add 0.05% size limit by player per rogue skill.", 5, 0, 100)]
        public float PartyPrisonerSizeLimitBySkill { get; set; } = 5;

        [SettingNumeric("{=setting_main_feature_LevelsPerAttributePoint}Levels Per Attribute Point", "{=setting_main_feature_LevelsPerAttributePoint_desc}Description: Vanilla game every 4 levels gain one attribute, not you can set it.", 4f, 1, 4)]
        public float LevelsPerAttributePoint { get; set; } = 1;

        [SettingNumeric("{=setting_main_feature_companion_limit}Companion Limit", "{=setting_main_feature_companion_limit_desc}Description: every clan tier add companion limit.", 1, 1, 10)]
        public float CompanionLimit { get; set; } = 2f;

        [SettingNumeric("{=setting_main_feature_HideoutBattlePlayerMaxTroopCount}Hideout Max Troop Count", "{=setting_main_feature_HideoutBattlePlayerMaxTroopCount_desc}Description: Every clan lier add extra people number of attacking hideout.", 0, 1, 10)]
        public float HideoutBattlePlayerMaxTroopCount { get; set; } = 3f;

    }
}
