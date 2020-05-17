using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace Wang.GauntletUI
{
    public class WangVM : ViewModel
    {
        private readonly Action _onClose;

        private readonly Action<MobileParty> _openPartyAsManage;

        private readonly Clan _clan;

        private PlayerUpdateTracker _playerUpdateTracker;


        private WangClanMembersVM _clanMembers;

        private CanvassVM _canvassVM;

        private ClanFiefsVM _clanFiefs;

        private ModSettingVM _clanIncome;

        private HeroVM _leader;

        private ImageIdentifierVM _clanBanner;

        private bool _isPartiesSelected;

        private bool _isMembersSelected;

        private bool _isFiefsSelected;

        private bool _isIncomeSelected;

        private bool _isRenownProgressComplete;


        private string _doneLbl;

        private string _name;

        private string _leaderText;

        private int _minRenownForCurrentTier;

        private int _currentRenown;

        private int _currentTier = -1;

        private int _nextTierRenown;

        private int _nextTier;

        private string _currentRenownText;

        private string _membersText;

        private string _canvassText;

        private string _fiefsText;

        private string _incomeText;

        private BasicTooltipViewModel _renownHint;

        private HintViewModel _clanBannerHint;

        [DataSourceProperty]
        public HeroVM Leader
        {
            get
            {
                return _leader;
            }
            set
            {
                if (value != _leader)
                {
                    _leader = value;
                    OnPropertyChanged("Leader");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM ClanBanner
        {
            get
            {
                return _clanBanner;
            }
            set
            {
                if (value != _clanBanner)
                {
                    _clanBanner = value;
                    OnPropertyChanged("ClanBanner");
                }
            }
        }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        [DataSourceProperty]
        public string LeaderText
        {
            get
            {
                return _leaderText;
            }
            set
            {
                if (value != _leaderText)
                {
                    _leaderText = value;
                    OnPropertyChanged("LeaderText");
                }
            }
        }

        [DataSourceProperty]
        public WangClanMembersVM ClanMembers
        {
            get
            {
                return _clanMembers;
            }
            set
            {
                if (value != _clanMembers)
                {
                    _clanMembers = value;
                    OnPropertyChanged("ClanMembers");
                }
            }
        }

        [DataSourceProperty]
        public CanvassVM CanvassVM
        {
            get
            {
                return _canvassVM;
            }
            set
            {
                if (value != _canvassVM)
                {
                    _canvassVM = value;
                    OnPropertyChanged("CanvassVM");
                }
            }
        }

        [DataSourceProperty]
        public ClanFiefsVM ClanFiefs
        {
            get
            {
                return _clanFiefs;
            }
            set
            {
                if (value != _clanFiefs)
                {
                    _clanFiefs = value;
                    OnPropertyChanged("ClanFiefs");
                }
            }
        }

        [DataSourceProperty]
        public ModSettingVM ModSettings
        {
            get
            {
                return _clanIncome;
            }
            set
            {
                if (value != _clanIncome)
                {
                    _clanIncome = value;
                    OnPropertyChanged("ModSettings");
                }
            }
        }

        [DataSourceProperty]
        public bool IsMembersSelected
        {
            get
            {
                return _isMembersSelected;
            }
            set
            {
                if (value != _isMembersSelected)
                {
                    _isMembersSelected = value;
                    OnPropertyChanged("IsMembersSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsPartiesSelected
        {
            get
            {
                return _isPartiesSelected;
            }
            set
            {
                if (value != _isPartiesSelected)
                {
                    _isPartiesSelected = value;
                    OnPropertyChanged("IsPartiesSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsFiefsSelected
        {
            get
            {
                return _isFiefsSelected;
            }
            set
            {
                if (value != _isFiefsSelected)
                {
                    _isFiefsSelected = value;
                    OnPropertyChanged("IsFiefsSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsIncomeSelected
        {
            get
            {
                return _isIncomeSelected;
            }
            set
            {
                if (value != _isIncomeSelected)
                {
                    _isIncomeSelected = value;
                    OnPropertyChanged("IsIncomeSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsRenownProgressComplete
        {
            get
            {
                return _isRenownProgressComplete;
            }
            set
            {
                if (value != _isRenownProgressComplete)
                {
                    _isRenownProgressComplete = value;
                    OnPropertyChanged("IsRenownProgressComplete");
                }
            }
        }

        [DataSourceProperty]
        public string DoneLbl
        {
            get
            {
                return _doneLbl;
            }
            set
            {
                if (value != _doneLbl)
                {
                    _doneLbl = value;
                    OnPropertyChanged("DoneLbl");
                }
            }
        }

        [DataSourceProperty]
        public string CurrentRenownText
        {
            get
            {
                return _currentRenownText;
            }
            set
            {
                if (value != _currentRenownText)
                {
                    _currentRenownText = value;
                    OnPropertyChanged("CurrentRenownText");
                }
            }
        }


        [DataSourceProperty]
        public int NextTierRenown
        {
            get
            {
                return _nextTierRenown;
            }
            set
            {
                if (value != _nextTierRenown)
                {
                    _nextTierRenown = value;
                    OnPropertyChanged("NextTierRenown");
                }
            }
        }

        [DataSourceProperty]
        public int CurrentTier
        {
            get
            {
                return _currentTier;
            }
            set
            {
                if (value != _currentTier)
                {
                    _currentTier = value;
                    OnPropertyChanged("CurrentTier");
                }
            }
        }

        [DataSourceProperty]
        public int MinRenownForCurrentTier
        {
            get
            {
                return _minRenownForCurrentTier;
            }
            set
            {
                if (value != _minRenownForCurrentTier)
                {
                    _minRenownForCurrentTier = value;
                    OnPropertyChanged("MinRenownForCurrentTier");
                }
            }
        }

        [DataSourceProperty]
        public int NextTier
        {
            get
            {
                return _nextTier;
            }
            set
            {
                if (value != _nextTier)
                {
                    _nextTier = value;
                    OnPropertyChanged("NextTier");
                }
            }
        }

        [DataSourceProperty]
        public int CurrentRenown
        {
            get
            {
                return _currentRenown;
            }
            set
            {
                if (value != _currentRenown)
                {
                    _currentRenown = value;
                    OnPropertyChanged("CurrentRenown");
                }
            }
        }

        [DataSourceProperty]
        public string MembersText
        {
            get
            {
                return _membersText;
            }
            set
            {
                if (value != _membersText)
                {
                    _membersText = value;
                    OnPropertyChanged("MembersText");
                }
            }
        }

        [DataSourceProperty]
        public string CanvassText
        {
            get
            {
                return _canvassText;
            }
            set
            {
                if (value != _canvassText)
                {
                    _canvassText = value;
                    OnPropertyChanged("PartiesText");
                }
            }
        }

        [DataSourceProperty]
        public string FiefsText
        {
            get
            {
                return _fiefsText;
            }
            set
            {
                if (value != _fiefsText)
                {
                    _fiefsText = value;
                    OnPropertyChanged("FiefsText");
                }
            }
        }

        [DataSourceProperty]
        public string IncomeText
        {
            get
            {
                return _incomeText;
            }
            set
            {
                if (value != _incomeText)
                {
                    _incomeText = value;
                    OnPropertyChanged("OtherText");
                }
            }
        }

        [DataSourceProperty]
        public BasicTooltipViewModel RenownHint
        {
            get
            {
                return _renownHint;
            }
            set
            {
                if (value != _renownHint)
                {
                    _renownHint = value;
                    OnPropertyChanged("RenownHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel ClanBannerHint
        {
            get
            {
                return _clanBannerHint;
            }
            set
            {
                if (value != _clanBannerHint)
                {
                    _clanBannerHint = value;
                    OnPropertyChanged("ClanBannerHint");
                }
            }
        }

        public WangVM(Action onClose, Action<MobileParty> openPartyAsManage, Action openBannerEditor)
        {
            _onClose = onClose;
            _openPartyAsManage = openPartyAsManage;
            _clan = Hero.MainHero.Clan;
            _playerUpdateTracker = PlayerUpdateTracker.Current;
            ClanMembers = new WangClanMembersVM(RefreshCategoryValues);
            CanvassVM = new CanvassVM(RefreshCategoryValues);
            ClanFiefs = new ClanFiefsVM(RefreshCategoryValues);
            ModSettings = new ModSettingVM(RefreshCategoryValues);
            SetSelectedCategory(0);
            Leader = new HeroVM(_clan.Leader);
            CurrentRenown = (int)Clan.PlayerClan.Renown;
            CurrentTier = Clan.PlayerClan.Tier;
            if (Campaign.Current.Models.ClanTierModel.HasUpcomingTier(Clan.PlayerClan))
            {
                NextTierRenown = Clan.PlayerClan.RenownRequirementForNextTier;
                MinRenownForCurrentTier = Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(CurrentTier);
                NextTier = Clan.PlayerClan.Tier + 1;
                IsRenownProgressComplete = false;
            }
            else
            {
                NextTierRenown = 1;
                MinRenownForCurrentTier = 1;
                NextTier = 0;
                IsRenownProgressComplete = true;
            }
            RenownHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetClanRenownTooltip(Clan.PlayerClan));
            UpdateBannerVisuals();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Name = Hero.MainHero.Clan.Name.ToString();
            MembersText = new TextObject("{=wang_learning_skill}").ToString();
            CanvassText = new TextObject("{=wang_information}").ToString();
            IncomeText = new TextObject("{=wang_mod_setting}WangModSetting").ToString();
            FiefsText = new TextObject("{=wang_log}").ToString();
            DoneLbl = GameTexts.FindText("str_done").ToString();
            LeaderText = GameTexts.FindText("str_sort_by_leader_name_label").ToString();
            GameTexts.SetVariable("TIER", Clan.PlayerClan.Tier);
            CurrentRenownText = GameTexts.FindText("str_clan_tier").ToString();
            _clanMembers?.RefreshValues();
            _canvassVM?.RefreshValues();
            _clanFiefs?.RefreshValues();
            _clanIncome?.RefreshValues();
            _leader?.RefreshValues();
        }


        public void UpdateBannerVisuals()
        {
            ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(_clan.Banner), nineGrid: true);
            ClanBannerHint = new HintViewModel(new TextObject("{=t1lSXN9O}Your clan's standard carried into battle").ToString());
            RefreshValues();
        }

        private void SetSelectedCategory(int index)
        {
            ClanMembers.IsSelected = false;
            CanvassVM.IsSelected = false;
            ClanFiefs.IsSelected = false;
            ModSettings.IsSelected = false;
            switch (index)
            {
                case 0:
                    ClanMembers.IsSelected = true;
                    break;
                case 1:
                    CanvassVM.IsSelected = true;
                    CanvassVM.RefreshClan();
                    break;
                case 2:
                    ClanFiefs.IsSelected = true;
                    break;
                default:
                    ModSettings.IsSelected = true;
                    break;
            }
            IsMembersSelected = ClanMembers.IsSelected;
            IsPartiesSelected = CanvassVM.IsSelected;
            IsFiefsSelected = ClanFiefs.IsSelected;
            IsIncomeSelected = ModSettings.IsSelected;
        }

        private void RefreshCategoryValues()
        {
            ClanFiefs.RefreshFiefsList();
            ClanMembers.RefreshMembersList();
            // CanvassVM.RefreshClan();
            ModSettings.RefreshList();
        }

        private void ExecuteClose()
        {
            _onClose();
        }

    }
}
