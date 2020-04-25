using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Wang.GauntletUI
{
    public class GauntletWangScreen : ScreenBase
    {

        public static bool Show = false;

        private WangVM _dataSource;

        private GauntletLayer _gauntletLayer;

        private SpriteCategory _clanCategory;


        public GauntletWangScreen()
        {
            Show = true;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            LoadingWindow.DisableGlobalLoadingWindow();
            if (_gauntletLayer.Input.IsHotKeyReleased("Exit") || _gauntletLayer.Input.IsGameKeyReleased(34))
            {
                CloseWangScreen();
            }
        }

        private void OpenPartyAsManage(MobileParty party)
        {
            PartyScreenManager.OpenScreenAsManageTroops(party);
        }

        private void OpenBannerEditorWithPlayerClan()
        {
            Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<BannerEditorState>());
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
            _clanCategory = spriteData.SpriteCategories["ui_clan"];
            _clanCategory.Load(resourceContext, uIResourceDepot);
            _gauntletLayer = new GauntletLayer(1);
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));

            _gauntletLayer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_gauntletLayer);
            AddLayer(_gauntletLayer);
            _dataSource = new WangVM(CloseWangScreen, OpenPartyAsManage, OpenBannerEditorWithPlayerClan);
            _gauntletLayer.LoadMovie("WangScreen", _dataSource);
            //   Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ClanScreen));
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(_gauntletLayer);
            _gauntletLayer.IsFocusLayer = false;
            //_gauntletLayer.ReleaseMovie("")
            ScreenManager.TryLoseFocus(_gauntletLayer);
            //Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.None));
            _clanCategory.Unload();
            _dataSource = null;
            _gauntletLayer = null;
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            PartyBase.MainParty.Visuals.SetMapIconAsDirty();
            _dataSource?.UpdateBannerVisuals();
        }

        private void CloseWangScreen()
        {
            Show = false;
            ScreenManager.PopScreen();
            // Game.Current.GameStateManager.PopState();
        }
    }
}
