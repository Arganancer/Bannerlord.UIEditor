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
        #region Public Properties

        public Action OnModulesLoaded { get; private set; }
        public Type? Target { get; private set; }
        public CallSequence CallSequence { get; private set; }
        public Type SubscriberType { get; private set; }

        #endregion

        #region Public Fields

        public List<ModuleLoadedSubscription> CallAfterList;

        #endregion

        #region Constructors

        public ModuleLoadedSubscription(Type _subscriberType, Action _onModulesLoaded)
        {
            CallAfterList = new List<ModuleLoadedSubscription>();
            SubscriberType = _subscriberType;
            OnModulesLoaded = _onModulesLoaded;
        }

        #endregion

        #region Public Methods

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

        #endregion

        #region Private Methods

        private void SwapSubscriptions(ModuleLoadedSubscription x, ModuleLoadedSubscription y)
        {
            Action onModulesLoaded = x.OnModulesLoaded;
            Type target = x.Target;
            var callSequence = x.CallSequence;
            Type subscriberType = x.SubscriberType;
            List<ModuleLoadedSubscription> callAfterList = x.CallAfterList;

            x.OnModulesLoaded = y.OnModulesLoaded;
            x.Target = y.Target;
            x.CallSequence = y.CallSequence;
            x.SubscriberType = y.SubscriberType;
            x.CallAfterList = y.CallAfterList;

            y.OnModulesLoaded = onModulesLoaded;
            y.Target = target;
            y.CallSequence = callSequence;
            y.SubscriberType = subscriberType;
            y.CallAfterList = callAfterList;
        }

        #endregion
    }
}
