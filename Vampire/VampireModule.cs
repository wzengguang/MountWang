using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Vampire
{
    public class VampireModule : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony.DEBUG = false;
            FileLog.Reset();
            Harmony harmony = new Harmony("mod.bannerlord.Vampire");

            harmony.PatchAll();


        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                AddBehaviors(gameStarterObject as CampaignGameStarter);
            }
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new VampireCampaignBehavior());
        }

    }

}
