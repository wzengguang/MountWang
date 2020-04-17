using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Wang
{
    public class CustomTroopLimitModel : DefaultTroopCountLimitModel
    {
        public override int GetHideoutBattlePlayerMaxTroopCount()
        {
            return TroopCountLimitConfig.HideoutLimit;
        }


    }
}
