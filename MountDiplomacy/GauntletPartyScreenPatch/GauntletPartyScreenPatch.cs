using HarmonyLib;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace Wang
{
    [HarmonyPatch(typeof(ScreenBase))]
    public class GauntletPartyScreenPatch
    {
        internal static GauntletLayer screenLayer;

        [HarmonyPostfix]
        [HarmonyPatch("AddLayer")]
        private static void PostfixActivate(ScreenBase __instance, ScreenLayer layer)
        {
            if (__instance is GauntletPartyScreen && screenLayer == null)
            {
                GauntletPartyScreen obj = (GauntletPartyScreen)__instance;
                PartyVM value = Traverse.Create(obj).Field<PartyVM>("_dataSource").Value;
                screenLayer = new GauntletLayer(99);
                screenLayer.LoadMovie("AutoPartyManager", new AutoPartyManagerVM(value));
                screenLayer.InputRestrictions.SetInputRestrictions();
                obj.AddLayer(screenLayer);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("RemoveLayer")]
        private static void PrefixFinalize(ScreenBase __instance, ScreenLayer layer)
        {
            if (__instance is GauntletPartyScreen && screenLayer != null)
            {
                GauntletPartyScreen obj = (GauntletPartyScreen)__instance;
                GauntletLayer layer2 = screenLayer;
                screenLayer = null;
                obj.RemoveLayer(layer2);
            }
        }

    }
}
