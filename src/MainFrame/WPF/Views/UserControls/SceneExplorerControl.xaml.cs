using System;
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

        public SceneExplorerControl()
        {
            InitializeComponent();
        }

        private IFocusManager? m_FocusManager;

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
        }

        private void OnFocusChanged(object _sender, IFocusable? _focusedItem)
        {
            //throw new NotImplementedException();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs _e)
        {
            base.OnGiveFeedback(_e);

            Cursor cursor = Cursors.No;
            if (_e.Effects.HasFlag(DragDropEffects.Move))
            {
                cursor = Cursors.SizeWE;
            }
            else if (_e.Effects.HasFlag(DragDropEffects.Copy))
            {
                cursor = Cursors.Cross;
            }

            Mouse.SetCursor(cursor);

            _e.Handled = true;
        }

        private WidgetViewModel CreateWidget(IWidgetTemplate _widgetTemplate)
        {
            return new(_widgetTemplate.Name, m_WidgetManager!.CreateWidget(m_GauntletManager!.UIContext!, _widgetTemplate), m_CanvasEditorControl!, PublicContainer) {IsReadonly = false};
        }

        private void WidgetItem_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof(FocusableWidgetTemplate)) || _e.Data.GetDataPresent(nameof(WidgetViewModel)))
            {
                StackPanel stackPanel = (StackPanel)_sender;
                var point = _e.GetPosition(stackPanel);
                TreeViewItem target = ((StackPanel)_sender).GetVisualAncestorOfType<TreeViewItem>()!;
                WidgetViewModel targetWidget = (target.DataContext as WidgetViewModel)!;

                if (_e.Data.GetData(nameof( WidgetViewModel )) is WidgetViewModel newWidget &&
                    (targetWidget.Equals(newWidget) || targetWidget.WidgetExistsInParents(newWidget)))
                {
                    _e.Effects = DragDropEffects.None;
                }
                else if (point.Y <= 3 || point.Y >= stackPanel.ActualHeight - 3)
                {
                    if (targetWidget == RootWidget || point.Y > stackPanel.ActualHeight)
                    {
                        _e.Effects = DragDropEffects.None;
                    }
                    else
                    {
                        _e.Effects = DragDropEffects.Move;
                    }
                }
                else
                {
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
            if (_e.Data.GetDataPresent(nameof(FocusableWidgetTemplate)) || _e.Data.GetDataPresent(nameof(WidgetViewModel)))
            {
                StackPanel stackPanel = (StackPanel)_sender;
                var point = _e.GetPosition(stackPanel);
                TreeViewItem target = ((StackPanel)_sender).GetVisualAncestorOfType<TreeViewItem>()!;
                WidgetViewModel parent;
                var targetIndex = int.MaxValue;

                if (_e.Data.GetData(nameof(WidgetViewModel)) is not WidgetViewModel viewModel)
                {
                    FocusableWidgetTemplate focusableWidgetTemplate = (FocusableWidgetTemplate)_e.Data.GetData(nameof(FocusableWidgetTemplate))!;
                    viewModel = CreateWidget(focusableWidgetTemplate.WidgetTemplate);
                }
                else
                {
                    RootWidget!.RemoveChildren(viewModel);
                }

                if (point.Y <= 3 || point.Y >= stackPanel.ActualHeight - 3)
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
                    if (point.Y >= stackPanel.ActualHeight - 3)
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
            if (_e.Data.GetDataPresent(nameof(FocusableWidgetTemplate)))
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
            if (RootWidget is null && _e.Data.GetDataPresent(nameof(FocusableWidgetTemplate)))
            {
                FocusableWidgetTemplate widgetTemplate = (FocusableWidgetTemplate)_e.Data.GetData(nameof(FocusableWidgetTemplate))!;
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
                data.SetData(nameof(WidgetViewModel), m_SelectedWidgetViewModel);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
