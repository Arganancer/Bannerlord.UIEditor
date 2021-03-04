using System;
using System.Windows.Controls;
using System.Windows.Threading;
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
                        Control.ContentStackPanel.Children.Remove(m_PrimaryChild.LayoutElement.Control);
                        m_PrimaryChild.Parent = null;
                    }

                    m_PrimaryChild = value;
                    if (m_PrimaryChild is not null)
                    {
                        m_PrimaryChild.Parent = this;
                        m_PrimaryChild.Dock = Orientation.GetPrimaryDock();
                        Control.ContentStackPanel.Children.Insert(0, m_PrimaryChild.LayoutElement.Control);
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
                        Control.ContentStackPanel.Children.Remove(m_SecondaryChild.LayoutElement.Control);
                        m_SecondaryChild.Parent = null;
                    }

                    m_SecondaryChild = value;
                    if (m_SecondaryChild is not null)
                    {
                        m_SecondaryChild.Parent = this;
                        m_SecondaryChild.Dock = Orientation.GetSecondaryDock();
                        Control.ContentStackPanel.Children.Add(m_SecondaryChild.LayoutElement.Control);
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
                    PrimaryChild.Dock = Orientation.GetPrimaryDock();
                }

                if (SecondaryChild is not null)
                {
                    SecondaryChild.Dock = Orientation.GetSecondaryDock();
                }
            }
        }

        private Orientation m_Orientation;
        private LayoutContainer? m_PrimaryChild;
        private LayoutContainer? m_SecondaryChild;

        public ParentContainer(FloatingPanelParentControl _layoutElement, IPublicContainer _publicContainer, Dispatcher _dispatcher) : base(_layoutElement, _publicContainer, _dispatcher)
        {
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
                Dispatcher.Invoke(() =>
                {
                    LayoutElement.DesiredWidth = 0;
                    LayoutElement.DesiredHeight = 0;
                    LayoutElement.RefreshResizerBorders();
                });
                return;
            }

            Dispatcher.Invoke(() =>
            {
                PrimaryChild?.Refresh();
                SecondaryChild?.Refresh();
                LayoutElement.RefreshResizerBorders(GetBorders().ToArray());
            });
        }

        public override void Dispose()
        {
        }

        public void RemoveChild(LayoutContainer _layoutContainer)
        {
            if (_layoutContainer.IsPrimaryChild)
            {
                PrimaryChild = null;
            }
            else
            {
                SecondaryChild = null;
            }

            Refresh();
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
            var nextContainer = dock.IsPrimary() ? PrimaryChild : SecondaryChild;

            Orientation = containerParams[0] switch
            {
                "HorizontalLayoutContainer" => Orientation.Horizontal,
                "VerticalLayoutContainer" => Orientation.Vertical
            };

            if (nextContainer is null)
            {
                if (_path.Length > 1)
                {
                    nextContainer = new ParentContainer(new FloatingPanelParentControl(), PublicContainer, Dispatcher) {Dock = dock};
                }
                else
                {
                    PanelControl panelControl = new();
                    panelControl.Create(PublicContainer);
                    panelControl.Load();
                    nextContainer = new PanelTabContainer(panelControl, PublicContainer, Dispatcher) {Dock = dock};
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
            if (_dock.IsPrimary())
            {
                if (PrimaryChild is null)
                {
                    PrimaryChild = _container;
                }
                else if (PrimaryChild is PanelTabContainer panelTabContainer)
                {
                    if (_container is PanelTabContainer newPanelTabContainer)
                    {
                        panelTabContainer.MergeInto(newPanelTabContainer);
                    }
                }
                else if (PrimaryChild is ParentContainer parentContainer)
                {
                    parentContainer.AddChild(_container, _dock);
                }
            }
            else
            {
                if (SecondaryChild is null)
                {
                    SecondaryChild = _container;
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

            Refresh();
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
    }
}
