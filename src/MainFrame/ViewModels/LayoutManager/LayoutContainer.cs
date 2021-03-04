using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Resources;

namespace Bannerlord.UIEditor.MainFrame
{
    public abstract class LayoutContainer : IDisposable
    {
        public ParentContainer? Parent { get; set; }

        public Dispatcher Dispatcher { get; }

        public abstract ILayoutElement LayoutElement { get; }

        public abstract Dock Dock { get; set; }
        public bool IsPrimaryChild => Equals(Parent?.PrimaryChild);
        public bool IsOnlyChild => Parent?.PrimaryChild is null || Parent?.SecondaryChild is null;

        protected IPublicContainer PublicContainer { get; }

        protected LayoutContainer(IPublicContainer _publicContainer, Dispatcher _dispatcher)
        {
            PublicContainer = _publicContainer;
            Dispatcher = _dispatcher;
        }

        public abstract void Dispose();
        public abstract void Refresh();

        public abstract T? FindContainer<T>(Control _control) where T : LayoutContainer;
    }

    public abstract class LayoutContainer<T> : LayoutContainer where T : ILayoutElement
    {
        protected T Control { get; }

        protected LayoutContainer(T _layoutElement, IPublicContainer _publicContainer, Dispatcher _dispatcher) : base(_publicContainer, _dispatcher)
        {
            Control = _layoutElement;
            _layoutElement.DesiredHeightChanged += OnDesiredHeightChanged;
            _layoutElement.DesiredWidthChanged += OnDesiredWidthChanged;
        }

        protected List<Dock> GetBorders()
        {
            List<Dock> output = new();
            switch (Dock)
            {
                case Dock.Left:
                    if (Parent is null || !IsOnlyChild)
                    {
                        output.Add(Dock.Right);
                    }
                    break;
                case Dock.Top:
                    if (Parent is null || !IsOnlyChild)
                    {
                        output.Add(Dock.Bottom);
                    }
                    break;
                case Dock.Right:
                    if (Parent is null)
                    {
                        output.Add(Dock.Left);
                    }
                    break;
                case Dock.Bottom:
                    if (Parent is null)
                    {
                        output.Add(Dock.Top);
                    }
                    break;
            }

            return output;
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
