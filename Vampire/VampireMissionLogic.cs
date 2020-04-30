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

        private MissionTime _nextSlowCheckTime;

        private float _slow_rec;

        private float _slow_rec_per;

        private int _pkill;

        private bool _playerinbattle;

        public Agent PlayerAgent => base.Mission.MainAgent;

        public override void AfterStart()
        {
            InitBattleTest();
        }

        public bool CheckPlayerAlive()
        {
            if (PlayerAgent != null)
            {
                return PlayerAgent.IsActive();
            }
            return false;
        }

        private void InitBattleTest()
        {
            if (Config.Instance.debug_mess.Length > 2)
            {
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("Vampire:" + Config.Instance.debug_mess).ToString(), Colors.White));
            }
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("Vampire:" + Config.Instance.Heal_maxhp.ToString()).ToString(), Colors.White));
            _playerinbattle = true;
            if (Config.Instance.Start_delay > 0f)
            {
                _nextCheckTime = MissionTime.SecondsFromNow(Config.Instance.Start_delay);
            }
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, int weaponKind, int currentWeaponUsageIndex)
        {

            if (!affectedAgent.IsHuman)
            {
                return;
            }
            bool num = CheckPlayerAlive() && affectedAgent.IsHuman && affectorAgent == PlayerAgent && damage > 0 && _slow_rec <= 0f;
            if (Config.Instance.Injury_delay > 0f && _slow_rec <= 0f && (affectedAgent == PlayerAgent || affectorAgent == PlayerAgent) && damage > 0)
            {
                _nextCheckTime = MissionTime.SecondsFromNow(Config.Instance.Injury_delay);
            }
            if (!num || !(affectedAgent.Health <= 0f))
            {
                return;
            }
            _pkill++;
            if (Config.Instance.Display_info)
            {
                MBTextManager.SetTextVariable("NAME", affectedAgent.Character.Name);
                MBTextManager.SetTextVariable("KS", _pkill);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_kill1}Kill[{NAME}],Current Kills[{KS}]").ToString(), Colors.White));
            }
            if (Config.Instance.Heal_maxhp < 0)
            {
                Config.Instance.Heal_maxhp = 1;
            }
            if (Config.Instance.Heal_maxhp > 100)
            {
                Config.Instance.Heal_maxhp = 100;
            }
            if (!CheckPlayerAlive() || PlayerAgent.Health >= PlayerAgent.HealthLimit * (float)Config.Instance.Heal_maxhp / 100f)
            {
                return;
            }
            int num2 = (int)Math.Floor((double)Hero.MainHero.GetSkillValue(DefaultSkills.Medicine) / (double)Config.Instance.Heal_level_per);
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (num2 > Config.Instance.Heal_skill_kill_max)
            {
                num2 = Config.Instance.Heal_skill_kill_max;
            }
            int num3 = _pkill * Config.Instance.Heal_kill_per;
            if (num3 > Config.Instance.Heal_kill_max)
            {
                num3 = Config.Instance.Heal_kill_max;
            }
            float num4 = 0f;
            float num5 = affectedAgent.HealthLimit * 0.01f * (float)num3;
            float num6 = PlayerAgent.HealthLimit * 0.01f * (float)num2;
            num4 += num5;
            num4 += num6;
            if (PlayerAgent.Health + num4 >= PlayerAgent.HealthLimit)
            {
                if (Config.Instance.Slow_heal_mode)
                {
                    _slow_rec = PlayerAgent.HealthLimit - PlayerAgent.Health;
                    _slow_rec_per = _slow_rec / (float)Config.Instance.Slow_heal_sec;
                    _nextSlowCheckTime = MissionTime.SecondsFromNow(1f);
                    MBTextManager.SetTextVariable("SLOW_SC", Config.Instance.Slow_heal_sec);
                    MBTextManager.SetTextVariable("SLOW_N1", _slow_rec);
                    InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_slow1}Will recover {SLOW_N1} health in {SLOW_SC} seconds").ToString(), Colors.White));
                }
                else
                {
                    PlayerAgent.Health = PlayerAgent.HealthLimit;
                }
            }
            else if (Config.Instance.Slow_heal_mode)
            {
                _slow_rec = num4;
                _slow_rec_per = _slow_rec / (float)Config.Instance.Slow_heal_sec;
                _nextSlowCheckTime = MissionTime.SecondsFromNow(1f);
                MBTextManager.SetTextVariable("SLOW_SC", Config.Instance.Slow_heal_sec);
                MBTextManager.SetTextVariable("SLOW_N1", _slow_rec);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_slow1}Will recover {SLOW_N1} health in {SLOW_SC} seconds").ToString(), Colors.White));
            }
            else
            {
                PlayerAgent.Health += num4;
            }
            if (Config.Instance.Display_info)
            {
                MBTextManager.SetTextVariable("KNUM1", num3);
                MBTextManager.SetTextVariable("RNUM1", num5);
                MBTextManager.SetTextVariable("KNUM2", num2);
                MBTextManager.SetTextVariable("RNUM2", num6);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_kill2}Life Recovery{KNUM1}%({RNUM1}),Medical Skill Bonus{KNUM2}%({RNUM2})").ToString(), Colors.White));
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (_playerinbattle && _slow_rec <= 0f)
            {
                if (!_nextCheckTime.IsPast)
                {
                    return;
                }
                if (Config.Instance.Heal_delay <= 0f)
                {
                    Config.Instance.Heal_delay = 1f;
                }
                _nextCheckTime = MissionTime.SecondsFromNow(Config.Instance.Heal_delay);
                if (Config.Instance.Heal_maxhp < 0)
                {
                    Config.Instance.Heal_maxhp = 1;
                }
                if (Config.Instance.Heal_maxhp > 100)
                {
                    Config.Instance.Heal_maxhp = 100;
                }
                if (!CheckPlayerAlive() || PlayerAgent.Health >= PlayerAgent.HealthLimit * (float)Config.Instance.Heal_maxhp / 100f)
                {
                    return;
                }
                int num = (int)Math.Floor((double)Hero.MainHero.GetSkillValue(DefaultSkills.Medicine) / (double)Config.Instance.Heal_level_per);
                if (num > Config.Instance.Heal_skill_passive_max)
                {
                    num = Config.Instance.Heal_skill_passive_max;
                }
                if (num > 0)
                {
                    float num2 = PlayerAgent.HealthLimit * 0.01f * (float)num;
                    if (Config.Instance.Slow_heal_mode)
                    {
                        _slow_rec = num2;
                        _slow_rec_per = _slow_rec / (float)Config.Instance.Slow_heal_sec;
                        _nextSlowCheckTime = MissionTime.SecondsFromNow(1f);
                        MBTextManager.SetTextVariable("SLOW_SC", Config.Instance.Slow_heal_sec);
                        MBTextManager.SetTextVariable("SLOW_N1", _slow_rec);
                        InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_slow1}Will recover {SLOW_N1} health in {SLOW_SC} seconds").ToString(), Colors.White));
                    }
                    else
                    {
                        PlayerAgent.Health += num2;
                    }
                    if (Config.Instance.Display_info)
                    {
                        MBTextManager.SetTextVariable("KNUM", num);
                        MBTextManager.SetTextVariable("RNUM", num2);
                        InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_kill3}Medical Skill Recovery{KNUM}%({RNUM})").ToString(), Colors.White));
                    }
                }
            }
            else
            {
                if (!_nextSlowCheckTime.IsPast)
                {
                    return;
                }
                _nextSlowCheckTime = MissionTime.SecondsFromNow(1f);
                _slow_rec -= _slow_rec_per;
                if (_slow_rec <= 0f)
                {
                    _slow_rec_per = 0f;
                }
                if (_slow_rec > 0f)
                {
                    MBTextManager.SetTextVariable("SLOW_N2", _slow_rec_per);
                    MBTextManager.SetTextVariable("SLOW_N3", _slow_rec);
                    if (CheckPlayerAlive() && !(PlayerAgent.Health >= PlayerAgent.HealthLimit * (float)Config.Instance.Heal_maxhp / 100f))
                    {
                        PlayerAgent.Health += _slow_rec_per;
                    }
                    InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=str_slow2}This time restore {SLOW_N2} health, remaining {SLOW_N3} health").ToString(), Colors.White));
                }
            }
        }
    }

}
