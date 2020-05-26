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
    public class BanditsSetting : SettingBase
    {
        public static BanditsSetting Instance { get; private set; }

        public BanditsSetting()
        {
            Instance = this;
        }
        public override int Order { get; set; } = 99;
        public override string Name { get; set; } = "{=setting_bandits}Bandits Setting";

        [SettingNumeric("{=setting_bandits_NumberOfMaximumLooterParties}Max Parties", "{=setting_bandits_NumberOfMaximumLooterParties_desc}Description: Number of maximum looter parties, default 300.", 300f, 0, 400f)]
        public float NumberOfMaximumLooterParties { get; set; } = 200;

        [SettingNumeric("{=setting_bandits_NumberOfMinimumBanditPartiesInAHideoutToInfestIt}Min Hideout Parties", "{=setting_bandits_NumberOfMinimumBanditPartiesInAHideoutToInfestIt_desc}Description: Number of minimum bandit parties in a hideout to InfestIt, default 2.", 2f, 0f, 10f)]
        public float NumberOfMinimumBanditPartiesInAHideoutToInfestIt { get; set; } = 2;
        [SettingNumeric("{=setting_bandits_NumberOfMaximumBanditPartiesInEachHideout}Max Hideout Parties", "{=setting_bandits_NumberOfMaximumBanditPartiesInEachHideout_desc}Description: Number of maximum bandit parties in each hideout, default 4.", 4f, 0f, 10f)]
        public float NumberOfMaximumBanditPartiesInEachHideout { get; set; } = 4;
        [SettingNumeric("{=setting_bandits_NumberOfMaximumBanditPartiesAroundEachHideout}Max Parties Around Hideout", "{=setting_bandits_NumberOfMaximumBanditPartiesAroundEachHideout_desc}Description: Number of maximum bandit parties around each hideout, default 8.", 8f, 0f, 10f)]
        public float NumberOfMaximumBanditPartiesAroundEachHideout { get; set; } = 8;
        [SettingNumeric("{=setting_bandits_NumberOfMaximumHideoutsAtEachBanditFaction}Max Hideout", "{=setting_bandits_NumberOfMaximumHideoutsAtEachBanditFaction_desc}Description: Number of maximum hideouts at each bandit faction, default 10.", 10f, 0f, 20f)]
        public float NumberOfMaximumHideoutsAtEachBanditFaction { get; set; } = 10;
        [SettingNumeric("{=setting_bandits_NumberOfInitialHideoutsAtEachBanditFaction}Initial Hideout", "{=setting_bandits_NumberOfInitialHideoutsAtEachBanditFaction_desc}Description: Number of initial hideouts at each bandit faction default 3.", 3f, 0f, 10f)]
        public float NumberOfInitialHideoutsAtEachBanditFaction { get; set; } = 3;

    }
}
