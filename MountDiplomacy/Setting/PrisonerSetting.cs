using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class PrisonerSetting : SettingBase
    {
        public static PrisonerSetting Instance { get; private set; }

        public PrisonerSetting()
        {
            Instance = this;
        }

        public override int Order { get; set; } = 2;

        public override string Name { get; set; } = "{=setting_prisorner}Prisorner Management";

        [SettingBoolean("{=setting_prisorner_is_active}Enable prisorner management", "{=setting_prisorner_is_active_desc}Description: When tranfer all other prioner in Party UI, then amount of tranfered will not exceed the prionser limit. you can use order setting to deterimine which troop has the first priority.{newline}The fixed troop contains who can upgrate to it.The fixed prisoner troop will has the most priority when tranfer all other prioner to player party.And will not be selled out when tranfer all player party prionser to other prioner.", true)]
        public bool IsEnabled { get; set; } = true;


        [SettingBoolean("{=setting_prisorner_enable_filter}Enable Filter By Order", "{=setting_prisorner_enable_filter_desc}Description: When tranfer all other prioner to player in Party UI, if true, only tranfer which is satisfied with order condition below.If false the order condition means prior tranfering ", true)]
        public bool EnableFilter { get; set; } = true;

        [SettingString("{=setting_prisioner_order1}First Order", null, nameof(Selector))]
        public string Order1 { get; set; } = "PrisonerSettingIsMountedArcher";

        [SettingNumeric("{=setting_prisioner_order1_min_tier}First Order Min Tier", "{=setting_prisioner_order1_desc}Description: First order of filter Troop, tier smaller than then min tier will be ignore.", 1f, 0f, 6f)]
        public float Order1MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_order2}Second Order", null, nameof(Selector))]
        public string Order2 { get; set; } = "PrisonerSettingTier";

        [SettingNumeric("{=setting_prisioner_order2_min_tier}Second Order Min Tier", "{=setting_prisioner_order2_desc}Description: Second order of filter Troop, tier smaller than then min tier will be ignore.", 1f, 0f, 6f)]
        public float Order2MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_order3}Third Order", null, nameof(Selector))]
        public string Order3 { get; set; } = "PrisonerSettingIsMounted";

        [SettingNumeric("{=setting_prisioner_order3_min_tier}Third Order Min Tier", "{=setting_prisioner_order3_desc}Description: Third order of filter Troop, tier smaller than then min tier will be ignore.", 1f, 0f, 6f)]
        public float Order3MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_order4}Fourth Order", null, nameof(Selector))]
        public string Order4 { get; set; } = "PrisonerSettingIsArcher";

        [SettingNumeric("{=setting_prisioner_order4_min_tier}Fourth Order Min Tier", "{=setting_prisioner_order4_desc}Description: Fourth order of filter Troop, tier smaller than then min tier will be ignore.", 1f, 0f, 6f)]
        public float Order4MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_fixed1}First Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed1 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed1_min}First Fixed Troop Min Tier", "{=setting_prisioner_fixed1_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed1MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_fixed2}Second Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed2 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed2_min}Second Fixed Troop Min Tier", "{=setting_prisioner_fixed2_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed2MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_fixed3}Third Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed3 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed3_min}Third Fixed Troop Min Tier", "{=setting_prisioner_fixed3_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed3MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_fixed4}Fourth Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed4 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed4_min}Fourth Fixed Troop Min Tier", "{=setting_prisioner_fixed4_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed4MinTier { get; set; } = 1;

        [SettingString("{=setting_prisioner_fixed5}Fifth Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed5 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed5_min}Fifth Fixed Troop Min Tier", "{=setting_prisioner_fixed5_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed5MinTier { get; set; } = 1;

        [SettingString("{=setting_prisioner_fixed6}Sixth Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed6 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed6_min}Sixth Fixed Troop Min Tier", "{=setting_prisioner_fixed6_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed6MinTier { get; set; } = 1;

        [SettingString("{=setting_prisioner_fixed7}Seventh Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed7 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed7_min}Seventh Fixed Troop Min Tier", "{=setting_prisioner_fixed7_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed7MinTier { get; set; } = 1;

        [SettingString("{=setting_prisioner_fixed8}Eighth Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed8 { get; set; }

        [SettingNumeric("{=setting_prisioner_fixed8_min}Eighth Fixed Troop Min Tier", "{=setting_prisioner_fixed8_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed8MinTier { get; set; } = 1;


        [SettingString("{=setting_prisioner_fixed9}Ninth Fixed Troop", null, nameof(TopTroopsSelector))]
        public string Fixed9 { get; set; }
        [SettingNumeric("{=setting_prisioner_fixed9_min}Ninth Fixed Troop Min Tier", "{=setting_prisioner_fixed9_min_desc}tranfer all action transfer prior to player Troop or not transfer out. Tie must bigger than this value or equal.", 1f, 0f, 6f)]
        public float Fixed9MinTier { get; set; } = 1;

        [JsonIgnore]
        public IReadOnlyList<string> Selector { get { return new List<string> { "{=PrisonerSettingTier}Tier", "{=PrisonerSettingIsArcher}IsArcher", "{=PrisonerSettingIsMounted}IsMounted", "{=PrisonerSettingIsInfantry}Infantry", "{=PrisonerSettingIsMountedArcher}MountedArcher" }; } }

        [JsonIgnore]
        private Dictionary<CharacterObject, List<CharacterObject>> _findTopTroopCache = new Dictionary<CharacterObject, List<CharacterObject>>();

        public int OrderTroop(CharacterObject character, string order, float minTier = 0)
        {
            switch (order)
            {
                case "PrisonerSettingTier":
                    return character.Tier < minTier ? 0 : character.Tier;
                case "PrisonerSettingIsMounted":
                    return character.IsMounted && !character.IsArcher && character.Tier >= minTier ? 1 : 0;
                case "PrisonerSettingIsArcher":
                    return character.IsArcher && character.Tier >= minTier ? 1 : 0;
                case "PrisonerSettingIsInfantry":
                    return character.IsInfantry && character.Tier >= minTier ? 1 : 0;
                case "PrisonerSettingIsMountedArcher":
                    return character.IsMounted && character.IsArcher && character.Tier >= minTier ? 1 : 0;
            }
            return 0;
        }


        public int IsFixTroop(CharacterObject character)
        {
            var fixedTroop = new List<string> { Fixed1, Fixed2, Fixed3, Fixed4, Fixed5, Fixed6, Fixed7, Fixed8, Fixed9 };
            var fixMinTier = new float[] { Fixed1MinTier, Fixed2MinTier, Fixed3MinTier, Fixed4MinTier, Fixed5MinTier, Fixed6MinTier, Fixed7MinTier, Fixed8MinTier, Fixed9MinTier };
            var tops = FindTopTroop(character);

            for (int i = 0; i < fixedTroop.Count; i++)
            {
                if (tops.Exists(a => a.StringId == fixedTroop[i]) && character.Tier >= fixMinTier[i])
                {
                    return fixedTroop.Count - i;
                }
            }
            return 0;
        }

        private List<CharacterObject> FindTopTroop(CharacterObject character)
        {
            if (!_findTopTroopCache.ContainsKey(character))
            {
                var tops = new List<CharacterObject>();
                FindTopTroop(character, tops);
                _findTopTroopCache.Add(character, tops);
            }
            return _findTopTroopCache[character];

        }


        private void FindTopTroop(CharacterObject character, List<CharacterObject> characters)
        {
            if ((character.UpgradeTargets == null || character.UpgradeTargets.Length == 0))
            {
                characters.Add(character);
            }
            else
            {
                foreach (var item in character.UpgradeTargets)
                {
                    FindTopTroop(item, characters);
                }

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

                    Func<CharacterObject, int> orderby = character =>
                     {

                         var culture = (int)character.Culture.GetCultureCode();
                         if (culture < 0)
                         {
                             culture = 1000;
                         }

                         if (character.Culture.IsMainCulture)
                         {
                             return culture;
                         }
                         else if (character.Culture.IsBandit)
                         {
                             return 10 * culture;
                         }
                         else
                         {
                             return 100 * culture;
                         }
                     };

                    topTroops = new Dictionary<string, TextObject>();
                    foreach (var character in Find().OrderBy(orderby).ThenBy(a => a.DefaultFormationGroup))
                    {
                        topTroops.Add(character.StringId, character.Name);
                    }
                }
                return topTroops;
            }
        }

        private HashSet<CharacterObject> Find()
        {
            HashSet<CharacterObject> characterObjects = new HashSet<CharacterObject>();

            foreach (var item in CharacterObject.All.Where(a => IsTroop(a)))
            {
                if (item.UpgradeTargets != null && item.UpgradeTargets.Length > 0)
                {
                    foreach (var next in item.UpgradeTargets)
                    {
                        if (next.UpgradeTargets == null || next.UpgradeTargets.Length == 0)
                        {
                            characterObjects.Add(next);
                        }
                    }
                }
            }


            return characterObjects;
        }


        private bool IsTroop(CharacterObject character)
        {
            return character != null && !character.IsTemplate && (character.Occupation == Occupation.Soldier || character.Occupation == Occupation.Mercenary || character.Occupation == Occupation.Bandit || character.Occupation == Occupation.Gangster || character.Occupation == Occupation.CaravanGuard);
        }



    }
}
