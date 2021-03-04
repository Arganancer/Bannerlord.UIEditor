using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent
{
    /// <summary>
    /// Interaction logic for FloatingPanelParentControl.xaml
    /// TODO: Implement MultiDock: https://stackoverflow.com/questions/2247402/implementing-a-multidock-window-system-like-blend-visual-studio-in-wpf
    /// </summary>
    public partial class FloatingPanelParentControl : UserControl, INotifyPropertyChanged, ILayoutElement
    {
        public Orientation Orientation
        {
            get => m_Orientation;
            set
            {
                if (m_Orientation != value)
                {
                    m_Orientation = value;
                    OnPropertyChanged();
                }
            }
        }

        public double DesiredWidth
        {
            get => m_DesiredWidth;
            set
            {
                m_DesiredWidth = value;
                OnDesiredWidthChanged(m_DesiredWidth);
            }
        }

        public double DesiredHeight
        {
            get => m_DesiredHeight;
            set
            {
                m_DesiredHeight = value;
                OnDesiredHeightChanged(m_DesiredHeight);
            }
        }

        public double TotalBorderWidth => ResizeableControl.TotalBorderWidth;
        public double TotalBorderHeight => ResizeableControl.TotalBorderHeight;

        public Control Control => this;
        public Dock CurrentDock => DockPanel.GetDock(this);

        private Orientation m_Orientation;
        private double m_DesiredWidth;
        private double m_DesiredHeight;

        public FloatingPanelParentControl()
        {
            InitializeComponent();
            ResizeableControl.LayoutElement = this;
        }

        public event EventHandler<double>? DesiredWidthChanged;
        public event EventHandler<double>? DesiredHeightChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void DiscreteSetDesiredSize(double _desiredWidth, double _desiredHeight)
        {
            m_DesiredWidth = _desiredWidth;
            m_DesiredHeight = _desiredHeight;
        }

        public void RefreshResizerBorders(params Dock[] _enabledSides)
        {
            ResizeableControl.AdjustBorders(_enabledSides);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private void OnDesiredWidthChanged(double _e)
        {
            DesiredWidthChanged?.Invoke(this, _e);
        }

        private void OnDesiredHeightChanged(double _e)
        {
            DesiredHeightChanged?.Invoke(this, _e);
        }
    }
}
