using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.Main
{
    internal class UIEditorLauncher : Module
    {
        public static UIEditorLauncher Instance { get; private set; } = null!;

        private IGlobalEventManager? m_GlobalEventManager;
        private InvokeGlobalEvent? m_OnLaunchUIEditor;

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

        public void LaunchUIEditor()
        {
            m_OnLaunchUIEditor?.Invoke();
        }
    }
}
