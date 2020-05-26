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
    public class MainSetting : SettingBase
    {
        public static MainSetting Instance { get; private set; }

        public MainSetting()
        {
            Instance = this;
        }
        public override int Order { get; set; } = 0;
        public override string Name { get; set; } = "{=setting_main}Main";

        [SettingBlank("{=setting_main_feature_total_upgrade_recruit}Total Upgrade Recruit", "{=setting_main_feature_total_upgrade_recruit_desc}Description: In player troop UI, can uprade all troop, and recruit all prionser. if troop has 2 next tier. then order it to trooplist header.")]
        public DateTime TotalUpradeAndRecruit { get; set; }

        [SettingBlank("{=setting_main_feature_leaningSkill}Leaning Skill", "{=setting_main_feature_leaningSkill_leaning_skill}Description: As you can see in one tab, set skill componion leaning from other.")]
        public DateTime LearningSkill { get; set; }

        [SettingBlank("{=setting_main_feature_clan_relation}Clan Relation Management", "{=setting_main_feature_leaningSkill_clan_relation_desc}Description: As you can see in one tab, can let one companion to enhance relation between player and clan leader, but this cost some money.")]
        public DateTime ClanRelation { get; set; }

        [SettingBlank("{=setting_main_feature_main_quest_expiration_time}Longer Expiration Time Of Main Quest.", "{=setting_main_feature_main_quest_expiration_time_desc}Description: vanilla game main quest will failed after ten years, not it will failed after 100 years.")]
        public DateTime MainQuestExpirationTime { get; set; }

        [SettingBlank("{=setting_main_feature_workshop_no_produce_player_craft}Workshop No Produce Weapon Has Been Crafed By Player if the weapon name contains * character.", "{=setting_main_feature_workshop_no_produce_player_craft_desc}Description: Workshop no produce weapon has been crafed by player. also not appear in tournament.")]
        public DateTime WorkShopNoProducePlayerCraft { get; set; }


        [SettingBlank("{=setting_main_feature_fix_companion_formation}Fix companion formation", "{=setting_main_feature_fix_companion_formation_desc}Description: Vanilla reset companion formation when reload. Now it is fixed.")]
        public DateTime FixCompanionFormation { get; set; }

        [SettingBlank("{=setting_main_feature_no_smelt_fixed_weapon}Smelt No Fixed Weapon", "{=setting_main_feature_no_smelt_fixed_weapon_desc}Description: Fixed weapon will not be semlted.")]
        public DateTime NoSmeltFixedWeapon { get; set; }


        [SettingBlank("{=setting_main_feature_perks}Perks", "{=setting_main_feature_perks_desc}Description: Implement using longbow on horse perk and using crossbow on horse perk. Implement all perks about adding arrow and bolt limit. Implement all perks about ScoutSpeed.")]
        public DateTime Perks { get; set; }



    }
}
