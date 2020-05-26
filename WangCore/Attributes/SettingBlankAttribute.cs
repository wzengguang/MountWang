using System;

namespace Wang.Setting.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingBlankAttribute : SettingBaseAttribute
    {

        public SettingBlankAttribute(string name, string des) : base(name, des)
        {
        }
    }
}
