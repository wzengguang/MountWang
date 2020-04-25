using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Wang
{
    public class CustomBanditsCampaignBehavior : BanditsCampaignBehavior
    {
        private enum PlayerInteraction
        {
            None,
            Friendly,
            PaidOffParty,
            Hostile
        }


        private Dictionary<MobileParty, PlayerInteraction> _interactedBandits = new Dictionary<MobileParty, PlayerInteraction>();
        private bool _hideoutsAndBanditsAreInited;
        private int _numberOfMaximumLooterParties => Campaign.Current.Models.BanditDensityModel.NumberOfMaximumLooterParties;

        private float _radiusAroundPlayerPartySquared => MobileParty.MainParty.SeeingRange * MobileParty.MainParty.SeeingRange * 1.25f;

        private float _numberOfMinimumBanditPartiesInAHideoutToInfestIt => Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;

        private int _numberOfMaxBanditPartiesAroundEachHideout => Campaign.Current.Models.BanditDensityModel.NumberOfMaximumBanditPartiesAroundEachHideout;

        private int _numberOfMaxHideoutsAtEachBanditFaction => Campaign.Current.Models.BanditDensityModel.NumberOfMaximumHideoutsAtEachBanditFaction;

        private int _numberOfInitialHideoutsAtEachBanditFaction => Campaign.Current.Models.BanditDensityModel.NumberOfInitialHideoutsAtEachBanditFaction;

        private int _numberOfMaximumBanditPartiesInEachHideout => Campaign.Current.Models.BanditDensityModel.NumberOfMaximumBanditPartiesInEachHideout;

        public override void RegisterEvents()
        {
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, WeeklyTick);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyTick);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, OnAfterNewGameCreated);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnPartyDestroyed);

        }


        public new void HourlyTick()
        {
            if (MBNetwork.IsClient)
            {
                return;
            }
            if (!_hideoutsAndBanditsAreInited && _numberOfMaxHideoutsAtEachBanditFaction > 0)
            {
                InitHideoutsAndBandits();
            }
            if (Campaign.Current.IsNight)
            {
                List<Clan> list = Clan.BanditFactions.ToList();
                int num = _numberOfMaxBanditPartiesAroundEachHideout + _numberOfMaximumBanditPartiesInEachHideout + 1;
                int num2 = 0;
                int num3 = 0;
                foreach (Clan item in list)
                {
                    num2 += item.Parties.Count();
                }
                foreach (Settlement item2 in Settlement.All)
                {
                    if (item2.IsHideout() && item2.Hideout.IsInfested)
                    {
                        num3++;
                    }
                }
                int num4 = num3 * num;
                int num5 = Math.Min(1 + MBRandom.RoundRandomized((float)(num4 + _numberOfMaximumLooterParties - num2) * 0.01f), _numberOfMaxHideoutsAtEachBanditFaction * (_numberOfMaxBanditPartiesAroundEachHideout + _numberOfMaximumBanditPartiesInEachHideout + 1) + _numberOfMaximumLooterParties);
                if (num5 > 0)
                {
                    SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(num5);
                }
            }
        }


        private static bool IsLooterFaction(IFaction faction)
        {
            return !faction.Culture.CanHaveSettlement;
        }


        private void InitHideoutsAndBandits()
        {
            _hideoutsAndBanditsAreInited = true;
            int num = 0;
            foreach (Clan banditFaction in Clan.BanditFactions)
            {
                if (!IsLooterFaction(banditFaction))
                {
                    for (int i = 0; i < _numberOfInitialHideoutsAtEachBanditFaction; i++)
                    {
                        FillANewHideoutWithBandits(banditFaction);
                        num++;
                    }
                }
            }
            int num2 = (int)(0.5f * (float)(_numberOfMaxBanditPartiesAroundEachHideout * num + _numberOfMaximumLooterParties));
            if (num2 > 0)
            {
                SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(num2);
            }
        }
        private void FillANewHideoutWithBandits(Clan faction)
        {
            Hideout hideout = SelectARandomHideout(faction, isInfestedHideoutNeeded: false, sameFactionIsNeeded: true, selectingFurtherToOthersNeeded: true);
            if (hideout != null)
            {
                for (int i = 0; (float)i < _numberOfMinimumBanditPartiesInAHideoutToInfestIt; i++)
                {
                    AddBanditToHideout(hideout);
                }
            }
        }
        private void SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(int numberOfBanditsWillBeSpawned)
        {
            List<Clan> list = Clan.BanditFactions.ToList();
            Dictionary<Clan, int> dictionary = new Dictionary<Clan, int>(list.Count);
            foreach (Clan item in list)
            {
                dictionary.Add(item, 0);
            }
            foreach (Settlement item2 in Settlement.All)
            {
                if (item2.IsHideout() && item2.Hideout.IsInfested)
                {
                    dictionary[item2.OwnerClan]++;
                }
            }
            int num = _numberOfMaxBanditPartiesAroundEachHideout + _numberOfMaximumBanditPartiesInEachHideout + 1;
            int num2 = _numberOfMaxHideoutsAtEachBanditFaction * num;
            int num3 = 0;
            foreach (Clan item3 in list)
            {
                num3 += item3.Parties.Count();
            }
            numberOfBanditsWillBeSpawned = Math.Max(0, Math.Min(numberOfBanditsWillBeSpawned, list.Count((Clan f) => !IsLooterFaction(f)) * num2 + _numberOfMaximumLooterParties - num3));
            numberOfBanditsWillBeSpawned = (int)Math.Ceiling((float)numberOfBanditsWillBeSpawned * 0.667f) + MBRandom.RandomInt(numberOfBanditsWillBeSpawned / 3);
            for (int i = 0; i < numberOfBanditsWillBeSpawned; i++)
            {
                Clan clan = null;
                float num4 = 1f;
                for (int j = 0; j < list.Count; j++)
                {
                    float num5 = 1f;
                    if (IsLooterFaction(list[j]))
                    {
                        num5 = (float)list[j].Parties.Count() / (float)_numberOfMaximumLooterParties;
                    }
                    else
                    {
                        int num6 = dictionary[list[j]];
                        if (num6 > 0)
                        {
                            num5 = (float)list[j].Parties.Count() / (float)(num6 * num);
                        }
                    }
                    if (num5 < 1f && (clan == null || num5 < num4))
                    {
                        clan = list[j];
                        num4 = num5;
                    }
                }
                if (clan == null)
                {
                    break;
                }
                SpawnAPartyInFaction(clan);
            }
        }

        private void SpawnAPartyInFaction(Clan selectedFaction)
        {
            PartyTemplateObject defaultPartyTemplate = selectedFaction.DefaultPartyTemplate;
            int numberOfCreated = defaultPartyTemplate.NumberOfCreated;
            defaultPartyTemplate.IncrementNumberOfCreated();
            Settlement settlement = null;
            if (IsLooterFaction(selectedFaction))
            {
                settlement = SelectARandomSettlementForLooterParty();
            }
            else
            {
                settlement = SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: true, sameFactionIsNeeded: true)?.Owner.Settlement;
                if (settlement == null)
                {
                    settlement = SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: false, sameFactionIsNeeded: true)?.Owner.Settlement;
                    if (settlement == null)
                    {
                        settlement = SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: false, sameFactionIsNeeded: false)?.Owner.Settlement;
                    }
                }
            }
            MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(selectedFaction.StringId + "_" + numberOfCreated);
            if (settlement == null)
            {
                return;
            }
            TextObject textObject = selectedFaction.Name;
            if (Game.Current.IsDevelopmentMode)
            {
                textObject = new TextObject("{=V2czbgW3}{NUMBER_OF_BANDITS} {BANDIT_FACTION_NAME}");
                textObject.SetTextVariable("NUMBER_OF_BANDITS", numberOfCreated);
                textObject.SetTextVariable("BANDIT_FACTION_NAME", selectedFaction.Name);
            }
            float num = 45f * (IsLooterFaction(selectedFaction) ? 1.5f : 1f);
            float num2 = 0f;


            int count = 0;
            float gameProcess2 = MiscHelper.GetGameProcess();
            for (int j = 0; j < defaultPartyTemplate.Stacks.Count; j++)
            {
                int numberToAdd = (int)(gameProcess2 * (float)(defaultPartyTemplate.Stacks[j].MaxValue - defaultPartyTemplate.Stacks[j].MinValue)) + defaultPartyTemplate.Stacks[j].MinValue;
                count += numberToAdd * 3;
            }


            mobileParty.InitializeMobileParty(textObject, defaultPartyTemplate, settlement.GatePosition, num, num2, MobileParty.PartyTypeEnum.Bandit, count);
            int num3 = 0;
            Vec2 vec = mobileParty.Position2D;
            while (vec.DistanceSquared(MobileParty.MainParty.Position2D) < _radiusAroundPlayerPartySquared)
            {
                num3++;
                vec = mobileParty.FindReachablePointAroundPosition(vec, num, num2);
                if (num3 > 10)
                {
                    break;
                }
            }
            if (vec != mobileParty.Position2D)
            {
                mobileParty.Position2D = vec;
            }
            InitBanditParty(mobileParty, textObject, selectedFaction, settlement);
            mobileParty.Name = selectedFaction.Name;
            mobileParty.Aggressiveness = 1f - 0.2f * MBRandom.RandomFloat;
            mobileParty.SetMovePatrolAroundPoint(settlement.IsTown ? settlement.GatePosition : settlement.Position2D);
        }

        private void InitBanditParty(MobileParty banditParty, TextObject name, Clan faction, Settlement homeSettlement)
        {
            banditParty.Name = name;
            banditParty.Party.Owner = faction.Leader;
            banditParty.Party.Visuals.SetMapIconAsDirty();
            banditParty.HomeSettlement = homeSettlement;
            CreatePartyTrade(banditParty);
            foreach (ItemObject item in ItemObject.All)
            {
                if (item.IsFood)
                {
                    int num = IsLooterFaction(banditParty.MapFaction) ? 8 : 16;
                    int num2 = MBRandom.RoundRandomized((float)banditParty.MemberRoster.TotalManCount * (1f / (float)item.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                    if (num2 > 0)
                    {
                        banditParty.ItemRoster.AddToCounts(item, num2);
                    }
                }
            }
        }
        private static void CreatePartyTrade(MobileParty banditParty)
        {
            _ = banditParty.Party.TotalStrength;
            int initialGold = (int)(10f * (float)banditParty.Party.MemberRoster.TotalManCount * (0.5f + 1f * MBRandom.RandomFloat));
            banditParty.InitializePartyTrade(initialGold);
        }
        private Settlement SelectARandomSettlementForLooterParty()
        {

            var possibles = new Dictionary<Settlement, float>();

            foreach (var settlement in Settlement.All)
            {
                if (settlement.IsTown || settlement.IsCastle)
                {
                    if (settlement.IsStarving)
                    {
                        possibles.Add(settlement, 0);
                        continue;
                    }
                    var rate = (settlement.Town.FoodStocks * settlement.Town.Security);
                    if (rate < 2000)
                    {
                        possibles.Add(settlement, rate);
                    }
                }

                if (settlement.IsVillage && settlement.IsRaided)
                {
                    possibles.Add(settlement, 0);
                }

            }
            if (possibles.Count == 0 || MBRandom.RandomFloat < 0.2)
            {
                return null;
            }
            var candidit = possibles.OrderBy(a => a.Value).Take(possibles.Count > 10 ? Math.Max(10, possibles.Count / 2) : possibles.Count).Select(a => a.Key).ToList();

            var selected = candidit[new Random().Next(0, candidit.Count)];

            return selected;


            int num = 0;
            foreach (Settlement item in Settlement.All)
            {
                if (item.IsTown || item.IsVillage)
                {
                    int num2 = CalculateDistanceScore(item.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
                    num += num2;
                }
            }
            int num3 = MBRandom.RandomInt(num);
            foreach (Settlement item2 in Settlement.All)
            {
                if (item2.IsTown || item2.IsVillage)
                {
                    int num4 = CalculateDistanceScore(item2.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
                    num3 -= num4;
                    if (num3 <= 0)
                    {
                        return item2;
                    }
                }
            }
            return null;
        }

        private static int CalculateDistanceScore(float distance)
        {
            int result = 2;
            if (distance < 10000f)
            {
                result = 8;
            }
            else if (distance < 40000f)
            {
                result = 6;
            }
            else if (distance < 160000f)
            {
                result = 4;
            }
            else if (distance < 420000f)
            {
                result = 3;
            }
            return result;
        }
        private void OnPartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
        {
            if (_interactedBandits.ContainsKey(mobileParty))
            {
                _interactedBandits.Remove(mobileParty);
            }
        }

        private Hideout SelectARandomHideout(Clan faction, bool isInfestedHideoutNeeded, bool sameFactionIsNeeded, bool selectingFurtherToOthersNeeded = false)
        {
            int num = 0;
            float num2 = Campaign.AverageDistanceBetweenTwoTowns * 0.33f * Campaign.AverageDistanceBetweenTwoTowns * 0.33f;
            foreach (Settlement settlement in Campaign.Current.Settlements)
            {
                if (settlement.IsHideout() && (settlement.Culture.Equals(faction.Culture) || !sameFactionIsNeeded) && ((isInfestedHideoutNeeded && ((Hideout)settlement.GetComponent(typeof(Hideout))).IsInfested) || (!isInfestedHideoutNeeded && !((Hideout)settlement.GetComponent(typeof(Hideout))).IsInfested)))
                {
                    int num3 = 1;
                    if (selectingFurtherToOthersNeeded)
                    {
                        float num4 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        float num5 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        foreach (Settlement settlement2 in Campaign.Current.Settlements)
                        {
                            if (settlement2.IsHideout() && ((Hideout)settlement2.GetComponent(typeof(Hideout))).IsInfested)
                            {
                                float num6 = settlement.Position2D.DistanceSquared(settlement2.Position2D);
                                if (settlement.Culture == settlement2.Culture && num6 < num4)
                                {
                                    num4 = num6;
                                }
                                if (num6 < num5)
                                {
                                    num5 = num6;
                                }
                            }
                        }
                        num3 = (int)Math.Max(1f, num4 / num2 + 5f * (num5 / num2));
                    }
                    num += num3;
                }
            }
            int num7 = MBRandom.RandomInt(num);
            foreach (Settlement settlement3 in Campaign.Current.Settlements)
            {
                if (settlement3.IsHideout() && (settlement3.Culture.Equals(faction.Culture) || !sameFactionIsNeeded) && ((isInfestedHideoutNeeded && ((Hideout)settlement3.GetComponent(typeof(Hideout))).IsInfested) || (!isInfestedHideoutNeeded && !((Hideout)settlement3.GetComponent(typeof(Hideout))).IsInfested)))
                {
                    int num8 = 1;
                    if (selectingFurtherToOthersNeeded)
                    {
                        float num9 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        float num10 = Campaign.MapDiagonal * Campaign.MapDiagonal;
                        foreach (Settlement settlement4 in Campaign.Current.Settlements)
                        {
                            if (settlement4.IsHideout() && ((Hideout)settlement4.GetComponent(typeof(Hideout))).IsInfested)
                            {
                                float num11 = settlement3.Position2D.DistanceSquared(settlement4.Position2D);
                                if (settlement3.Culture == settlement4.Culture && num11 < num9)
                                {
                                    num9 = num11;
                                }
                                if (num11 < num10)
                                {
                                    num10 = num11;
                                }
                            }
                        }
                        num8 = (int)Math.Max(1f, num9 / num2 + 5f * (num10 / num2));
                    }
                    num7 -= num8;
                    if (num7 < 0)
                    {
                        return settlement3.GetComponent(typeof(Hideout)) as Hideout;
                    }
                }
            }
            return null;
        }
    }
}
