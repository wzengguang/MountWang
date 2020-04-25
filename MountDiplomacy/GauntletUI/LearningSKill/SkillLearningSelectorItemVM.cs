using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;

namespace Wang.GauntletUI
{
    public class SkillLearningSelectorItemVM : SelectorItemVM
    {
        public SkillObject Skill
        {
            get;
            private set;
        }

        public SkillLearningSelectorItemVM(SkillObject skill, bool isAvailable, string hint)
            : base("")
        {

            Skill = skill;
            base.StringItem = skill.Name.ToString();
            base.CanBeSelected = isAvailable;
        }
    }
}
