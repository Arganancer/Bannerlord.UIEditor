using System;
using HarmonyLib;

namespace Bannerlord.UIEditor.Core
{
    public interface ISubModuleEventNotifier
    {
        #region Events and Delegates

        event EventHandler BeforeInitialModuleScreenSetAsRoot;
        event EventHandler<float> ApplicationTick;

        #endregion

        #region Public Methods

        void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription);

        #endregion
    }
}
