using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Interaction logic for WidgetListControl.xaml
    /// </summary>
    public partial class WidgetListControl : ConnectedUserControl
    {
        public ObservableCollection<FocusableWidgetTemplate> WidgetTemplates { get; }

        public FocusableWidgetTemplate? SelectedWidgetTemplate
        {
            get => m_SelectedWidgetTemplate;
            set
            {
                if (m_SelectedWidgetTemplate != value)
                {
                    m_SelectedWidgetTemplate = value;
                    if (m_SelectedWidgetTemplate != null)
                    {
                        FocusManager?.SetFocus(SelectedWidgetTemplate);
                    }
                    OnPropertyChanged();
                }
            }
        }

        private IWidgetManager? WidgetManager
        {
            set
            {
                if (m_WidgetManager != value)
                {
                    if (WidgetTemplates.Count > 0)
                    {
                        Dispatcher.Invoke(() => WidgetTemplates.Clear());
                    }

                    m_WidgetManager = value;
                    if (m_WidgetManager is not null)
                    {
                        foreach (IWidgetTemplate widgetTemplate in m_WidgetManager.WidgetTemplates)
                        {
                            Dispatcher.Invoke(() => WidgetTemplates.Add(new FocusableWidgetTemplate(widgetTemplate)));
                        }
                    }
                }
            }
        }

        private IFocusManager? FocusManager
        {
            get => m_FocusManager;
            set
            {
                if (m_FocusManager is not null)
                {
                    m_FocusManager.FocusChanged -= OnFocusChanged;
                }
                m_FocusManager = value;
                if (m_FocusManager is not null)
                {
                    m_FocusManager.FocusChanged += OnFocusChanged;
                }
            }
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusedItem)
        {
            if (_focusedItem is FocusableWidgetTemplate focusableWidgetTemplate)
            {
                SelectedWidgetTemplate = focusableWidgetTemplate;
            }
            else
            {
                SelectedWidgetTemplate = null;
            }
        }

        private IWidgetManager? m_WidgetManager;
        private FocusableWidgetTemplate? m_SelectedWidgetTemplate;
        private IFocusManager? m_FocusManager;
        private ICursorManager m_CursorManager;

        public WidgetListControl()
        {
            WidgetTemplates = new ObservableCollection<FocusableWidgetTemplate>();
            DataContext = this;
            InitializeComponent();
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);

            PublicContainer.ConnectToModule<IFocusManager>(this,
                _focusManager => FocusManager = _focusManager,
                _ => FocusManager = null);

            m_CursorManager = PublicContainer.GetModule<ICursorManager>();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs _e)
        {
            base.OnGiveFeedback(_e);

            if (_e.Effects.HasFlag(DragDropEffects.Move))
            {
                m_CursorManager.SetCursor(CursorIcon.InsertIcon);
            }
            else if (_e.Effects.HasFlag(DragDropEffects.Copy))
            {
                m_CursorManager.SetCursor(CursorIcon.AddIcon);
            }
            else
            {
                m_CursorManager.SetCursor(CursorIcon.NoIcon);
            }

            _e.Handled = true;
        }
        
        private void Widget_OnMouseMove(object _sender, MouseEventArgs _e)
        {
            if (_e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new();
                data.SetData(nameof( FocusableWidgetTemplate ), SelectedWidgetTemplate!);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
