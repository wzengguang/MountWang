using HarmonyLib;
using Helpers;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Wang
{
    public static class BanditsCampaignBehaviorExtension
    {

        public static bool IsLooterFaction(this BanditsCampaignBehavior instance, IFaction faction)
        {
            var result = Traverse.Create(instance).Method("IsLooterFaction", new Type[1]
             {
            typeof(IFaction)
             }).GetValue(faction);

            return (bool)result;
        }
        public static Hideout SelectARandomHideout(this BanditsCampaignBehavior instance, Clan faction, bool isInfestedHideoutNeeded, bool sameFactionIsNeeded, bool selectingFurtherToOthersNeeded = false)
        {
            var result = Traverse.Create(instance).Method("SelectARandomHideout", new Type[4]
             {
            typeof(Clan),typeof(bool),typeof(bool),typeof(bool)
             }).GetValue(faction, isInfestedHideoutNeeded, sameFactionIsNeeded, selectingFurtherToOthersNeeded);

            return (Hideout)result;
        }
        public static int NumberOfMaxBanditPartiesAroundEachHideout(this BanditsCampaignBehavior instance)
        {
            return Traverse.Create(instance).Field<int>("_numberOfMaxBanditPartiesAroundEachHideout").Value;
        }

        public static int NumberOfMaximumBanditPartiesInEachHideout(this BanditsCampaignBehavior instance)
        {
            return Traverse.Create(instance).Field<int>("_numberOfMaximumBanditPartiesInEachHideout").Value;
        }

        public static int NumberOfMaxHideoutsAtEachBanditFaction(this BanditsCampaignBehavior instance)
        {
            return Traverse.Create(instance).Field<int>("_numberOfMaxHideoutsAtEachBanditFaction").Value;
        }

        public static int NumberOfMaximumLooterParties(this BanditsCampaignBehavior instance)
        {
            return Traverse.Create(instance).Field<int>("_numberOfMaximumLooterParties").Value;
        }
        public static int RadiusAroundPlayerPartySquared(this BanditsCampaignBehavior instance)
        {
            return Traverse.Create(instance).Field<int>("_radiusAroundPlayerPartySquared").Value;
        }

        public static Settlement SelectARandomSettlementForLooterParty(this BanditsCampaignBehavior instance)
        {
            if (MBRandom.RandomFloat < 0.5)
            {
                return null;
            }

            Dictionary<Settlement, int> banditBySettlement = new Dictionary<Settlement, int>();

            List<MobileParty> banditParties = new List<MobileParty>();

            foreach (var item in Clan.BanditFactions)
            {
                foreach (var party in item.Parties)
                {
                    if (party.HomeSettlement == null || party.HomeSettlement.IsHideout())
                    {
                        continue;
                    }
                    if (!banditBySettlement.ContainsKey(party.HomeSettlement))
                    {
                        banditBySettlement.Add(party.HomeSettlement, 1);
                        continue;
                    }
                    banditBySettlement[party.HomeSettlement] += 1;
                }
            }

            var possibles = new Dictionary<Settlement, float>();

            foreach (var settlement in Settlement.All)
            {

                if (settlement.IsTown)
                {
                    if (banditBySettlement.ContainsKey(settlement) && banditBySettlement[settlement] > 2)
                    {
                        continue;
                    }

                    if (settlement.IsStarving || settlement.Town.FoodStocks < 50 || settlement.Town.Security < 80)
                    {
                        possibles.Add(settlement, settlement.Town.FoodStocks * settlement.Town.Security);
                        continue;
                    }
                }

                if (settlement.IsVillage && settlement.IsRaided)
                {
                    if (banditBySettlement.ContainsKey(settlement) && banditBySettlement[settlement] > 1)
                    {
                        continue;
                    }
                    possibles.Add(settlement, 0);
                    continue;
                }
                if (MBRandom.RandomFloat < 0.05)
                {
                    possibles.Add(settlement, 0);
                }

            }
            if (possibles.Count == 0)
            {
                return null;
            }
            var candidit = possibles.OrderBy(a => a.Value).Take(possibles.Count > 10 ? Math.Max(10, possibles.Count / 2) : possibles.Count).Select(a => a.Key).ToList();

            var selected = candidit[new Random().Next(0, candidit.Count)];

            return selected;

        }
    }


    [HarmonyPatch(typeof(BanditsCampaignBehavior))]
    public class BanditsCampaignBehaviorPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch("SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement")]
        private static void PostfixActivate(BanditsCampaignBehavior __instance, int numberOfBanditsWillBeSpawned)
        {
            //(1+(n3*13+200-n2)*0.01,620)
            // InformationManager.DisplayMessage(new InformationMessage("SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement."));

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

            int num = __instance.NumberOfMaxBanditPartiesAroundEachHideout() + __instance.NumberOfMaximumBanditPartiesInEachHideout() + 1;
            int num2 = __instance.NumberOfMaxHideoutsAtEachBanditFaction() * num;
            int num3 = 0;
            foreach (Clan item3 in list)
            {
                num3 += item3.Parties.Count();
            }
            numberOfBanditsWillBeSpawned = Math.Max(0, Math.Min(numberOfBanditsWillBeSpawned, list.Count((Clan f) => !__instance.IsLooterFaction(f)) * num2 + __instance.NumberOfMaximumLooterParties() - num3));
            numberOfBanditsWillBeSpawned = (int)Math.Ceiling((float)numberOfBanditsWillBeSpawned * 0.667f) + MBRandom.RandomInt(numberOfBanditsWillBeSpawned / 3);
            for (int i = 0; i < numberOfBanditsWillBeSpawned; i++)
            {
                Clan clan = null;
                float num4 = 1f;
                for (int j = 0; j < list.Count; j++)
                {
                    float num5 = 1f;
                    if (__instance.IsLooterFaction(list[j]))
                    {
                        num5 = (float)list[j].Parties.Count() / (float)__instance.NumberOfMaximumLooterParties();
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



                BanditsCampaignBehaviorPatch.SpawnAPartyInFaction(__instance, clan);

                // SpawnAPartyInFaction(clan);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpawnAPartyInFaction")]
        private static void SpawnAPartyInFaction(BanditsCampaignBehavior __instance, Clan selectedFaction)
        {

            // InformationManager.DisplayMessage(new InformationMessage("SpawnAPartyInFaction."));

            PartyTemplateObject defaultPartyTemplate = selectedFaction.DefaultPartyTemplate;
            int numberOfCreated = defaultPartyTemplate.NumberOfCreated;
            defaultPartyTemplate.IncrementNumberOfCreated();
            Settlement settlement = null;
            if (__instance.IsLooterFaction(selectedFaction))
            {
                settlement = __instance.SelectARandomSettlementForLooterParty();
            }
            else
            {
                settlement = __instance.SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: true, sameFactionIsNeeded: true)?.Owner.Settlement;
                if (settlement == null)
                {
                    settlement = __instance.SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: false, sameFactionIsNeeded: true)?.Owner.Settlement;
                    if (settlement == null)
                    {
                        settlement = __instance.SelectARandomHideout(selectedFaction, isInfestedHideoutNeeded: false, sameFactionIsNeeded: false)?.Owner.Settlement;
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
            float num = 45f * (__instance.IsLooterFaction(selectedFaction) ? 1.5f : 1f);
            float num2 = 0f;


            if (BanditConfig.BanditMultiple == 1)
            {
                int count = 0;
                float gameProcess2 = MiscHelper.GetGameProcess();
                for (int j = 0; j < defaultPartyTemplate.Stacks.Count; j++)
                {
                    int numberToAdd = (int)(gameProcess2 * (float)(defaultPartyTemplate.Stacks[j].MaxValue - defaultPartyTemplate.Stacks[j].MinValue)) + defaultPartyTemplate.Stacks[j].MinValue;
                    count += numberToAdd * BanditConfig.BanditMultiple;
                }

                mobileParty.InitializeMobileParty(textObject, defaultPartyTemplate, settlement.GatePosition, num, num2, MobileParty.PartyTypeEnum.Bandit, count);

            }
            else
            {
                mobileParty.InitializeMobileParty(textObject, defaultPartyTemplate, settlement.GatePosition, num, num2, MobileParty.PartyTypeEnum.Bandit);

            }

            int num3 = 0;
            Vec2 vec = mobileParty.Position2D;
            while (vec.DistanceSquared(MobileParty.MainParty.Position2D) < __instance.RadiusAroundPlayerPartySquared())
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

            Traverse.Create(__instance).Method("InitBanditParty", new Type[4] { typeof(MobileParty), typeof(TextObject), typeof(Clan), typeof(Settlement) }).GetValue(mobileParty, textObject, selectedFaction, settlement);

            // InitBanditParty(mobileParty, textObject, selectedFaction, settlement);
            mobileParty.Name = selectedFaction.Name;
            mobileParty.Aggressiveness = 1f - 0.2f * MBRandom.RandomFloat;
            mobileParty.SetMovePatrolAroundPoint(settlement.IsTown ? settlement.GatePosition : settlement.Position2D);
        }



    }
}
