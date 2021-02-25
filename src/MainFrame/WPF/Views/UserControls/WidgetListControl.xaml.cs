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
        public bool IsLoading
        {
            get => m_IsLoading;
            set
            {
                if (m_IsLoading != value)
                {
                    m_IsLoading = value;
                    Dispatcher.Invoke(() => LoadingSpinner.IsLoading = m_IsLoading);
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<WidgetCategoryViewModel> WidgetTemplateCategories
        {
            get => m_WidgetTemplateCategories;
            set
            {
                m_WidgetTemplateCategories = value;
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
                    if (m_WidgetManager is not null)
                    {
                        m_WidgetManager.IsWorkingChanged -= OnWidgetManagerIsWorkingChanged;
                    }
                    m_WidgetManager = value;
                    if (m_WidgetManager is not null)
                    {
                        m_WidgetManager.IsWorkingChanged += OnWidgetManagerIsWorkingChanged;
                        IsLoading = m_WidgetManager.IsWorking;
                        if (!m_WidgetManager.IsWorking)
                        {
                            OnWidgetManagerIsWorkingChanged(this, m_WidgetManager.IsWorking);
                        }
                    }
                }
            }
        }

        private void OnWidgetManagerIsWorkingChanged(object _sender, bool _isWorking)
        {
            if (!_isWorking)
            {
                if (WidgetTemplateCategories.Count > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        foreach (WidgetCategoryViewModel widgetTemplateCategory in WidgetTemplateCategories)
                        {
                            widgetTemplateCategory.SelectedWidgetChanged -= OnWidgetCategorySelectedWidgetChanged;
                            widgetTemplateCategory.Dispose();
                        }

                        WidgetTemplateCategories.Clear();
                    });
                }

                foreach (IWidgetCategory widgetCategory in m_WidgetManager!.WidgetTemplateCategories)
                {
                    var widgetCategoryVM = new WidgetCategoryViewModel(Dispatcher, FocusManager!, widgetCategory);
                    widgetCategoryVM.SelectedWidgetChanged += OnWidgetCategorySelectedWidgetChanged;

                    Dispatcher.Invoke(() => WidgetTemplateCategories.Add(widgetCategoryVM));
                }
            }
            IsLoading = _isWorking;
        }

        private IFocusManager? FocusManager { get; set; }
        private IWidgetManager? m_WidgetManager;
        private FocusableWidgetTemplate? m_SelectedWidgetTemplate;
        private ICursorManager m_CursorManager = null!;
        private ObservableCollection<WidgetCategoryViewModel> m_WidgetTemplateCategories = new();
        private bool m_IsLoading;

        public WidgetListControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IFocusManager>(this,
                _focusManager => FocusManager = _focusManager,
                _ => FocusManager = null);

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);

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

        private void OnWidgetCategorySelectedWidgetChanged(object _sender, FocusableWidgetTemplate _e)
        {
            SelectedWidgetTemplate = _e;
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
