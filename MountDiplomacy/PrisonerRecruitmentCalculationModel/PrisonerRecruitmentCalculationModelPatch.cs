using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultPrisonerRecruitmentCalculationModel))]
    public class PrisonerRecruitmentCalculationModelPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetDailyRecruitedPrisoners")]
        public static bool GetDailyRecruitedPrisoners(ref float[] __result, MobileParty mainParty)
        {
            __result = PrisonerRecruitChanceSetting.Instance.Chances();
            return false;
        }
    }
}
