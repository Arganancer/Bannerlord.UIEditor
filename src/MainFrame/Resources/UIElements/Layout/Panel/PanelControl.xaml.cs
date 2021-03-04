using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame.Resources.Panel
{
    public delegate void MovePanel(PanelControl _target, PanelControl _origin, IPanel? _panel, DropTarget _dropTarget, int _targetIndex = -1);

    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl : ILayoutElement
    {
        public event MovePanel? MovePanel;
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

        public bool IsLayoutDragDropEnabled
        {
            get => m_IsLayoutDragDropEnabled;
            set
            {
                m_IsLayoutDragDropEnabled = value;
                OnPropertyChanged();
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
        private bool m_IsLayoutDragDropEnabled;
        private ILayoutDragManager m_LayoutDragManager = null!;

        public PanelControl()
        {
            InitializeComponent();
            ResizableControl.LayoutElement = this;
            Loaded += OnLoaded;
        }

        public event EventHandler<double>? DesiredWidthChanged;
        public event EventHandler<double>? DesiredHeightChanged;

        public void DiscreteSetDesiredSize(double _desiredWidth, double _desiredHeight)
        {
            m_DesiredWidth = _desiredWidth;
            m_DesiredHeight = _desiredHeight;
        }

        public void RefreshResizerBorders(params Dock[] _enabledSides)
        {
            ResizableControl.AdjustBorders(_enabledSides);
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            DropTargetLeftButton.DragOver += (_sender, _args) => DropTargetButton_OnDragOver((Button)_sender, _args, DropTarget.Left);
            DropTargetRightButton.DragOver += (_sender, _args) => DropTargetButton_OnDragOver((Button)_sender, _args, DropTarget.Right);
            DropTargetTopButton.DragOver += (_sender, _args) => DropTargetButton_OnDragOver((Button)_sender, _args, DropTarget.Top);
            DropTargetBottomButton.DragOver += (_sender, _args) => DropTargetButton_OnDragOver((Button)_sender, _args, DropTarget.Bottom);
            DropTargetCenterButton.DragOver += (_sender, _args) => DropTargetButton_OnDragOver((Button)_sender, _args, DropTarget.Center);

            DropTargetLeftButton.Drop += (_sender, _args) => DropTargetButton_OnDrop((Button)_sender, _args, DropTarget.Left);
            DropTargetRightButton.Drop += (_sender, _args) => DropTargetButton_OnDrop((Button)_sender, _args, DropTarget.Right);
            DropTargetTopButton.Drop += (_sender, _args) => DropTargetButton_OnDrop((Button)_sender, _args, DropTarget.Top);
            DropTargetBottomButton.Drop += (_sender, _args) => DropTargetButton_OnDrop((Button)_sender, _args, DropTarget.Bottom);
            DropTargetCenterButton.Drop += (_sender, _args) => DropTargetButton_OnDrop((Button)_sender, _args, DropTarget.Center);
        }

        public override void Load()
        {
            base.Load();

            m_LayoutDragManager = PublicContainer.GetModule<ILayoutDragManager>();
            m_LayoutDragManager.LayoutDragDropEnabledChanged += OnLayoutDragDropEnabledChanged;
            OnLayoutDragDropEnabledChanged(m_LayoutDragManager.IsPanelBeingDragged);
        }

        public override void Unload()
        {
            m_LayoutDragManager.LayoutDragDropEnabledChanged -= OnLayoutDragDropEnabledChanged;
            base.Unload();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                //CursorManager.Instance!.SetCursor(CursorIcon.InsertIcon);
            }
        }

        public void AddPanelContent(IPanel _panelContentControl, bool _setSelected, int _targetIndex)
        {
            TabItem tabItem = new() {Header = _panelContentControl.PanelName, Content = _panelContentControl};
            if(_targetIndex >= 0)
            {
                TabControl.Items.Insert(_targetIndex, tabItem);
            }
            else
            {
                TabControl.Items.Add(tabItem);
            }
            tabItem.IsSelected = _setSelected;

            UpdateTabVisibility();
        }

        public void RemovePanelContent(IPanel _panelContentControl)
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

        protected virtual void OnMovePanel(PanelControl _target, PanelControl _origin, IPanel? _panel, DropTarget _dropTarget, int _targetIndex = -1)
        {
            MovePanel?.Invoke(_target, _origin, _panel, _dropTarget, _targetIndex);
        }

        private void OnLayoutDragDropEnabledChanged(bool _isDragDropEnabled)
        {
            IsLayoutDragDropEnabled = _isDragDropEnabled;
        }

        private void OnDesiredWidthChanged(double _e)
        {
            DesiredWidthChanged?.Invoke(this, _e);
        }

        private void OnDesiredHeightChanged(double _e)
        {
            DesiredHeightChanged?.Invoke(this, _e);
        }

        private void UpdateTabVisibility()
        {
            if (!IsLoaded)
            {
                return;
            }

            var visibility = TabControl.Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            foreach (TabItem child in TabControl.Items.OfType<TabItem>())
            {
                child.Visibility = visibility;
            }
        }

        private void OnLoaded(object _sender, RoutedEventArgs _e)
        {
            ResizableControl.LayoutElement = this;
            UpdateTabVisibility();
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
                data.SetData(typeof( PanelControl ), this);
                m_LayoutDragManager.StartDragging();
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                m_LayoutDragManager.StopDragging();
            }
        }

        private void DropTargetContainer_OnDragOver(object _sender, DragEventArgs _e)
        {
            DropTargetButton_OnDragOver(DropTargetCenterButton, _e, DropTarget.Center);
        }

        private void DropTargetContainer_OnDrop(object _sender, DragEventArgs _e)
        {
            if(_e.Data.GetData(typeof(PanelControl)) is PanelControl panelControl)
            {
                if (DragEventIsHandledByTabs(out TabItem? targetTabItem))
                {
                    PerformTabItemDrop(targetTabItem!, panelControl, _e);
                }
                else
                {
                    DropTargetButton_OnDrop(DropTargetCenterButton, _e, DropTarget.Center);
                }
            }
        }

        private void DropTargetButton_OnDragOver(Button _sender, DragEventArgs _e, DropTarget _dropTarget)
        {
            if (!_e.Data.GetDataPresent(typeof( PanelControl )))
            {
                _e.Effects = DragDropEffects.None;
                return;
            }

            if (Equals(_e.Data.GetData(typeof( PanelControl )), this))
            {
                return;
            }

            if (_e.Handled)
            {
                return;
            }

            DropTargetButtons.Visibility = Visibility.Visible;
            DropTargetPreviewViewbox.Visibility = Visibility.Visible;

            switch (_dropTarget)
            {
                case DropTarget.Left:
                    DropTargetPreviewViewbox.Margin = new Thickness(0, 0, ActualWidth * 0.5, 0);
                    break;
                case DropTarget.Right:
                    DropTargetPreviewViewbox.Margin = new Thickness(ActualWidth * 0.5, 0, 0, 0);
                    break;
                case DropTarget.Top:
                    DropTargetPreviewViewbox.Margin = new Thickness(0, 0, 0, ActualHeight * 0.5);
                    break;
                case DropTarget.Bottom:
                    DropTargetPreviewViewbox.Margin = new Thickness(0, ActualHeight * 0.5, 0, 0);
                    break;
                case DropTarget.Center:
                    DropTargetPreviewViewbox.Margin = new Thickness(0, 0, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof( _dropTarget ), _dropTarget, null);
            }

            _e.Effects = DragDropEffects.Move;
            _e.Handled = true;
        }

        private void DropTargetButton_OnDrop(Button _sender, DragEventArgs _e, DropTarget _dropTarget)
        {
            if (_e.Handled || !_e.Data.GetDataPresent(typeof( PanelControl )))
            {
                return;
            }

            IPanel? panel = _e.Data.GetDataPresent(typeof( IPanel )) ? (IPanel)_e.Data.GetData(typeof( IPanel ))! : null;

            if (Equals(_e.Data.GetData(typeof( PanelControl )), this))
            {
                return;
            }

            PanelControl panelControl = (PanelControl)_e.Data.GetData(typeof( PanelControl ))!;
            OnMovePanel(this, panelControl, panel, _dropTarget);
            _e.Effects = DragDropEffects.Move;
            _e.Handled = true;
            DropTargetButtons.Visibility = Visibility.Hidden;
            DropTargetPreviewViewbox.Visibility = Visibility.Hidden;
        }

        private void DropTarget_OnDragLeave(object _sender, DragEventArgs _e)
        {
            if (_e.Handled)
            {
                return;
            }

            if (_e.Source.Equals(DropTargetContainer))
            {
                if (!CursorHelper.IsCursorInside(DropTargetContainer))
                {
                    DropTargetButtons.Visibility = Visibility.Hidden;
                    DropTargetPreviewViewbox.Visibility = Visibility.Hidden;
                    _e.Handled = true;
                }
            }
        }

        private bool DragEventIsHandledByTabs(out TabItem? _tabItem)
        {
            TabItem[] tabItems = TabControl.Items.OfType<TabItem>().ToArray();
            if (tabItems.Length > 1)
            {
                foreach (TabItem? tabItem in tabItems)
                {
                    if(CursorHelper.IsCursorInside(tabItem))
                    {
                        _tabItem = tabItem;
                        return true;
                    }
                }
            }

            _tabItem = null;
            return false;
        }

        private void TabItem_PreviewMouseMove(object _sender, MouseEventArgs _e)
        {
            if (_e.Source is TabItem tabItemTarget && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new();
                data.SetData(typeof( PanelControl ), this);
                data.SetData(typeof( IPanel ), tabItemTarget.Content);
                m_LayoutDragManager.StartDragging();
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                m_LayoutDragManager.StopDragging();
            }
        }

        private void DropTargetTabItem_OnDrop(object _sender, DragEventArgs _e)
        {
            if (_e.Source is TabItem targetTabItem && _e.Data.GetData(typeof(PanelControl)) is PanelControl sourcePanelControl)
            {
                PerformTabItemDrop(targetTabItem, sourcePanelControl, _e);
            }
        }

        private void PerformTabItemDrop(TabItem _targetTabItem, PanelControl _sourcePanelControl, DragEventArgs _e)
        {
            if (_targetTabItem.Parent is TabControl newParent)
            {
                if (_e.Data.GetData(typeof(IPanel)) is IPanel panel)
                {
                    TabItem tabItemSource = _sourcePanelControl.TabControl.Items.OfType<TabItem>().FirstOrDefault(_x => _x.Content.Equals(panel))!;
                    if (_targetTabItem.Equals(tabItemSource))
                    {
                        return;
                    }

                    var targetIndex = newParent.Items.IndexOf(_targetTabItem);

                    if (_targetTabItem.Parent.Equals(tabItemSource.Parent))
                    {
                        newParent.Items.Remove(tabItemSource);
                        newParent.Items.Insert(targetIndex, tabItemSource);
                        tabItemSource.IsSelected = true;
                    }
                    else
                    {
                        OnMovePanel(this, _sourcePanelControl, panel, DropTarget.Center, targetIndex);
                    }

                    _e.Handled = true;
                }
            }
        }
    }
}
