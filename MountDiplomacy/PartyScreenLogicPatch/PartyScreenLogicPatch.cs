using HarmonyLib;
using System;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Wang
{
    [HarmonyPatch(typeof(PartyScreenLogic), "Initialize", new Type[]
{
    typeof(PartyBase),
    typeof(MobileParty),
    typeof(bool),
    typeof(TextObject),
    typeof(int),
    typeof(TextObject)
})]
    public class PartyScreenLogicPatch
    {


        private static void Postfix(PartyScreenLogic __instance, PartyBase leftParty, MobileParty ownerParty, bool isDismissMode, TextObject leftPartyName, int lefPartySizeLimit, TextObject header = null)
        {
            SortPartyHelpers.SortPartyScreen(__instance);
        }
    }

}
