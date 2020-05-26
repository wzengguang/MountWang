using Newtonsoft.Json;

namespace Wang.Setting
{
    public abstract class SettingBase : ISetting
    {

        [JsonIgnore]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual string Description { get; set; }

        [JsonIgnore]
        public virtual int Order { get; set; } = 100;


    }


}