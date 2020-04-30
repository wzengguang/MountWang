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
    [HarmonyPatch(typeof(DefaultPrisonerRecruitmentCalculationModel))]
    public class PrisonerRecruitmentCalculationModelPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetDailyRecruitedPrisoners")]
        public static bool GetDailyRecruitedPrisoners(ref float[] __result, MobileParty mainParty)
        {
            var f = new float[RecruitConfig.RecruitChange.Length];

            for (int i = 0; i < f.Length; i++)
            {
                f[i] = RecruitConfig.RecruitChange[i];
            }
            __result = f;
            return false;
        }
    }
}
