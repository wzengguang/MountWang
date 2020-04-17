using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandBox.GauntletUI;
using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.TwoDimension;
namespace Wang.LogScreen
{
    public class LogScreenManager : MapView
    {
        public bool IsEncyclopediaOpen
        {
            get;
            protected set;
        }
        private EncyclopediaHomeVM _homeDatasource;

        private EncyclopediaNavigatorVM _navigatorDatasource;

        private Action<Vec2> _setMapCameraPosition;

        private EncyclopediaData _encyclopediaData;

        private Game _game;

        protected override void CreateLayout()
        {
            base.CreateLayout();
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
            spriteData.SpriteCategories["ui_encyclopedia"].Load(resourceContext, uIResourceDepot);
            _setMapCameraPosition = base.MapScreen.SetMapCameraPosition;
            _homeDatasource = new EncyclopediaHomeVM(new EncyclopediaPageArgs(null, null));
            _navigatorDatasource = new EncyclopediaNavigatorVM(ExecuteLink, CloseEncyclopedia);
            _game = Game.Current;
            Game game = _game;
            game.AfterTick = (Action<float>)Delegate.Combine(game.AfterTick, new Action<float>(OnTick));
        }

        internal void OnTick(float dt)
        {
            _encyclopediaData?.OnTick();
        }

        private EncyclopediaPageVM ExecuteLink(string pageId, object obj, bool needsRefresh)
        {
            _navigatorDatasource.NavBarString = string.Empty;
            if (_encyclopediaData == null)
            {
                _encyclopediaData = new EncyclopediaData(this, ScreenManager.TopScreen, _homeDatasource, _navigatorDatasource, _setMapCameraPosition);
            }
            if (pageId == "LastPage")
            {
                Tuple<string, object> lastPage = _navigatorDatasource.GetLastPage();
                pageId = lastPage.Item1;
                obj = lastPage.Item2;
            }
            base.IsEncyclopediaOpen = true;
            return _encyclopediaData.ExecuteLink(pageId, obj, needsRefresh);
        }

        protected override void OnFinalize()
        {
            Game game = _game;
            game.AfterTick = (Action<float>)Delegate.Remove(game.AfterTick, new Action<float>(OnTick));
            _game = null;
            _homeDatasource = null;
            _navigatorDatasource = null;
            _encyclopediaData = null;
            base.OnFinalize();
        }

        public override void CloseEncyclopedia()
        {
            _encyclopediaData.CloseEncyclopedia();
            _encyclopediaData = null;
            base.IsEncyclopediaOpen = false;
        }

    }
}
