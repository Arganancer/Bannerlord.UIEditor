using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public class WidgetViewModel : IConnectedObject, INotifyPropertyChanged, IFocusable
    {
        public UIEditorWidget Widget { get; }

        public string? Name
        {
            get => m_Name;
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReadonly
        {
            get => m_IsReadonly;
            protected set
            {
                m_IsReadonly = value;
                Widget.IsReadonly = m_IsReadonly;
                OnPropertyChanged();
            }
        }

        public virtual bool IsFocused
        {
            get => m_IsFocused;
            set
            {
                m_IsFocused = value;

                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => m_IsExpanded;
            set
            {
                m_IsExpanded = value;
                OnPropertyChanged();
            }
        }

        protected IPublicContainer PublicContainer { get; }

        private bool m_IsFocused;
        private string? m_Name;
        private bool m_IsReadonly;
        private bool m_IsExpanded;

        public WidgetViewModel(string _name, UIEditorWidget _widget, IPublicContainer _publicContainer)
        {
            Name = _name;
            Widget = _widget;
            IsReadonly = true;
            PublicContainer = _publicContainer;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<IConnectedObject>? Disposing;

        public void Dispose()
        {
            OnDisposing(this);
        }

        protected virtual void OnDisposing(IConnectedObject _e)
        {
            Disposing?.Invoke(this, _e);
        }

        protected void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
