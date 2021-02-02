﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame;

namespace Bannerlord.UIEditor.AppContext
{
    internal class ApplicationManager : Module, IApplicationManager
    {
        #region Fields

        private Thread? m_MainWindowThread;
        private UIEditorApplication m_App = null!;
        private Dispatcher m_Dispatcher = null!;

        #endregion

        #region IApplicationManager Members

        public Window? MainWindow
        {
            get => m_App.MainWindow;
            set
            {
                if (m_App.MainWindow != value)
                {
                    m_App.MainWindow = value;
                }
            }
        }

        public void Dispatch(Action _action, bool _invokeSynchronously = true)
        {
            if (_invokeSynchronously)
            {
                m_Dispatcher.Invoke(_action);
            }
            else
            {
                m_Dispatcher.BeginInvoke(_action);
            }
        }

        #endregion

        #region Module Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IApplicationManager>();
        }

        public override void Load()
        {
            base.Load();

            StartApplicationThread();
        }

        protected override void Dispose(bool _disposing)
        {
            base.Dispose(_disposing);

            m_App.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            m_MainWindowThread?.Join();
        }

        #endregion

        #region Private Methods

        [STAThread]
        private void StartApplicationThread()
        {
            if (m_MainWindowThread is not null)
            {
                return;
            }

            ManualResetEvent mainWindowOpened = new(false);
            m_MainWindowThread = new Thread(() =>
            {
                m_Dispatcher = Dispatcher.CurrentDispatcher;
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(m_Dispatcher));

                m_App = new UIEditorApplication {ShutdownMode = ShutdownMode.OnExplicitShutdown};

                mainWindowOpened.Set();

                m_App.Run();
            }) {Name = "MainWindowThread"};

            m_MainWindowThread.SetApartmentState(ApartmentState.STA);
            m_MainWindowThread.IsBackground = true;
            m_MainWindowThread.Start();

            mainWindowOpened.WaitOne();
        }

        #endregion
    }
}
