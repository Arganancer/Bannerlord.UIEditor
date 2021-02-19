using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public partial class WidgetAttributesControl : ConnectedUserControl
    {
        public WidgetViewModel? SelectedWidget
        {
            get => m_SelectedWidget;
            private set
            {
                if (m_SelectedWidget != value)
                {
                    m_SelectedWidget = value;
                    OnPropertyChanged();
                }
            }
        }

        public Dictionary<string, bool> ExpandedAttributeCategories { get; } = new();

        private IGauntletManager? GauntletManager { get; set; }

        private IWidgetManager? WidgetManager { get; set; }

        private WidgetViewModel? m_SelectedWidget;

        public WidgetAttributesControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IWidgetListControl>(this,
                OnWidgetListControlRegistered,
                OnWidgetListControlUnregistering);

            PublicContainer.ConnectToModule<ISceneExplorerControl>(this,
                OnSceneExplorerControlRegistered,
                OnSceneExplorerControlUnregistering);

            PublicContainer.ConnectToModule<IGauntletManager>(this,
                _gauntletManager => GauntletManager = _gauntletManager,
                _ => GauntletManager = null);

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);
        }

        private void OnSelectedWidgetTemplateChanged(object _sender, IWidgetTemplate? _selectedWidgetTemplate)
        {
            if (_selectedWidgetTemplate is not null)
            {
                if (GauntletManager?.UIContext is not null)
                {
                    UIEditorWidget widget = WidgetManager!.CreateWidget(GauntletManager.UIContext, _selectedWidgetTemplate);
                    SelectedWidget = new WidgetViewModel(widget.Name, widget, null) {IsReadonly = true};
                }
            }
            else
            {
                SelectedWidget = null;
            }
        }

        private void OnSelectedWidgetChanged(object _sender, WidgetViewModel? _selectedWidget)
        {
            SelectedWidget = _selectedWidget;
        }

        private void OnWidgetListControlRegistered(IWidgetListControl _widgetListControl)
        {
            _widgetListControl.SelectedWidgetTemplateChanged += OnSelectedWidgetTemplateChanged;
        }

        private void OnWidgetListControlUnregistering(IWidgetListControl _widgetListControl)
        {
            _widgetListControl.SelectedWidgetTemplateChanged -= OnSelectedWidgetTemplateChanged;
        }

        private void OnSceneExplorerControlRegistered(ISceneExplorerControl _sceneExplorerControl)
        {
            _sceneExplorerControl.SelectedWidgetChanged += OnSelectedWidgetChanged;
        }

        private void OnSceneExplorerControlUnregistering(ISceneExplorerControl _sceneExplorerControl)
        {
            _sceneExplorerControl.SelectedWidgetChanged -= OnSelectedWidgetChanged;
        }

        private void Expander_OnExpanded(object _sender, RoutedEventArgs _e)
        {
            string attributeCategory = ((AttributeCategory)((Expander)_sender).DataContext).Name;
            ExpandedAttributeCategories[attributeCategory] = true;
        }

        private void Expander_OnCollapsed(object _sender, RoutedEventArgs _e)
        {
            string attributeCategory = ((AttributeCategory)((Expander)_sender).DataContext).Name;
            ExpandedAttributeCategories[attributeCategory] = false;
        }

        private void Expander_OnLoaded(object _sender, RoutedEventArgs _e)
        {
            Expander expander = (Expander)_sender;
            string attributeCategory = ((AttributeCategory)expander.DataContext).Name;
            if (!ExpandedAttributeCategories.TryGetValue(attributeCategory, out var isExpanded))
            {
                isExpanded = false;
            }

            expander.IsExpanded = isExpanded;
        }
    }
}
