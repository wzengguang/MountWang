using System;

namespace Wang.Setting.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingBooleanAttribute : SettingBaseAttribute
    {
        public bool DefaultValue { get; private set; }

        public SettingBooleanAttribute(String name, String des, bool defaultValue = true) : base(name, des)
        {
            DefaultValue = defaultValue;
        }
    }
}
