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
    public class CustomPrisonerRecruitmentCalculationModel : DefaultPrisonerRecruitmentCalculationModel
    {

        public override float[] GetDailyRecruitedPrisoners(MobileParty mainParty)
        {



            if (RecruitConfig.RecruitChange == null || RecruitConfig.RecruitChange.Length < 5)
            {
                return base.GetDailyRecruitedPrisoners(mainParty);
            }

            var f = new float[RecruitConfig.RecruitChange.Length];

            for (int i = 0; i < f.Length; i++)
            {
                f[i] = RecruitConfig.RecruitChange[i];
            }
            return f;
        }
    }
}
