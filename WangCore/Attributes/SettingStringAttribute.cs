using System;

namespace Wang.Setting.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingStringAttribute : SettingBaseAttribute
    {
        public string SelectorProperty { get; set; }

        public SettingStringAttribute(string name, string des, string selectorProperty) : base(name, des)
        {
            SelectorProperty = selectorProperty;
        }
    }
}
