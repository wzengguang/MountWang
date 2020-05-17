using HarmonyLib;
using SandBox.Source.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultTroopCountLimitModel), "GetHideoutBattlePlayerMaxTroopCount")]
    public class DefaultTroopCountLimitModelPatch
    {
        private static void Postfix(ref int __result)
        {
            //     InformationManager.DisplayMessage(new InformationMessage(__result.ToString()));

            __result += Clan.PlayerClan.Tier * 3;
        }
    }
    //[HarmonyPatch(typeof(HideoutMissionController), "HideoutMissionController", MethodType.Constructor)]
    //public class DefaultTroopCountLimitModelPatchs
    //{
    //    private static void HideoutMissionController(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, int firstPhaseTroopCount)
    //    {

    //        InformationManager.DisplayMessage(new InformationMessage(playerSide.ToString()));

    //    }
    //}
}
