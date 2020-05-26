using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using HarmonyLib;
using Wang.Setting;

namespace Wang
{

    [HarmonyPatch(typeof(DefaultClanTierModel))]
    public class DefaultClanModelPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetCompanionLimitForTier")]
        private static bool GetCompanionLimitForTier(int clanTier, ref int __result)
        {
            __result = clanTier * (int)CommonSetting.Instance.CompanionLimit + 3;
            return false;
        }
    }
}
