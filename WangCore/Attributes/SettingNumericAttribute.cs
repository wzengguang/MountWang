using System;

namespace Wang.Setting.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingNumericAttribute : SettingBaseAttribute
    {
        public float DefaultValue { get; private set; }
        public float Min { get; private set; }
        public float Max { get; private set; }
        public bool IsDiscrete { get; private set; }

        public SettingNumericAttribute(String name, String des, float defaultValue, float min, float max, bool isDiscrete = true) : base(name, des)
        {
            DefaultValue = defaultValue;
            Max = max;
            Min = min;
            IsDiscrete = isDiscrete;
        }
    }
}
