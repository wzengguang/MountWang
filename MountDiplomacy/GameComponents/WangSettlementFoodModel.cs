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

namespace Wang.GameComponents
{

    public class WangSettlementFoodModel : DefaultSettlementFoodModel
    {
        private static readonly TextObject _prosperityText = GameTexts.FindText("str_prosperity", null);

        public override float CalculateTownFoodStocksChange(Town town, StatExplainer explanation = null)
        {
            var result = base.CalculateTownFoodStocksChange(town, explanation);


            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            var num = 0f;
            var prosperity = (int)town.Owner.Settlement.Prosperity;
            if (prosperity < 5000)
            {
                num = prosperity / 50 - prosperity / 70;

            }
            else if (prosperity < 10000)
            {
                num = prosperity / 50 - prosperity / 60;
            }
            else if (prosperity < 20000)
            {
                num = 0;
            }
            explainedNumber.Add(num, _prosperityText);

            return explainedNumber.ResultNumber;
        }
    }
}