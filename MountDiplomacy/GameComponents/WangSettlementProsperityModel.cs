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

            var prosperity = fortification.Owner.Settlement.Prosperity;

            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            float foodChange = Campaign.Current.Models.SettlementFoodModel.CalculateTownFoodStocksChange(fortification, null);
            if (foodChange < -1 && Math.Abs(fortification.FoodStocks / foodChange) < 20 && prosperity < 1000)
            {
                explainedNumber.Add(-result, _surplusFoodText);
            }
            else if (SettlementSetting.Instance.boostProsperityGrowth > 0)
            {
                var factor = SettlementSetting.Instance.boostProsperityGrowth * result;
                explainedNumber.Add((float)factor, _surplusFoodText);
            }


            if (prosperity > 11000 && explainedNumber.ResultNumber > 1f)
            {
                explainedNumber.Add((int)Math.Sqrt(explainedNumber.ResultNumber) - explainedNumber.ResultNumber, _surplusFoodText);
            }

            return explainedNumber.ResultNumber;

        }
    }
}
