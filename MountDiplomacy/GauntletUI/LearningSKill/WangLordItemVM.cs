using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.EncyclopediaItems;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Wang.GauntletUI
{
    public class WangLordItemVM : ViewModel
    {
        private readonly Action<WangLordItemVM> _onCharacterSelect;

        private readonly Hero _hero;

        private ImageIdentifierVM _visual;

        private ImageIdentifierVM _banner_9;

        private bool _isSelected;

        private bool _isChild;

        private string _name;

        private string _locationText;

        private string _relationToMainHeroText;

        private string _governorOfText;

        private string _currentActionText;

        private HeroViewModel _heroModel;

        private MBBindingList<EncyclopediaSkillVM> _skills;

        private MBBindingList<EncyclopediaTraitItemVM> _traits;

        private SelectorVM<SkillLearningSelectorItemVM> _learningSkillSelection;

        public bool Initialized
        {
            get;
            set;
        }

        [DataSourceProperty]
        public SelectorVM<SkillLearningSelectorItemVM> LearningSkillSelection
        {
            get
            {
                return _learningSkillSelection;
            }
            set
            {
                if (value != _learningSkillSelection)
                {
                    _learningSkillSelection = value;
                    OnPropertyChanged("LearningSkillSelection");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<EncyclopediaSkillVM> Skills
        {
            get
            {
                return _skills;
            }
            set
            {
                if (value != _skills)
                {
                    _skills = value;
                    OnPropertyChanged("Skills");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<EncyclopediaTraitItemVM> Traits
        {
            get
            {
                return _traits;
            }
            set
            {
                if (value != _traits)
                {
                    _traits = value;
                    OnPropertyChanged("Traits");
                }
            }
        }

        [DataSourceProperty]
        public HeroViewModel HeroModel
        {
            get
            {
                return _heroModel;
            }
            set
            {
                if (value != _heroModel)
                {
                    _heroModel = value;
                    OnPropertyChanged("HeroModel");
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
        public bool IsChild
        {
            get
            {
                return _isChild;
            }
            set
            {
                if (value != _isChild)
                {
                    _isChild = value;
                    OnPropertyChanged("IsChild");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM Visual
        {
            get
            {
                return _visual;
            }
            set
            {
                if (value != _visual)
                {
                    _visual = value;
                    OnPropertyChanged("Visual");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM Banner_9
        {
            get
            {
                return _banner_9;
            }
            set
            {
                if (value != _banner_9)
                {
                    _banner_9 = value;
                    OnPropertyChanged("Banner_9");
                }
            }
        }

        [DataSourceProperty]
        public string LocationText
        {
            get
            {
                return _locationText;
            }
            set
            {
                if (value != _locationText)
                {
                    _locationText = value;
                    OnPropertyChanged("LocationText");
                }
            }
        }

        [DataSourceProperty]
        public string CurrentActionText
        {
            get
            {
                return _currentActionText;
            }
            set
            {
                if (value != _currentActionText)
                {
                    _currentActionText = value;
                    OnPropertyChanged("CurrentActionText");
                }
            }
        }

        [DataSourceProperty]
        public string RelationToMainHeroText
        {
            get
            {
                return _relationToMainHeroText;
            }
            set
            {
                if (value != _relationToMainHeroText)
                {
                    _relationToMainHeroText = value;
                    OnPropertyChanged("RelationToMainHeroText");
                }
            }
        }

        [DataSourceProperty]
        public string GovernorOfText
        {
            get
            {
                return _governorOfText;
            }
            set
            {
                if (value != _governorOfText)
                {
                    _governorOfText = value;
                    OnPropertyChanged("GovernorOfText");
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

        public WangLordItemVM(Hero hero, Action<WangLordItemVM> onCharacterSelect)
        {
            _hero = hero;
            _onCharacterSelect = onCharacterSelect;
            CharacterCode characterCode = CharacterCode.CreateFrom(hero.CharacterObject);
            Visual = new ImageIdentifierVM(characterCode);
            Skills = new MBBindingList<EncyclopediaSkillVM>();
            Traits = new MBBindingList<EncyclopediaTraitItemVM>();
            Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(hero.ClanBanner), nineGrid: true);
            HeroModel = new HeroViewModel();
            HeroModel.FillFrom(_hero);
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Name = _hero.Name.ToString();
            CurrentActionText = ((_hero != Hero.MainHero) ? CampaignUIHelper.GetHeroBehaviorText(_hero) : "");
            if (_hero.PartyBelongedToAsPrisoner != null)
            {
                TextObject textObject = new TextObject("{=a8nRxITn}Prisoner of {PARTY_NAME}");
                textObject.SetTextVariable("PARTY_NAME", _hero.PartyBelongedToAsPrisoner.Name);
                LocationText = textObject.ToString();
            }
            else
            {
                LocationText = ((_hero != Hero.MainHero) ? StringHelpers.GetLastKnownLocation(_hero).ToString() : " ");
            }
            UpdateProperties();
        }

        private void ExecuteLocationLink(string link)
        {
            Campaign.Current.EncyclopediaManager.GoToLink(link);
        }

        public void UpdateProperties()
        {
            RelationToMainHeroText = "";
            GovernorOfText = "";
            Skills.Clear();
            Traits.Clear();
            UpdateLearningSkillSelection();
            foreach (SkillObject item in SkillObject.All)
            {
                Skills.Add(new EncyclopediaSkillVM(item, _hero.GetSkillValue(item)));
            }
            foreach (TraitObject heroTrait in CampaignUIHelper.GetHeroTraits())
            {
                if (_hero.GetTraitLevel(heroTrait) != 0)
                {
                    Traits.Add(new EncyclopediaTraitItemVM(heroTrait, _hero));
                }
            }
            IsChild = _hero.IsChild;
            if (_hero != Hero.MainHero)
            {
                RelationToMainHeroText = CampaignUIHelper.GetHeroRelationToHeroText(_hero, Hero.MainHero).ToString();
            }
            if (_hero.GovernorOf != null)
            {
                GameTexts.SetVariable("SETTLEMENT_NAME", _hero.GovernorOf.Owner.Settlement.EncyclopediaLinkWithName);
                GovernorOfText = GameTexts.FindText("str_governor_of_label").ToString();
            }
            HeroModel = new HeroViewModel();
            HeroModel.FillFrom(_hero);
            Banner_9 = new ImageIdentifierVM(BannerCode.CreateFrom(_hero.ClanBanner), nineGrid: true);
        }

        private void OnLearningSkillSelectionChange(SelectorVM<SkillLearningSelectorItemVM> obj)
        {
            if (Initialized && obj.SelectedItem != null)
            {
                Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>().SetHeroLearningSkill(_hero, obj.SelectedItem.Skill);
            }
        }

        public void UpdateLearningSkillSelection()
        {
            LearningSkillSelection = new SelectorVM<SkillLearningSelectorItemVM>(0, OnLearningSkillSelectionChange);
            LearningSkillSelection.SetOnChangeAction(null);


            foreach (SkillObject skillObject in SkillObject.All)
            {
                SkillLearningSelectorItemVM item = new SkillLearningSelectorItemVM(skillObject, true, "");
                LearningSkillSelection.AddItem(item);
            }
            var current = Campaign.Current.GetCampaignBehavior<HeroLearningSkillBehaviour>().getHeroLearningSkill(_hero);

            for (int i = 0; i < LearningSkillSelection.ItemList.Count; i++)
            {
                if (current != null && current == LearningSkillSelection.ItemList[i].Skill)
                {
                    LearningSkillSelection.SelectedIndex = i;
                    break;
                }
            }
            LearningSkillSelection.SetOnChangeAction(OnLearningSkillSelectionChange);
        }

        private void ExecuteLink()
        {
            Campaign.Current.EncyclopediaManager.GoToLink(_hero.EncyclopediaLink);
        }

        private void OnCharacterSelect()
        {
            _onCharacterSelect(this);
        }

        protected virtual void ExecuteBeginHint()
        {
            InformationManager.AddTooltipInformation(typeof(Hero), _hero);
        }

        protected virtual void ExecuteEndHint()
        {
            InformationManager.HideInformations();
        }

        public Hero GetHero()
        {
            return _hero;
        }


    }
}
