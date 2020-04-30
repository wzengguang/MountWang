using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Wang.Perks
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    class ScoutPartySpeedPatch
    {

        private static void Postfix(ref float __result, MobileParty mobileParty, float baseSpeed, StatExplainer explanation)
        {
            if (mobileParty.LeaderHero == null)
            {
                return;
            }

            var leader = mobileParty.LeaderHero;
            PartyBase party = mobileParty.Party;
            ExplainedNumber bonuses = new ExplainedNumber(__result, explanation);
            TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
            switch (faceTerrainType)
            {
                case TerrainType.Forest:
                    if (leader.GetPerkValue(DefaultPerks.Scouting.ForestLore))
                    {
                        bonuses.AddFactor(baseSpeed * 0.05f, DefaultPerks.Scouting.ForestLore.Name);
                    }
                    break;
                case TerrainType.Desert:
                    if (leader.GetPerkValue(DefaultPerks.Scouting.DesertLore))
                    {
                        bonuses.AddFactor(baseSpeed * 0.05f, DefaultPerks.Scouting.DesertLore.Name);
                    }
                    break;
                case TerrainType.Mountain:
                    if (leader.GetPerkValue(DefaultPerks.Scouting.HillsLore))
                    {
                        bonuses.AddFactor(baseSpeed * 0.05f, DefaultPerks.Scouting.HillsLore.Name);
                    }

                    break;
                case TerrainType.Swamp:
                    if (leader.GetPerkValue(DefaultPerks.Scouting.MarshesLore))
                    {
                        bonuses.AddFactor(baseSpeed * 0.05f, DefaultPerks.Scouting.MarshesLore.Name);
                    }

                    break;
                case TerrainType.Water:
                case TerrainType.Bridge:
                case TerrainType.River:
                case TerrainType.ShallowRiver:


                    break;
            }
            //第一层
            if (!Campaign.Current.IsNight)
            {
                if (leader.GetPerkValue(DefaultPerks.Scouting.Pathfinder) || (mobileParty.Scout != null && mobileParty.Scout.GetPerkValue(DefaultPerks.Scouting.Pathfinder)))
                {
                    bonuses.AddFactor(baseSpeed * 0.01f, DefaultPerks.Scouting.Pathfinder.Name);
                }
            }
            else
            {
                if (leader.GetPerkValue(DefaultPerks.Scouting.TorchCarriers) || (mobileParty.Scout != null && mobileParty.Scout.GetPerkValue(DefaultPerks.Scouting.TorchCarriers)))
                {
                    bonuses.AddFactor(baseSpeed * 0.02f, DefaultPerks.Scouting.TorchCarriers.Name);
                }
            }
            //2
            if (leader.GetPerkValue(DefaultPerks.Scouting.Navigator) || (mobileParty.Scout != null && mobileParty.Scout.GetPerkValue(DefaultPerks.Scouting.Navigator)))
            {
                bonuses.AddFactor(baseSpeed * 0.01f, DefaultPerks.Scouting.Navigator.Name);
            }

            var scoutingGrasslandNavigator = PerkObject.FindFirst(a => a.Name.GetID() == "Ekqj9IFR");
            if (scoutingGrasslandNavigator != null && leader.GetPerkValue(scoutingGrasslandNavigator))
            {
                bonuses.AddFactor(baseSpeed * 0.05f, scoutingGrasslandNavigator.Name);
            }
            var ScoutingExtra2 = PerkObject.FindFirst(a => a.Name.GetID() == "P68GX3zY");//{=P68GX3zY}Lay of the land", "{=RchM1puc} Extra 3% movement speed on map.
            if (ScoutingExtra2 != null && leader.GetPerkValue(ScoutingExtra2))
            {
                bonuses.AddFactor(baseSpeed * 0.03f, ScoutingExtra2.Name);
            }
            bonuses.LimitMin(1f);
            __result = bonuses.ResultNumber;
        }
    }
}
