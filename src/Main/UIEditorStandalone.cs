using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Main.Modules;

namespace Bannerlord.UIEditor.Main
{
    internal class UIEditorStandalone
    {
        private static ModuleCoordinator m_ModuleCoordinator = null!;
        private static SubModuleEventNotifier m_SubModuleEventNotifier = null!;
        private static IGlobalEventManager m_GlobalEventManager = null!;

        private static bool m_IsUIEditorRunning = true;

        private static void Main(string[] _args)
        {
            var startupPublicContainer = PublicContainerHierarchy.Instance.GetPublicContainerInfo("Main");

            m_ModuleCoordinator = new ModuleCoordinator();
            m_ModuleCoordinator.Start(startupPublicContainer);

            m_SubModuleEventNotifier = (SubModuleEventNotifier)m_ModuleCoordinator.MainPublicContainer.GetModule<ISubModuleEventNotifier>();
            m_SubModuleEventNotifier.OnModulesLoaded();

            m_GlobalEventManager = m_ModuleCoordinator.MainPublicContainer.GetModule<IGlobalEventManager>();
            m_GlobalEventManager.GetEvent("OnCloseUIEditor").EventHandler += OnCloseUIEditor;

            m_ModuleCoordinator.AddPublicContainer(PublicContainerHierarchy.Instance.GetPublicContainerInfo("UIEditor"));
            
            while (m_IsUIEditorRunning)
            {
                Thread.Sleep(17);
                m_SubModuleEventNotifier.OnApplicationTick(17);
            }
        }

        private static void OnCloseUIEditor(object _sender, IEnumerable<object> _params)
        {
            m_ModuleCoordinator.Stop();
            m_IsUIEditorRunning = false;
        }
    }
}
