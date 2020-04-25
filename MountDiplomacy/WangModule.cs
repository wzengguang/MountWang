using EnhanceLordTroop;
using HarmonyLib;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Wang.GauntletUI;

namespace Wang
{
    public class WangModule : MBSubModuleBase
    {
        private static string FILE_NAME = BasePath.Name + "Modules/Wang/ModuleData/config.xml";


        public void InitConfig()
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true
            };
            using (XmlReader reader = XmlReader.Create(FILE_NAME, settings))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);
                XpMultiplierConfig.Init(xmlDocument);
                SortPartyConfig.init(xmlDocument);
                SiegeConfig.Init(xmlDocument);
                RecruitConfig.Init(xmlDocument);
                PrisonerEscapeConfig.Init(xmlDocument);
                SettlementMillitiaConfig.Init(xmlDocument);
                TroopCountLimitConfig.Init(xmlDocument);
                BanditConfig.Init(xmlDocument);
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            InitConfig();
            Harmony.DEBUG = false;
            FileLog.Reset();
            Harmony harmony = new Harmony("mod.bannerlord.wang");
            harmony.PatchAll();



        }


        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                ReplaceGameModel(campaignGameStarter);
                AddBehaviour(gameStarterObject as CampaignGameStarter);
            }
            Help.Original = null;
        }

        private void AddBehaviour(CampaignGameStarter gameStarterObject)
        {
            gameStarterObject.AddBehavior(new HeroLearningSkillBehaviour());
            //  gameStarterObject.AddBehavior(new CustomBanditsCampaignBehavior());//会自动覆盖原来的。
            gameStarterObject.AddBehavior(new CustomTownRecruitPrisonersCampaignBehavior());
            gameStarterObject.AddBehavior(new AddXpToLordTroopBehaviour());
        }


        private void ReplaceGameModel(CampaignGameStarter starter)
        {
            IList<GameModel> list = starter.Models as IList<GameModel>;
            if (list == null)
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is DefaultPrisonerRecruitmentCalculationModel)
                {
                    list[i] = new CustomPrisonerRecruitmentCalculationModel();
                }

                if (list[i] is DefaultSettlementMilitiaModel)
                {
                    list[i] = new CustomSettlementMilitiaModel();
                }
                if (list[i] is SettlementGarrisonModel)
                {
                    list[i] = new CustomSettlementGarrisonModel();
                }

                if (list[i] is DefaultSiegeEventModel)
                {
                    list[i] = new CustomSiegeEventModel();
                }

                if (list[i] is DefaultBanditDensityModel)
                {
                    list[i] = new CustomBanditDensityModel();
                }
            }
        }


        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            //  Campaign.Current.CampaignBehaviorManager.AddBehavior(new CustomPoliticalStagnationAndBorderIncidentCampaignBehavior());
            //CampaignEvents.RemoveListeners(Campaign.Current.GetCampaignBehavior<BanditsCampaignBehavior>());
            //Campaign.Current.CampaignBehaviorManager.RemoveBehavior<BanditsCampaignBehavior>();
        }


        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            ShowWar();

            if (Campaign.Current != null && Campaign.Current.GameStarted && InputKey.M.IsPressed() && !GauntletWangScreen.Show)
            {
                ScreenManager.PushScreen(new GauntletWangScreen());

            }
        }

        /// <summary>
        /// 显示战争状态
        /// </summary>
        private void ShowWar()
        {
            if (Campaign.Current != null && Campaign.Current.GameStarted && InputKey.Home.IsPressed())
            {
                string text = "";
                int num = 0;
                foreach (Kingdom item in Kingdom.All)
                {
                    if (item != null)
                    {
                        num++;

                        var s = Help.CheckOwnSettlementOccupyedByFaction(item).Count();

                        text += $"{num}.{s} {item.Name}({Math.Round(item.MapFaction.TotalStrength)}) 战争于 ";
                        int num2 = 0;
                        foreach (Kingdom item2 in Kingdom.All.OrderBy((Kingdom w) => w.Name.ToString()))
                        {
                            if (item2 != null && !item.Name.Equals(item2.Name) && item.IsAtWarWith(item2))
                            {

                                text += $"{item2.Name} 和 ";
                                num2++;
                            }
                        }

                        text = text.Substring(0, text.Length - 3);

                        //text = ((num2 <= 0) ? (text.Substring(0, text.Length - 8) + " 和平于") : text.Substring(0, text.Length - 5));
                        text += ";\n\n";
                    }
                }
                InformationManager.ShowInquiry(new InquiryData("战争状态", text, isAffirmativeOptionShown: true, isNegativeOptionShown: false, "OK", "", null, null));
            }

        }

    }
}
