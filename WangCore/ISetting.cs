using System;

namespace Wang.Setting
{
    public interface ISetting
    {

        string Name { get; set; }
        string Description { get; set; }
        int Order { get; set; }

    }


}