using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Bannerlord.UIEditor.MainFrame.Resources
{
    public partial class LoadingControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), 
                typeof( bool ), 
                typeof( LoadingControl ),
                new UIPropertyMetadata(false, IsLoadingChangedCallback) );

        private static void IsLoadingChangedCallback(DependencyObject _d, DependencyPropertyChangedEventArgs _e)
        {
            LoadingControl instance = (LoadingControl)_d;
            instance.PathVisibility = (bool)_e.NewValue ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public Visibility PathVisibility
        {
            get => m_PathVisibility;
            set
            {
                if (m_PathVisibility != value)
                {
                    m_PathVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility m_PathVisibility = Visibility.Hidden;

        public LoadingControl()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
