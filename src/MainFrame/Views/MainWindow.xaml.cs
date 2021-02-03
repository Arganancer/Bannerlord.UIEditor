using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IModule, INotifyPropertyChanged
    {
        #region Properties

        public bool Disposed { get; private set; }

        public ObservableCollection<IWidgetTemplate> WidgetTemplates { get; }

        public IWidgetTemplate? SelectedWidgetTemplate
        {
            get => m_SelectedWidgetTemplate;
            set
            {
                if (m_SelectedWidgetTemplate != value)
                {
                    m_SelectedWidgetTemplate = value;
                    if (m_SelectedWidgetTemplate is not null)
                    {
                        if (GauntletManager?.UIContext is not null)
                        {
                            SelectedWidget = m_WidgetManager!.CreateWidget(GauntletManager.UIContext, m_SelectedWidgetTemplate);
                        }
                    }
                    else
                    {
                        SelectedWidget = null;
                    }

                    OnPropertyChanged();
                }
            }
        }

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

        private IPublicContainer PublicContainer { get; set; } = null!;

        private IWidgetManager? WidgetManager
        {
            get => m_WidgetManager;
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

        private IGauntletManager? GauntletManager { get; set; }

        #endregion

        #region Fields

        private IWidgetManager? m_WidgetManager;
        private IWidgetTemplate? m_SelectedWidgetTemplate;
        private UIEditorWidget? m_SelectedWidget;

        #endregion

        #region Constructors

        public MainWindow()
        {
            WidgetTemplates = new ObservableCollection<IWidgetTemplate>();
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region IConnectedObject Members

        public event EventHandler<IConnectedObject>? Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!Disposed)
            {
                OnDisposing();
            }

            Disposed = true;
        }

        #endregion

        #region IModule Members

        public void Create(IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;
        }

        public void Load()
        {
            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);

            PublicContainer.ConnectToModule<IGauntletManager>(this,
                _gauntletManager => GauntletManager = _gauntletManager,
                _ => GauntletManager = null);
        }

        public void Unload()
        {
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Protected Methods

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        #endregion
    }
}
