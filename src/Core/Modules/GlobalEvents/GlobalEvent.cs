using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public delegate void GlobalEventHandler(object _sender, IEnumerable<object> _params);

    internal class GlobalEvent : IGlobalEvent
    {
        #region IGlobalEvent Members

        public event GlobalEventHandler? EventHandler;

        #endregion

        #region Public Methods

        public void OnEvent(object _sender, IEnumerable<object> _params)
        {
            EventHandler?.Invoke(_sender, _params);
        }

        #endregion
    }

    public interface IGlobalEvent
    {
        #region Events and Delegates

        event GlobalEventHandler? EventHandler;

        #endregion
    }
}
