using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomClan;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Wang.GauntletUI.Canvass;

namespace Wang.GauntletUI
{
    public class CanvassVM : KingdomCategoryVM
    {
        private bool _isSelected;
        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        private string _bannerText;

        private string _nameText;

        private string _influenceText;

        private string _membersText;

        private string _fiefsText;

        private string _typeText;
        private string _canvassInfoTextText;
        private string _canvassRelationGainText;


        private MBBindingList<KingdomClanItemVM> _clans = new MBBindingList<KingdomClanItemVM>();

        private KingdomClanItemVM _currentSelectedClan;

        private KingdomClanSortControllerVM _clanSortController;

        private SelectorVM<CompanionSelectorItemVM> _companionSelector;
        private string _currentCanvassClanText;

        public CanvassVM(Action onRefresh)
        {
            base.IsAcceptableItemSelected = false;
            this.ClanSortController = new CanvassKingdomClanSortControllerVM(ref this._clans);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
            this.InfluenceText = GameTexts.FindText("str_influence", null).ToString();
            this.FiefsText = GameTexts.FindText("str_fiefs", null).ToString();
            this.MembersText = GameTexts.FindText("str_members", null).ToString();
            this.BannerText = GameTexts.FindText("str_banner", null).ToString();
            this.TypeText = GameTexts.FindText("str_sort_by_type_label", null).ToString();
            base.CategoryNameText = new TextObject("{=j4F7tTzy}Clan", null).ToString();
            base.NoItemSelectedText = GameTexts.FindText("str_kingdom_no_clan_selected", null).ToString();
            this.CanvassInfoText = new TextObject("{=canvass_info}asign hero to canvass clan", null).ToString();

        }


        private void OnClanSelection(KingdomClanItemVM clan)
        {
            if (this._currentSelectedClan != clan)
            {
                this.SetCurrentSelectedClan(clan);
            }
        }


        private void SetCurrentSelectedClan(KingdomClanItemVM clan)
        {
            if (clan != this.CurrentSelectedClan)
            {
                if (this.CurrentSelectedClan != null)
                {
                    this.CurrentSelectedClan.IsSelected = false;
                }
                this.CurrentSelectedClan = clan;
                this.CurrentSelectedClan.IsSelected = true;

                base.IsAcceptableItemSelected = (this.CurrentSelectedClan != null);

                RefreshCompanionSelector();
            }
        }



        public void RefreshClan()
        {
            this.RefreshClanList();
            foreach (KingdomClanItemVM kingdomClanItemVM in this.Clans)
            {
                kingdomClanItemVM.Refresh();
            }
        }

        private void ExecuteSupport()
        {

        }

