using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.MainFrame
{
    [Panel(300, 300, _isOpen: true, _path: "Right")]
    public partial class WidgetAttributesControl : ConnectedUserControl, IPanel
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

        public ISettingCategory SettingCategory { get; set; } = null!;

        public string PanelName => "Properties";

#if !STANDALONE_EDITOR
        private IGauntletManager? GauntletManager { get; set; }
#endif
        private IWidgetManager? WidgetManager { get; set; }

        private WidgetViewModel? m_FocusedWidget;

        public WidgetAttributesControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.ConnectToModule<IFocusManager>(this,
                OnFocusManagerRegistered,
                OnFocusManagerUnregistering);

#if !STANDALONE_EDITOR
            PublicContainer.ConnectToModule<IGauntletManager>(this,
                _gauntletManager => GauntletManager = _gauntletManager,
                _ => GauntletManager = null);
#endif

            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusable)
        {
            if (_focusable is not null)
            {
                if (_focusable is FocusableWidgetTemplate focusableWidgetTemplate)
                {
#if !STANDALONE_EDITOR
                    if (GauntletManager?.UIContext is not null)
                    {
                        UIEditorWidget widget = WidgetManager!.CreateWidget(GauntletManager.UIContext, focusableWidgetTemplate.WidgetTemplate);
                        FocusedWidget = new WidgetViewModel(widget.Name, widget, PublicContainer);
                    }
#else
                    UIEditorWidget widget = WidgetManager!.CreateWidget(null!, WidgetManager.WidgetTemplateCategories.SelectMany(_x => _x.WidgetTemplates).First(_x => _x.Name == "Widget"));
                    FocusedWidget = new WidgetViewModel(widget.Name, widget, PublicContainer);
#endif
                }
                else if (_focusable is WidgetViewModel widgetViewModel)
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
            SettingCategory.SetSetting($"{attributeCategory}_Expanded", true);
        }

        private void Expander_OnCollapsed(object _sender, RoutedEventArgs _e)
        {
            string attributeCategory = ((AttributeCategory)((Expander)_sender).DataContext).Name;
            SettingCategory.SetSetting($"{attributeCategory}_Expanded", false);
        }

        private void Expander_OnLoaded(object _sender, RoutedEventArgs _e)
        {
            Expander expander = (Expander)_sender;
            string attributeCategory = ((AttributeCategory)expander.DataContext).Name;
            expander.IsExpanded = SettingCategory.GetSetting($"{attributeCategory}_Expanded", expander.IsExpanded);
        }
    }
}
