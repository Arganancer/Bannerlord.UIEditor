using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl : INotifyPropertyChanged, ILayoutElement
    {
        public Control Control => this;
        public Dock CurrentDock => DockPanel.GetDock(this);
        public Orientation Orientation { get; set; }

        public string HeaderName
        {
            get => m_HeaderName;
            set
            {
                if (m_HeaderName != value)
                {
                    m_HeaderName = value;
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

        private string m_HeaderName = "";
        private double m_DesiredWidth;
        private double m_DesiredHeight;

        public PanelControl()
        {
            InitializeComponent();
            ResizableControl.LayoutElement = this;
        }

        public event EventHandler<double>? DesiredWidthChanged;
        public event EventHandler<double>? DesiredHeightChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void DiscreteSetDesiredSize(double _desiredWidth, double _desiredHeight)
        {
            m_DesiredWidth = _desiredWidth;
            m_DesiredHeight = _desiredHeight;
        }

        public void RefreshResizerBorders()
        {
            ResizableControl.RefreshResizerBorders();
        }

        public void AddPanelContent(IPanel _panelContentControl, bool _setSelected)
        {
            TabItem tabItem = new() {Header = _panelContentControl.PanelName, Content = _panelContentControl};
            TabControl.Items.Add(tabItem);
            tabItem.IsSelected = _setSelected;

            UpdateTabVisibility();
        }

        private void OnDesiredWidthChanged(double _e)
        {
            DesiredWidthChanged?.Invoke(this, _e);
        }

        private void OnDesiredHeightChanged(double _e)
        {
            DesiredHeightChanged?.Invoke(this, _e);
        }

        private void RemovePanelContent(IPanel _panelContentControl)
        {
            var tabItem = TabControl.Items.OfType<TabItem>().FirstOrDefault(_t => Equals(_t.Content, _panelContentControl));
            if (tabItem is null)
            {
                return;
            }

            TabControl.Items.Remove(tabItem);

            var firstTabItem = TabControl.Items.OfType<TabItem>().FirstOrDefault();
            if (firstTabItem is not null)
            {
                firstTabItem.IsSelected = true;
            }

            UpdateTabVisibility();
        }

        private void UpdateTabVisibility()
        {
            Visibility visibility = TabControl.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            foreach (TabItem child in TabControl.Items.OfType<TabItem>())
            {
                child.Visibility = visibility;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private void TabControl_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            if (_e.Source is TabControl tabControl)
            {
                if (tabControl.SelectedItem is TabItem selectedTabItem)
                {
                    HeaderName = selectedTabItem.Header as string ?? string.Empty;
                    _e.Handled = true;
                }
            }
        }

        private void HeaderPanel_OnMouseMove(object _sender, MouseEventArgs _e)
        {
            if (_e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new();
                data.SetData(typeof(ILayoutElement), this);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            if(e.Effects.HasFlag(DragDropEffects.Move))
            {
                CursorManager.Instance!.SetCursor(CursorIcon.InsertIcon);
            }
        }
    }
}
