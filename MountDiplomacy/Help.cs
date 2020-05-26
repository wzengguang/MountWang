using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Wang.Setting;

namespace Wang
{
    public static class Help
    {

        public static bool IsPlayerCraft(this ItemObject item)
        {
            return item.Name.ToString().Contains("*");
        }


        /// <summary>
        /// 必须在OnGame start 初始化该值为null。
        /// </summary>

        public static Dictionary<IFaction, List<Settlement>> Original;




        public static List<IFaction> CheckOwnSettlementOccupyedByFaction(IFaction faction)
        {
            List<IFaction> factions = new List<IFaction>();

            if (!faction.IsKingdomFaction || !GetSettlementOriginalOwner().ContainsKey(faction.MapFaction))
            {
                return factions;
            }

            foreach (var item in GetSettlementOriginalOwner()[faction.MapFaction])
            {
                if (item.OwnerClan.MapFaction != faction.MapFaction)
                {
                    if (!factions.Contains(item.OwnerClan.MapFaction))
                    {
                        factions.Add(item.OwnerClan.MapFaction);
                    }
                }
            }
            return factions;
        }


        public static Dictionary<IFaction, List<Settlement>> GetSettlementOriginalOwner()
        {
            if (Original != null && Original.Count == Kingdom.All.Where(a => a.Ruler != Hero.MainHero).Count())
            {
                return Original;
            }
            // InformationManager.DisplayMessage(new InformationMessage("Init SettlementOriginalOwner"));
            Original = new Dictionary<IFaction, List<Settlement>>();

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
                    Original.Add(item, new List<Settlement>());

                    foreach (var settlement in Settlement.All.Where(a => a.IsCastle || a.IsTown))
                    {
                        if (OriginalOwner[item.StringId].Contains(settlement.StringId))
                        {
                            Original[item].Add(settlement);
                        }

                    }
                }

            }

            if (Original.Count == 0)
            {
                Original = null;
            }

            return Original;
        }



        public static double GetFactionWarDuration(IFaction faction, IFaction faction2)
        {
            CampaignWar campaignWar = (from war in Campaign.Current.FactionManager.FindCampaignWarsBetweenFactions(faction, faction2)?.ToList()
                                       orderby war.StartDate - CampaignTime.Now descending
                                       select war).FirstOrDefault();
            double toDays = (CampaignTime.Now - campaignWar.StartDate).ToDays;
            return toDays;
        }
        public static List<IFaction> GetNearFactions(IFaction kingdom, IEnumerable<IFaction> kingdoms, int take = 4)
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



        /// <summary>
        /// 两国家在停战协定状态
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="faction2"></param>
        /// <returns></returns>
        public static bool AtTruce(IFaction faction, IFaction faction2)
        {
            // var days = FactionManager.GetDaysSinceTruceWithFaction(faction, faction2);

            var days = GetDaysSinceTruceWithFaction(faction, faction2);
            if ((days < DiplomacySetting.Instance.TruceDays) && days > 0)
            {
                return true;
            }
            return false;
        }

        public static float GetDaysSinceTruceWithFaction(IFaction faction, IFaction faction2)
        {
            IReadOnlyList<LogEntry> gameActionLogs = Campaign.Current.LogEntryHistory.GameActionLogs;
            for (int i = gameActionLogs.Count - 1; i > 0; i--)
            {
                LogEntry logEntry = gameActionLogs[0];
                if (logEntry is MakePeaceLogEntry && ((((MakePeaceLogEntry)logEntry).Faction1 == faction.MapFaction && ((MakePeaceLogEntry)logEntry).Faction2 == faction2.MapFaction) || (((MakePeaceLogEntry)logEntry).Faction1 == faction2.MapFaction && ((MakePeaceLogEntry)logEntry).Faction2 == faction.MapFaction)))
                {
                    var days = (float)(CampaignTime.Now.ToDays - logEntry.GameTime.ToDays);
                    return days;
                }
            }
            return 0f;
        }


        public static bool CanDeclareWar(IFaction faction, IFaction faction2, bool checkNears = false, bool checkTruce = false)
        {
            var atWars = Kingdom.All.Where(a => a != faction && a.IsAtWarWith(faction)).Count();
            var atWars2 = Kingdom.All.Where(a => a != faction2 && a.IsAtWarWith(faction2)).Count();

            var days = FactionManager.GetDaysSinceTruceWithFaction(faction, faction2);
            if (atWars > 1 || atWars2 > 2 || (checkTruce && AtTruce(faction, faction2)))
            {
                return false;
            }

            if (checkNears)
            {
                var nears = GetNearFactions(faction, Kingdom.All, 4);
                if (!nears.Exists(a => a.MapFaction == faction2.MapFaction))
                {
                    return false;
                }
            }

            var settlementByOccupyed = CheckOwnSettlementOccupyedByFaction(faction).ToList();

            var rate = (atWars > 1 ? 1f : (atWars == 1 ? 0.5f : 0.1f));
            if (settlementByOccupyed.Count != 0 && !settlementByOccupyed.Contains(faction2) && MBRandom.RandomFloat < rate)
            {
                return false;
            }

            return true;
        }


        public static bool HasMount(this Agent agent)
        {
            var item = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
            return item != null && item.HasHorseComponent;
        }

    }
}
