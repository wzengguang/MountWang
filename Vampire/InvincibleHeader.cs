using HarmonyLib;
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

    [HarmonyPatch(typeof(Agent), "HandleBlow")]
    public class InvincibleHeader
    {
        private static bool Prefix(Agent __instance, ref Blow b)
        {
            if (b.VictimBodyPart == BoneBodyPartType.Head && __instance.IsHuman && __instance.IsHero)
            {
                var obj = __instance.Character as CharacterObject;
                if (obj.HeroObject != Hero.MainHero && obj.HeroObject.Clan != null && obj.HeroObject.Clan.Leader == Hero.MainHero)
                {
                    b.InflictedDamage = 1;
                }
            }
            return true;
        }
    }
}
