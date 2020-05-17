using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;

namespace Wang.patchs
{
    [HarmonyPatch(typeof(CharacterRelationCampaignBehavior), "OnSettlementOwnerChanged")]
    public class CharacterRelationCampaignBehaviorPatch
    {

        private static bool Prefix(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            if ((detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRevolt) && oldOwner != null && oldOwner.MapFaction != null && oldOwner.MapFaction.Leader != oldOwner && oldOwner.IsAlive && oldOwner.MapFaction.Leader != Hero.MainHero)
            {
                //float value = settlement.GetValue(true);
                //int num = (int)((1.0 + Math.Max(1.0, Math.Sqrt((double)(value / 100000f)))) * (double)((newOwner.MapFaction != oldOwner.MapFaction) ? 1f : 0.5f));
                //ChangeRelationAction.ApplyRelationChangeBetweenHeroes(oldOwner, oldOwner.MapFaction.Leader, -num, false);
                if (oldOwner.Clan != null && settlement != null)
                {
                    oldOwner.Clan.Influence -= (float)(settlement.IsTown ? 50 : 25);
                }
            }

            return false;
        }

    }
}
