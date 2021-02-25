using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public class WidgetCategoryViewModel : INotifyPropertyChanged, IDisposable
    {
        public event EventHandler<FocusableWidgetTemplate>? SelectedWidgetChanged;

        public string Name
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

        public ObservableCollection<FocusableWidgetTemplate> WidgetTemplates
        {
            get => m_WidgetTemplates;
            set
            {
                m_WidgetTemplates = value;
                OnPropertyChanged();
            }
        }

        public FocusableWidgetTemplate? SelectedWidgetTemplate
        {
            get => m_SelectedWidgetTemplate;
            set
            {
                if (m_SelectedWidgetTemplate != value)
                {
                    m_SelectedWidgetTemplate = value;
                    if (m_SelectedWidgetTemplate is not null)
                    {
                        OnSelectedWidgetChanged(m_SelectedWidgetTemplate);
                    }

                    if (m_SelectedWidgetTemplate != null)
                    {
                        m_FocusManager.SetFocus(SelectedWidgetTemplate);
                    }

                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<FocusableWidgetTemplate> m_WidgetTemplates = new();
        private FocusableWidgetTemplate? m_SelectedWidgetTemplate;
        private string m_Name = null!;
        private readonly IFocusManager m_FocusManager;

        public WidgetCategoryViewModel(Dispatcher _dispatcher, IFocusManager _focusManager, IWidgetCategory _widgetCategory)
        {
            Name = _widgetCategory.Name;
            foreach (var widgetTemplate in _widgetCategory.WidgetTemplates)
            {
                _dispatcher.Invoke(() => WidgetTemplates.Add(new FocusableWidgetTemplate(widgetTemplate)));
            }

            m_FocusManager = _focusManager;
            m_FocusManager.FocusChanged += OnFocusChanged;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Dispose()
        {
            m_FocusManager.FocusChanged -= OnFocusChanged;
            WidgetTemplates.Clear();
            m_SelectedWidgetTemplate = null;
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusedItem)
        {
            if (SelectedWidgetTemplate?.Equals(_focusedItem) ?? false)
            {
                return;
            }

            if (_focusedItem is FocusableWidgetTemplate focusableWidgetTemplate && WidgetTemplates.Contains(focusableWidgetTemplate))
            {
                SelectedWidgetTemplate = focusableWidgetTemplate;
            }
            else
            {
                SelectedWidgetTemplate = null;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private void OnSelectedWidgetChanged(FocusableWidgetTemplate _e)
        {
            SelectedWidgetChanged?.Invoke(this, _e);
        }
    }
}
