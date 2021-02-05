using System;
using System.Collections.ObjectModel;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Interaction logic for WidgetListControl.xaml
    /// </summary>
    public partial class WidgetListControl : ConnectedUserControl, IWidgetListControl
    {
        #region Properties

        public event EventHandler<IWidgetTemplate?>? SelectedWidgetTemplateChanged; 

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

        #endregion

        #region Fields

        private IWidgetManager? m_WidgetManager;
        private IWidgetTemplate? m_SelectedWidgetTemplate;

        #endregion

        #region Constructors

        public WidgetListControl()
        {
            WidgetTemplates = new ObservableCollection<IWidgetTemplate>();
            InitializeComponent();
        }

        #endregion

        #region ConnectedUserControl Members

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

        #endregion

        protected virtual void OnSelectedWidgetTemplateChanged(IWidgetTemplate? _selectedWidgetTemplate)
        {
            SelectedWidgetTemplateChanged?.Invoke(this, _selectedWidgetTemplate);
        }
    }
}
