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
using Wang.Setting;
using Wang.Setting.Attributes;

namespace EnhanceLordTroop
{
    public class AddXpToLordTroopSetting : SettingBase
    {
        public static AddXpToLordTroopSetting Instance { get; private set; }

        public AddXpToLordTroopSetting()
        {
            Instance = this;
        }
        public override string Name { get; set; } = "{=setting_add_xp_to_lord_troop}Add Xp to Lord Troop";
        public int[] GetTierXps()
        {
            return new int[] { (int)Tier1, (int)Tier2, (int)Tier3, (int)Tier4, (int)Tier5, (int)Tier6, (int)Tier6 };
        }

        public float[] GetTierRatio()
        {
            return new float[] { Tier1Ratio, Tier2Ratio, Tier3Ratio, Tier4Ratio, Tier5Ratio, Tier6Ratio, 0.05f, 0, 0, 0 };
        }

        [SettingBoolean("{=setting_add_xp_to_lord_troop_is_enabled}Enable This Tweak", "{=setting_add_xp_to_lord_troop_is_enabled_desc}Enable: Add xp to lord troops daily. every tier troop get the amount of xp base on you set below. the final ratio of each troop according to you set below. Thus, when tie1 troop ratio is more than you set, tie1 troop will not gain xp from this tweak.", true)]
        public bool IsEnabled { get; set; } = true;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier1}tier1 gain xp", null, 50f, 10f, 100f)]
        public float Tier1 { get; set; } = 50f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier2}tier2 gain xp", null, 100f, 10f, 1000f)]
        public float Tier2 { get; set; } = 100f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier3}tier3 gain xp", null, 100f, 10f, 1000f)]
        public float Tier3 { get; set; } = 100f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier4}tier4 gain xp", null, 100f, 10f, 1000f)]
        public float Tier4 { get; set; } = 100f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier5}tier5 gain xp", null, 100f, 10f, 1000f)]
        public float Tier5 { get; set; } = 100f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier6}tier6 gain xp", "{=setting_add_xp_to_lord_troop_tier_desc}The xp of every tier troop gained.", 100f, 10f, 1000f)]
        public float Tier6 { get; set; } = 100f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier1_ratio}tier1 ratio", null, 0.05f, 0f, 1f, false)]
        public float Tier1Ratio { get; set; } = 0.05f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier2_ratio}tier2 ratio", null, 0.15f, 0f, 1f, false)]
        public float Tier2Ratio { get; set; } = 0.15f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier3_ratio}tier3 ratio", null, 0.3f, 0f, 1f, false)]
        public float Tier3Ratio { get; set; } = 0.3f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier4_ratio}tier4 ratio", null, 0.2f, 0f, 1f, false)]
        public float Tier4Ratio { get; set; } = 0.2f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier5_ratio}tier5 ratio", null, 0.15f, 0f, 1f, false)]
        public float Tier5Ratio { get; set; } = 0.15f;

        [SettingNumeric("{=setting_add_xp_to_lord_troop_tier6_ratio}tier6 ratio", "{=setting_add_xp_to_lord_troop_tier_ratio_desc}The final proportion of each tier troop. The troop which proportion exceed value can not gain xp from this setting.", 0.1f, 0f, 1f, false)]
        public float Tier6Ratio { get; set; } = 0.1f;

    }
}
