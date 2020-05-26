using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wang.Setting.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingBaseAttribute : Attribute
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public SettingBaseAttribute(String name, String des)
        {
            Name = name;
            Description = des;
        }

    }
}
