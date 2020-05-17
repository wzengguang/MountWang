using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Vampire
{
    internal class VampireCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, FindBattle);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void FindBattle(IMission misson)
        {
            Mission obj = (Mission)misson;
            bool flag = obj.CombatType == Mission.MissionCombatType.Combat;
            bool flag2 = obj.IsFieldBattle;

            if (flag && Mission.Current.Scene != null)
            {
                Mission.Current.AddMissionBehaviour(new VampireMissionLogic());
            }
        }
    }

}
