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

            if (fortification.Owner.IsStarving || result < 0)
            {
                return result;
            }

            var prosperity = fortification.Owner.Settlement.Prosperity;

            ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
            float foodChange = Campaign.Current.Models.SettlementFoodModel.CalculateTownFoodStocksChange(fortification, null);
            if (foodChange < -1 && Math.Abs(fortification.FoodStocks / foodChange) < 20 && prosperity < 1000 && result > 1)
            {
                explainedNumber.Add(-result, _surplusFoodText);
            }
            else if (SettlementSetting.Instance.boostProsperityGrowth > 0)
            {
                var factor = SettlementSetting.Instance.boostProsperityGrowth * result > 1 ? result : 1;
                explainedNumber.Add((float)factor, _surplusFoodText);
            }


            var r = result + explainedNumber.ResultNumber;

            if (prosperity > 11000 && r > 1f)
            {
                explainedNumber.Add((int)Math.Sqrt(r) - r, _surplusFoodText);
            }

            return result + explainedNumber.ResultNumber;

        }
    }
}
