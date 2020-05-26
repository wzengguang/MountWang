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
    public class PrisonerRecruitChanceSetting : SettingBase
    {
        public static PrisonerRecruitChanceSetting Instance { get; private set; }

        public PrisonerRecruitChanceSetting()
        {
            Instance = this;
        }

        public float[] Chances()
        {
            return new float[] { Tier0, Tier1, Tier2, Tier3, Tier4, Tier5, Tier6 };
        }

        public override string Name { get; set; } = "{=setting_prisorner_recruit}Recruit Prisorner Chance";


        [SettingBoolean("{=setting_town_recruit_prisoner}Enable Town Recruit Prisoner", "{=setting_town_recruit_prisoner_desc}Description: Enable town recruit prisoner. But has chance failed, failed chance of player town is 50%, ai is 20%.Town recurit chance is as same as party, below you set. Change this setting need  restart game.", true)]
        public bool TownRecruitIsEnabled { get; set; } = true;


        [SettingNumeric("{=setting_prisorner_recruit_tier0}Tier0 Chance", "{=setting_prisorner_recruit_tier0_desc}Description: Recruit prisorner chance of tier0, default 1.", 1f, 0f, 1f, false)]
        public float Tier0 { get; set; } = 1f;

        [SettingNumeric("{=setting_prisorner_recruit_tier1}Tier1 Chance", "{=setting_prisorner_recruit_tier1_desc}Description: Recruit prisorner chance of tier1, default 0.5.", 0.5f, 0f, 1f, false)]
        public float Tier1 { get; set; } = 0.5f;

        [SettingNumeric("{=setting_prisorner_recruit_tier2}Tier2 Chance", "{=setting_prisorner_recruit_tier2_desc}Description: Recruit prisorner chance of tier2, default 0.3.", 0.3f, 0f, 1f, false)]
        public float Tier2 { get; set; } = 0.3f;

        [SettingNumeric("{=setting_prisorner_recruit_tier3}Tier3 Chance", "{=setting_prisorner_recruit_tier3_desc}Description: Recruit prisorner chance of tier4, default:0.2.", 0.2f, 0f, 1f, false)]
        public float Tier3 { get; set; } = 0.2f;

        [SettingNumeric("{=setting_prisorner_recruit_tier4}Tier4 Chance", "{=setting_prisorner_recruit_tier4_desc}Description: Recruit prisorner chance of tier4, default 0.1.", 0.1f, 0f, 1f, false)]
        public float Tier4 { get; set; } = 0.1f;

        [SettingNumeric("{=setting_prisorner_recruit_tier5}Tier5 Chance", "{=setting_prisorner_recruit_tier5_desc}Description: Recruit prisorner chance of tier5, default 0.", 0.05f, 0f, 1f, false)]
        public float Tier5 { get; set; } = 0.05f;

        [SettingNumeric("{=setting_prisorner_recruit_tier6}Tier6 Chance", "{=setting_prisorner_recruit_tier6_desc}Description: Recruit prisorner chance of tier6, default 0.", 0.05f, 0f, 1f, false)]
        public float Tier6 { get; set; } = 0.05f;

    }
}
