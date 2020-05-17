using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Mission;

namespace Vampire
{
    [HarmonyPatch]
    public class InvincibleHeader
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHumanStats")]
        private static void Postfix(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {

            if (agent.IsHero && agent.IsFemale)
            {
                Equipment spawnEquipment = agent.SpawnEquipment;
                agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum() + 99;

            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHorseStats")]
        private static void UpdateHorseStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {
            if (agent.RiderAgent != null && agent.RiderAgent.IsHero)
            {
                var riding = (agent.RiderAgent.Character as CharacterObject).GetSkillValue(DefaultSkills.Riding);

                agentDrivenProperties.ArmorTorso *= 1f + (float)Math.Sqrt(riding / 100f);
            }
        }

    }
}
