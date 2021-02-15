using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Bannerlord.UIEditor.Core
{
    public class ConnectedWindow : Window, IModule, INotifyPropertyChanged
    {
        #region Properties

        public bool Disposed { get; private set; }

        protected IPublicContainer PublicContainer { get; private set; } = null!;

        #endregion

        #region Fields

        //private ManualResetEvent m_WaitForInitialized;

        private readonly List<IModule> m_Children = new();

        #endregion

        #region IConnectedObject Members

        public event EventHandler<IConnectedObject>? Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IModule child in m_Children)
            {
                child.Dispose();
            }

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

            foreach (FieldInfo fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
            {
                if (typeof( ConnectedUserControl ).IsAssignableFrom(fieldInfo.FieldType))
                {
                    if (fieldInfo.GetValue(this) is IModule module)
                    {
                        m_Children.Add(module);
                    }
                }
            }

            foreach (IModule child in m_Children)
            {
                child.Create(PublicContainer);
            }
        }

        public virtual void Load()
        {
            foreach (IModule child in m_Children)
            {
                child.Load();
            }
        }

        public virtual void Unload()
        {
            foreach (IModule child in m_Children)
            {
                child.Unload();
            }
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
