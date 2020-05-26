using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Wang.Setting;

namespace Wang.patchs
{
    [HarmonyPatch(typeof(DefaultSettlementEconomyModel))]
    public class DefaultSettlementEconomyModelPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetTownGoldChange")]
        private static bool GetTownGoldChange(Town town, ref int __result)
        {
            if (SettlementSetting.Instance.TownCapitalFactor == 0f)
            {
                return true;
            }

            float num = town.Prosperity * 7 * (SettlementSetting.Instance.TownCapitalFactor + 1) - (float)town.Gold;
            __result = MathF.Round(0.2f * num);

            return false;
        }
    }
}
