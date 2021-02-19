namespace Bannerlord.UIEditor.Core
{
    public interface IGlobalEventManager
    {
        IGlobalEvent GetEvent(string _eventName);
        InvokeGlobalEvent GetEventInvoker(string _eventName, object _sender, bool _pushEventToApplicationTick = false);
    }
}
