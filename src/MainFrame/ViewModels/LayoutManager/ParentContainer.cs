using System;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent;
using Bannerlord.UIEditor.MainFrame.Resources.Panel;

namespace Bannerlord.UIEditor.MainFrame
{
    public class ParentContainer : LayoutContainer<FloatingPanelParentControl>
    {
        public LayoutContainer? PrimaryChild
        {
            get => m_PrimaryChild;
            set
            {
                if (m_PrimaryChild != value)
                {
                    if (m_PrimaryChild is not null)
                    {
                        m_PrimaryChild.Parent = null;
                    }

                    m_PrimaryChild = value;
                    if (m_PrimaryChild is not null)
                    {
                        m_PrimaryChild.Parent = this;
                    }
                }
            }
        }

        public LayoutContainer? SecondaryChild
        {
            get => m_SecondaryChild;
            set
            {
                if (m_SecondaryChild != value)
                {
                    if (m_SecondaryChild is not null)
                    {
                        m_SecondaryChild.Parent = null;
                    }

                    m_SecondaryChild = value;
                    if (m_SecondaryChild is not null)
                    {
                        m_SecondaryChild.Parent = this;
                    }
                }
            }
        }

        public Orientation Orientation
        {
            get => m_Orientation;
            set
            {
                m_Orientation = value;
                if (PrimaryChild is not null)
                {
                    PrimaryChild.Dock = Orientation == Orientation.Horizontal ? Dock.Left : Dock.Top;
                }

                if (SecondaryChild is not null)
                {
                    SecondaryChild.Dock = Orientation == Orientation.Horizontal ? Dock.Right : Dock.Bottom;
                }
            }
        }

        private Orientation m_Orientation;
        private LayoutContainer? m_PrimaryChild;
        private LayoutContainer? m_SecondaryChild;

        public ParentContainer(FloatingPanelParentControl _layoutElement) : base(_layoutElement)
        {
            _layoutElement.NewParentRequested += OnNewParentRequested;
        }

        public override T? FindContainer<T>(Control _control) where T : class
        {
            if (Equals(_control, Control))
            {
                return this as T;
            }

            return PrimaryChild?.FindContainer<T>(_control) ?? SecondaryChild?.FindContainer<T>(_control);
        }

        public override void Refresh()
        {
            if (PrimaryChild is null && SecondaryChild is null)
            {
                LayoutElement.DesiredWidth = 0;
                LayoutElement.DesiredHeight = 0;
                LayoutElement.RefreshResizerBorders(true);
                return;
            }
            LayoutElement.RefreshResizerBorders();
            PrimaryChild?.Refresh();
            SecondaryChild?.Refresh();
        }

        public override void Dispose()
        {
        }

        public void InsertPanel(
            string[] _path,
            double _desiredWidth,
            double _desiredHeight,
            bool _isSelected,
            IPanel _panel)
        {
            string[] containerParams = _path[0].Split(':');
            var dock = (Dock)Enum.Parse(typeof( Dock ), containerParams[1]);
            var nextContainer = dock == Dock.Left || dock == Dock.Top ? PrimaryChild : SecondaryChild;

            Orientation = containerParams[0] switch
            {
                "HorizontalLayoutContainer" => Orientation.Horizontal,
                "VerticalLayoutContainer" => Orientation.Vertical
            };

            if (nextContainer is null)
            {
                if (_path.Length > 1)
                {
                    nextContainer = new ParentContainer(new FloatingPanelParentControl()) {Dock = dock};
                }
                else
                {
                    PanelControl panelControl = new();
                    nextContainer = new PanelTabContainer(panelControl) {Dock = dock};
                }

                AddChild(nextContainer, dock);
            }

            if (nextContainer is PanelTabContainer stackContainer)
            {
                stackContainer.AddPanel(_panel, _isSelected);
                stackContainer.LayoutElement.DiscreteSetDesiredSize(_desiredWidth, _desiredHeight);
            }
            else if (nextContainer is ParentContainer parentContainer)
            {
                parentContainer.InsertPanel(_path.SubArray(1), _desiredWidth, _desiredHeight, _isSelected, _panel);
            }
        }

        public void AddChild(LayoutContainer _container, Dock _dock)
        {
            if (_dock == Dock.Left || _dock == Dock.Top)
            {
                if(PrimaryChild is null)
                {
                    PrimaryChild = _container;
                    Control.ContentStackPanel.Children.Insert(0, _container.LayoutElement.Control);
                }
                else if(PrimaryChild is PanelTabContainer panelTabContainer)
                {
                    if(_container is PanelTabContainer newPanelTabContainer)
                    {
                        panelTabContainer.MergeInto(newPanelTabContainer);
                    }
                }
                else if(PrimaryChild is ParentContainer parentContainer)
                {
                    parentContainer.AddChild(_container, _dock);
                }
            }
            else
            {
                if(SecondaryChild is null)
                {
                    SecondaryChild = _container;
                    Control.ContentStackPanel.Children.Add(_container.LayoutElement.Control);
                }
                else if (SecondaryChild is PanelTabContainer panelTabContainer)
                {
                    if (_container is PanelTabContainer newPanelTabContainer)
                    {
                        panelTabContainer.MergeInto(newPanelTabContainer);
                    }
                }
                else if (SecondaryChild is ParentContainer parentContainer)
                {
                    parentContainer.AddChild(_container, _dock);
                }
            }
        }

        public void FitWidthToChildren()
        {
            FloatingPanelParentControl floatingPanelParentControl = (FloatingPanelParentControl)LayoutElement.Control;
            var childWidth = Orientation == Orientation.Horizontal
                ? (PrimaryChild?.LayoutElement.DesiredWidth ?? 0) + (SecondaryChild?.LayoutElement.DesiredWidth ?? 0)
                : Math.Max(PrimaryChild?.LayoutElement.DesiredWidth ?? 0, SecondaryChild?.LayoutElement.DesiredWidth ?? 0);
            var totalDesiredWidth = childWidth + floatingPanelParentControl.TotalBorderWidth;
            LayoutElement.DesiredWidth = totalDesiredWidth;
        }

        public void FitHeightToChildren()
        {
            FloatingPanelParentControl floatingPanelParentControl = (FloatingPanelParentControl)LayoutElement.Control;
            var childHeight = Orientation == Orientation.Vertical
                ? (PrimaryChild?.LayoutElement.DesiredHeight ?? 0) + (SecondaryChild?.LayoutElement.DesiredHeight ?? 0)
                : Math.Max(PrimaryChild?.LayoutElement.DesiredHeight ?? 0, SecondaryChild?.LayoutElement.DesiredHeight ?? 0);
            var totalDesiredHeight = childHeight + floatingPanelParentControl.TotalBorderHeight;
            LayoutElement.DesiredHeight = totalDesiredHeight;
        }

        private void OnNewParentRequested(FloatingPanelParentControl _newParent, PanelControl _panelElement, Dock _dock)
        {
            var panelTabContainer = LayoutManager.Instance.FindContainer<PanelTabContainer>(_panelElement);
            var parentContainer = LayoutManager.Instance.FindContainer<ParentContainer>(_newParent);
            if (panelTabContainer is null || parentContainer is null)
            {
                return;
            }

            ParentContainer parent = panelTabContainer.Parent!;
            parent.Control.ContentStackPanel.Children.Remove(_panelElement);
            if (panelTabContainer.IsPrimaryChild)
            {
                parent.PrimaryChild = null;
            }
            else
            {
                parent.SecondaryChild = null;
            }
            parent.Refresh();

            parentContainer.AddChild(panelTabContainer, _dock);
            parentContainer.Refresh();
        }
    }
}
