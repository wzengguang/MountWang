using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Wang.Perks
{
    [HarmonyPatch(typeof(DefaultPerks), "InitializePerks")]
    class DefaultPerksPatch
    {
        private static void Postfix(ref PerkObject ___OneHandedExtraHP, ref PerkObject ___OneHandedEdgePlacement,
            ref PerkObject ___TwoHandedExtraHP, ref PerkObject ___TwoHandedExtraDamage,
            ref PerkObject ___PolearmExtraHp,
            ref PerkObject ___AthleticsEndurance, ref PerkObject ___AthleticsDexterous,
            ref PerkObject ___MedicinePreventiveMedicine, ref PerkObject ___MedicineSelfMedication,
            ref PerkObject ___ScoutingExtra1,
            ref int[] ____tierSkillRequirements)
        {
            ___OneHandedExtraHP.Initialize("{=3xuwVbfs}Extra HP I", "{=xxD6WAnM}Hitpoints increased by 3.", DefaultSkills.OneHanded, ____tierSkillRequirements[0], ___OneHandedEdgePlacement, SkillEffect.PerkRole.Personal, 6f);

            ___TwoHandedExtraHP.Initialize("{=7Jwuax4w}Extra HP", "{=6RUssLxi}Hitpoints increased by 3%.", DefaultSkills.TwoHanded, ____tierSkillRequirements[0], ___TwoHandedExtraDamage, SkillEffect.PerkRole.Personal, 6f);
            ___PolearmExtraHp.Initialize("{=7Jwuax4w}Extra HP", "{=6RUssLxi}Hitpoints increased by 3%.", DefaultSkills.Polearm, ____tierSkillRequirements[0], null, SkillEffect.PerkRole.Personal, 6f);
            ___AthleticsEndurance.Initialize("{=kvOavzcs}Endurance", "{=01jmBrDb}+4 hitpoints.", DefaultSkills.Athletics, ____tierSkillRequirements[2], ___AthleticsDexterous, SkillEffect.PerkRole.Personal, 8f);
            ___MedicinePreventiveMedicine.Initialize("{=wI393cla}Preventive Medicine", "{=HIVRbq8l}Increase character's hit points by 10.", DefaultSkills.Medicine, ____tierSkillRequirements[0], ___MedicineSelfMedication, SkillEffect.PerkRole.Personal, 20f);
            ___ScoutingExtra1.Initialize("{=dDKOoD3e}Healty Scout", "{=ufYwLbZR} Extra 8 hitpoints.", DefaultSkills.Scouting, ____tierSkillRequirements[9], null, SkillEffect.PerkRole.PartyMember, 16f, SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.AddFactor);

        }
    }
}
