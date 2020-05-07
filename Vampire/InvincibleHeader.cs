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

    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "UpdateHumanStats")]
    public class InvincibleHeader
    {
        private static void Postfix(Agent agent, AgentDrivenProperties agentDrivenProperties)
        {

            if (agent.IsHero && agent.IsFemale)
            {
                Equipment spawnEquipment = agent.SpawnEquipment;
                agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum() + 99;
            }
        }
    }
}
