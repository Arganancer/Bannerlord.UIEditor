using System.Diagnostics;
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
    /// Add multi select for items in the tree view.
    /// Add deleting items in the tree view.
    /// Set new dropped widgets as selected.
    /// </summary>
    public partial class SceneExplorerControl : ConnectedUserControl
    {
        public DrawableWidgetViewModel? RootWidget
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

        private DrawableWidgetViewModel? m_RootWidget;

        private DrawableWidgetViewModel? m_SelectedWidgetViewModel;

        private IFocusManager? m_FocusManager;
        private ICursorManager m_CursorManager = null!;
        private ISceneManager m_SceneManager = null!;

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
            m_SceneManager = PublicContainer.GetModule<ISceneManager>();
            m_SceneManager.RootWidgetChanged += (_, _rootWidget) => RootWidget = _rootWidget;

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
                m_CursorManager!.SetCursor(CursorIcon.InsertIcon);
            }
            else if (_e.Effects.HasFlag(DragDropEffects.Copy))
            {
                m_CursorManager!.SetCursor(CursorIcon.AddIcon);
            }
            else
            {
                m_CursorManager!.SetCursor(CursorIcon.NoIcon);
            }

            _e.Handled = true;
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusedItem)
        {
            //throw new NotImplementedException();
        }

        private DrawableWidgetViewModel CreateWidget(IWidgetTemplate _widgetTemplate)
        {
            return new(_widgetTemplate.Name, m_WidgetManager!.CreateWidget(m_GauntletManager!.UIContext!, _widgetTemplate), m_CanvasEditorControl!, PublicContainer);
        }

        private void WidgetItem_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( FocusableWidgetTemplate )) || _e.Data.GetDataPresent(nameof( WidgetViewModel )))
            {
                TreeItemGrid grid = (TreeItemGrid)_sender;
                var point = _e.GetPosition(grid);
                TreeViewItem target = grid.GetVisualAncestorOfType<TreeViewItem>()!;
                DrawableWidgetViewModel targetWidget = (target.DataContext as DrawableWidgetViewModel)!;

                if (_e.Data.GetData(nameof( WidgetViewModel )) is DrawableWidgetViewModel newWidget &&
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
                DrawableWidgetViewModel? parent;
                var targetIndex = int.MaxValue;
                bool removeViewModel = false;

                if (_e.Data.GetData(nameof( WidgetViewModel )) is not DrawableWidgetViewModel viewModel)
                {
                    FocusableWidgetTemplate focusableWidgetTemplate = (FocusableWidgetTemplate)_e.Data.GetData(nameof( FocusableWidgetTemplate ))!;
                    viewModel = CreateWidget(focusableWidgetTemplate.WidgetTemplate);
                }
                else
                {
                    removeViewModel = true;
                }

                if (point.Y <= grid.InsertDistanceTolerance + (grid.TopBorderHighlight ? 2 : 0) ||
                    point.Y >= grid.ActualHeight - grid.InsertDistanceTolerance - (grid.BottomBorderHighlight ? 2 : 0))
                {
                    DrawableWidgetViewModel sibling = (target.DataContext as DrawableWidgetViewModel)!;
                    if (sibling == RootWidget)
                    {
                        _e.Handled = true;
                        return;
                    }

                    TreeViewItem parentTarget = target.GetVisualAncestorOfType<TreeViewItem>()!;
                    parent = (parentTarget.DataContext as DrawableWidgetViewModel);

                    if (parent is not null)
                    {
                        targetIndex = parent.GetIndexOfChild(sibling);
                        if (point.Y >= grid.ActualHeight - grid.InsertDistanceTolerance)
                        {
                            targetIndex++;
                        }
                    }

                }
                else
                {
                    target.IsExpanded = true;
                    parent = (target.DataContext as DrawableWidgetViewModel);
                }

                if(parent is null)
                {
                    Trace.WriteLine("Parent was null during a drag and drop operation.");
                    return;
                }

                if(removeViewModel)
                {
                    RootWidget!.RemoveChildren(viewModel);
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
                m_SceneManager.RootWidget = CreateWidget(widgetTemplate.WidgetTemplate);
                Dispatcher.Invoke(() => SceneTreeView.Items.Add(RootWidget));
                _e.Handled = true;
            }
        }

        private void SceneTreeView_OnSelectedItemChanged(object _sender, RoutedPropertyChangedEventArgs<object> _e)
        {
            var widgetViewModel = _e.NewValue as DrawableWidgetViewModel;
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
