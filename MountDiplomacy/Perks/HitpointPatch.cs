using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Library;

namespace Wang.Perks
{
    [HarmonyPatch(typeof(DefaultCharacterStatsModel), "MaxHitpoints")]
    class HitpointPatch
    {
        public static void Postfix(ref int __result, CharacterObject character, StatExplainer explanation = null)
        {
            ExplainedNumber bonuses = new ExplainedNumber(__result, explanation);
            //PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ExtraHp, character, ref bonuses);
            //PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.ExtraHp, character, ref bonuses);
            //PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.ExtraHp, character, ref bonuses);
            //PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.Endurance, character, ref bonuses);
            //PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.PreventiveMedicine, character, ref bonuses);

            var ScoutingExtra1 = PerkObject.FindFirst(a => a.Name.GetID() == "dDKOoD3e");
            if (ScoutingExtra1 != null)
            {
                PerkHelper.AddPerkBonusForCharacter(ScoutingExtra1, character, ref bonuses);
            }

            __result = MBMath.Round(bonuses.ResultNumber);
        }

    }
}
