using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior))]
    public class DiplomaticBartersBehaviorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ConsiderWar")]
        public static bool ConsiderWar(IFaction mapFaction, IFaction otherMapFaction)
        {

            if (!mapFaction.IsKingdomFaction || !otherMapFaction.IsKingdomFaction || !Help.CanDeclareWar(mapFaction, otherMapFaction, true, true))
            {
                return false;
            }

            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch("ConsiderDefection")]
        private static bool ConsiderDefection(Clan clan1, Kingdom kingdom)
        {
            if (clan1.Kingdom.Leader == Hero.MainHero)
            {
                return true;
            }

            return !DiplomacySetting.Instance.DisableClanJobHop;
        }
    }
}
