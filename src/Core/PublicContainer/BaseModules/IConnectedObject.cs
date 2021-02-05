using System;

namespace Bannerlord.UIEditor.Core
{
    public interface IConnectedObject : IDisposable
    {
        #region Events and Delegates

        event EventHandler<IConnectedObject> Disposing;

        #endregion
    }
}
