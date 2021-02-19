using System;
using TaleWorlds.Library;

namespace Bannerlord.UIEditor.Core
{
    public abstract class ConnectedGauntletViewModel : ViewModel, IModule
    {
        protected bool Disposed { get; private set; }

        protected IPublicContainer PublicContainer { get; private set; } = null!;

        public event EventHandler<IConnectedObject>? Disposing;

        public void Dispose()
        {
            if (!Disposed)
            {
                OnDisposing();
                Dispose(true);
            }
        }

        public virtual void Create(IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;
        }

        /// <inheritdoc />
        public virtual void Load()
        {
        }

        /// <inheritdoc />
        public virtual void Unload()
        {
        }

        protected virtual void Dispose(bool _disposing)
        {
            // Execute if resources have not already been disposed.
            if (!Disposed && _disposing)
            {
                Disposed = true;
            }
        }

        private void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }
    }
}
