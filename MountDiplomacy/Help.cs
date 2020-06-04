using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Wang.Setting;

namespace Wang
{
    public static class Help
    {

        public static bool IsPlayerCraft(this ItemObject item)
        {
            return item.Name.ToString().Contains("*");
        }
    }
}