        private void RefreshClanList()
        {
            this.Clans.Clear();
            foreach (Clan clan in Clan.All.Where(a => !a.IsMinorFaction && a.Kingdom != null && !a.IsKingdomFaction))
            {
                this.Clans.Add(new CanvassKingdomClanItemVM(clan, new Action<KingdomClanItemVM>(this.OnClanSelection)));
            }
            if (this.Clans.Count > 0)
            {
                this.SetCurrentSelectedClan(this.Clans.FirstOrDefault<KingdomClanItemVM>());
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }

        public void RefreshCompanionSelector()
        {
            CompanionSelector = new SelectorVM<CompanionSelectorItemVM>(0, OnCompanionSelectorChange);
            CompanionSelector.SetOnChangeAction(null);

            CompanionSelector.AddItem(new CompanionSelectorItemVM(null, true, null));
            CompanionSelector.SelectedIndex = 0;
            foreach (Hero hero in Clan.PlayerClan.Companions)
            {
                var item = new CompanionSelectorItemVM(hero, true, "");
                CompanionSelector.AddItem(item);
            }

            var current = Campaign.Current.GetCampaignBehavior<CanvassBehavior>().GetCurrent(this.CurrentSelectedClan?.Clan);

            for (int i = 0; i < CompanionSelector.ItemList.Count; i++)
            {
                if (current != null && current == CompanionSelector.ItemList[i].Hero)
                {
                    CompanionSelector.SelectedIndex = i;
                    break;
                }
            }
            CompanionSelector.SetOnChangeAction(OnCompanionSelectorChange);

            UpdateCanvassRelationGainText(current, CurrentSelectedClan.Clan);
            RefreshCurrentCanvassClanText();
        }

        private void OnCompanionSelectorChange(SelectorVM<CompanionSelectorItemVM> obj)
        {
            if (this.CurrentSelectedClan != null && obj.SelectedItem != null)
            {
                var canvass = Campaign.Current.GetCampaignBehavior<CanvassBehavior>();

                canvass.UpdateCanvass(obj.SelectedItem.Hero, CurrentSelectedClan.Clan);

            }
            UpdateCanvassRelationGainText(obj?.SelectedItem?.Hero, CurrentSelectedClan.Clan);
            RefreshCurrentCanvassClanText();
        }

        private void UpdateCanvassRelationGainText(Hero hero, Clan clan)
        {
            if (hero == null)
            {
                this.CanvassRelationGainText = " ";
                return;
            }

            var relation = CanvassBehavior.GetExpectRelation(hero, clan, 7);
            MBTextManager.SetTextVariable("CANVASS_COST", CanvassBehavior.GetExpectGoldCostOfRelation(clan, relation));

            MBTextManager.SetTextVariable("CANVASS_RELATION", relation);
            this.CanvassRelationGainText = new TextObject("{=canvass_relation_gain}expect get {CANVASS_RELATION} relation weekly.cost {CANVASS_COST} gold", null).ToString();
        }


        private void RefreshCurrentCanvassClanText()
        {
            var clan = Campaign.Current.GetCampaignBehavior<CanvassBehavior>().GetCurrentClan();

            if (clan != null)
            {
                CurrentCanvassClanText = new TextObject("{=canvass_current_clan}current canvass clan is ").ToString() + clan?.Name.ToString();
            }
            else
            {
                CurrentCanvassClanText = new TextObject("{=canvass_current_no_clan}no chosen clan").ToString();
            }


        }

        public String CurrentCanvassClanText
        {
            get { return _currentCanvassClanText; }
            set
            {
                if (value != _currentCanvassClanText)
                {
                    _currentCanvassClanText = value;
                    OnPropertyChanged(nameof(CurrentCanvassClanText));
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<CompanionSelectorItemVM> CompanionSelector
        {
            get
            {
                return _companionSelector;
            }
            set
            {
                if (value != _companionSelector)
                {
                    _companionSelector = value;
                    OnPropertyChanged(nameof(CompanionSelector));
                }
            }
        }

        [DataSourceProperty]
        public KingdomClanSortControllerVM ClanSortController
        {
            get
            {
                return this._clanSortController;
            }
            set
            {
                if (value != this._clanSortController)
                {
                    this._clanSortController = value;
                    base.OnPropertyChanged(nameof(ClanSortController));
                }
            }
        }

        [DataSourceProperty]
        public KingdomClanItemVM CurrentSelectedClan
        {
            get
            {
                return this._currentSelectedClan;
            }
            set
            {
                if (value != this._currentSelectedClan)
                {
                    this._currentSelectedClan = value;
                    base.OnPropertyChanged(nameof(CurrentSelectedClan));
                }
            }
        }

        [DataSourceProperty]
        public string CanvassRelationGainText
        {
            get
            {
                return this._canvassRelationGainText;
            }
            set
            {
                if (value != this._canvassRelationGainText)
                {
                    this._canvassRelationGainText = value;
                    base.OnPropertyChanged(nameof(CanvassRelationGainText));
                }
            }
        }

        [DataSourceProperty]
        public string CanvassInfoText
        {
            get
            {
                return this._canvassInfoTextText;
            }
            set
            {
                if (value != this._canvassInfoTextText)
                {
                    this._canvassInfoTextText = value;
                    base.OnPropertyChanged(nameof(CanvassInfoText));
                }
            }
        }

        [DataSourceProperty]
        public string BannerText
        {
            get
            {
                return this._bannerText;
            }
            set
            {
                if (value != this._bannerText)
                {
                    this._bannerText = value;
                    base.OnPropertyChanged(nameof(BannerText));
                }
            }
        }

        [DataSourceProperty]
        public string TypeText
        {
            get
            {
                return this._typeText;
            }
            set
            {
                if (value != this._typeText)
                {
                    this._typeText = value;
                    base.OnPropertyChanged(nameof(TypeText));
                }
            }
        }

        [DataSourceProperty]
        public string NameText
        {
            get
            {
                return this._nameText;
            }
            set
            {
                if (value != this._nameText)
                {
                    this._nameText = value;
                    base.OnPropertyChanged(nameof(NameText));
                }
            }
        }

        [DataSourceProperty]
        public string InfluenceText
        {
            get
            {
                return this._influenceText;
            }
            set
            {
                if (value != this._influenceText)
                {
                    this._influenceText = value;
                    base.OnPropertyChanged(nameof(InfluenceText));
                }
            }
        }

        [DataSourceProperty]
        public string FiefsText
        {
            get
            {
                return this._fiefsText;
            }
            set
            {
                if (value != this._fiefsText)
                {
                    this._fiefsText = value;
                    base.OnPropertyChanged(nameof(FiefsText));
                }
            }
        }

        [DataSourceProperty]
        public string MembersText
        {
            get
            {
                return this._membersText;
            }
            set
            {
                if (value != this._membersText)
                {
                    this._membersText = value;
                    base.OnPropertyChanged(nameof(MembersText));
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<KingdomClanItemVM> Clans
        {
            get
            {
                return this._clans;
            }
            set
            {
                if (value != this._clans)
                {
                    this._clans = value;
                    base.OnPropertyChanged(nameof(Clans));
                }
            }
        }


    }
}

