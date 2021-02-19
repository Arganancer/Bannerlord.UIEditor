using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public partial class WidgetAttributesControl : ConnectedUserControl
    {
        public UIEditorWidget? SelectedWidget
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

        private IGauntletManager? GauntletManager { get; set; }

        private IWidgetManager? WidgetManager { get; set; }

        private UIEditorWidget? m_SelectedWidget;

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
                    SelectedWidget = WidgetManager!.CreateWidget(GauntletManager.UIContext, _selectedWidgetTemplate);
                }
            }
            else
            {
                SelectedWidget = null;
            }
        }

        private void OnWidgetListControlRegistered(IWidgetListControl _widgetListControl)
        {
            _widgetListControl.SelectedWidgetTemplateChanged += OnSelectedWidgetTemplateChanged;
        }

        private void OnWidgetListControlUnregistering(IWidgetListControl _widgetListControl)
        {
            _widgetListControl.SelectedWidgetTemplateChanged -= OnSelectedWidgetTemplateChanged;
        }
    }
}
