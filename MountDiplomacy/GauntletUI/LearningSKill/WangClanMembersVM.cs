using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private string _kickFromClanText;

        private HintViewModel _kickFromClanActionHint;

        private bool _isAnyValidMemberSelected;

        private bool _canKickCurrentMemberFromClan;

        [DataSourceProperty]
        public HintViewModel KickFromClanActionHint
        {
            get
            {
                return _kickFromClanActionHint;
            }
            set
            {
                if (value != _kickFromClanActionHint)
                {
                    _kickFromClanActionHint = value;
                    OnPropertyChanged("KickFromClanActionHint");
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
        public bool CanKickCurrentMemberFromClan
        {
            get
            {
                return _canKickCurrentMemberFromClan;
            }
            set
            {
                if (value != _canKickCurrentMemberFromClan)
                {
                    _canKickCurrentMemberFromClan = value;
                    OnPropertyChanged("CanKickCurrentMemberFromClan");
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
        public string KickFromClanText
        {
            get
            {
                return _kickFromClanText;
            }
            set
            {
                if (value != _kickFromClanText)
                {
                    _kickFromClanText = value;
                    OnPropertyChanged("KickFromClanText");
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
            KickFromClanActionHint = new HintViewModel();
            RefreshMembersList();
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            FamilyText = GameTexts.FindText("str_family_group").ToString();
            KickFromClanText = GameTexts.FindText("str_kick_from_clan").ToString();
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
            bool flag = member.GetHero() == Hero.MainHero;
            bool flag2 = _faction.Companions.Contains(member.GetHero());
            bool flag3 = Campaign.Current.IssueManager.IssueSolvingCompanionList.Contains(member.GetHero());
            CanKickCurrentMemberFromClan = (!flag && flag2 && !flag3);
            KickFromClanActionHint.HintText = CampaignUIHelper.GetKickFromClanReasonString(flag, flag2, flag3);
            if (member != null)
            {
                member.IsSelected = true;
                member.Initialized = true;
                member.UpdateLearningSkillSelection();
            }
        }

        private void ExecuteKickCurrentMemberFromClan()
        {
            if (CanKickCurrentMemberFromClan)
            {
                InformationManager.ShowInquiry(new InquiryData(string.Empty, GameTexts.FindText("str_kick_companion_from_clan_inquiry").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), OnKickFromClan, null));
            }
        }

        private void OnKickFromClan()
        {
            RemoveCompanionAction.ApplyByFire(_faction, CurrentSelectedMember.GetHero());
            _onRefresh?.Invoke();
        }

        private void ExecuteLink(string link)
        {
            Campaign.Current.EncyclopediaManager.GoToLink(link);
        }
    }
}
