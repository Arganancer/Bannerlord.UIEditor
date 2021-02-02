using System;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IModule
    {
        #region Properties

        public bool Disposed { get; private set; }

        private IPublicContainer PublicContainer { get; set; } = null!;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
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
        }

        public void Unload()
        {
        }

        #endregion

        #region Protected Methods

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        #endregion
    }
}
