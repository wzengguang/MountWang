using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomClan;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang.GauntletUI.Canvass
{
    public class CanvassKingdomClanItemVM : KingdomClanItemVM
    {

        public CanvassKingdomClanItemVM(Clan clan, Action<KingdomClanItemVM> onSelect) : base(clan, onSelect)
        {

        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            var traits = "";
            foreach (TraitObject heroTrait in CampaignUIHelper.GetHeroTraits())
            {
                if (Clan.Leader.GetTraitLevel(heroTrait) != 0)
                {
                    traits += CampaignUIHelper.GetTraitTooltipText(heroTrait, Clan.Leader) + " ";
                }
            }

            this.TierText += traits;
        }

        public new void Refresh()
        {
            base.Refresh();
        }

        protected override void OnSelect()
        {
            base.OnSelect();
        }
    }
}

