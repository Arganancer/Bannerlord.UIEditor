using System;
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
    public partial class WidgetListControl : ConnectedUserControl, IWidgetListControl
    {
        public ObservableCollection<IWidgetTemplate> WidgetTemplates { get; }

        public IWidgetTemplate? SelectedWidgetTemplate
        {
            get => m_SelectedWidgetTemplate;
            set
            {
                if (m_SelectedWidgetTemplate != value)
                {
                    m_SelectedWidgetTemplate = value;

                    OnSelectedWidgetTemplateChanged(m_SelectedWidgetTemplate);
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
                            Dispatcher.Invoke(() => WidgetTemplates.Add(widgetTemplate));
                        }
                    }
                }
            }
        }

        private IWidgetManager? m_WidgetManager;
        private IWidgetTemplate? m_SelectedWidgetTemplate;

        public WidgetListControl()
        {
            WidgetTemplates = new ObservableCollection<IWidgetTemplate>();
            DataContext = this;
            InitializeComponent();
        }

        public event EventHandler<IWidgetTemplate?>? SelectedWidgetTemplateChanged;

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            PublicContainer.RegisterModule<IWidgetListControl>(this);
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs _e)
        {
            base.OnGiveFeedback(_e);

            Cursor cursor = Cursors.No;
            if (_e.Effects.HasFlag(DragDropEffects.Move))
            {
                cursor = Cursors.SizeWE;
            }
            else if (_e.Effects.HasFlag(DragDropEffects.Copy))
            {
                cursor = Cursors.Cross;
            }

            Mouse.SetCursor(cursor);

            _e.Handled = true;
        }

        private void OnSelectedWidgetTemplateChanged(IWidgetTemplate? _selectedWidgetTemplate)
        {
            SelectedWidgetTemplateChanged?.Invoke(this, _selectedWidgetTemplate);
        }

        /// <summary>
        /// TODO: Add Drag and Drop between this, SceneExplorer, and Canvas:
        /// General: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/drag-and-drop-overview?view=netframeworkdesktop-4.8
        /// Walkthrough: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/walkthrough-enabling-drag-and-drop-on-a-user-control?view=netframeworkdesktop-4.8
        /// </summary>
        private void Widget_OnMouseMove(object _sender, MouseEventArgs _e)
        {
            if (_e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new();
                data.SetData(nameof( IWidgetTemplate ), SelectedWidgetTemplate!);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
