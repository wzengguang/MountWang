using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options;
using TaleWorlds.TwoDimension;

namespace Wang.GauntletUI.ModSettings
{
    public class WangOptionsItemWidget : Widget
    {
        [DataSourceProperty]
        public Action HoverBegin { get; set; } = null;
        [DataSourceProperty]
        public Action HoverEnd { get; set; } = null;

        private OptionsScreenWidget _screenWidget;

        private bool _eventsRegistered;

        private bool _initialized;

        private Widget _dropdownExtensionParentWidget;

        private OptionsDropdownWidget _dropdownWidget;

        private ButtonWidget _booleanToggleButtonWidget;

        private List<Sprite> _graphicsSprites;

        private int _optionTypeID;

        private string _optionDescription;

        private string _optionTitle;

        private string[] _imageIDs;

        public Widget BooleanOption { get; set; }

        public Widget NumericOption { get; set; }

        public Widget StringOption { get; set; }

        public Widget GameKeyOption { get; set; }

        public Widget ActionOption { get; set; }

        public Widget InputOption { get; set; }
        public Widget BlankOption { get; set; }

        public WangOptionsItemWidget(UIContext context) : base(context)
        {
            this._optionTypeID = -1;
            this._graphicsSprites = new List<Sprite>();
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
            if (!this._initialized)
            {
                if (this.ImageIDs != null)
                {
                    int j;
                    int i;
                    for (i = 0; i < this.ImageIDs.Length; i = j + 1)
                    {
                        if (this.ImageIDs[i] != string.Empty)
                        {
                            Sprite item = base.Context.SpriteList.SingleOrDefault((Sprite s) => s.Name == this.ImageIDs[i]);
                            this._graphicsSprites.Add(item);
                        }
                        j = i;
                    }
                }
                this.RefreshVisibilityOfSubItems();
                this._initialized = true;
            }
            if (!this._eventsRegistered)
            {
                this.RegisterHoverEvents();
                this._eventsRegistered = true;
            }
        }

        protected override void OnHoverBegin()
        {
            HoverBegin?.Invoke();

            base.OnHoverBegin();
            this.SetCurrentOption(false, false, -1);
        }

        protected override void OnHoverEnd()
        {
            HoverEnd?.Invoke();
            base.OnHoverEnd();
            this.ResetCurrentOption();
        }

        private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
        {
            if (this._optionTypeID == 3)
            {
                if (fromHoverOverDropdown)
                {
                    if (this._graphicsSprites.Count > hoverDropdownItemIndex)
                    {
                        Sprite sprite = this._graphicsSprites[hoverDropdownItemIndex];
                    }
                }
                else if (this._graphicsSprites.Count > this._dropdownWidget.CurrentSelectedIndex && this._dropdownWidget.CurrentSelectedIndex >= 0)
                {
                    Sprite sprite2 = this._graphicsSprites[this._dropdownWidget.CurrentSelectedIndex];
                }
                OptionsScreenWidget screenWidget = this._screenWidget;
                if (screenWidget == null)
                {
                    return;
                }
                screenWidget.SetCurrentOption(this, null);
                return;
            }
            else if (this._optionTypeID == 0)
            {
                int num = this._booleanToggleButtonWidget.IsSelected ? 0 : 1;
                if (this._graphicsSprites.Count > num)
                {
                    Sprite sprite3 = this._graphicsSprites[num];
                }
                OptionsScreenWidget screenWidget2 = this._screenWidget;
                if (screenWidget2 == null)
                {
                    return;
                }
                screenWidget2.SetCurrentOption(this, null);
                return;
            }
            else
            {
                OptionsScreenWidget screenWidget3 = this._screenWidget;
                if (screenWidget3 == null)
                {
                    return;
                }
                screenWidget3.SetCurrentOption(this, null);
                return;
            }
        }

        public void SetCurrentScreenWidget(OptionsScreenWidget screenWidget)
        {
            this._screenWidget = screenWidget;
        }

        private void ResetCurrentOption()
        {
            OptionsScreenWidget screenWidget = this._screenWidget;
            if (screenWidget == null)
            {
                return;
            }
            screenWidget.SetCurrentOption(null, null);
        }

        private void RegisterHoverEvents()
        {
            foreach (Widget widget in base.AllChildren)
            {
                widget.PropertyChanged += this.Child_PropertyChanged;
            }
            if (this.OptionTypeID == 0)
            {
                this._booleanToggleButtonWidget = (this.BooleanOption.GetChild(0) as ButtonWidget);
                this._booleanToggleButtonWidget.PropertyChanged += this.BooleanOption_PropertyChanged;
                return;
            }
            if (this.OptionTypeID == 3)
            {
                this._dropdownWidget = (this.StringOption.GetChild(1) as OptionsDropdownWidget);
                this._dropdownExtensionParentWidget = this._dropdownWidget.DropdownClipWidget;
                foreach (Widget widget2 in this._dropdownExtensionParentWidget.AllChildren)
                {
                    widget2.PropertyChanged += this.DropdownItem_PropertyChanged1;
                }
            }
        }

        private void BooleanOption_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, object propertyValue)
        {
            if (propertyName == "IsSelected")
            {
                this.SetCurrentOption(false, true, -1);
            }
        }

        private void Child_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, object propertyValue)
        {
            if (propertyName == "IsHovered")
            {
                if ((bool)propertyValue)
                {
                    this.SetCurrentOption(false, false, -1);
                    return;
                }
                this.ResetCurrentOption();
            }
        }

        private void DropdownItem_PropertyChanged1(PropertyOwnerObject childWidget, string propertyName, object propertyValue)
        {
            if (propertyName == "IsHovered")
            {
                if ((bool)propertyValue)
                {
                    Widget widget = childWidget as Widget;
                    this.SetCurrentOption(true, false, widget.ParentWidget.GetChildIndex(widget));
                    return;
                }
                this.ResetCurrentOption();
            }
        }

        private void RefreshVisibilityOfSubItems()
        {
            this.BooleanOption.IsVisible = (this.OptionTypeID == 0);
            this.NumericOption.IsVisible = (this.OptionTypeID == 1);
            this.StringOption.IsVisible = (this.OptionTypeID == 3);
            this.GameKeyOption.IsVisible = (this.OptionTypeID == 2);
            this.InputOption.IsVisible = (this.OptionTypeID == 4);
            this.BlankOption.IsVisible = (OptionTypeID == 6);
            if (this.ActionOption != null)
            {
                this.ActionOption.IsVisible = (this.OptionTypeID == 5);
            }
        }

        public int OptionTypeID
        {
            get
            {
                return this._optionTypeID;
            }
            set
            {
                if (this._optionTypeID != value)
                {
                    this._optionTypeID = value;
                    base.OnPropertyChanged(this._optionTypeID, nameof(OptionTypeID));
                }
            }
        }

        public string OptionTitle
        {
            get
            {
                return this._optionTitle;
            }
            set
            {
                if (this._optionTitle != value)
                {
                    this._optionTitle = value;
                }
            }
        }

        public string[] ImageIDs
        {
            get
            {
                return this._imageIDs;
            }
            set
            {
                if (this._imageIDs != value)
                {
                    this._imageIDs = value;
                }
            }
        }

        public string OptionDescription
        {
            get
            {
                return this._optionDescription;
            }
            set
            {
                if (this._optionDescription != value)
                {
                    this._optionDescription = value;
                }
            }
        }

    }
}
