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
    public class DiplomacySetting : SettingBase
    {
        [JsonIgnore]
        private static Dictionary<IFaction, List<Settlement>> _factionOriginalSettlement;

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


        [SettingBoolean("{=setting_diplomacy_make_peace_strategy_plus}Make Peace Strategy Plus", "{=setting_diplomacy_make_peace_strategy_plus_desc}Description: Tweak vanilla making peace strategy.{newline}1: With the war days continue, more easy to make peace.{newline}2: The faction has more declaring war faction, the faction tend to make peace.{newline}3: the faction settlement occupyed by who, tend to less possible to make peace.", true)]
        public bool EnableMakePeaceStrategyPlus { get; set; } = true;

        [SettingNumeric("{=setting_diplomacy_peace_decision}Relation Effect Of MakePeace Decision", "{=setting_diplomacy_peace_decision_desc}Description: When player propose a peace decision. the relation between player and lord make a effect of lord support", 0f, 0f, 10, true)]
        public float RelationEffectOfMakePeaceDecision { get; set; } = 3f;



        public static bool CanDeclareWar(IFaction faction, IFaction faction2, bool checkNears = false, bool checkTruce = false)
        {
            var atWars = Kingdom.All.Where(a => a != faction && a.IsAtWarWith(faction)).Count();
            var atWars2 = Kingdom.All.Where(a => a != faction2 && a.IsAtWarWith(faction2)).Count();

            if (atWars > 1 || atWars2 > 2 || (checkTruce && DiplomacySetting.InTruce(faction, faction2)))
            {
                return false;
            }

            if (checkNears)
            {
                var nears = DiplomacySetting.GetNearFactionsWithFaction(faction, Kingdom.All, 4);
                if (!nears.Exists(a => a.MapFaction == faction2.MapFaction))
                {
                    return false;
                }
            }

            var settlementByOccupyed = GetAllFactionOccupyFactionSettlement(faction).ToList();

            var rate = (atWars > 1 ? 1f : (atWars == 1 ? 0.5f : 0.1f));
            if (settlementByOccupyed.Count != 0 && !settlementByOccupyed.Contains(faction2) && MBRandom.RandomFloat < rate)
            {
                return false;
            }

            return true;
        }

        public static List<IFaction> GetNearFactionsWithFaction(IFaction kingdom, IEnumerable<IFaction> kingdoms, int take = 4)
        {

            Dictionary<IFaction, float> distances = new Dictionary<IFaction, float>();
            List<Settlement> list = kingdom.Settlements.Where(a => a.IsTown || a.IsCastle).ToList();

            foreach (IFaction item in kingdoms)
            {
                if (item == kingdom)
                {
                    continue;
                }

                float num6 = 1000f;
                foreach (Settlement settlement in item.Settlements.Where(a => a.IsTown || a.IsCastle))
                {
                    foreach (Settlement settlement1 in list)
                    {
                        if (Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, settlement1, num6, out float distance))
                        {
                            num6 = distance;
                        }
                    }
                }
                distances.Add(item, num6);
            }
            var order = distances.OrderBy(a => a.Value).ToList();

            take = distances.Count > take ? take : distances.Count;
            return order.Select(a => a.Key).Take(take).ToList();

        }

        public static bool InTruce(IFaction faction, IFaction faction2)
        {
            var days = DiplomacySetting.GetElapsedDaysSinceTruceWithFaction(faction, faction2);
            if (days < DiplomacySetting.Instance.TruceDays)
            {
                return true;
            }
            return false;
        }

        public static float GetElapsedDaysSinceTruceWithFaction(IFaction faction, IFaction faction2)
        {
            var stance = faction.GetStanceWith(faction2);
            if (stance.IsNeutral)
            {
                return Math.Abs(stance.PeaceDeclarationDate.ElapsedDaysUntilNow);
            }
            return 0f;
        }

        public static List<IFaction> GetAllFactionOccupyFactionSettlement(IFaction faction)
        {
            return DiplomacySetting.GetFactionSettlementOccupyedByOtherFactions(faction).Keys.ToList();
        }

        public static List<Settlement> GetFactionSettlementOccupyedByFaction(IFaction faction, IFaction other)
        {
            var factions = DiplomacySetting.GetFactionSettlementOccupyedByOtherFactions(faction);

            if (factions.ContainsKey(other))
            {
                return factions[other];
            }
            return new List<Settlement>();
        }

        public static Dictionary<IFaction, List<Settlement>> GetFactionSettlementOccupyedByOtherFactions(IFaction faction)
        {
            Dictionary<IFaction, List<Settlement>> factions = new Dictionary<IFaction, List<Settlement>>();

            if (!faction.IsKingdomFaction || !DiplomacySetting.GetAllFactionOriginalSettlement().ContainsKey(faction.MapFaction))
            {
                return factions;
            }

            foreach (var item in DiplomacySetting.GetAllFactionOriginalSettlement()[faction.MapFaction])
            {
                if (item.OwnerClan.MapFaction != faction.MapFaction)
                {
                    if (!factions.ContainsKey(item.OwnerClan.MapFaction))
                    {
                        factions.Add(item.OwnerClan.MapFaction, new List<Settlement> { item });
                    }
                    else
                    {
                        factions[item.OwnerClan.MapFaction].Add(item);
                    }
                }
            }
            return factions;
        }


        public static Dictionary<IFaction, List<Settlement>> GetAllFactionOriginalSettlement()
        {
            if (_factionOriginalSettlement != null && _factionOriginalSettlement.Count == Kingdom.All.Where(a => a.Ruler != Hero.MainHero).Count())
            {
                return _factionOriginalSettlement;
            }
            // InformationManager.DisplayMessage(new InformationMessage("Init SettlementOriginalOwner"));
            _factionOriginalSettlement = new Dictionary<IFaction, List<Settlement>>();

            var OriginalOwner = new Dictionary<string, List<string>>();
            OriginalOwner.Add("aserai", new List<string>
                {
                    "castle_A1","castle_A2","castle_A3","castle_A4","castle_A5","castle_A6","castle_A7","castle_A8","castle_A9","town_A1","town_A2","town_A3","town_A4","town_A5","town_A6","town_A7","town_A8"
                });
            OriginalOwner.Add("khuzait", new List<string>
                {
                    "castle_K1","castle_K2","castle_K3","castle_K4","castle_K5","castle_K6","castle_K7","castle_K8","castle_K9","town_K1","town_K2","town_K3","town_K4","town_K5","town_K6"
                });
            OriginalOwner.Add("battania", new List<string>
                {
                    "castle_B1","castle_B2","castle_B3","castle_B4","castle_B5","castle_B6","castle_B7","castle_B8","town_B1","town_B2","town_B3","town_B4","town_B5"
                });
            OriginalOwner.Add("vlandia", new List<string>
                {
                    "castle_V1","castle_V2","castle_V3","castle_V4","castle_V5","castle_V6","castle_V7","castle_V8","town_V1","town_V2","town_V3","town_V5","town_V6","town_V7","town_V8","town_V9"
                });
            OriginalOwner.Add("sturgia", new List<string>
                {
                    "castle_S1","castle_S2","castle_S3","castle_S4","castle_S5","castle_S6","castle_S7","castle_S8","town_S1","town_S2","town_S3","town_S4","town_S5","town_S6","town_S7"
                });
            OriginalOwner.Add("empire_s", new List<string>
                {
                    "castle_ES1","castle_ES2","castle_ES3","castle_ES4","castle_ES5","castle_ES6","castle_ES7","castle_ES8","town_ES1","town_ES2","town_ES3","town_ES4","town_ES5","town_ES6","town_ES7"
                });
            OriginalOwner.Add("empire_w", new List<string>
                {
                    "castle_EW1","castle_EW2","castle_EW3","castle_EW4","castle_EW5","castle_EW6","castle_EW7","castle_EW8","town_EW1","town_EW2","town_EW3","town_EW4","town_EW5","town_EW6"
                });
            OriginalOwner.Add("empire", new List<string>
                {
                    "castle_EN1","castle_EN2","castle_EN3","castle_EN4","castle_EN5","castle_EN6","castle_EN7","castle_EN8","castle_EN9","town_EN1","town_EN2","town_EN3","town_EN4","town_EN5","town_EN6"
                });

            foreach (var item in Kingdom.All)
            {

                if (OriginalOwner.ContainsKey(item.StringId))
                {
                    _factionOriginalSettlement.Add(item, new List<Settlement>());

                    foreach (var settlement in Settlement.All.Where(a => a.IsCastle || a.IsTown))
                    {
                        if (OriginalOwner[item.StringId].Contains(settlement.StringId))
                        {
                            _factionOriginalSettlement[item].Add(settlement);
                        }

                    }
                }

            }

            if (_factionOriginalSettlement.Count == 0)
            {
                _factionOriginalSettlement = null;
            }

            return _factionOriginalSettlement;
        }

    }
}
