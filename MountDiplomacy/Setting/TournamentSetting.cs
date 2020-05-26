using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Options.ManagedOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using Wang.Setting.Attributes;

namespace Wang.Setting
{
    public class TournamentSetting : SettingBase
    {
        public static TournamentSetting Instance { get; private set; }

        public TournamentSetting()
        {
            Instance = this;
        }
        public override string Name { get; set; } = "{=setting_tournament}Tournament";


        [SettingBoolean("{=setting_tournament_bet_gold}Enable Bet Gold", "{=setting_tournament_bet_gold_desc}Description: Rewrite gain the gold from bet. when player win more, does not affect bet.")]
        public bool BetGoldEnabled { get; set; } = true;

        [SettingBoolean("{=setting_tournament_prize}Enable prize", "{=setting_tournament_prize_desc}Description: The weapon by Player crafted will not appear in prize. And Increase the chance of gain horse. And with the game days become bigger, the value of the prize become more bigger.")]
        public bool PrizeEnabled { get; set; } = true;

    }
}
