using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Bannerlord.UIEditor.MainFrame
{
    public partial class LoadingControl : INotifyPropertyChanged
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
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }
    }
}
