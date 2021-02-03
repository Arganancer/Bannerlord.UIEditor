using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public delegate void InvokeGlobalEvent(params object[] _params);

    internal class GlobalEventManager : Module, IGlobalEventManager
    {
        #region Fields

        private ISubModuleEventNotifier m_SubModuleEventNotifier = null!;

        private readonly Dictionary<string, GlobalEvent> m_GlobalEventHandlers;
        private readonly ConcurrentQueue<Action> m_QueuedApplicationTickEvents;

        #endregion

        #region Constructors

        public GlobalEventManager()
        {
            m_QueuedApplicationTickEvents = new ConcurrentQueue<Action>();
            m_GlobalEventHandlers = new Dictionary<string, GlobalEvent>();
        }

        #endregion

        #region IGlobalEventManager Members

        public IGlobalEvent GetEvent(string _eventName)
        {
            return GetGlobalEvent(_eventName);
        }

        /// <summary>
        /// TODO: For more versatility, change this so the subscriber specifies which thread he wishes to receive the event on.
        /// </summary>
        public InvokeGlobalEvent GetEventInvoker(string _eventName, object _sender, bool _pushEventToApplicationTick)
        {
            GlobalEvent globalEvent = GetGlobalEvent(_eventName);
            if (_pushEventToApplicationTick)
            {
                return _params => m_QueuedApplicationTickEvents.Enqueue(() => globalEvent.OnEvent(_sender, _params));
            }

            return _params => globalEvent.OnEvent(_sender, _params);
        }

        #endregion

        #region Module Members

        public override void Load()
        {
            base.Load();

            m_SubModuleEventNotifier = PublicContainer.GetModule<ISubModuleEventNotifier>();
            m_SubModuleEventNotifier.ApplicationTick += OnApplicationTick;
        }

        public override void Unload()
        {
            m_SubModuleEventNotifier.ApplicationTick -= OnApplicationTick;
            base.Unload();
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IGlobalEventManager>();
        }

        #endregion

        #region Private Methods

        private void OnApplicationTick(object _sender, float _deltaT)
        {
            while (m_QueuedApplicationTickEvents.TryDequeue(out Action invokeEvent))
            {
                invokeEvent();
            }
        }

        private GlobalEvent GetGlobalEvent(string _eventName)
        {
            if (!m_GlobalEventHandlers.TryGetValue(_eventName, out GlobalEvent globalEvent))
            {
                globalEvent = new GlobalEvent();
                m_GlobalEventHandlers.Add(_eventName, globalEvent);
            }

            return globalEvent;
        }

        #endregion
    }
}
