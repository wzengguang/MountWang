using Wang.Setting.Attributes;

namespace Wang.Setting
{
    public class SettlementSetting : SettingBase
    {
        public static SettlementSetting Instance { get; private set; }

        public SettlementSetting()
        {
            Instance = this;
        }

        public override string Name { get; set; } = "{=setting_settlement}Settlement Related Setting";
        public override string Description { get; set; } = "{=setting_settlement_desc}settlement related setting";

        [SettingBoolean("{=setting_settlement_millitia_is_enabled}Enable Settlement Millitia Tweak", "{=setting_settlement_millitia_desc}Description: Reduce decommissioning rate. Increase the amout of prosperity. Change it need to restart game.", true)]
        public bool MillitiaIsEnabled { get; set; } = true;

        [SettingBoolean("{=setting_settlement_prosperity_is_enabled}Enable Settlement Prosperity Tweak", "{=setting_settlement_prosperity_desc}Description: When prosperity more than 10000, reduce increasing speed.  Change it need to restart game.", true)]
        public bool ProsperityEnabled { get; set; } = true;


        [SettingNumeric("{=setting_settlement_prosperity_boost}boost Prosperity Growth Factor", "{=setting_settlement_prosperity_boost_desc}Description: When food enough, boost prosperity growth, the prosperity is more lower, the result effect is more power. Set 0 to disabled this function.", 0, 0, 20)]
        public float boostProsperityGrowth { get; set; } = 10;


        [SettingBoolean("{=setting_settlement_sacrifice_is_enabled}Enable Settlement Sacrifice Tweak", "{=setting_settlement_sacrifice_desc}Description: Break in a besieged settmentment without sacrificing troop.  Change it need to restart game.", true)]
        public bool SacrificeEnabled { get; set; } = true;


        [SettingNumeric("{=setting_settlement_elite_troop_rate}Elite Troop Multiple", "{=setting_settlement_elite_troop_rate_desc}Description: The multiple of spawn elite troop rate of millitia. default is 1. ", 1f, 1f, 5f)]
        public float EliteTroopRate { get; set; } = 2f;


        [SettingNumeric("{=setting_settlement_town_capital_factor}TownCapitalFactor", "{=setting_settlement_town_capital_factor_desc}Description: Town has more capital. The value means add how many multiple of vanilla. Set 0 to disable this function.", 0f, 0f, 10)]
        public float TownCapitalFactor { get; set; } = 1f;



        [SettingNumeric("{=setting_settlement_food_is_enabled}Enable Settlement Food Tweak", "{=setting_settlement_food_desc}Description: Prosperity reduce to this value.Set to 1 disable this function.", 1f, 0.1f, 1f, false)]
        public float ProsperityNeedFoodMultiple { get; set; } = 0.5f;
    }
}
