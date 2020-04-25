using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace EnhanceLordTroop
{
    public class EnhanceLordTroopModule : MBSubModuleBase
    {
        private static string FILE_NAME = BasePath.Name + "Modules/WangEnhanceLordTroop/ModuleData/config.xml";


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
                XpMultiplierConfigBase.Init(xmlDocument);
            }
        }
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            InformationManager.DisplayMessage(new InformationMessage("Enable EnhanceLordTroopModule"));

        }


        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                AddBehaviour(gameStarterObject as CampaignGameStarter);
            }
        }

        private void AddBehaviour(CampaignGameStarter gameStarterObject)
        {
            gameStarterObject.AddBehavior(new AddXpToLordTroopBehaviour());
        }
    }
}
