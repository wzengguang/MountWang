using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace Wang
{

    [HarmonyPatch(typeof(PartyVM), "ExecuteDone")]
    class PartyVMPatch
    {

        private static void Postfix()
        {
            var behavior = Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>();

            foreach (var item in MobileParty.MainParty.MemberRoster.Where(a => a.Character.IsHero))
            {
                behavior.SetHeroFormation(item.Character.HeroObject, (int)item.Character.CurrentFormationClass);
            }
        }
    }
}
