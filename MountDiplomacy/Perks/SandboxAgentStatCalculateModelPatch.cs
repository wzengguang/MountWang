using HarmonyLib;
using Helpers;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Wang.Perks
{
    [HarmonyPatch]
    class SandboxAgentStatCalculateModelPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "GetPerkEffectsOnAgent")]
        private static void GetPerkEffectsOnAgent(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData rightHandEquippedItem)
        {
            CharacterObject characterObject = agent.Character as CharacterObject;
            if (characterObject == null)
            {
                return;
            }
            ItemObject offHandItemObject = null;
            EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
            if (wieldedItemIndex != EquipmentIndex.None)
            {
                offHandItemObject = agent.Equipment[wieldedItemIndex].CurrentUsageItem.Item;
            }
            if (offHandItemObject != null && offHandItemObject.PrimaryWeapon.IsShield)
            {

            }
            if (rightHandEquippedItem == null)
            {
                return;
            }
            if (rightHandEquippedItem.RelevantSkill == DefaultSkills.OneHanded)
            {

            }
            else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.TwoHanded)
            {
                PerkObject perkObject = rightHandEquippedItem.ThrustSpeed < 85 ? DefaultPerks.TwoHanded.PowerBasher : (rightHandEquippedItem.ThrustSpeed > 100 ? DefaultPerks.TwoHanded.SpeedBasher : null);
                if (perkObject != null)
                {
                    agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= 1 + perkObject.SecondaryBonus / 100f;
                }

                PerkObject swingPerk = rightHandEquippedItem.SwingSpeed < 85 ? DefaultPerks.TwoHanded.PowerBasher : (rightHandEquippedItem.SwingSpeed > 100 ? DefaultPerks.TwoHanded.SpeedBasher : null);
                if (swingPerk != null)
                {
                    agentDrivenProperties.SwingSpeedMultiplier *= 1 + swingPerk.SecondaryBonus / 100f;
                }

                //双手大于100效果。
                var bonus = (characterObject.GetSkillValue(DefaultSkills.TwoHanded) - 200) * 0.2f;
                if (bonus > 0)
                {
                    agentDrivenProperties.SwingSpeedMultiplier *= 1 + bonus / 100;
                    agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= 1 + bonus / 100;
                }
            }
            else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Polearm)
            {

            }
            else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Bow)
            {
                if (characterObject.GetPerkValue(DefaultPerks.Bow.Marksman))
                {
                    agentDrivenProperties.WeaponInaccuracy *= 1.1f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Bow.FasterAim))
                {
                    agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= 1.1f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Bow.Ranger))
                {
                    agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 0.6f;
                }
            }
            else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Crossbow)
            {
                if (characterObject.GetPerkValue(DefaultPerks.Crossbow.FastReload))
                {
                    agentDrivenProperties.ReloadSpeed *= 1.1f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Crossbow.HastyReload))
                {
                    agentDrivenProperties.ReloadSpeed *= 1.15f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Crossbow.VolleyCommander))
                {
                    agentDrivenProperties.ReloadSpeed *= 1.2f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Crossbow.ImprovedAim))
                {
                    agentDrivenProperties.WeaponInaccuracy *= 1.2f;
                }
            }
            else if (rightHandEquippedItem.RelevantSkill == DefaultSkills.Throwing)
            {
                if (characterObject.GetPerkValue(DefaultPerks.Throwing.SteadyHand))
                {
                    agentDrivenProperties.WeaponInaccuracy *= 1.15f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Throwing.PerfectAccuracy))
                {
                    agentDrivenProperties.WeaponInaccuracy *= 1.3f;
                }
                if (characterObject.GetPerkValue(DefaultPerks.Throwing.Extra2))
                {
                    agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = 0f;
                }
            }

            if (agent.HasMount)
            {
                if (characterObject.GetPerkValue(DefaultPerks.Riding.Sharpshooter))
                {
                    agentDrivenProperties.WeaponInaccuracy *= 1.15f;
                }

                if (characterObject.GetPerkValue(DefaultPerks.Riding.MountedArcher)
                    && (rightHandEquippedItem.RelevantSkill == DefaultSkills.Bow
                    || rightHandEquippedItem.RelevantSkill == DefaultSkills.Crossbow
                    || rightHandEquippedItem.RelevantSkill == DefaultSkills.Throwing))
                {
                    agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 0.9f;
                    agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 0.9f;
                    agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians *= 0.9f;

                }
                else if (characterObject.GetPerkValue(DefaultPerks.Riding.Cavalry)
                   && (rightHandEquippedItem.RelevantSkill == DefaultSkills.OneHanded
                   || rightHandEquippedItem.RelevantSkill == DefaultSkills.TwoHanded
                   || rightHandEquippedItem.RelevantSkill == DefaultSkills.Polearm))
                {
                    agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 0.9f;
                    agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 0.9f;
                    agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians *= 0.9f;
                }

            }
        }

    }
}
