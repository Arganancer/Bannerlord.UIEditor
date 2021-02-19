using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public enum CallSequence
    {
        CallBefore,
        CallAfter
    }

    public class ModuleLoadedSubscription
    {
        public Action OnModulesLoaded { get; private set; }
        public Type? Target { get; private set; }
        public CallSequence CallSequence { get; private set; }
        public Type SubscriberType { get; private set; }

        public List<ModuleLoadedSubscription> CallAfterList;

        public ModuleLoadedSubscription(Type _subscriberType, Action _onModulesLoaded)
        {
            CallAfterList = new List<ModuleLoadedSubscription>();
            SubscriberType = _subscriberType;
            OnModulesLoaded = _onModulesLoaded;
        }

        public void CallModulesLoaded()
        {
            OnModulesLoaded();
            foreach (ModuleLoadedSubscription moduleLoadedSubscription in CallAfterList)
            {
                moduleLoadedSubscription.CallModulesLoaded();
            }
        }

        public bool HandlesSubscription(ModuleLoadedSubscription _otherSubscription)
        {
            if (_otherSubscription.Target == SubscriberType)
            {
                if (_otherSubscription.CallSequence == CallSequence.CallBefore)
                {
                    SwapSubscriptions(this, _otherSubscription);
                }

                CallAfterList.Add(_otherSubscription);
                return true;
            }

            if (Target == _otherSubscription.SubscriberType)
            {
                if (CallSequence == CallSequence.CallAfter)
                {
                    SwapSubscriptions(this, _otherSubscription);
                }

                CallAfterList.Add(_otherSubscription);
                return true;
            }

            foreach (ModuleLoadedSubscription subscription in CallAfterList)
            {
                if (subscription.HandlesSubscription(_otherSubscription))
                {
                    return true;
                }
            }

            return false;
        }

        private void SwapSubscriptions(ModuleLoadedSubscription _x, ModuleLoadedSubscription _y)
        {
            Action onModulesLoaded = _x.OnModulesLoaded;
            Type target = _x.Target;
            var callSequence = _x.CallSequence;
            Type subscriberType = _x.SubscriberType;
            List<ModuleLoadedSubscription> callAfterList = _x.CallAfterList;

            _x.OnModulesLoaded = _y.OnModulesLoaded;
            _x.Target = _y.Target;
            _x.CallSequence = _y.CallSequence;
            _x.SubscriberType = _y.SubscriberType;
            _x.CallAfterList = _y.CallAfterList;

            _y.OnModulesLoaded = onModulesLoaded;
            _y.Target = target;
            _y.CallSequence = callSequence;
            _y.SubscriberType = subscriberType;
            _y.CallAfterList = callAfterList;
        }
    }
}
