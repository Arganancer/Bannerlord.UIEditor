using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public delegate void GlobalEventHandler(object _sender, IEnumerable<object> _params);

    internal class GlobalEvent : IGlobalEvent
    {
        public event GlobalEventHandler? EventHandler;

        public void OnEvent(object _sender, IEnumerable<object> _params)
        {
            EventHandler?.Invoke(_sender, _params);
        }
    }

    public interface IGlobalEvent
    {
        event GlobalEventHandler? EventHandler;
    }
}
