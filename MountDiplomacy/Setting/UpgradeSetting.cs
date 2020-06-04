using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaleWorlds.CampaignSystem;
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
    public class UpgradeSetting : SettingBase
    {
        public static UpgradeSetting Instance { get; private set; }

        public UpgradeSetting()
        {
            Instance = this;
        }
        public override int Order { get; set; } = 99;
        public override string Name { get; set; } = "{=setting_upgrade}Upgrade Setting";


        [SettingBoolean("{=setting_upgrade_enable}Enable Upgrade Setting", "{=setting_upgrade_enable_desc}Description: When upgrade troop having two target, you can set which target will automatic select. When troop two target have been set, it will be random.", true)]
        public bool IsEnabled { get; set; } = true;

        [SettingString("{=setting_upgrade_troop1}Troop1", null, nameof(TopTroopsSelector))]
        public string Troop1 { get; set; }

        [SettingString("{=setting_upgrade_troop2}Troop2", null, nameof(TopTroopsSelector))]
        public string Troop2 { get; set; }


        [SettingString("{=setting_upgrade_troop3}Troop3", null, nameof(TopTroopsSelector))]
        public string Troop3 { get; set; }


        [SettingString("{=setting_upgrade_troop4}Troop4", null, nameof(TopTroopsSelector))]
        public string Troop4 { get; set; }


        [SettingString("{=setting_upgrade_troop5}Troop5", null, nameof(TopTroopsSelector))]
        public string Troop5 { get; set; }


        [SettingString("{=setting_upgrade_troop6}Troop6", null, nameof(TopTroopsSelector))]
        public string Troop6 { get; set; }


        [SettingString("{=setting_upgrade_troop7}Troop7", null, nameof(TopTroopsSelector))]
        public string Troop7 { get; set; }


        [SettingString("{=setting_upgrade_troop8}Troop8", null, nameof(TopTroopsSelector))]
        public string Troop8 { get; set; }


        [SettingString("{=setting_upgrade_troop9}Troop9", null, nameof(TopTroopsSelector))]
        public string Troop9 { get; set; }


        [SettingString("{=setting_upgrade_troop10}Troop10", null, nameof(TopTroopsSelector))]
        public string Troop10 { get; set; }

        public int FindUpgradeTopInSetting(CharacterObject characterObject)
        {
            var targets = new List<int>();

            for (int i = 0; i < characterObject.UpgradeTargets.Length; i++)
            {
                var tops = GetCharacterObjectUpgradeTops(characterObject.UpgradeTargets[i]);

                if (tops.Select(a => a.StringId).Intersect(AllTargetTopTroop).Any())
                {
                    targets.Add(i);
                }
            }

            if (targets.Count > 1)
            {
                var random = new Random().Next(0, targets.Count + 1);
                return random;
            }
            else if (targets.Count == 1)
            {
                return targets[0];
            }
            else
            {
                return -1;
            }
        }


        private List<CharacterObject> GetCharacterObjectUpgradeTops(CharacterObject characterObject)
        {

            List<CharacterObject> tops = new List<CharacterObject>();

            List<CharacterObject> currents = new List<CharacterObject> { characterObject };

            int s = 0;
            while (currents.Count > 0 && s++ < 10000)
            {
                var tempAdd = new List<CharacterObject>();
                for (int i = 0; i < currents.Count; i++)
                {
                    if (currents[i].UpgradeTargets == null || currents[i].UpgradeTargets.Length == 0)
                    {
                        tops.Add(currents[i]);
                    }
                    else
                    {
                        tempAdd.AddRange(currents[i].UpgradeTargets);
                    }
                }
                currents = tempAdd;
            }
            return tops;
        }


        [JsonIgnore]
        private IEnumerable<string> AllTargetTopTroop
        {
            get
            {
                return new List<string> { Troop1, Troop2, Troop3, Troop4, Troop5, Troop6, Troop7, Troop8, Troop9, Troop10 }.Where(a => !string.IsNullOrEmpty(a));
            }
        }


        [JsonIgnore]
        private Dictionary<string, TextObject> topTroops;
        [JsonIgnore]
        public Dictionary<string, TextObject> TopTroopsSelector
        {
            get
            {
                if (topTroops == null)
                {

                    topTroops = PrisonerSetting.GetAllTopTroop();
                }
                return topTroops;
            }
        }
    }
}
