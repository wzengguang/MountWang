using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace EnhanceLordTroop
{
    public class AddXpToLordTroopBehaviour : CampaignBehaviorBase
    {
        private float[] _troopScale = new float[10] { 0.05f, 0.15f, 0.3f, 0.2f, 0.15f, 0.1f, 0.05f, 0f, 0f, 0f };

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, DailyTick);

        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_troopScale", ref _troopScale);
        }

        private void UpdateTroopScale()
        {
            var ratio = AddXpToLordTroopSetting.Instance == null ? XpMultiplierConfigBase.PartyTroopRatio : AddXpToLordTroopSetting.Instance.GetTierRatio();

            int daysUntilNow = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
            if (daysUntilNow % 7 != 0 ||
                daysUntilNow < 30 ||
                Clan.PlayerClan.Settlements == null ||
                Clan.PlayerClan.Settlements.Count() <= 0)
            {
                for (int i = 0; i < ratio.Length; i++)
                {
                    _troopScale[i] = ratio[i];
                }
                return;
            }

            var playerTierTroopCount = new float[10];

            var playerSettlementCount = 0;

            foreach (var settlement in Clan.PlayerClan.Settlements)
            {
                if (!(settlement.IsTown || settlement.IsCastle) || settlement.Town.GarrisonParty == null)
                {
                    continue;
                }
                playerSettlementCount++;

                foreach (var el in settlement.Town.GarrisonParty.MemberRoster.Troops)
                {
                    if (el.IsHero || el.Tier > playerTierTroopCount.Length - 1)
                    {
                        continue;
                    }
                    playerTierTroopCount[el.Tier] += settlement.Town.GarrisonParty.MemberRoster.GetTroopCount(el);
                }


            }

            foreach (var item in Clan.PlayerClan.Parties)
            {
                foreach (var el in item.MemberRoster.Troops)
                {
                    if (el.IsHero || el.Tier > playerTierTroopCount.Length - 1)
                    {
                        continue;
                    }
                    playerTierTroopCount[el.Tier] += item.MemberRoster.GetTroopCount(el);
                }
            }

            var total = playerTierTroopCount.Sum();

            for (int i = 0; i < playerTierTroopCount.Length; i++)
            {
                playerTierTroopCount[i] = playerTierTroopCount[i] / total;
            }

            if (Hero.MainHero.IsFactionLeader && Hero.MainHero.MapFaction != null && Hero.MainHero.MapFaction.Settlements != null)
            {
                playerSettlementCount = Hero.MainHero.MapFaction.Settlements.Where(a => a.IsTown || a.IsCastle).Count();
            }

            var scale = Math.Max(0.2f, Math.Min(1, playerSettlementCount * 5 / Town.AllTowns.Count));

            for (int i = 0; i < _troopScale.Length; i++)
            {
                _troopScale[i] = ratio[i];
            }

            for (int i = playerTierTroopCount.Length - 1; i > 0; i--)
            {
                var sub = scale * (playerTierTroopCount[i] - _troopScale[i]);
                var average = sub / (playerTierTroopCount.Length - 1);

                for (int x = 0; x < _troopScale.Length; x++)
                {
                    if (x == i)
                    {
                        _troopScale[i] += sub;
                        continue;
                    }
                    var xScale = _troopScale[x] - average;
                    if (xScale < 0)
                    {
                        _troopScale[i] += average;
                        continue;
                    }
                    _troopScale[x] = xScale;
                }
            }
        }

        private void DailyTick(MobileParty __instance)
        {
            if (!XpMultiplierConfigBase.AddTroopXpEnabled || __instance.IsBandit || !__instance.IsActive || __instance.LeaderHero == null || __instance.LeaderHero.MapFaction.IsBanditFaction || __instance == MobileParty.MainParty)
            {
                return;
            }

            if (AddXpToLordTroopSetting.Instance != null && !AddXpToLordTroopSetting.Instance.IsEnabled)
            {
                return;
            }


            //foreach (LogEntry item2 in Campaign.Current.LogEntryHistory.GameActionLogs.Reverse())
            //{
            //    IEncyclopediaLog encyclopediaLog;
            //    if ((encyclopediaLog = (item2 as IEncyclopediaLog)) != null && encyclopediaLog.IsVisibleInEncyclopediaPageOf(_hero))
            //    {
            //    }
            //}
            try
            {
                UpdateTroopScale();
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage("EnhanceLordTroop Mod ERROR: In UpdateTroopScale Method, you can notice Modder"));
            }



            float[] ratio = _troopScale;
            var xps = AddXpToLordTroopSetting.Instance == null ? XpMultiplierConfigBase.TierXps : AddXpToLordTroopSetting.Instance.GetTierXps();

            var total = 0f + __instance.MemberRoster.Where(a => !a.Character.IsHero).Select(a => a.Number).Sum();
            List<CharacterObject>[] troopOrder = new List<CharacterObject>[7] { new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>(), new List<CharacterObject>() };

            var troopsCount = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            foreach (var troop in __instance.MemberRoster.Troops)
            {
                if (troop.IsHero || troop.Tier > 6)
                {
                    continue;
                }

                troopOrder[troop.Tier].Add(troop);
                troopsCount[troop.Tier] += __instance.MemberRoster.GetTroopCount(troop);
            }

            for (int i = troopOrder.Length - 1; i > 0; i--)
            {
                if (troopsCount[i] / total < ratio[i])
                {
                    var scale = Math.Cos(1.47 * troopsCount[i] / (ratio[i] * total));

                    foreach (var troop in troopOrder[i - 1])
                    {
                        if (troop.UpgradeTargets != null && troop.UpgradeTargets.Length > 0)
                        {
                            var xp = (int)(scale * __instance.MemberRoster.GetTroopCount(troop) * xps[i - 1]);
                            __instance.Party.MemberRoster.AddXpToTroop(xp, troop);
                        }
                    }
                }
            }

        }


    }
}
