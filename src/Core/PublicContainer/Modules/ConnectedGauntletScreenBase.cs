using System;
using TaleWorlds.Engine.Screens;

namespace Bannerlord.UIEditor.Core
{
    public abstract class ConnectedGauntletScreenBase : ScreenBase, IModule
    {
        #region Properties

        protected bool Disposed { get; private set; }

        protected IPublicContainer PublicContainer { get; private set; } = null!;

        #endregion

        #region IConnectedObject Members

        public event EventHandler<IConnectedObject>? Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!Disposed)
            {
                OnDisposing();
                Dispose(true);
            }
        }

        #endregion

        #region IModule Members

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

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool _disposing)
        {
            // Execute if resources have not already been disposed.
            if (!Disposed && _disposing)
            {
                Disposed = true;
            }
        }

        #endregion

        #region Private Methods

        private void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        #endregion
    }
}
