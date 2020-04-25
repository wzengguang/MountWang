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
        private static void Postfix(SmeltingVM __instance, ref ItemRoster ____playerItemRoster, Action ____updateYieldValuesAcion)
        {
            var locks = Campaign.Current.GetCampaignBehavior<InventoryLockTracker>().GetLocks().Select(a => a.Item).ToList();

            __instance.SmeltableItemList = new MBBindingList<SmeltingItemVM>();
            __instance.SortController.SetListToControl(__instance.SmeltableItemList);
            for (int i = 0; i < ____playerItemRoster.Count; i++)
            {
                ItemRosterElement elementCopyAtIndex = ____playerItemRoster.GetElementCopyAtIndex(i);
                if (elementCopyAtIndex.EquipmentElement.Item.IsCraftedWeapon && !locks.Contains(elementCopyAtIndex.EquipmentElement.Item))
                {
                    __instance.SmeltableItemList.Add(new SmeltingItemVM(elementCopyAtIndex.EquipmentElement.Item, newItem =>
                    {
                        if (newItem != __instance.CurrentSelectedItem)
                        {
                            if (__instance.CurrentSelectedItem != null)
                            {
                                __instance.CurrentSelectedItem.IsSelected = false;
                            }
                            __instance.CurrentSelectedItem = newItem;
                            __instance.CurrentSelectedItem.IsSelected = true;
                        }
                        ____updateYieldValuesAcion();


                    }, elementCopyAtIndex.Amount));
                }
            }
            if (__instance.SmeltableItemList.Count == 0)
            {
                __instance.CurrentSelectedItem = null;
            }
        }
    }
}
