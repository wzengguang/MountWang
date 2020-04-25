using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Wang
{
    [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "OnWarDeclared")]
    public class WorkshopsCampaignBehaviorPatch
    {
        private static void Postfix(IFaction faction1, IFaction faction2)
        {
            //工厂不再被没收了。
        }
    }
}
