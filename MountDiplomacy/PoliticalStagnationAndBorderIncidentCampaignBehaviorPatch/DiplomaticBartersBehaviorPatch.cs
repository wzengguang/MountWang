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

namespace Wang
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior))]
    public class DiplomaticBartersBehaviorPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConsiderWar")]
        public static void ConsiderWar(IFaction mapFaction, IFaction otherMapFaction)
        {
            if (!mapFaction.IsKingdomFaction || !otherMapFaction.IsKingdomFaction)
            {
                return;
            }

            DeclareWarBarterable declareWarBarterable = new DeclareWarBarterable(mapFaction.Leader, otherMapFaction);
            if (declareWarBarterable.GetValueForFaction(mapFaction) > 500 && Help.CanDeclareWar(mapFaction, otherMapFaction))
            {

                declareWarBarterable.Apply();

            }

        }


    }
}
