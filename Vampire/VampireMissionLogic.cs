using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Vampire
{
    internal class VampireMissionLogic : MissionLogic
    {
        private MissionTime _nextCheckTime;

        private int _pkill;

        public override void AfterStart()
        {
            InitBattleTest();
        }

        private void InitBattleTest()
        {
            _nextCheckTime = MissionTime.SecondsFromNow(Config.Instance.Start_delay);
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, int weaponKind, int currentWeaponUsageIndex)
        {

            if (damage <= 0
                || affectedAgent.Health > 0 || !affectedAgent.IsHuman
                || affectorAgent == null || affectorAgent.Character == null || !affectorAgent.IsHero
                || !affectorAgent.IsActive())
            {
                return;
            }

            int medicine = Math.Min(Config.Instance.Heal_skill_kill_max, (int)Math.Floor(affectorAgent.Character.GetSkillValue(DefaultSkills.Medicine) / (double)Config.Instance.Heal_level_per));

            float get = affectorAgent.IsMainAgent ? affectedAgent.HealthLimit * 0.1f : affectorAgent.HealthLimit * 0.5f;

            get += affectorAgent.HealthLimit * 0.01f * medicine;
            get = (int)get;

            if (affectorAgent.Health + get > affectorAgent.HealthLimit)
            {
                affectorAgent.Health = affectorAgent.HealthLimit;

                if (affectorAgent.MountAgent != null && affectorAgent.MountAgent.IsActive())
                {
                    affectorAgent.MountAgent.Health = Math.Min(affectorAgent.MountAgent.Health + affectorAgent.Health + get - affectorAgent.HealthLimit, affectorAgent.MountAgent.HealthLimit);
                }

                get = affectorAgent.Health + get - affectorAgent.HealthLimit;
            }
            else
            {
                affectorAgent.Health += get;
            }


            if (affectorAgent.IsMainAgent)
            {

                _pkill++;
                MBTextManager.SetTextVariable("NAME", affectedAgent.Character.Name);
                MBTextManager.SetTextVariable("RECOVER", get.ToString());
                MBTextManager.SetTextVariable("KS", _pkill);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_kill1}Kill[{NAME}],Current Kills[{KS}]").ToString(), Colors.White));
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

        }
    }

}
