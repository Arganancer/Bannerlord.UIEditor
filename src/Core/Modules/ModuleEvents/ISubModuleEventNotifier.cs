using System;

namespace Bannerlord.UIEditor.Core
{
    public interface ISubModuleEventNotifier
    {
        event EventHandler BeforeInitialModuleScreenSetAsRoot;
        event EventHandler<float> ApplicationTick;

        void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription);
    }
}
