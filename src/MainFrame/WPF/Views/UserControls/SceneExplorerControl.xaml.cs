using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// TODO:
    /// AddIcon multi select for items in the tree view.
    /// AddIcon deleting items in the tree view.
    /// Allow reorganizing items in the tree view via drag and drop.
    /// Allow selecting current widget from code.
    /// Set new dropped widgets as selected.
    /// </summary>
    public partial class SceneExplorerControl : ConnectedUserControl
    {
        public WidgetViewModel? RootWidget
        {
            get => m_RootWidget;
            set
            {
                if (m_RootWidget != value)
                {
                    m_RootWidget = value;
                    OnPropertyChanged();
                }
            }
        }

        private IGauntletManager? m_GauntletManager;
        private IWidgetManager? m_WidgetManager;
        private ICanvasEditorControl? m_CanvasEditorControl;

        private WidgetViewModel? m_RootWidget;

        private WidgetViewModel? m_SelectedWidgetViewModel;

        private IFocusManager? m_FocusManager;
        private ICursorManager m_CursorManager;

        public SceneExplorerControl()
        {
            InitializeComponent();
        }

        public override void Load()
        {
            base.Load();
            m_WidgetManager = PublicContainer.GetModule<IWidgetManager>();
            m_GauntletManager = PublicContainer.GetModule<IGauntletManager>();
            m_CanvasEditorControl = PublicContainer.GetModule<ICanvasEditorControl>();
            PublicContainer.ConnectToModule<IFocusManager>(this,
                _focusManager =>
                {
                    m_FocusManager = _focusManager;
                    m_FocusManager.FocusChanged += OnFocusChanged;
                },
                _ =>
                {
                    m_FocusManager!.FocusChanged -= OnFocusChanged;
                    m_FocusManager = null;
                });

            m_CursorManager = PublicContainer.GetModule<ICursorManager>();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs _e)
        {
            base.OnGiveFeedback(_e);

            if (_e.Effects.HasFlag(DragDropEffects.Move))
            {
                m_CursorManager.SetCursor(CursorIcon.InsertIcon);
            }
            else if (_e.Effects.HasFlag(DragDropEffects.Copy))
            {
                m_CursorManager.SetCursor(CursorIcon.AddIcon);
            }
            else
            {
                m_CursorManager.SetCursor(CursorIcon.NoIcon);
            }

            _e.Handled = true;
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusedItem)
        {
            //throw new NotImplementedException();
        }

        private WidgetViewModel CreateWidget(IWidgetTemplate _widgetTemplate)
        {
            return new(_widgetTemplate.Name, m_WidgetManager!.CreateWidget(m_GauntletManager!.UIContext!, _widgetTemplate), m_CanvasEditorControl!, PublicContainer) {IsReadonly = false};
        }

        private void WidgetItem_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( FocusableWidgetTemplate )) || _e.Data.GetDataPresent(nameof( WidgetViewModel )))
            {
                TreeItemGrid grid = (TreeItemGrid)_sender;
                var point = _e.GetPosition(grid);
                TreeViewItem target = grid.GetVisualAncestorOfType<TreeViewItem>()!;
                WidgetViewModel targetWidget = (target.DataContext as WidgetViewModel)!;

                if (_e.Data.GetData(nameof( WidgetViewModel )) is WidgetViewModel newWidget &&
                    (targetWidget.Equals(newWidget) || targetWidget.WidgetExistsInParents(newWidget)))
                {
                    grid.SetNoBorderHighlights();
                    _e.Effects = DragDropEffects.None;
                }
                else if (point.Y <= grid.InsertDistanceTolerance + (grid.TopBorderHighlight ? 2 : 0) ||
                         point.Y >= grid.ActualHeight - grid.InsertDistanceTolerance - (grid.BottomBorderHighlight ? 2 : 0))
                {
                    if (targetWidget == RootWidget || point.Y > grid.ActualHeight)
                    {
                        grid.SetNoBorderHighlights();
                        _e.Effects = DragDropEffects.None;
                    }
                    else
                    {
                        if (point.Y <= grid.InsertDistanceTolerance + (grid.TopBorderHighlight ? 2 : 0))
                        {
                            grid.TopBorderHighlight = true;
                        }
                        else
                        {
                            grid.BottomBorderHighlight = true;
                        }

                        _e.Effects = DragDropEffects.Move;
                    }
                }
                else
                {
                    grid.SetNoBorderHighlights();
                    _e.Effects = DragDropEffects.Copy;
                }

                _e.Handled = true;
            }
            else
            {
                _e.Effects = DragDropEffects.None;
            }
        }

        private void WidgetItem_OnDrop(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( FocusableWidgetTemplate )) || _e.Data.GetDataPresent(nameof( WidgetViewModel )))
            {
                TreeItemGrid grid = (TreeItemGrid)_sender;
                var point = _e.GetPosition(grid);
                TreeViewItem target = grid.GetVisualAncestorOfType<TreeViewItem>()!;
                WidgetViewModel parent;
                var targetIndex = int.MaxValue;

                if (_e.Data.GetData(nameof( WidgetViewModel )) is not WidgetViewModel viewModel)
                {
                    FocusableWidgetTemplate focusableWidgetTemplate = (FocusableWidgetTemplate)_e.Data.GetData(nameof( FocusableWidgetTemplate ))!;
                    viewModel = CreateWidget(focusableWidgetTemplate.WidgetTemplate);
                }
                else
                {
                    RootWidget!.RemoveChildren(viewModel);
                }

                if (point.Y <= grid.InsertDistanceTolerance + (grid.TopBorderHighlight ? 2 : 0) ||
                    point.Y >= grid.ActualHeight - grid.InsertDistanceTolerance - (grid.BottomBorderHighlight ? 2 : 0))
                {
                    WidgetViewModel sibling = (target.DataContext as WidgetViewModel)!;
                    if (sibling == RootWidget)
                    {
                        _e.Handled = true;
                        return;
                    }

                    TreeViewItem parentTarget = target.GetVisualAncestorOfType<TreeViewItem>()!;
                    parent = (parentTarget.DataContext as WidgetViewModel)!;

                    targetIndex = parent.GetIndexOfChild(sibling);
                    if (point.Y >= grid.ActualHeight - grid.InsertDistanceTolerance)
                    {
                        targetIndex++;
                    }
                }
                else
                {
                    target.IsExpanded = true;
                    parent = (target.DataContext as WidgetViewModel)!;
                }

                parent.AddChildren(targetIndex, viewModel);
                _e.Handled = true;
            }
        }

        private void SceneTreeView_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( FocusableWidgetTemplate )))
            {
                _e.Effects = RootWidget is null ? DragDropEffects.Copy : DragDropEffects.None;
                _e.Handled = true;
            }
            else
            {
                _e.Effects = DragDropEffects.None;
            }
        }

        private void SceneTreeView_OnDrop(object _sender, DragEventArgs _e)
        {
            if (RootWidget is null && _e.Data.GetDataPresent(nameof( FocusableWidgetTemplate )))
            {
                FocusableWidgetTemplate widgetTemplate = (FocusableWidgetTemplate)_e.Data.GetData(nameof( FocusableWidgetTemplate ))!;
                RootWidget = CreateWidget(widgetTemplate.WidgetTemplate);
                Dispatcher.Invoke(() => SceneTreeView.Items.Add(RootWidget));
                _e.Handled = true;
            }
        }

        private void SceneTreeView_OnSelectedItemChanged(object _sender, RoutedPropertyChangedEventArgs<object> _e)
        {
            var widgetViewModel = _e.NewValue as WidgetViewModel;
            m_SelectedWidgetViewModel = widgetViewModel;
            m_FocusManager?.SetFocus(m_SelectedWidgetViewModel);
        }

        private void WidgetItem_OnMouseMove(object _sender, MouseEventArgs _e)
        {
            if (_e.LeftButton == MouseButtonState.Pressed && m_SelectedWidgetViewModel is not null)
            {
                DataObject data = new();
                data.SetData(nameof( WidgetViewModel ), m_SelectedWidgetViewModel);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
