using System;

namespace Bannerlord.UIEditor.Core
{
    public interface IConnectedObject : IDisposable
    {
        event EventHandler<IConnectedObject> Disposing;
    }
}
