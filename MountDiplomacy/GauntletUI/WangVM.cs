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

        private readonly Action _openBannerEditor;

        private readonly Action<MobileParty> _openPartyAsManage;

        private readonly Clan _clan;

        private PlayerUpdateTracker _playerUpdateTracker;

        private const int _maxClanNameLetterCount = 50;

        private const int _minClanNameLetterCount = 1;

        private WangClanMembersVM _clanMembers;

        private ClanPartiesVM _clanParties;

        private ClanFiefsVM _clanFiefs;

        private ModSettingVM _clanIncome;

        private HeroVM _leader;

        private ImageIdentifierVM _clanBanner;

        private bool _isPartiesSelected;

        private bool _isMembersSelected;

        private bool _isFiefsSelected;

        private bool _isIncomeSelected;

        private bool _canChooseBanner;

        private bool _isRenownProgressComplete;

        private bool _playerCanChangeClanName;

        private bool _clanIsInAKingdom;

        private string _doneLbl;

        private string _name;

        private string _leaveKingdomText;

        private string _leaderText;

        private int _minRenownForCurrentTier;

        private int _currentRenown;

        private int _currentTier = -1;

        private int _nextTierRenown;

        private int _nextTier;

        private string _currentRenownText;

        private string _membersText;

        private string _partiesText;

        private string _fiefsText;

        private string _incomeText;

        private BasicTooltipViewModel _renownHint;

        private HintViewModel _clanBannerHint;

        private HintViewModel _changeClanNameHint;

        private string _financeText;

        private string _currentGoldText;

        private int _currentGold;

        private string _totalIncomeText;

        private int _totalIncome;

        private string _totalIncomeValueText;

        private string _totalExpensesText;

        private int _totalExpenses;

        private string _totalExpensesValueText;

        private string _dailyChangeText;

        private int _dailyChange;

        private string _dailyChangeValueText;

        private string _expectedGoldText;

        private int _expectedGold;

        private string _expenseText;

        private BasicTooltipViewModel _goldChangeTooltip;

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
        public ClanPartiesVM Informations
        {
            get
            {
                return _clanParties;
            }
            set
            {
                if (value != _clanParties)
                {
                    _clanParties = value;
                    OnPropertyChanged("Informations");
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
        public bool ClanIsInAKingdom
        {
            get
            {
                return _clanIsInAKingdom;
            }
            set
            {
                if (value != _clanIsInAKingdom)
                {
                    _clanIsInAKingdom = value;
                    OnPropertyChanged("ClanIsInAKingdom");
                }
            }
        }

        [DataSourceProperty]
        public bool PlayerCanChangeClanName
        {
            get
            {
                return _playerCanChangeClanName;
            }
            set
            {
                if (value != _playerCanChangeClanName)
                {
                    _playerCanChangeClanName = value;
                    OnPropertyChanged("PlayerCanChangeClanName");
                }
            }
        }

        [DataSourceProperty]
        public bool CanChooseBanner
        {
            get
            {
                return _canChooseBanner;
            }
            set
            {
                if (value != _canChooseBanner)
                {
                    _canChooseBanner = value;
                    OnPropertyChanged("CanChooseBanner");
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
        public string LeaveKingdomText
        {
            get
            {
                return _leaveKingdomText;
            }
            set
            {
                if (value != _leaveKingdomText)
                {
                    _leaveKingdomText = value;
                    OnPropertyChanged("LeaveKingdomText");
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
        public string PartiesText
        {
            get
            {
                return _partiesText;
            }
            set
            {
                if (value != _partiesText)
                {
                    _partiesText = value;
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

        [DataSourceProperty]
        public HintViewModel ChangeClanNameHint
        {
            get
            {
                return _changeClanNameHint;
            }
            set
            {
                if (value != _changeClanNameHint)
                {
                    _changeClanNameHint = value;
                    OnPropertyChanged("ChangeClanNameHint");
                }
            }
        }

        [DataSourceProperty]
        public BasicTooltipViewModel GoldChangeTooltip
        {
            get
            {
                return _goldChangeTooltip;
            }
            set
            {
                if (value != _goldChangeTooltip)
                {
                    _goldChangeTooltip = value;
                    OnPropertyChanged("GoldChangeTooltip");
                }
            }
        }

        [DataSourceProperty]
        public string CurrentGoldText
        {
            get
            {
                return _currentGoldText;
            }
            set
            {
                if (value != _currentGoldText)
                {
                    _currentGoldText = value;
                    OnPropertyChanged("CurrentGoldText");
                }
            }
        }

        [DataSourceProperty]
        public int CurrentGold
        {
            get
            {
                return _currentGold;
            }
            set
            {
                if (value != _currentGold)
                {
                    _currentGold = value;
                    OnPropertyChanged("CurrentGold");
                }
            }
        }

        [DataSourceProperty]
        public string ExpenseText
        {
            get
            {
                return _expenseText;
            }
            set
            {
                if (value != _expenseText)
                {
                    _expenseText = value;
                    OnPropertyChanged("ExpenseText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalIncomeText
        {
            get
            {
                return _totalIncomeText;
            }
            set
            {
                if (value != _totalIncomeText)
                {
                    _totalIncomeText = value;
                    OnPropertyChanged("TotalIncomeText");
                }
            }
        }

        [DataSourceProperty]
        public string FinanceText
        {
            get
            {
                return _financeText;
            }
            set
            {
                if (value != _financeText)
                {
                    _financeText = value;
                    OnPropertyChanged("FinanceText");
                }
            }
        }

        [DataSourceProperty]
        public int TotalIncome
        {
            get
            {
                return _totalIncome;
            }
            set
            {
                if (value != _totalIncome)
                {
                    _totalIncome = value;
                    OnPropertyChanged("TotalIncome");
                }
            }
        }

        [DataSourceProperty]
        public string TotalExpensesText
        {
            get
            {
                return _totalExpensesText;
            }
            set
            {
                if (value != _totalExpensesText)
                {
                    _totalExpensesText = value;
                    OnPropertyChanged("TotalExpensesText");
                }
            }
        }

        [DataSourceProperty]
        public int TotalExpenses
        {
            get
            {
                return _totalExpenses;
            }
            set
            {
                if (value != _totalExpenses)
                {
                    _totalExpenses = value;
                    OnPropertyChanged("TotalExpenses");
                }
            }
        }

        [DataSourceProperty]
        public string DailyChangeText
        {
            get
            {
                return _dailyChangeText;
            }
            set
            {
                if (value != _dailyChangeText)
                {
                    _dailyChangeText = value;
                    OnPropertyChanged("DailyChangeText");
                }
            }
        }

        [DataSourceProperty]
        public int DailyChange
        {
            get
            {
                return _dailyChange;
            }
            set
            {
                if (value != _dailyChange)
                {
                    _dailyChange = value;
                    OnPropertyChanged("DailyChange");
                }
            }
        }

        [DataSourceProperty]
        public string ExpectedGoldText
        {
            get
            {
                return _expectedGoldText;
            }
            set
            {
                if (value != _expectedGoldText)
                {
                    _expectedGoldText = value;
                    OnPropertyChanged("ExpectedGoldText");
                }
            }
        }

        [DataSourceProperty]
        public int ExpectedGold
        {
            get
            {
                return _expectedGold;
            }
            set
            {
                if (value != _expectedGold)
                {
                    _expectedGold = value;
                    OnPropertyChanged("ExpectedGold");
                }
            }
        }

        [DataSourceProperty]
        public string DailyChangeValueText
        {
            get
            {
                return _dailyChangeValueText;
            }
            set
            {
                if (value != _dailyChangeValueText)
                {
                    _dailyChangeValueText = value;
                    OnPropertyChanged("DailyChangeValueText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalExpensesValueText
        {
            get
            {
                return _totalExpensesValueText;
            }
            set
            {
                if (value != _totalExpensesValueText)
                {
                    _totalExpensesValueText = value;
                    OnPropertyChanged("TotalExpensesValueText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalIncomeValueText
        {
            get
            {
                return _totalIncomeValueText;
            }
            set
            {
                if (value != _totalIncomeValueText)
                {
                    _totalIncomeValueText = value;
                    OnPropertyChanged("TotalIncomeValueText");
                }
            }
        }

        public WangVM(Action onClose, Action<MobileParty> openPartyAsManage, Action openBannerEditor)
        {
            _onClose = onClose;
            _openPartyAsManage = openPartyAsManage;
            _openBannerEditor = openBannerEditor;
            _clan = Hero.MainHero.Clan;
            _playerUpdateTracker = PlayerUpdateTracker.Current;
            ClanMembers = new WangClanMembersVM(RefreshCategoryValues);
            Informations = new ClanPartiesVM(OnAnyExpenseChange, _openPartyAsManage, RefreshCategoryValues);
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
            GoldChangeTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetGoldTooltip(Clan.PlayerClan));
            ChangeClanNameHint = new HintViewModel();
            RefreshDailyValues();
            UpdateBannerVisuals();
            CanChooseBanner = true;
            PlayerCanChangeClanName = (_clan.Leader == Hero.MainHero);
            ClanIsInAKingdom = (_clan.Kingdom != null);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Name = Hero.MainHero.Clan.Name.ToString();
            CurrentGoldText = GameTexts.FindText("str_clan_finance_current_gold").ToString();
            TotalExpensesText = GameTexts.FindText("str_clan_finance_total_expenses").ToString();
            TotalIncomeText = GameTexts.FindText("str_clan_finance_total_income").ToString();
            DailyChangeText = GameTexts.FindText("str_clan_finance_daily_change").ToString();
            ExpectedGoldText = GameTexts.FindText("str_clan_finance_expected").ToString();
            ExpenseText = GameTexts.FindText("str_clan_expenses").ToString();
            MembersText = new TextObject("{=wang_learning_skill}").ToString();
            PartiesText = new TextObject("{=wang_information}").ToString();
            IncomeText = new TextObject("{=wang_mod_setting}WangModSetting").ToString();
            FiefsText = new TextObject("{=wang_log}").ToString();
            DoneLbl = GameTexts.FindText("str_done").ToString();
            LeaderText = GameTexts.FindText("str_sort_by_leader_name_label").ToString();
            FinanceText = GameTexts.FindText("str_finance").ToString();
            LeaveKingdomText = new TextObject("{=EuGCNdHG}Leave Kingdom").ToString();
            GameTexts.SetVariable("TIER", Clan.PlayerClan.Tier);
            CurrentRenownText = GameTexts.FindText("str_clan_tier").ToString();
            ChangeClanNameHint.HintText = (PlayerCanChangeClanName ? "" : new TextObject("{=GCaYjA5W}You need to be the leader of the clan to change it's name.").ToString());
            _clanMembers?.RefreshValues();
            _clanParties?.RefreshValues();
            _clanFiefs?.RefreshValues();
            _clanIncome?.RefreshValues();
            _leader?.RefreshValues();
        }

        private void ExecuteOpenBannerEditor()
        {
            _openBannerEditor();
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
            Informations.IsSelected = false;
            ClanFiefs.IsSelected = false;
            ModSettings.IsSelected = false;
            switch (index)
            {
                case 0:
                    ClanMembers.IsSelected = true;
                    break;
                case 1:
                    Informations.IsSelected = true;
                    break;
                case 2:
                    ClanFiefs.IsSelected = true;
                    break;
                default:
                    ModSettings.IsSelected = true;
                    break;
            }
            IsMembersSelected = ClanMembers.IsSelected;
            IsPartiesSelected = Informations.IsSelected;
            IsFiefsSelected = ClanFiefs.IsSelected;
            IsIncomeSelected = ModSettings.IsSelected;
        }

        public void RefreshDailyValues()
        {
            if (ModSettings != null)
            {
                CurrentGold = Hero.MainHero.Gold;
                ExplainedNumber goldChange = new ExplainedNumber(0f, (StringBuilder)null);
                Campaign.Current.Models.ClanFinanceModel.CalculateClanIncome(_clan, ref goldChange);
                TotalIncome = (int)goldChange.ResultNumber;
                ExplainedNumber goldChange2 = new ExplainedNumber(0f, (StringBuilder)null);
                Campaign.Current.Models.ClanFinanceModel.CalculateClanExpenses(_clan, ref goldChange2);
                TotalExpenses = (int)goldChange2.ResultNumber;
                DailyChange = (int)(TaleWorlds.Library.MathF.Abs(TotalIncome) - TaleWorlds.Library.MathF.Abs(TotalExpenses));
                ExpectedGold = CurrentGold + DailyChange;
                if (TotalIncome == 0)
                {
                    TotalIncomeValueText = GameTexts.FindText("str_clan_finance_value_zero").ToString();
                }
                else
                {
                    GameTexts.SetVariable("IS_POSITIVE", (TotalIncome > 0) ? 1 : 0);
                    GameTexts.SetVariable("NUMBER", Math.Abs(TotalIncome));
                    TotalIncomeValueText = GameTexts.FindText("str_clan_finance_value").ToString();
                }
                if (TotalExpenses == 0)
                {
                    TotalExpensesValueText = GameTexts.FindText("str_clan_finance_value_zero").ToString();
                }
                else
                {
                    GameTexts.SetVariable("IS_POSITIVE", (TotalExpenses > 0) ? 1 : 0);
                    GameTexts.SetVariable("NUMBER", Math.Abs(TotalExpenses));
                    TotalExpensesValueText = GameTexts.FindText("str_clan_finance_value").ToString();
                }
                if (DailyChange == 0)
                {
                    DailyChangeValueText = GameTexts.FindText("str_clan_finance_value_zero").ToString();
                    return;
                }
                GameTexts.SetVariable("IS_POSITIVE", (DailyChange > 0) ? 1 : 0);
                GameTexts.SetVariable("NUMBER", Math.Abs(DailyChange));
                DailyChangeValueText = GameTexts.FindText("str_clan_finance_value").ToString();
            }
        }

        private void RefreshCategoryValues()
        {
            ClanFiefs.RefreshFiefsList();
            ClanMembers.RefreshMembersList();
            Informations.RefreshPartiesList();
            ModSettings.RefreshList();
        }

        private void ExecuteChangeClanName()
        {
            GameTexts.SetVariable("MAX_LETTER_COUNT", 50);
            GameTexts.SetVariable("MIN_LETTER_COUNT", 1);
            InformationManager.ShowTextInquiry(new TextInquiryData(GameTexts.FindText("str_change_clan_name").ToString(), string.Empty, isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_done").ToString(), GameTexts.FindText("str_cancel").ToString(), OnChangeClanNameDone, null, shouldInputBeObfuscated: false, IsNewClanNameApplicable));
        }

        private bool IsNewClanNameApplicable(string input)
        {
            if (input.Length <= 50)
            {
                return input.Length >= 1;
            }
            return false;
        }

        private void OnChangeClanNameDone(string newClanName)
        {
            TextObject textObject = new TextObject(newClanName ?? "");
            _clan.InitializeClan(textObject, textObject, _clan.Culture, _clan.Banner);
            RefreshCategoryValues();
            RefreshValues();
        }

        private void OnAnyExpenseChange()
        {
            RefreshDailyValues();
        }

        private void ExecuteClose()
        {
            _onClose();
        }

        private void ExecuteLeaveKingdom()
        {
            if (!Clan.PlayerClan.Settlements.Any())
            {
                InformationManager.ShowInquiry(new InquiryData(new TextObject("{=3sxtCWPe}Leaving Kingdom").ToString(), new TextObject("{=BgqZWbga}The nobles of the realm will dislike you for abandoning your fealty. Are you sure you want to leave the Kingdom?").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, new TextObject("{=5Unqsx3N}Confirm").ToString(), GameTexts.FindText("str_cancel").ToString(), OnConfirmLeaveKingdom, null));
                return;
            }
            List<InquiryElement> inquiryElements = new List<InquiryElement>
        {
            new InquiryElement("keep", new TextObject("{=z8h0BRAb}Keep all holdings").ToString(), null, isEnabled: true, "Owned settlements remain under your control but nobles will dislike this dishonorable act and the kingdom will declare war on you."),
            new InquiryElement("dontkeep", new TextObject("{=JIr3Jc7b}Relinquish all holdings").ToString(), null, isEnabled: true, "Owned settlements are returned to the kingdom. This will avert a war and nobles will dislike you less for abandoning your fealty.")
        };
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=3sxtCWPe}Leaving Kingdom").ToString(), new TextObject("{=xtlIFKaa}Are you sure you want to leave the Kingdom?{newline}If so, choose how you want to leave the kingdom.").ToString(), inquiryElements, isExitShown: true, forceOneSelection: true, new TextObject("{=5Unqsx3N}Confirm").ToString(), string.Empty, OnConfirmLeaveKingdomWithOption, null));
        }

        private void OnConfirmLeaveKingdomWithOption(List<InquiryElement> obj)
        {
            InquiryElement inquiryElement = obj.FirstOrDefault();
            if (inquiryElement != null)
            {
                string a = inquiryElement.Identifier as string;
                if (a == "keep")
                {
                    ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(Clan.PlayerClan, Clan.PlayerClan.Kingdom);
                }
                else if (a == "dontkeep")
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan);
                }
                ClanIsInAKingdom = (_clan.Kingdom != null);
                UpdateBannerVisuals();
            }
        }

        private void OnConfirmLeaveKingdom()
        {
            if (Clan.PlayerClan.IsUnderMercenaryService)
            {
                ChangeKingdomAction.ApplyByLeaveKingdomAsMercenaryForNoPayment(Clan.PlayerClan, Clan.PlayerClan.Kingdom);
            }
            else
            {
                ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan);
            }
            ClanIsInAKingdom = (_clan.Kingdom != null);
            UpdateBannerVisuals();
        }
    }
}
