using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.Core
{
    public class ConnectedUserControl : UserControl, IModule, INotifyPropertyChanged
    {
        #region Properties

        public bool Disposed { get; private set; }

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
            }

            Disposed = true;
        }

        #endregion

        #region IModule Members

        public virtual void Create(IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;
        }

        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Protected Methods

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        protected void OnPropertyChanged([CallerMemberName] string _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        #endregion
    }
}
