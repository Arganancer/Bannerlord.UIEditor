using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame
{
    public partial class LoadingControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof( bool ), typeof( LoadingControl ), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);
                PathVisibility = value ? Visibility.Visible : Visibility.Hidden;
            }
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

        private Visibility m_PathVisibility = Visibility.Visible;

        public LoadingControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
