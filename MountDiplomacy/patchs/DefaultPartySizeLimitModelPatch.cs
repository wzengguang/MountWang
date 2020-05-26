using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using Wang.Setting;

namespace Wang.patchs
{
    [HarmonyPatch(typeof(DefaultPartySizeLimitModel), "GetPartyPrisonerSizeLimit")]
    public class DefaultPartySizeLimitModelPatch
    {

        private static void Postfix(ref int __result, PartyBase party, StatExplainer explanation = null)
        {

            if (party.Leader != null)
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(__result, explanation, null);
                var roguery = party.Leader.GetSkillValue(DefaultSkills.Roguery);
                roguery = Math.Max(1, roguery);
                var add = __result * roguery * CommonSetting.Instance.PartyPrisonerSizeLimitBySkill / 1000f;
                explainedNumber.Add((int)add, DefaultSkills.Roguery.Name);
                __result = (int)explainedNumber.ResultNumber;
            }

        }

    }
}
