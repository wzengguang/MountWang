using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Wang.GauntletUI;
using Wang.Setting;
using Wang.Setting.Attributes;

namespace Wang.GauntletUI.ModSettings
{
    public abstract class WangGenericOptionDataVM : ViewModel
    {
        protected SettingVM SettingVM { get; private set; }

        protected ISetting Setting { get; private set; }
        protected PropertyInfo Property { get; private set; }


        private readonly TextObject _nameObj;

        private readonly TextObject _descriptionObj;

        private string _description;

        private string _name;

        private int _optionTypeId = -1;

        private bool _isBlank;

        private bool _hasDescription;


        public WangGenericOptionDataVM(SettingVM settingVM, ISetting setting, PropertyInfo property)
        {
            SettingVM = settingVM;
            Setting = setting;
            Property = property;


            if (property.PropertyType == typeof(bool))
            {
                this.OptionTypeID = (int)WangOptionsDataType.BooleanOption;

            }
            else if (property.PropertyType == typeof(float))
            {
                this.OptionTypeID = (int)WangOptionsDataType.NumericOption;

            }
            else if (property.PropertyType == typeof(string))
            {
                this.OptionTypeID = (int)WangOptionsDataType.MultipleSelectionOption;

            }
            else if (property.PropertyType == typeof(DateTime))
            {
                this.OptionTypeID = (int)WangOptionsDataType.Blank;
                this.IsBlank = true;
            }

            foreach (var item in property.GetCustomAttributes(true))
            {
                if (item as SettingBaseAttribute != null)
                {
                    var b = item as SettingBaseAttribute;

                    _nameObj = new TextObject(b.Name);
                    if (!string.IsNullOrEmpty(b.Description))
                    {
                        _descriptionObj = new TextObject(b.Description);
                        _descriptionObj.SetTextVariable("newline", "{=!}\n");

                    }
                }
            }

            this.RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            this.Name = this._nameObj.ToString();
            if (this._descriptionObj != null)
            {
                this.Description = this._descriptionObj.ToString();
            }
            this.HasDescription = this._descriptionObj != null;

        }


        private void OnHover()
        {
            if (SettingVM != null)
            {
                SettingVM.OnHoverDescription = Description?.ToString();
                SettingVM.IsHover = true;
            }
        }

        private void OnHoverEnd()
        {
            if (SettingVM != null)
            {
                SettingVM.IsHover = false;

                SettingVM.OnHoverDescription = string.IsNullOrEmpty(Setting.Description) ? null : new TextObject(Setting.Description)?.ToString();
            }
        }

        [DataSourceProperty]
        public Action OnHoverAction => OnHover;
        [DataSourceProperty]
        public Action OnHoverEndAction => OnHoverEnd;


        [DataSourceProperty]
        public bool HasDescription
        {
            get
            {
                return this._hasDescription;
            }
            set
            {
                if (value != this._hasDescription)
                {
                    this._hasDescription = value;
                    base.OnPropertyChanged(nameof(HasDescription));
                }
            }
        }

        [DataSourceProperty]
        public bool IsBlank
        {
            get
            {
                return this._isBlank;
            }
            set
            {
                if (value != this._isBlank)
                {
                    this._isBlank = value;
                    base.OnPropertyChanged(nameof(IsBlank));
                }
            }
        }


        [DataSourceProperty]
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                if (value != this._description)
                {
                    this._description = value;
                    base.OnPropertyChanged(nameof(Description));
                }
            }
        }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    base.OnPropertyChanged(nameof(Name));
                }
            }
        }



        [DataSourceProperty]
        public int OptionTypeID
        {
            get
            {
                return this._optionTypeId;
            }
            set
            {
                if (value != this._optionTypeId)
                {
                    this._optionTypeId = value;
                    base.OnPropertyChanged(nameof(OptionTypeID));
                }
            }
        }

        public abstract void UpdateValue();

        //public abstract void Cancel();

        //public abstract bool IsChanged();

        //public abstract void SetValue(float value);

        //public abstract void ResetData();
    }
}
