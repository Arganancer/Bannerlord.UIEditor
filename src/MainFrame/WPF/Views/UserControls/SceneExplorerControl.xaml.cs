using System;
using System.Windows;
using System.Windows.Controls;
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
    /// </summary>
    public partial class SceneExplorerControl : ConnectedUserControl, ISceneExplorerControl
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

        public SceneExplorerControl()
        {
            InitializeComponent();
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            PublicContainer.RegisterModule<ISceneExplorerControl>(this);
        }

        public override void Load()
        {
            base.Load();
            m_WidgetManager = PublicContainer.GetModule<IWidgetManager>();
            m_GauntletManager = PublicContainer.GetModule<IGauntletManager>();
            m_CanvasEditorControl = PublicContainer.GetModule<ICanvasEditorControl>();
        }

        private WidgetViewModel CreateWidget(IWidgetTemplate _widgetTemplate)
        {
            return new(_widgetTemplate.Name, m_WidgetManager!.CreateWidget(m_GauntletManager!.UIContext!, _widgetTemplate), m_CanvasEditorControl!);
        }

        private void WidgetItem_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( IWidgetTemplate )))
            {
                StackPanel stackPanel = (StackPanel)_sender;
                var point = _e.GetPosition(stackPanel);
                if (point.Y <= 3 || point.Y >= stackPanel.ActualHeight - 3)
                {
                    TreeViewItem target = ((StackPanel)_sender).GetVisualAncestorOfType<TreeViewItem>()!;
                    WidgetViewModel sibling = (target.DataContext as WidgetViewModel)!;
                    if (sibling == RootWidget || point.Y > stackPanel.ActualHeight)
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
            if (_e.Data.GetDataPresent(nameof( IWidgetTemplate )))
            {
                StackPanel stackPanel = (StackPanel)_sender;
                var point = _e.GetPosition(stackPanel);
                TreeViewItem target = ((StackPanel)_sender).GetVisualAncestorOfType<TreeViewItem>()!;
                WidgetViewModel parent;
                IWidgetTemplate widgetTemplate = (IWidgetTemplate)_e.Data.GetData(nameof(IWidgetTemplate))!;
                int targetIndex = int.MaxValue;
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
                
                parent.AddChildren(targetIndex, CreateWidget(widgetTemplate));
                _e.Handled = true;
            }
        }

        private void SceneTreeView_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( IWidgetTemplate )))
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
            if (RootWidget is null && _e.Data.GetDataPresent(nameof( IWidgetTemplate )))
            {
                IWidgetTemplate widgetTemplate = (IWidgetTemplate)_e.Data.GetData(nameof( IWidgetTemplate ))!;
                RootWidget = CreateWidget(widgetTemplate);
                Dispatcher.Invoke(() => SceneTreeView.Items.Add(RootWidget));
                _e.Handled = true;
            }
        }
    }
}
