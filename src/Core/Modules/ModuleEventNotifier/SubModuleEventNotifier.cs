using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Bannerlord.UIEditor.Core
{
    public class SubModuleEventNotifier : Module, ISubModuleEventNotifier
    {
        #region Private Fields

        private List<ModuleLoadedSubscription> m_OnModulesLoadedSubscribers;

        #endregion

        #region ISubModuleEventNotifier Members

        public event EventHandler BeforeInitialModuleScreenSetAsRoot;
        public event EventHandler<float> GameTick;
        public event EventHandler<Harmony> PerformManualPatches;

        public void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription)
        {
            if (m_OnModulesLoadedSubscribers.Any(moduleLoadedSubscription => moduleLoadedSubscription.HandlesSubscription(_subscription)))
            {
                return;
            }

            m_OnModulesLoadedSubscribers.Add(_subscription);
        }

        #endregion

        #region Module Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            m_OnModulesLoadedSubscribers = new List<ModuleLoadedSubscription>();

            RegisterModule<ISubModuleEventNotifier>();
        }

        #endregion

        #region Public Methods

        public void OnBeforeInitialModuleScreenSetAsRoot()
        {
            BeforeInitialModuleScreenSetAsRoot?.Invoke(this, EventArgs.Empty);
        }

        public void OnGameTick(float _e)
        {
            GameTick?.Invoke(this, _e);
        }

        public void OnPerformManualPatches(Harmony _e)
        {
            PerformManualPatches?.Invoke(this, _e);
        }

        public void OnModulesLoaded()
        {
            foreach (ModuleLoadedSubscription moduleLoadedSubscription in m_OnModulesLoadedSubscribers)
            {
                moduleLoadedSubscription.CallModulesLoaded();
            }
        }

        #endregion
    }
}
