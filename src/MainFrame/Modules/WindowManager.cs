using System;
using System.ComponentModel;
using Bannerlord.UIEditor.AppContext;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    internal class WindowManager : Module, IWindowManager
    {
        private IGlobalEventManager m_GlobalEventManager = null!;
        private IApplicationManager m_ApplicationManager = null!;
        private InvokeGlobalEvent m_OnCloseUIEditor = null!;
        private MainWindow m_MainWindow = null!;

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IWindowManager>();

            m_ApplicationManager = PublicContainer.GetModule<IApplicationManager>();

            InitializeMainWindow();
        }

        public override void Load()
        {
            base.Load();

            m_GlobalEventManager = PublicContainer.GetModule<IGlobalEventManager>();
            m_OnCloseUIEditor = m_GlobalEventManager.GetEventInvoker("OnCloseUIEditor", this, true);

            m_MainWindow.Load();

            m_ApplicationManager.Dispatch(() =>
            {
                m_ApplicationManager.MainWindow = m_MainWindow;
                m_MainWindow.Show();
            });
        }

        public override void Unload()
        {
            m_MainWindow.Unload();

            base.Unload();
        }

        private void InitializeMainWindow()
        {
            m_ApplicationManager.Dispatch(() =>
            {
                m_MainWindow = new MainWindow();
                m_MainWindow.Create(PublicContainer);
                m_MainWindow.Closing += OnMainWindowClosing;
                m_MainWindow.Closed += OnMainWindowClosed;
            });
        }

        private void OnMainWindowClosing(object _sender, CancelEventArgs _e)
        {
            m_OnCloseUIEditor();
        }

        private void OnMainWindowClosed(object _sender, EventArgs _e)
        {
            m_ApplicationManager.MainWindow = null;
        }
    }
}
