using System;
using HarmonyLib;

namespace Bannerlord.UIEditor.Core
{
    public interface ISubModuleEventNotifier
    {
        #region Events and Delegates

        event EventHandler BeforeInitialModuleScreenSetAsRoot;
        event EventHandler<float> GameTick;
        event EventHandler<Harmony> PerformManualPatches;

        #endregion

        #region Public Methods

        void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription);

        #endregion
    }
}
