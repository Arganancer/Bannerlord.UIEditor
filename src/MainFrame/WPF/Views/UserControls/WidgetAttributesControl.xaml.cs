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
        public WidgetViewModel? FocusedWidget
        {
            get => m_FocusedWidget;
            private set
            {
                if (m_FocusedWidget != value)
                {
                    m_FocusedWidget = value;
                    OnPropertyChanged();
                }
            }
        }

        public Dictionary<string, bool> ExpandedAttributeCategories { get; } = new();

        private IGauntletManager? GauntletManager { get; set; }

        private IWidgetManager? WidgetManager { get; set; }

        private WidgetViewModel? m_FocusedWidget;

        public WidgetAttributesControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IFocusManager>(this,
                OnFocusManagerRegistered,
                OnFocusManagerUnregistering);

            PublicContainer.ConnectToModule<IGauntletManager>(this,
                _gauntletManager => GauntletManager = _gauntletManager,
                _ => GauntletManager = null);

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusable)
        {
            if (_focusable is not null)
            {
                if (_focusable is FocusableWidgetTemplate focusableWidgetTemplate )
                {
                    if (GauntletManager?.UIContext is not null)
                    {
                        UIEditorWidget widget = WidgetManager!.CreateWidget(GauntletManager.UIContext, focusableWidgetTemplate.WidgetTemplate);
                        FocusedWidget = new WidgetViewModel(widget.Name, widget, PublicContainer);
                    }
                }
                else if(_focusable is WidgetViewModel widgetViewModel)
                {
                    FocusedWidget = widgetViewModel;
                }
            }
            else
            {
                FocusedWidget = null;
            }
        }

        private void OnFocusManagerRegistered(IFocusManager _focusManager)
        {
            _focusManager.FocusChanged += OnFocusChanged;
        }

        private void OnFocusManagerUnregistering(IFocusManager _focusManager)
        {
            _focusManager.FocusChanged -= OnFocusChanged;
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
