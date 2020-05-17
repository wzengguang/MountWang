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
    public class WangSettlementProsperityModel : DefaultSettlementProsperityModel
    {
        private readonly TextObject _surplusFoodText = GameTexts.FindText("str_surplus_food", null);

        public override float CalculateProsperityChange(Town fortification, StatExplainer explanation = null)
        {
            var result = base.CalculateProsperityChange(fortification, explanation);

            if (fortification.Owner.IsStarving || result < 1)
            {
                return result;
            }


            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            float num = Campaign.Current.Models.SettlementFoodModel.CalculateTownFoodStocksChange(fortification, null);
            if (num < 1 || (num < -1 && Math.Abs(fortification.FoodStocks / num) < 20))
            {
                explainedNumber.Add(-result, _surplusFoodText);
                return explainedNumber.ResultNumber;
            }

            var prosperity = (int)fortification.Owner.Settlement.Prosperity;

            if (prosperity > 11000)
            {
                explainedNumber.Add((int)Math.Sqrt(result) - result, _surplusFoodText);

            }
            return explainedNumber.ResultNumber;

        }
    }
}
