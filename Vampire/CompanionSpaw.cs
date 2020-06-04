using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace Vampire
{

    public static class TraitHelper
    {

        public static bool trait(CharacterObject characterObject)
        {
            bool nice = true;
            nice &= characterObject.GetTraitLevel(DefaultTraits.Mercy) > -1;
            nice &= characterObject.GetTraitLevel(DefaultTraits.Valor) > -1;
            nice &= characterObject.GetTraitLevel(DefaultTraits.Honor) > -1;
            nice &= characterObject.GetTraitLevel(DefaultTraits.Generosity) > -1;
            nice &= characterObject.GetTraitLevel(DefaultTraits.Calculating) > -1;
            return nice;
        }
    }

    [HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
    public class CompanionSpawPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SpawnUrbanCharacters")]
        public static void Prefix(ref List<CharacterObject> ____companionTemplates)
        {
            ____companionTemplates = ____companionTemplates.Where((CharacterObject x) => TraitHelper.trait(x)).ToList();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnGameLoaded")]
        public static void OnGameLoaded(ref List<CharacterObject> ____companionTemplates)
        {
            ____companionTemplates = ____companionTemplates.Where((CharacterObject x) => TraitHelper.trait(x)).ToList();
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateCompanion")]
        private static bool CreateCompanion(CharacterObject companionTemplate)
        {
            if (Clan.PlayerClan.Companions != null)
            {
                if (MBRandom.RandomFloat < 1f / Math.Max(1, Clan.PlayerClan.Companions.Count() / 3f))
                {
                    return true;
                }

            }
            return false;
        }
    }
}
