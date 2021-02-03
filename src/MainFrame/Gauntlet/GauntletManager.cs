using Bannerlord.UIEditor.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;

namespace Bannerlord.UIEditor.MainFrame.Gauntlet
{
    public class GauntletManager : Module, IGauntletManager
    {
        #region IGauntletManager Members

        public UIEditorGauntletScreen UIEditorGauntletScreen { get; private set; } = null!;

        public UIContext? UIContext => UIEditorGauntletScreen.UIEditorGauntletLayer?._gauntletUIContext;

        #endregion

        #region Module Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            InitializeUIEditorGauntletScreen();

            RegisterModule<IGauntletManager>();
        }

        public override void Load()
        {
            base.Load();

            UIEditorGauntletScreen.Load();
            ScreenManager.PushScreen(UIEditorGauntletScreen);
        }

        public override void Unload()
        {
            if (ScreenManager.TopScreen.Equals(UIEditorGauntletScreen))
            {
                ScreenManager.PopScreen();
            }

            UIEditorGauntletScreen.Unload();

            base.Unload();
        }

        protected override void Dispose(bool _disposing)
        {
            UIEditorGauntletScreen.Dispose();

            base.Dispose(_disposing);
        }

        #endregion

        #region Private Methods

        private void InitializeUIEditorGauntletScreen()
        {
            UIEditorGauntletScreen = (UIEditorGauntletScreen)ViewCreatorManager.CreateScreenView<UIEditorGauntletScreen>();
            UIEditorGauntletScreen.Create(PublicContainer);
        }

        #endregion
    }
}
