using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.Main
{
    internal class UIEditorLauncher : Module
    {
        #region Consts/Statics

        public static UIEditorLauncher Instance { get; private set; } = null!;

        #endregion

        #region Fields

        private IGlobalEventManager? m_GlobalEventManager;
        private InvokeGlobalEvent m_OnLaunchUIEditor;

        #endregion

        #region Module Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            Instance = this;
        }

        public override void Load()
        {
            base.Load();

            m_GlobalEventManager = PublicContainer.GetModule<IGlobalEventManager>();
            m_OnLaunchUIEditor = m_GlobalEventManager.GetEventInvoker("OnLaunchUIEditor", this);
        }

        #endregion

        #region Public Methods

        public void LaunchUIEditor()
        {
            m_OnLaunchUIEditor();
        }

        #endregion
    }
}
