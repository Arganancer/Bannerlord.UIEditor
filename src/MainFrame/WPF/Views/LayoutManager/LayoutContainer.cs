using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public class LayoutContainer
    {
        public ILayoutElement LayoutElement { get; }
        public LayoutContainer? Parent { get; set; }

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

        public Dock Dock
        {
            get => LayoutElement.CurrentDock;
            set => DockPanel.SetDock(LayoutElement.Control, value);
        }

        protected bool IsPrimaryChild => Equals(Parent?.PrimaryChild);

        protected bool IsOnlyChild => Parent?.PrimaryChild is null || Parent?.SecondaryChild is null;

        private Orientation m_Orientation;
        private LayoutContainer? m_PrimaryChild;
        private LayoutContainer? m_SecondaryChild;

        public LayoutContainer(ILayoutElement _layoutElement)
        {
            LayoutElement = _layoutElement;
            LayoutElement.DesiredHeightChanged += OnDesiredHeightChanged;
            LayoutElement.DesiredWidthChanged += OnDesiredWidthChanged;
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
            var isPrimaryChild = dock == Dock.Left || dock == Dock.Top;
            var nextContainer = isPrimaryChild ? PrimaryChild : SecondaryChild;

            Orientation = containerParams[0] switch
            {
                "HorizontalLayoutContainer" => Orientation.Horizontal,
                "VerticalLayoutContainer" => Orientation.Vertical
            };

            if (nextContainer is null)
            {
                if (_path.Length > 1)
                {
                    nextContainer = new LayoutContainer(new FloatingPanelParentControl());
                }
                else
                {
                    PanelControl panelControl = new();
                    nextContainer = new StackContainer(panelControl);
                }

                nextContainer.Dock = dock;
                var container = ((FloatingPanelParentControl)LayoutElement).ContentStackPanel;
                if (isPrimaryChild)
                {
                    PrimaryChild = nextContainer;
                    container.Children.Insert(0, nextContainer.LayoutElement.Control);
                }
                else
                {
                    SecondaryChild = nextContainer;
                    container.Children.Add(nextContainer.LayoutElement.Control);
                }
            }

            if (nextContainer is StackContainer stackContainer)
            {
                stackContainer.AddPanel(_panel, _isSelected);
                stackContainer.LayoutElement.DiscreteSetDesiredSize(_desiredWidth, _desiredHeight);
            }
            else
            {
                nextContainer.InsertPanel(_path.SubArray(1), _desiredWidth, _desiredHeight, _isSelected, _panel);
            }
        }

        public void Refresh()
        {
            LayoutElement.RefreshResizerBorders();
            if (this is StackContainer)
            {
                OnDesiredWidthChanged(this, LayoutElement.DesiredWidth);
                OnDesiredHeightChanged(this, LayoutElement.DesiredHeight);
            }
            else
            {
                PrimaryChild?.Refresh();
                SecondaryChild?.Refresh();
            }
        }

        protected virtual void OnDesiredHeightChanged(object _sender, double _e)
        {
            if ((Parent is null && (Dock == Dock.Top || Dock == Dock.Bottom)) || (Parent?.Orientation == Orientation.Vertical && IsPrimaryChild && !IsOnlyChild))
            {
                LayoutElement.Control.Height = _e;
            }
            else
            {
                LayoutElement.Control.Height = double.NaN;
                Parent?.FitHeightToChildren();
            }
        }

        protected virtual void OnDesiredWidthChanged(object _sender, double _e)
        {
            if ((Parent is null && (Dock == Dock.Left || Dock == Dock.Right)) || (Parent?.Orientation == Orientation.Horizontal && IsPrimaryChild && !IsOnlyChild))
            {
                LayoutElement.Control.Width = _e;
            }
            else
            {
                LayoutElement.Control.Width = double.NaN;
                Parent?.FitWidthToChildren();
            }
        }

        protected void FitWidthToChildren()
        {
            FloatingPanelParentControl floatingPanelParentControl = (FloatingPanelParentControl)LayoutElement.Control;
            var childWidth = Orientation == Orientation.Horizontal
                ? (PrimaryChild?.LayoutElement.DesiredWidth ?? 0) + (SecondaryChild?.LayoutElement.DesiredWidth ?? 0)
                : Math.Max(PrimaryChild?.LayoutElement.DesiredWidth ?? 0, SecondaryChild?.LayoutElement.DesiredWidth ?? 0);
            var totalDesiredWidth = childWidth + floatingPanelParentControl.TotalBorderWidth;
            LayoutElement.DesiredWidth = totalDesiredWidth;
        }

        protected void FitHeightToChildren()
        {
            FloatingPanelParentControl floatingPanelParentControl = (FloatingPanelParentControl)LayoutElement.Control;
            var childHeight = Orientation == Orientation.Vertical 
                ? (PrimaryChild?.LayoutElement.DesiredHeight ?? 0) + (SecondaryChild?.LayoutElement.DesiredHeight ?? 0) 
                : Math.Max(PrimaryChild?.LayoutElement.DesiredHeight ?? 0, SecondaryChild?.LayoutElement.DesiredHeight ?? 0);
            var totalDesiredHeight = childHeight + floatingPanelParentControl.TotalBorderHeight;
            LayoutElement.DesiredHeight = totalDesiredHeight;
        }
    }

    public class StackContainer : LayoutContainer
    {
        private List<IPanel> Panels { get; } = new();

        public StackContainer(ILayoutElement _layoutElement) : base(_layoutElement)
        {
            LayoutElement.Control.SizeChanged += OnSizeChanged;
        }

        public void AddPanel(IPanel _panel, bool _isSelected)
        {
            Panels.Add(_panel);
            ((PanelControl)LayoutElement).AddPanelContent(_panel, Panels.Count == 1 || _isSelected);
        }

        private void OnSizeChanged(object _sender, SizeChangedEventArgs _e)
        {
            foreach (var panel in Panels)
            {
                if (_e.WidthChanged)
                {
                    panel.SettingCategory.SetSetting("Width", _e.NewSize.Width);
                }

                if (_e.HeightChanged)
                {
                    panel.SettingCategory.SetSetting("Height", _e.NewSize.Height);
                }

                LayoutElement.DiscreteSetDesiredSize(_e.NewSize.Width, _e.NewSize.Height);
            }
        }
    }
}
