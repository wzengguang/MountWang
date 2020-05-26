using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;

namespace Wang.GauntletUI.Canvass
{
    public class CompanionSelectorItemVM : SelectorItemVM
    {
        public Hero Hero
        {
            get;
            private set;
        }

        public CompanionSelectorItemVM(Hero hero, bool isAvailable, string hint)
            : base("")
        {
            Hero = hero;
            base.StringItem = hero == null ? new TextObject("{=wang_selector_none}please select a item").ToString() : hero.Name.ToString();
            base.CanBeSelected = isAvailable;
        }
    }
}
