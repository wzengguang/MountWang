using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.Smelting;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang
{
    [HarmonyPatch(typeof(SmeltingVM), "RefreshList")]
    public class SmeltingVMPatch
    {
        private static void Postfix(SmeltingVM __instance)
        {
            if (__instance.SmeltableItemList == null || __instance.SmeltableItemList.Count == 0)
            {
                return;
            }

            var locks = Campaign.Current.GetCampaignBehavior<InventoryLockTracker>().GetLocks().Select(a => a.Item).ToList();

            List<SmeltingItemVM> smeltingItemVMs = new List<SmeltingItemVM>();
            foreach (var item in __instance.SmeltableItemList)
            {
                if (locks.Contains(item.Item))
                {
                    smeltingItemVMs.Add(item);
                }

            }
            foreach (var item in smeltingItemVMs)
            {
                __instance.SmeltableItemList.Remove(item);
            }

            if (__instance.SmeltableItemList.Count == 0)
            {
                __instance.CurrentSelectedItem = null;
            }
        }
    }
}
