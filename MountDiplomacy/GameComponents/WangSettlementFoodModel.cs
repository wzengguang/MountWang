using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Wang.Setting;

namespace Wang.GameComponents
{

    public class WangSettlementFoodModel : DefaultSettlementFoodModel
    {
        private static readonly TextObject _prosperityText = GameTexts.FindText("str_prosperity", null);

        public override float CalculateTownFoodStocksChange(Town town, StatExplainer explanation = null)
        {
            var result = base.CalculateTownFoodStocksChange(town, explanation);

            if (SettlementSetting.Instance.ProsperityNeedFoodMultiple == 1)
            {
                return result;
            }

            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);

            var prosperity = (int)town.Owner.Settlement.Prosperity;
            var num = 0f;

            num = prosperity / 50 - SettlementSetting.Instance.ProsperityNeedFoodMultiple * prosperity / 50;
            explainedNumber.Add(num, _prosperityText);

            return result + explainedNumber.ResultNumber;
        }
    }
}