using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using HarmonyLib;
namespace Wang
{

    [HarmonyPatch(typeof(DefaultClanTierModel))]
    public class DefaultClanModelPatch
    {
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch("GetCompanionLimitForTier")]
        private static void GetCompanionLimitForTier(int clanTier, ref int __result)
        {

            __result = clanTier * 2 + 3;
        }
    }
}
