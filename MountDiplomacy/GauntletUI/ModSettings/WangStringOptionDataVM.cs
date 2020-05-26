using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Wang.Setting;

namespace Wang.GauntletUI.ModSettings
{
    class WangStringOptionDataVM : WangGenericOptionDataVM
    {
        private readonly Dictionary<int, string> _selectorIndex = new Dictionary<int, string>();

        public SelectorVM<SelectorItemVM> _selector;

        public WangStringOptionDataVM(SettingVM settingVM, ISetting setting, PropertyInfo property, PropertyInfo selector) : base(settingVM, setting, property)
        {
            try
            {
                var value = (string)property.GetValue(setting);
                List<TextObject> list = new List<TextObject>() { new TextObject("{=wang_selector_none}please select a item") };
                _selectorIndex.Add(0, null);
                var selectedIndex = 0;

                var strs = selector.GetValue(setting);

                if (strs is IReadOnlyList<string>)
                {
                    var datas = (IReadOnlyList<string>)strs;

                    for (int i = 0; i < datas.Count; i++)
                    {
                        var text = new TextObject(datas[i]);
                        list.Add(text);
                        _selectorIndex.Add(i + 1, text.GetID());
                        if (value == text.GetID())
                        {
                            selectedIndex = i + 1;
                        }
                    }
                }
                else if (strs is Dictionary<string, TextObject>)
                {
                    var datas = (Dictionary<string, TextObject>)strs;

                    for (int i = 0; i < datas.Count; i++)
                    {
                        list.Add(datas.ElementAt(i).Value);
                        _selectorIndex.Add(i + 1, datas.ElementAt(i).Key);
                        if (value == datas.ElementAt(i).Key)
                        {
                            selectedIndex = i + 1;
                        }

                    }
                }

                this._selector = new SelectorVM<SelectorItemVM>(list, selectedIndex, new Action<SelectorVM<SelectorItemVM>>(this.UpdateValue));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.FlattenException());

            }
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            SelectorVM<SelectorItemVM> selector = this._selector;
            if (selector == null)
            {
                return;
            }
            selector.RefreshValues();
        }
        public void UpdateValue(SelectorVM<SelectorItemVM> selector)
        {
            if (selector.SelectedIndex >= 0)
            {
                Property.SetValue(Setting, _selectorIndex[selector.SelectedIndex]);
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> Selector
        {
            get
            {
                return this._selector;
            }
            set
            {
                if (value != this._selector)
                {
                    this._selector = value;
                    base.OnPropertyChanged(nameof(Selector));
                }
            }
        }


        public override void UpdateValue()
        {
            if (Selector.SelectedIndex >= 0)
            {
                Property.SetValue(Setting, _selectorIndex[Selector.SelectedIndex]);
            }
        }
    }
}
