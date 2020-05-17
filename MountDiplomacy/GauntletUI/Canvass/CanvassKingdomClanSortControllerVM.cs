using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomClan;
using TaleWorlds.Library;

namespace Wang.GauntletUI.Canvass
{
    public class CanvassKingdomClanSortControllerVM : KingdomClanSortControllerVM
    {
        private readonly ItemBannerComparer _bannerComparer;
        private readonly MBBindingList<KingdomClanItemVM> _listToControl;
        private readonly CanvassKingdomClanSortControllerVM.ItemNameComparer _nameComparer;

        private readonly CanvassKingdomClanSortControllerVM.ItemTypeComparer _typeComparer;

        private readonly CanvassKingdomClanSortControllerVM.ItemInfluenceComparer _influenceComparer;

        private readonly CanvassKingdomClanSortControllerVM.ItemMembersComparer _membersComparer;

        private readonly CanvassKingdomClanSortControllerVM.ItemFiefsComparer _fiefsComparer;
        private int _bannerState;
        private bool _isBannerSelected;

        [DataSourceProperty]
        public int BannerState
        {
            get
            {
                return this._bannerState;
            }
            set
            {
                if (value != this._bannerState)
                {
                    this._bannerState = value;
                    base.OnPropertyChanged(nameof(BannerState));
                }
            }
        }

        [DataSourceProperty]
        public bool IsBannerSelected
        {
            get
            {
                return this._isBannerSelected;
            }
            set
            {
                if (value != this._isBannerSelected)
                {
                    this._isBannerSelected = value;
                    base.OnPropertyChanged(nameof(IsBannerSelected));
                }
            }
        }

        public CanvassKingdomClanSortControllerVM(ref MBBindingList<KingdomClanItemVM> listToControl) : base(ref listToControl)
        {
            this._listToControl = listToControl;
            this._bannerComparer = new ItemBannerComparer();
            this._influenceComparer = new CanvassKingdomClanSortControllerVM.ItemInfluenceComparer();
            this._membersComparer = new CanvassKingdomClanSortControllerVM.ItemMembersComparer();
            this._nameComparer = new CanvassKingdomClanSortControllerVM.ItemNameComparer();
            this._fiefsComparer = new CanvassKingdomClanSortControllerVM.ItemFiefsComparer();
            this._typeComparer = new CanvassKingdomClanSortControllerVM.ItemTypeComparer();
        }

        private void ExecuteSortByBanner()
        {
            int bannerState = this.BannerState;
            this.SetAllStates(SortState.Default);
            this.BannerState = (bannerState + 1) % 3;
            if (this.BannerState == 0)
            {
                this.BannerState++;
            }
            this._bannerComparer.SetSortMode(this.BannerState == 1);
            this._listToControl.Sort(this._bannerComparer);
            this.IsBannerSelected = true;
        }

        private void ExecuteSortByName()
        {
            int nameState = this.NameState;
            this.SetAllStates(CanvassKingdomClanSortControllerVM.SortState.Default);
            this.NameState = (nameState + 1) % 3;
            if (this.NameState == 0)
            {
                this.NameState++;
            }
            this._nameComparer.SetSortMode(this.NameState == 1);
            this._listToControl.Sort(this._nameComparer);
            this.IsNameSelected = true;
        }

        private void ExecuteSortByType()
        {
            int typeState = this.TypeState;
            this.SetAllStates(CanvassKingdomClanSortControllerVM.SortState.Default);
            this.TypeState = (typeState + 1) % 3;
            if (this.TypeState == 0)
            {
                this.TypeState++;
            }
            this._typeComparer.SetSortMode(this.TypeState == 1);
            this._listToControl.Sort(this._typeComparer);
            this.IsTypeSelected = true;
        }

        private void ExecuteSortByInfluence()
        {
            int influenceState = this.InfluenceState;
            this.SetAllStates(CanvassKingdomClanSortControllerVM.SortState.Default);
            this.InfluenceState = (influenceState + 1) % 3;
            if (this.InfluenceState == 0)
            {
                this.InfluenceState++;
            }
            this._influenceComparer.SetSortMode(this.InfluenceState == 1);
            this._listToControl.Sort(this._influenceComparer);
            this.IsInfluenceSelected = true;
        }

        private void ExecuteSortByMembers()
        {
            int membersState = this.MembersState;
            this.SetAllStates(CanvassKingdomClanSortControllerVM.SortState.Default);
            this.MembersState = (membersState + 1) % 3;
            if (this.MembersState == 0)
            {
                this.MembersState++;
            }
            this._membersComparer.SetSortMode(this.MembersState == 1);
            this._listToControl.Sort(this._membersComparer);
            this.IsMembersSelected = true;
        }

        private void ExecuteSortByFiefs()
        {
            int fiefsState = this.FiefsState;
            this.SetAllStates(CanvassKingdomClanSortControllerVM.SortState.Default);
            this.FiefsState = (fiefsState + 1) % 3;
            if (this.FiefsState == 0)
            {
                this.FiefsState++;
            }
            this._fiefsComparer.SetSortMode(this.FiefsState == 1);
            this._listToControl.Sort(this._fiefsComparer);
            this.IsFiefsSelected = true;
        }
        private void SetAllStates(SortState state)
        {
            this.BannerState = (int)state;
            this.InfluenceState = (int)state;
            this.IsFiefsSelected = false;
            this.MembersState = (int)state;
            this.NameState = (int)state;
            this.TypeState = (int)state;
            this.IsBannerSelected = false;
            this.IsInfluenceSelected = false;
            this.IsFiefsSelected = false;
            this.IsNameSelected = false;
            this.IsMembersSelected = false;
            this.IsTypeSelected = false;
        }
        private class ItemBannerComparer : ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.Clan.Kingdom.StringId.CompareTo(x.Clan.Kingdom.StringId) * -1;
                }
                return y.Clan.Kingdom.StringId.CompareTo(x.Clan.Kingdom.StringId);
            }
        }

        private abstract class ItemComparerBase : IComparer<KingdomClanItemVM>
        {
            protected bool _isAcending;

            public void SetSortMode(bool isAcending)
            {
                this._isAcending = isAcending;
            }

            public abstract int Compare(KingdomClanItemVM x, KingdomClanItemVM y);
        }
        private enum SortState
        {
            Default,
            Ascending,
            Descending
        }
        private class ItemNameComparer : CanvassKingdomClanSortControllerVM.ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString()) * -1;
                }
                return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString());
            }
        }

        private class ItemTypeComparer : CanvassKingdomClanSortControllerVM.ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.ClanType.CompareTo(x.ClanType) * -1;
                }
                return y.ClanType.CompareTo(x.ClanType);
            }
        }

        private class ItemInfluenceComparer : CanvassKingdomClanSortControllerVM.ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.Influence.CompareTo(x.Influence) * -1;
                }
                return y.Influence.CompareTo(x.Influence);
            }
        }

        private class ItemMembersComparer : CanvassKingdomClanSortControllerVM.ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.Members.Count.CompareTo(x.Members.Count) * -1;
                }
                return y.Members.Count.CompareTo(x.Members.Count);
            }
        }

        private class ItemFiefsComparer : CanvassKingdomClanSortControllerVM.ItemComparerBase
        {
            public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
            {
                if (this._isAcending)
                {
                    return y.Fiefs.Count.CompareTo(x.Fiefs.Count) * -1;
                }
                return y.Fiefs.Count.CompareTo(x.Fiefs.Count);
            }
        }
    }
}
