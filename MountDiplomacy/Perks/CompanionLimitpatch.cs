using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace Wang.Perks
{

    [HarmonyPatch(typeof(Clan), "CompanionLimit", MethodType.Getter)]
    public class CompanionLimitpatch
    {
        private static void Postfix(ref int __result, ref Clan __instance)
        {
            if (__instance.Leader != null && __instance.Leader.GetPerkValue(DefaultPerks.Steward.Reeve))
            {
                __result += 1;
            }

            if (__instance.Leader != null && __instance.Leader.GetPerkValue(DefaultPerks.Steward.Ruler) && __instance.Settlements != null)
            {
                __result += __instance.Settlements.Where(a => a.IsTown).Count();
            }
        }
    }
}
