using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Wang
{
    //[HarmonyPatch(typeof(DefaultSiegeEventModel))]
    //public class SiegeEventModelPatch
    //{
    //    [HarmonyPostfix]
    //    [HarmonyPatch("GetConstructionProgressPerHour")]
    //    private static void GetConstructionProgressPerHour(ref float __result, SiegeEngineType type, SiegeEvent siegeEvent, ISiegeEventSide side, StatExplainer explanation = null)
    //    {
    //        __result = __result / SiegeConfig.ConstructionProgressPerHourMutiplier;

    //    }
    //}
}
