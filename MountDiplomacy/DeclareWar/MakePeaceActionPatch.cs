using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.KingdomDiplomacy;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(MakePeaceAction))]
    public class MakePeaceActionPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ApplyInternal")]
        private static void ApplyInternal(IFaction faction1, IFaction faction2, int dailyTributeFrom1To2)
        {
            StanceLink stanceWith = faction1.GetStanceWith(faction2);
            Traverse.Create(stanceWith).Field("_peaceDeclarationDate").SetValue(CampaignTime.Now);
        }
    }


    [HarmonyPatch(typeof(StanceLink))]
    public class StanceLinkPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetDailyTributePaid")]
        private static void GetDailyTributePaid(IFaction faction, StanceLink __instance, ref int ____dailyTributeFrom1To2)
        {
            if (__instance.PeaceDeclarationDate.ElapsedDaysUntilNow > 50)
            {
                ____dailyTributeFrom1To2 = 0;
            }
        }
    }


    [HarmonyPatch(typeof(KingdomDiplomacyItemVM))]
    public class KingdomDiplomacyItemVMPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdateDiplomacyProperties")]
        private static void UpdateDiplomacyProperties(KingdomDiplomacyItemVM __instance, ref IFaction ____playerKingdom, ref IFaction ___Faction1, ref IFaction ___Faction2, ref string ____faction2Name)
        {
            StanceLink stanceWith = ____playerKingdom.GetStanceWith(___Faction2);
            if (stanceWith.IsNeutral)
            {
                var days = (int)stanceWith.PeaceDeclarationDate.ElapsedDaysUntilNow;
                if (stanceWith.GetDailyTributePaid(____playerKingdom) != 0)
                {
                    var paid = -stanceWith.GetDailyTributePaid(____playerKingdom);
                    __instance.Faction2Name = $"{___Faction2.Name.ToString()}({paid}$/d)({days}d)";
                }
                else
                {
                    __instance.Faction2Name = ___Faction2.Name.ToString() + (days < 10000 ? $"({days}d)" : "");
                }
            }
            else
            {
                var days = (int)stanceWith.WarStartDate.ElapsedDaysUntilNow;
                __instance.Faction2Name = ___Faction2.Name.ToString() + (days < 10000 ? $"({days}d)" : "");
            }

        }
    }
}
