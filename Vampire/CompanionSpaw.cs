using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

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

  

    }
}
