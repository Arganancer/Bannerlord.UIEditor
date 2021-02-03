using Bannerlord.UIEditor.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;

namespace Bannerlord.UIEditor.MainFrame.Gauntlet
{
    public class UIEditorGauntletScreen : ConnectedGauntletScreenBase
    {
        #region Properties

        public UIEditorVM UIEditorVM { get; private set; } = null!;
        public GauntletLayer? UIEditorGauntletLayer { get; private set; }

        #endregion

        #region Fields

        private GauntletMovie? m_UIEditorMovie;

        private BackdropOverlayVM m_BackgroundOverlayVM = null!;
        private GauntletLayer? m_BackdropOverlayGauntletLayer;
        private GauntletMovie? m_BackdropOverlayMovie;

        #endregion

        #region ConnectedGauntletScreenBase Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            m_BackgroundOverlayVM = new BackdropOverlayVM();
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

        #endregion

        #region ScreenBase Members

        protected override void OnInitialize()
        {
            base.OnInitialize();

            m_BackdropOverlayGauntletLayer = new GauntletLayer(9990);
            m_BackdropOverlayMovie = m_BackdropOverlayGauntletLayer.LoadMovie("BackdropOverlayScreen", m_BackgroundOverlayVM);
            m_BackdropOverlayGauntletLayer.InputRestrictions.SetInputRestrictions();
            AddLayer(m_BackdropOverlayGauntletLayer);

            UIEditorGauntletLayer = new GauntletLayer(9991) {IsFocusLayer = true};
            m_UIEditorMovie = UIEditorGauntletLayer.LoadMovie("LiveEditingScreen", UIEditorVM);
            UIEditorGauntletLayer.InputRestrictions.SetInputRestrictions();
            AddLayer(UIEditorGauntletLayer);
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

            UIEditorGauntletLayer?.ReleaseMovie(m_UIEditorMovie);
            UIEditorVM.OnFinalize();
            RemoveLayer(UIEditorGauntletLayer);

            m_BackdropOverlayGauntletLayer?.ReleaseMovie(m_BackdropOverlayMovie);
            m_BackgroundOverlayVM.OnFinalize();
            RemoveLayer(m_BackdropOverlayGauntletLayer);
        }

        #endregion
    }
}
