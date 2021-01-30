using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIEditor.Core;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.UIEditor.Main
{
    public class UIEditorSubModule : MBSubModuleBase
    {
        #region Static/Const Fields

        private const string HarmonyName = "Bannerlord.UIEditor";

        #endregion

        #region Private Fields

        private ModuleCoordinator m_ModuleCoordinator = null!;
        private SubModuleEventNotifier m_SubModuleEventNotifier = null!;

        #endregion

        #region MBSubModuleBase Members

        protected override void OnSubModuleLoad()
        {
            // TODO: Find a better solution for this once we have a clearer idea of which assemblies belong in which PublicContainer.
            List<Assembly> startupAssemblies = new()
            {
                Assembly.Load(typeof( ModuleCoordinator ).Assembly.FullName!) // Core
            };

            m_ModuleCoordinator = new ModuleCoordinator(startupAssemblies);
            m_ModuleCoordinator.Start();

            m_SubModuleEventNotifier = (SubModuleEventNotifier)m_ModuleCoordinator.PublicContainer.GetModule<ISubModuleEventNotifier>()!;
            m_SubModuleEventNotifier.OnModulesLoaded();

            Harmony harmony = new(HarmonyName);
            foreach (Assembly assembly in startupAssemblies)
            {
                harmony.PatchAll(assembly);
            }

            PerformManualPatches(harmony);

            base.OnSubModuleLoad();
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

        #endregion

        #region Private Methods

        private void PerformManualPatches(Harmony _harmony)
        {
            m_SubModuleEventNotifier.OnPerformManualPatches(_harmony);
        }

        #endregion
    }
}
