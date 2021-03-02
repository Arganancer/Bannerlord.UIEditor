using System;
using System.Windows.Controls;
using Bannerlord.UIEditor.MainFrame.Resources;

namespace Bannerlord.UIEditor.MainFrame
{
    public abstract class LayoutContainer : IDisposable
    {
        public ParentContainer? Parent { get; set; }

        public abstract ILayoutElement LayoutElement { get; }

        public abstract Dock Dock { get; set; }
        public bool IsPrimaryChild => Equals(Parent?.PrimaryChild);
        public bool IsOnlyChild => Parent?.PrimaryChild is null || Parent?.SecondaryChild is null;

        public abstract void Dispose();
        public abstract void Refresh();

        public abstract T? FindContainer<T>(Control _control) where T : LayoutContainer;
    }

    public abstract class LayoutContainer<T> : LayoutContainer where T : ILayoutElement
    {
        protected T Control { get; }

        protected LayoutContainer(T _layoutElement)
        {
            Control = _layoutElement;
            _layoutElement.DesiredHeightChanged += OnDesiredHeightChanged;
            _layoutElement.DesiredWidthChanged += OnDesiredWidthChanged;
        }

        public override ILayoutElement LayoutElement => Control;

        public override Dock Dock
        {
            get => LayoutElement.CurrentDock;
            set => DockPanel.SetDock(LayoutElement.Control, value);
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
    }
}
