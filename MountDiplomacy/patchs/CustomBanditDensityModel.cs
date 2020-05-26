using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultBanditDensityModel))]
    public class CustomBanditDensityModel
    {
        [HarmonyPostfix]
        [HarmonyPatch("NumberOfMaximumLooterParties", MethodType.Getter)]
        private static void Postfix1(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfMaximumLooterParties;
        }

        [HarmonyPostfix]
        [HarmonyPatch("NumberOfMinimumBanditPartiesInAHideoutToInfestIt", MethodType.Getter)]
        private static void Postfix2(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;
        }
        [HarmonyPostfix]
        [HarmonyPatch("NumberOfMaximumBanditPartiesInEachHideout", MethodType.Getter)]
        private static void Postfix3(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfMaximumBanditPartiesInEachHideout;
        }
        [HarmonyPostfix]
        [HarmonyPatch("NumberOfMaximumBanditPartiesAroundEachHideout", MethodType.Getter)]
        private static void Postfix4(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfMaximumBanditPartiesAroundEachHideout;
        }
        [HarmonyPostfix]
        [HarmonyPatch("NumberOfMaximumHideoutsAtEachBanditFaction", MethodType.Getter)]
        private static void Postfix5(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfMaximumHideoutsAtEachBanditFaction;
        }
        [HarmonyPostfix]
        [HarmonyPatch("NumberOfInitialHideoutsAtEachBanditFaction", MethodType.Getter)]
        private static void Postfix6(ref int __result)
        {
            __result = (int)BanditsSetting.Instance.NumberOfInitialHideoutsAtEachBanditFaction;
        }
    }
}
