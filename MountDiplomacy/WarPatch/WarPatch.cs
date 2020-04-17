using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Wang.WarPatch
{
    [HarmonyPatch(typeof(PoliticalStagnationAndBorderIncidentCampaignBehavior), "ThinkAboutDeclaringWar")]
    public class AddPatchThinkAboutDeclaringPeace
    {
        private static void Prefix(PoliticalStagnationAndBorderIncidentCampaignBehavior __instance, Kingdom kingdom)
        {

            List<IFaction> possibleKingdomsToDeclareWar = FactionHelper.GetPossibleKingdomsToDeclareWar(kingdom);
            float num = 0f;
            IFaction faction = null;
            foreach (IFaction item in FilterByDistance(kingdom, possibleKingdomsToDeclareWar))
            {
                float scoreOfDeclaringWar = Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(kingdom, item);

                if (kingdom.Culture == item.Culture)
                {
                    scoreOfDeclaringWar *= 0.5f;
                }

                if (scoreOfDeclaringWar > num)
                {
                    faction = item;
                    num = scoreOfDeclaringWar;
                }
            }
            if (faction != null && MBRandom.RandomFloat < Math.Min(0.25f, num / 100000f))
            {
                DeclareWarAction.ApplyDeclareWarOverProvocation(kingdom, faction);
            }
        }

        public static List<IFaction> FilterByDistance(Kingdom kingdom, List<IFaction> possibleKingdomsToDeclareWar)
        {
            Dictionary<IFaction, float> distances = new Dictionary<IFaction, float>();
            List<Settlement> list = kingdom.Settlements.ToList();

            foreach (IFaction item in possibleKingdomsToDeclareWar)
            {
                float num6 = 10000f;
                foreach (Settlement settlement in item.Settlements.Where(a => a.IsTown || a.IsCastle))
                {
                    foreach (Settlement settlement1 in list.Where(a => a.IsTown || a.IsCastle))
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

            if (distances.Count > 4)
            {
                return order.Take(4).Select(a => a.Key).ToList();
            }
            else
            {
                return order.Select(a => a.Key).ToList();
            }
        }

    }

}
