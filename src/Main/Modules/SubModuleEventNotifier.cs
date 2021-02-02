using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.Main.Modules
{
    internal class SubModuleEventNotifier : Module, ISubModuleEventNotifier
    {
        #region Fields

        private readonly List<ModuleLoadedSubscription> m_OnModulesLoadedSubscribers;

        #endregion

        #region Constructors

        public SubModuleEventNotifier()
        {
            m_OnModulesLoadedSubscribers = new List<ModuleLoadedSubscription>();
        }

        #endregion

        #region ISubModuleEventNotifier Members

        public event EventHandler? BeforeInitialModuleScreenSetAsRoot;
        public event EventHandler<float>? ApplicationTick;

        public void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription)
        {
            if (m_OnModulesLoadedSubscribers.Any(_moduleLoadedSubscription => _moduleLoadedSubscription.HandlesSubscription(_subscription)))
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
            RegisterModule<ISubModuleEventNotifier>();
        }

        #endregion

        #region Public Methods

        public void OnBeforeInitialModuleScreenSetAsRoot()
        {
            BeforeInitialModuleScreenSetAsRoot?.Invoke(this, EventArgs.Empty);
        }

        public void OnApplicationTick(float _e)
        {
            ApplicationTick?.Invoke(this, _e);
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
