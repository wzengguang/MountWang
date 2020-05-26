using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using Wang.Setting;

namespace Wang
{


    [HarmonyPatch(typeof(SPInventoryVM))]

    class SPInventoryVMPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch("ExecuteSellAllItems")]
        private static bool ExecuteSellAllItems(SPInventoryVM __instance, ref InventoryLogic ____inventoryLogic, ref CharacterObject ____currentCharacter)
        {
            var golds = 0;
            var leftGold = __instance.LeftInventoryOwnerGold;

            __instance.IsRefreshed = false;
            for (int i = __instance.RightItemListVM.Count - 1; i >= 0; i--)
            {
                SPItemVM spitemVM = __instance.RightItemListVM[i];
                if (spitemVM != null && !spitemVM.IsFiltered && !spitemVM.IsLocked)
                {
                    golds += spitemVM.ItemCost * spitemVM.ItemRosterElement.Amount;
                    if (__instance.IsTrading && golds > leftGold)
                    {
                        break;
                    }
                    TransferCommand command = TransferCommand.Transfer(spitemVM.ItemRosterElement.Amount, InventoryLogic.InventorySide.PlayerInventory, InventoryLogic.InventorySide.OtherInventory, spitemVM.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, ____currentCharacter, !__instance.IsInWarSet);
                    ____inventoryLogic.AddTransferCommand(command);
                }
            }
            Traverse.Create(__instance).Method("RefreshInformationValues").GetValue();
            Traverse.Create(__instance).Method("ExecuteRemoveZeroCounts").GetValue();
            __instance.IsRefreshed = true;

            return false;
        }



        [HarmonyPrefix]
        [HarmonyPatch("SaveItemLockStates")]
        private static bool SaveItemLockStates(SPInventoryVM __instance)
        {
            if (!CommonSetting.Instance.LockNoMiss)
            {
                return true;
            }

            List<EquipmentElement> list = Campaign.Current.GetCampaignBehavior<IInventoryLockTracker>().GetLocks().ToList();

            foreach (SPItemVM spitemVM in __instance.RightItemListVM)
            {
                if (spitemVM.IsLocked && !list.Exists(a => a.Item.StringId == spitemVM.ItemRosterElement.EquipmentElement.Item.StringId))
                {
                    list.Add(spitemVM.ItemRosterElement.EquipmentElement);
                }
            }
            Campaign.Current.GetCampaignBehavior<IInventoryLockTracker>().SetLocks(list);

            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch("AfterTransfer")]
        private static void AfterTransfer(SPInventoryVM __instance, InventoryLogic inventoryLogic, List<TransferCommandResult> results)
        {
            if (!CommonSetting.Instance.LockNoMiss)
            {
                return;
            }

            List<EquipmentElement> list = Campaign.Current.GetCampaignBehavior<IInventoryLockTracker>().GetLocks().ToList();

            foreach (SPItemVM spitemVM in __instance.RightItemListVM)
            {
                if (list.Exists(a => a.Item.StringId == spitemVM.ItemRosterElement.EquipmentElement.Item.StringId))
                {
                    spitemVM.IsLocked = true;
                }
            }

        }


    }
}
