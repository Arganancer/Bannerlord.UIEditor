using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.MainFrame.Resources.Panel;

namespace Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent
{
    public delegate void NewParentRequested(FloatingPanelParentControl _newParent, PanelControl _panelElement, Dock _dock);

    /// <summary>
    /// Interaction logic for FloatingPanelParentControl.xaml
    /// TODO: Implement MultiDock: https://stackoverflow.com/questions/2247402/implementing-a-multidock-window-system-like-blend-visual-studio-in-wpf
    /// </summary>
    public partial class FloatingPanelParentControl : UserControl, INotifyPropertyChanged, ILayoutElement
    {
        public event NewParentRequested? NewParentRequested;

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

        public void RefreshResizerBorders(bool _disableAll = false)
        {
            ResizeableControl.RefreshResizerBorders(_disableAll);
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

        private void ContentStackPanel_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(typeof( ILayoutElement )))
            {
                _e.Effects = DragDropEffects.Move;
                _e.Handled = true;
            }
        }

        private void ContentStackPanel_OnDragLeave(object _sender, DragEventArgs _e)
        {
        }

        private void ContentStackPanel_OnDrop(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(typeof( PanelControl )))
            {
                _e.Effects = DragDropEffects.Move;
                OnNewParentRequested(this, (PanelControl)_e.Data.GetData(typeof( PanelControl ))!, Dock.Left);
                _e.Handled = true;
            }
        }

        private void OnNewParentRequested(FloatingPanelParentControl _newParent, PanelControl _panelElement, Dock _dock)
        {
            NewParentRequested?.Invoke(_newParent, _panelElement, _dock);
        }
    }
}
