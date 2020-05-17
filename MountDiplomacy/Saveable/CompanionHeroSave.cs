using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Wang.Saveable
{
    public class CompanionHeroSave
    {
        [SaveableProperty(1)]
        public Hero Hero { get; set; }

        [SaveableProperty(2)]
        public SkillObject SkillObject { get; set; }

        [SaveableProperty(3)]
        public int WashPerkTime { get; set; }

        [SaveableProperty(4)]
        public int Formation { get; set; }


    }

}
