using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Wang.GameComponents
{

    public class WangDefaultTroopSacrificeModel : DefaultTroopSacrificeModel
    {
        public override int GetLostTroopCountForBreakingInBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
        {
            // return this.GetLostTroopCount(party, siegeEvent);

            return 0;
        }
    }
}