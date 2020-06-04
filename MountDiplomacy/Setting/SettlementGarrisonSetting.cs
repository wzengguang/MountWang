using TaleWorlds.SaveSystem;
using Wang.Setting.Attributes;

namespace Wang.Setting
{
    public class SettlementGarrisonSetting : SettingBase
    {
        public static SettlementGarrisonSetting Instance { get; private set; }

        public SettlementGarrisonSetting()
        {
            Instance = this;
        }

        public override string Name { get; set; } = "{=setting_settlement_g}Settlement Garrison Setting";
        public override string Description { get; set; } = "{=setting_settlement_g_desc}settlement Garrison setting";

        [SettingBoolean("{=setting_settlement_g_leave}Disable Player Clan Leave Troop To Garrison", "{=setting_settlement_g_leave_desc}Description: Disable Player Clan Leave Troop To Garrison.", true)]
        public bool DisablePlayerClanLeaveTroopToGarrison { get; set; } = true;

    }
}
