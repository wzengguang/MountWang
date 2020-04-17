using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace Wang
{
    public class CustomBanditDensityModel : DefaultBanditDensityModel
    {

        public override int NumberOfMaximumLooterParties => BanditConfig.NumberOfMaximumLooterParties;

        public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt => BanditConfig.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;

        public override int NumberOfMaximumBanditPartiesInEachHideout => BanditConfig.NumberOfMaximumBanditPartiesAroundEachHideout;

        public override int NumberOfMaximumBanditPartiesAroundEachHideout => BanditConfig.NumberOfMaximumBanditPartiesAroundEachHideout;

        public override int NumberOfMaximumHideoutsAtEachBanditFaction => BanditConfig.NumberOfMaximumHideoutsAtEachBanditFaction;

        public override int NumberOfInitialHideoutsAtEachBanditFaction => BanditConfig.NumberOfInitialHideoutsAtEachBanditFaction;
    }
}
