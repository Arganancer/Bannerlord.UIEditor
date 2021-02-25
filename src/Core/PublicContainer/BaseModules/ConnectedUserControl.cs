using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.Core
{
    public class ConnectedUserControl : UserControl, IModule, INotifyPropertyChanged
    {
        public bool Disposed { get; private set; }

        protected IPublicContainer PublicContainer { get; private set; } = null!;

        public event EventHandler<IConnectedObject>? Disposing;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Dispose()
        {
            if (!Disposed)
            {
                OnDisposing();
            }

            Disposed = true;
        }

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

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        protected void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
