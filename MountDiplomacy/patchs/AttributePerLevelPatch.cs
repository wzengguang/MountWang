using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using Wang.Setting;

namespace Wang
{
    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "get_LevelsPerAttributePoint")]
    public class AttributePerLevelPatch
    {
        private static void Postfix(ref int __result)
        {
            __result = (int)CommonSetting.Instance.LevelsPerAttributePoint;
        }
    }
}
