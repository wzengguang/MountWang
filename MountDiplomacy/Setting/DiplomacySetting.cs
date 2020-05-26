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
    public class DiplomacySetting : SettingBase
    {
        public static DiplomacySetting Instance { get; private set; }

        public DiplomacySetting()
        {
            Instance = this;
        }
        public override string Name { get; set; } = "{=setting_diplomacy}Kingdom Diplomacy";




        [SettingBoolean("{=setting_diplomacy_smart_chose_faction_to_declare_war}smart chose faction of declaring war", "{=setting_diplomacy_smart_chose_faction_to_declare_war_desc}Now in this mod：{newline}1. Ai will not declare war on countries in truce.{newline}2. Ai prefers to fight against countries that have occupied their own town (castle).{newline}3. Ai prefers to fight against the four closest countries.{newline}4. Countries with the same culture are less likely to fight each other.{newline}5. Adjusted the maximum number of ai declared war to prevent beating, max value is 3.{newline}6. If Ai is in war on a country, if the ai army is stronger than the country, ai territory is bigger than the belligerent country. Ai will declare war on the second, otherwise, ai will not proactively declare war on the second belligerent country.{newline}7. Large countries is more inclined to fight against multiple weak countries at the same time.{newline}8.Ai who is more powerfull,is more likely besieged by multiple weaker countries.")]
        public bool EnableSmartChoseFactionToDeclareWar { get; set; } = true;


        [SettingBoolean("{=setting_diplomacy_clan_job_hop}Disabled Clan Job-Hop", "{=setting_diplomacy_clan_job_hop_desc}Description: Disabled ai clan job-hop between kingdom. not worked to palyer.", true)]
        public bool DisableClanJobHop { get; set; } = true;

        /// <summary>
        /// 停战时间
        /// </summary>
        [SettingNumeric("{=setting_truce_days}Truce Days", "{=setting_truce_days_desc}Description: When two faction make peace, after this value days possible to declare war.", 20f, 1f, 100f)]
        public float TruceDays { get; set; } = 20;

        /// <summary>
        /// 战败后被释放，多少天后重新出现。
        /// </summary>
        [SettingNumeric("{=setting_diplomacy_lord_reform}Lord Reform Days", "{=setting_diplomacy_lord_reform_desc}Description: When a lord is defeated and released by player. after these days, the lord will be come out with leading his troop. Default value is 3.", 3f, 3f, 100f)]
        public float PrisonerDaysLeftToRespawn { get; set; } = 7;

    }
}
