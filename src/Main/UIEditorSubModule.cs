#if STABLE_DEBUG || BETA_DEBUG
using System;
#endif
using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Main.Modules;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.UIEditor.Main
{
    public class UIEditorSubModule : MBSubModuleBase
    {
        private const string HarmonyName = "Bannerlord.UIEditor";

        private ModuleCoordinator m_ModuleCoordinator = null!;
        private SubModuleEventNotifier m_SubModuleEventNotifier = null!;
        private IGlobalEventManager m_GlobalEventManager = null!;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var startupPublicContainer = PublicContainerHierarchy.Instance.GetPublicContainerInfo("Main");

            m_ModuleCoordinator = new ModuleCoordinator();
            m_ModuleCoordinator.Start(startupPublicContainer);

            m_SubModuleEventNotifier = (SubModuleEventNotifier)m_ModuleCoordinator.MainPublicContainer.GetModule<ISubModuleEventNotifier>();
            m_SubModuleEventNotifier.OnModulesLoaded();

            m_GlobalEventManager = m_ModuleCoordinator.MainPublicContainer.GetModule<IGlobalEventManager>();
            m_GlobalEventManager.GetEvent("OnLaunchUIEditor").EventHandler += OnLaunchUIEditor;
            m_GlobalEventManager.GetEvent("OnCloseUIEditor").EventHandler += OnCloseUIEditor;

            Harmony harmony = new(HarmonyName);

            harmony.PatchAll();
            foreach (Assembly assembly in startupPublicContainer.DefaultAssemblies)
            {
                harmony.PatchAll(assembly);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            m_ModuleCoordinator.Stop();

            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage($"Loaded UIEditor {typeof( UIEditorSubModule ).Assembly.GetName().Version!.ToString(3)}"));
            m_SubModuleEventNotifier.OnBeforeInitialModuleScreenSetAsRoot();
        }

        protected override void OnApplicationTick(float _deltaTime)
        {
            m_SubModuleEventNotifier.OnApplicationTick(_deltaTime);
            base.OnApplicationTick(_deltaTime);
        }

        private void OnLaunchUIEditor(object _sender, IEnumerable<object> _params)
        {
            if (!m_ModuleCoordinator.ContainsPublicContainer("UIEditor"))
            {
                m_ModuleCoordinator.AddPublicContainer(PublicContainerHierarchy.Instance.GetPublicContainerInfo("UIEditor"));
            }
#if STABLE_DEBUG || BETA_DEBUG
            else
            {
                throw new Exception("Tried launching the UIEditor while it was already open.");
            }
#endif
        }

        private void OnCloseUIEditor(object _sender, IEnumerable<object> _params)
        {
            if (m_ModuleCoordinator.ContainsPublicContainer("UIEditor"))
            {
                m_ModuleCoordinator.RemovePublicContainer("UIEditor");
            }
#if STABLE_DEBUG || BETA_DEBUG
            else
            {
                throw new Exception("Tried closing the UIEditor while it was already closed.");
            }
#endif
        }
    }
}
