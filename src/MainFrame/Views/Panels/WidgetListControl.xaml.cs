using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    [Panel(180, 300, _isOpen: true, _path: "Left;HorizontalLayoutContainer:Left")]
    public partial class WidgetListControl : IPanel
    {
        public bool IsLoading
        {
            get => m_IsLoading;
            set
            {
                if (m_IsLoading != value)
                {
                    m_IsLoading = value;
                    Dispatcher.Invoke(() =>
                    {
                        WidgetListPanel.Visibility = m_IsLoading ? Visibility.Collapsed : Visibility.Visible;
                    });
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

        public string PanelName => "Toolbox";

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

        private IFocusManager? FocusManager { get; set; }

        private ObservableCollection<WidgetCategoryViewModel> m_WidgetTemplateCategories = new();
        private FocusableWidgetTemplate? m_SelectedWidgetTemplate;
        private bool m_IsLoading;
        private IWidgetManager? m_WidgetManager;
        private ICursorManager m_CursorManager = null!;
        public ISettingCategory SettingCategory { get; set; } = null!;

        public WidgetListControl()
        {
            InitializeComponent();
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

        private void Expander_OnExpanded(object _sender, RoutedEventArgs _e)
        {
            string attributeCategory = ((WidgetCategoryViewModel)((Expander)_sender).DataContext).Name;
            SettingCategory.SetSetting($"{attributeCategory}_Expanded", true);
        }

        private void Expander_OnCollapsed(object _sender, RoutedEventArgs _e)
        {
            string attributeCategory = ((WidgetCategoryViewModel)((Expander)_sender).DataContext).Name;
            SettingCategory.SetSetting($"{attributeCategory}_Expanded", false);
        }

        private void Expander_OnLoaded(object _sender, RoutedEventArgs _e)
        {
            Expander expander = (Expander)_sender;
            string attributeCategory = ((WidgetCategoryViewModel)expander.DataContext).Name;
            expander.IsExpanded = SettingCategory.GetSetting($"{attributeCategory}_Expanded", expander.IsExpanded);
        }
    }
}
