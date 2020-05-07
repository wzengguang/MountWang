using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Wang.GauntletUI
{
    public class WangClanMembersVM : ViewModel
    {
        private readonly Clan _faction;

        private readonly Action _onRefresh;

        private bool _isSelected;

        private MBBindingList<WangLordItemVM> _companions;

        private MBBindingList<WangLordItemVM> _family;

        private WangLordItemVM _currentSelectedMember;

        private string _familyText;

        private string _traitsText;

        private string _clanRoleText;

        private string _companionsText;

        private string _wishPerksText;

        private HintViewModel _canWashPerksHint;

        private bool _isAnyValidMemberSelected;

        private bool _canWashPerks;

        [DataSourceProperty]
        public HintViewModel CanWashPerksHint
        {
            get
            {
                return _canWashPerksHint;
            }
            set
            {
                if (value != _canWashPerksHint)
                {
                    _canWashPerksHint = value;
                    OnPropertyChanged("CanWashPerksHint");
                }
            }
        }

        [DataSourceProperty]
        public bool IsAnyValidMemberSelected
        {
            get
            {
                return _isAnyValidMemberSelected;
            }
            set
            {
                if (value != _isAnyValidMemberSelected)
                {
                    _isAnyValidMemberSelected = value;
                    OnPropertyChanged("IsAnyValidMemberSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool CanWashPerks
        {
            get
            {
                return _canWashPerks;
            }
            set
            {
                if (value != _canWashPerks)
                {
                    _canWashPerks = value;
                    OnPropertyChanged("CanWashPerks");
                }
            }
        }

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
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        [DataSourceProperty]
        public string FamilyText
        {
            get
            {
                return _familyText;
            }
            set
            {
                if (value != _familyText)
                {
                    _familyText = value;
                    OnPropertyChanged("FamilyText");
                }
            }
        }

        [DataSourceProperty]
        public string TraitsText
        {
            get
            {
                return _traitsText;
            }
            set
            {
                if (value != _traitsText)
                {
                    _traitsText = value;
                    OnPropertyChanged("TraitsText");
                }
            }
        }

        [DataSourceProperty]
        public string LearnSkillFromOtherText
        {
            get
            {
                return _clanRoleText;
            }
            set
            {
                if (value != _clanRoleText)
                {
                    _clanRoleText = value;
                    OnPropertyChanged("ClanRoleText");
                }
            }
        }

        [DataSourceProperty]
        public string WishPerksText
        {
            get
            {
                return _wishPerksText;
            }
            set
            {
                if (value != _wishPerksText)
                {
                    _wishPerksText = value;
                    OnPropertyChanged("WishPerksText");
                }
            }
        }

        [DataSourceProperty]
        public string CompanionsText
        {
            get
            {
                return _companionsText;
            }
            set
            {
                if (value != _companionsText)
                {
                    _companionsText = value;
                    OnPropertyChanged("CompanionsText");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<WangLordItemVM> Companions
        {
            get
            {
                return _companions;
            }
            set
            {
                if (value != _companions)
                {
                    _companions = value;
                    OnPropertyChanged("Companions");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<WangLordItemVM> Family
        {
            get
            {
                return _family;
            }
            set
            {
                if (value != _family)
                {
                    _family = value;
                    OnPropertyChanged("Family");
                }
            }
        }

        [DataSourceProperty]
        public WangLordItemVM CurrentSelectedMember
        {
            get
            {
                return _currentSelectedMember;
            }
            set
            {
                if (value != _currentSelectedMember)
                {
                    _currentSelectedMember = value;
                    OnPropertyChanged("CurrentSelectedMember");
                    IsAnyValidMemberSelected = (value != null);
                }
            }
        }

        public WangClanMembersVM(Action onRefresh)
        {
            _onRefresh = onRefresh;
            _faction = Hero.MainHero.Clan;
            Family = new MBBindingList<WangLordItemVM>();
            Companions = new MBBindingList<WangLordItemVM>();
            CanWashPerksHint = new HintViewModel();
            RefreshMembersList();
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            FamilyText = GameTexts.FindText("str_family_group").ToString();
            WishPerksText = new TextObject("{=wang_washPerk}").ToString();
            TraitsText = GameTexts.FindText("str_traits_group").ToString();
            LearnSkillFromOtherText = new TextObject("{=str_learning_skill}").ToString();
            Family.ApplyActionOnAllItems(delegate (WangLordItemVM x)
            {
                x.RefreshValues();
            });
            Companions.ApplyActionOnAllItems(delegate (WangLordItemVM x)
            {
                x.RefreshValues();
            });
        }

        public void RefreshMembersList()
        {
            Family.Clear();
            Companions.Clear();
            List<Hero> list = new List<Hero>();
            foreach (Hero noble in _faction.Nobles)
            {
                HeroHelper.SetLastSeenLocation(noble, willUpdateImmediately: true);
                if (noble.IsAlive)
                {
                    if (noble == Hero.MainHero)
                    {
                        list.Insert(0, noble);
                    }
                    else
                    {
                        list.Add(noble);
                    }
                }
            }
            IEnumerable<Hero> enumerable = _faction.Companions.Where((Hero m) => m.IsPlayerCompanion);
            foreach (Hero item in list)
            {
                Family.Add(new WangLordItemVM(item, OnMemberSelection));
            }
            foreach (Hero item2 in enumerable)
            {
                Companions.Add(new WangLordItemVM(item2, OnMemberSelection));
            }
            GameTexts.SetVariable("COMPANION_COUNT", _faction.Companions.Count());
            GameTexts.SetVariable("COMPANION_LIMIT", _faction.CompanionLimit);
            CompanionsText = GameTexts.FindText("str_companions_group").ToString();
            OnMemberSelection(GetDefaultMember());
        }

        private WangLordItemVM GetDefaultMember()
        {
            if (Family.Any())
            {
                return Family.First();
            }
            if (Companions.Any())
            {
                return Companions.First();
            }
            return null;
        }

        private void OnMemberSelection(WangLordItemVM member)
        {
            if (CurrentSelectedMember != null)
            {
                CurrentSelectedMember.IsSelected = false;
                CurrentSelectedMember.Initialized = false;
            }
            member.Initialized = false;
            CurrentSelectedMember = member;
            RefreshCanWashPerks();
            CanWashPerksHint.HintText = new TextObject("{=wang_washHint}").ToString();
            if (member != null)
            {
                member.IsSelected = true;
                member.Initialized = true;
                member.UpdateLearningSkillSelection();
            }
        }

        private void WishCurrentMemberPerks()
        {

            if (CanWashPerks)
            {
                var time = (3 - Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>().GetWishPerkTime(CurrentSelectedMember.GetHero())).ToString();
                MBTextManager.SetTextVariable("WASH_TIME", time);
                InformationManager.ShowInquiry(new InquiryData(string.Empty, new TextObject("{=wang_washPerkConfirm}").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), OnWishPerks, null));
            }
        }

        private void OnWishPerks()
        {
            var hero = CurrentSelectedMember.GetHero();
            hero.ClearPerks();
            hero.HeroDeveloper.GetType().GetMethod("DiscoverOpenedPerks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(hero.HeroDeveloper, null);

            WashAttributes(hero);
            RefreshCanWashPerks(true);
        }

        private void RefreshCanWashPerks(bool wash = false)
        {
            var be = Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>();

            CanWashPerks = (wash ? be.SetWishPerkTime(CurrentSelectedMember.GetHero()) : be.GetWishPerkTime(CurrentSelectedMember.GetHero())) < 3;
        }


        private void WashAttributes(Hero hero)
        {
            try
            {
                var totalAttributes = 0;
                for (int i = 0; i < 6; i++)
                {
                    totalAttributes += hero.GetAttributeValue((CharacterAttributesEnum)i);
                }

                hero.HeroDeveloper.UnspentAttributePoints += totalAttributes;
                hero.ClearAttributes();
            }
            catch (Exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("WashAttributes"));
            }
        }


        private void ExecuteLink(string link)
        {
            Campaign.Current.EncyclopediaManager.GoToLink(link);
        }
    }
}
