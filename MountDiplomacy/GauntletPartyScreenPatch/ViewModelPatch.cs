using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Wang
{

    //[HarmonyPatch(typeof(ViewModel), "ExecuteCommand")]
    public class ViewModelPatch
    {
        private static bool Prefix(ViewModel __instance, string commandName, object[] parameters)
        {
            if (!(__instance is PartyVM))
            {
                return true;
            }
            PartyVM partyVM = (PartyVM)__instance;
            if (commandName == "ExecuteRecruitAll")
            {
                partyVM.ExecuteRecruitAll();
                return false;
            }
            if (commandName == "ExecuteUpgradeAll")
            {
                partyVM.ExecuteUpgradeAll();
                return false;
            }
            return true;
        }
    }
}
