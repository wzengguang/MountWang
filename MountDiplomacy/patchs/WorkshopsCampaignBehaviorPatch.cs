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
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch]
    public class WorkshopsCampaignBehaviorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "OnWarDeclared")]
        private static bool OnWarDeclared(IFaction faction1, IFaction faction2)
        {
            return !CommonSetting.Instance.WorkshopNoConfiscate;

        }




        /// <summary>
        /// 工坊不生产自己制造的物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopsCampaignBehavior), "IsProducable")]
        private static bool IsProducable(ItemObject item, ref bool __result)
        {
            __result = !item.MultiplayerItem && !item.NotMerchandise && !item.IsPlayerCraft();

            return false;
        }



    }
}
