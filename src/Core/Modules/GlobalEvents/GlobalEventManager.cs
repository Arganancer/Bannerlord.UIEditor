using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public delegate void InvokeGlobalEvent(params object[] _params);

    internal class GlobalEventManager : Module, IGlobalEventManager
    {
        #region Fields

        private readonly Dictionary<string, GlobalEvent> m_GlobalEventHandlers;

        #endregion

        #region Constructors

        public GlobalEventManager()
        {
            m_GlobalEventHandlers = new Dictionary<string, GlobalEvent>();
        }

        #endregion

        #region IGlobalEventManager Members

        public IGlobalEvent GetEvent(string _eventName)
        {
            return GetGlobalEvent(_eventName);
        }

        public InvokeGlobalEvent GetEventInvoker(string _eventName, object _sender)
        {
            GlobalEvent globalEvent = GetGlobalEvent(_eventName);
            return _params => globalEvent.OnEvent(_sender, _params);
        }

        #endregion

        #region Module Members

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IGlobalEventManager>();
        }

        #endregion

        #region Private Methods

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
