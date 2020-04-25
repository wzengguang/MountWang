using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Wang.GauntletUI
{
    public class OrderTroopSelectorItemVM : SelectorItemVM
    {
        public TroopSortType SortType
        {
            get;
            private set;
        }

        public OrderTroopSelectorItemVM(TroopSortType sortType, bool isAvailable = true)
            : base("")
        {
            SortType = sortType;
            switch (sortType)
            {
                case TroopSortType.TierDesc:
                    base.StringItem = new TextObject(("{=order_TierDesc}")).ToString();
                    break;
                case TroopSortType.TierAsc:
                    base.StringItem = new TextObject(("{=order_TierAsc}")).ToString();
                    break;
                case TroopSortType.TierDescType:
                    base.StringItem = new TextObject(("{=order_TierDescType}")).ToString();
                    break;
                case TroopSortType.TierAscType:
                    base.StringItem = new TextObject(("{=order_TierAscType}")).ToString();
                    break;
                case TroopSortType.MountRangeTierDesc:
                    base.StringItem = new TextObject(("{=order_MountRangeTierDesc}")).ToString();
                    break;
                case TroopSortType.MountRangeTierAsc:
                    base.StringItem = new TextObject(("{=order_MountRangeTierAsc}")).ToString();
                    break;
                case TroopSortType.CultureTierDesc:
                    base.StringItem = new TextObject(("{=order_CultureTierDesc}")).ToString();
                    break;
                case TroopSortType.CultureTierAsc:
                    base.StringItem = new TextObject(("{=order_CultureTierAsc}")).ToString();
                    break;
                default:
                    base.StringItem = new TextObject(("{=order_None}")).ToString();
                    break;
            }

            base.CanBeSelected = isAvailable;
        }
    }
}
