using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Gauntlet.ViewModels;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;

namespace Bannerlord.UIEditor.Gauntlet
{
    public class UIEditorGauntletScreen : ConnectedGauntletScreenBase
    {
        public UIEditorVM UIEditorVM { get; private set; } = null!;
        public GauntletLayer? UIEditorGauntletLayer { get; private set; }

        private GauntletMovie? m_UIEditorMovie;

        private BackdropOverlayVM m_BackdropOverlayVM = null!;
        private GauntletLayer? m_BackdropOverlayGauntletLayer;
        private GauntletMovie? m_BackdropOverlayMovie;

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            m_BackdropOverlayVM = new BackdropOverlayVM();
            UIEditorVM = new UIEditorVM();
            UIEditorVM.Create(PublicContainer);
        }

        public override void Load()
        {
            base.Load();
            UIEditorVM.Load();
        }

        public override void Unload()
        {
            UIEditorVM.Unload();
            base.Unload();
        }

        protected override void Dispose(bool _disposing)
        {
            UIEditorVM.Dispose();
            base.Dispose(_disposing);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            m_BackdropOverlayGauntletLayer = new GauntletLayer(9990);
            m_BackdropOverlayMovie = m_BackdropOverlayGauntletLayer.LoadMovie("BackdropOverlayScreen", m_BackdropOverlayVM);
            m_BackdropOverlayGauntletLayer.InputRestrictions.SetInputRestrictions();
            AddLayer(m_BackdropOverlayGauntletLayer);

            CreateLiveEditingGauntletLayer();
        }

        private void CreateLiveEditingGauntletLayer()
        {
            UIEditorGauntletLayer = new GauntletLayer(9991) { IsFocusLayer = true };
            m_UIEditorMovie = UIEditorGauntletLayer.LoadMovie("LiveEditingScreen", UIEditorVM);
            UIEditorGauntletLayer.InputRestrictions.SetInputRestrictions();
            AddLayer(UIEditorGauntletLayer);
        }

        private void DestroyLiveEditingGauntletLayer()
        {
            m_UIEditorMovie?.Release();
            UIEditorGauntletLayer?.ReleaseMovie(m_UIEditorMovie);
            RemoveLayer(UIEditorGauntletLayer);
            UIEditorGauntletLayer = null;
        }

        public void RefreshLiveEditingScreen()
        {
            DestroyLiveEditingGauntletLayer();

            CreateLiveEditingGauntletLayer();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ScreenManager.TrySetFocus(UIEditorGauntletLayer);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            if (UIEditorGauntletLayer is not null)
            {
                UIEditorGauntletLayer.IsFocusLayer = false;
            }

            ScreenManager.TryLoseFocus(UIEditorGauntletLayer);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();

            UIEditorVM.OnFinalize();
            DestroyLiveEditingGauntletLayer();

            m_BackdropOverlayVM.OnFinalize();
            m_BackdropOverlayMovie?.Release();
            m_BackdropOverlayGauntletLayer?.ReleaseMovie(m_BackdropOverlayMovie);
            RemoveLayer(m_BackdropOverlayGauntletLayer);
            m_BackdropOverlayGauntletLayer = null;
        }
    }
}
