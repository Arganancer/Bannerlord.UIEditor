using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent;

namespace Bannerlord.UIEditor.MainFrame.Resources.Resizer
{
    /// <summary>
    /// Reference: https://channel9.msdn.com/Blogs/OneCode/How-to-resize-WPF-panel-at-Runtime
    /// </summary>
    public class Resizer : Thumb
    {
        public static DependencyProperty CursorIconProperty = DependencyProperty.Register("CursorIcon", typeof( CursorIcon ), typeof( ResizableControl ));
        public static DependencyProperty ThumbDirectionProperty = DependencyProperty.Register("ThumbDirection", typeof( ResizeDirection ), typeof( Resizer ));

        public ResizeDirection ThumbDirection
        {
            get => (ResizeDirection)GetValue(ThumbDirectionProperty);
            set => SetValue(ThumbDirectionProperty, value);
        }

        public CursorIcon CursorIcon
        {
            get => (CursorIcon)GetValue(CursorIconProperty);
            set
            {
                SetValue(CursorIconProperty, value);
                Cursor = CursorManager.Instance!.GetCursor(value);
            }
        }

        public ILayoutElement? LayoutElement { get; set; }

        private Control? Control => LayoutElement?.Control;

        static Resizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(typeof(Resizer)));
        }

        public Resizer()
        {
            DragDelta += Resizer_DragDelta;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LayoutElement = this.GetVisualAncestorOfType<ILayoutElement>();
        }

        private void Resizer_DragDelta(object _sender, DragDeltaEventArgs _dragDeltaEvent)
        {
            if(LayoutElement is null || Control is null)
            {
                return;
            }

            switch (ThumbDirection)
            {
                case ResizeDirection.TopLeft:
                    ResizeTop(_dragDeltaEvent);
                    ResizeLeft(_dragDeltaEvent);
                    break;
                case ResizeDirection.Left:
                    ResizeLeft(_dragDeltaEvent);
                    break;
                case ResizeDirection.BottomLeft:
                    ResizeBottom(_dragDeltaEvent);
                    ResizeLeft(_dragDeltaEvent);
                    break;
                case ResizeDirection.Bottom:
                    ResizeBottom(_dragDeltaEvent);
                    break;
                case ResizeDirection.BottomRight:
                    ResizeBottom(_dragDeltaEvent);
                    ResizeRight(_dragDeltaEvent);
                    break;
                case ResizeDirection.Right:
                    ResizeRight(_dragDeltaEvent);
                    break;
                case ResizeDirection.TopRight:
                    ResizeTop(_dragDeltaEvent);
                    ResizeRight(_dragDeltaEvent);
                    break;
                case ResizeDirection.Top:
                    ResizeTop(_dragDeltaEvent);
                    break;
            }

            _dragDeltaEvent.Handled = true;
        }

        private void ResizeRight(DragDeltaEventArgs _dragDeltaEvent)
        {
            var deltaHorizontal = Math.Min(-_dragDeltaEvent.HorizontalChange, Control!.ActualWidth - Control.MinWidth);
            LayoutElement!.DesiredWidth = Control.ActualWidth - deltaHorizontal;
        }

        private void ResizeTop(DragDeltaEventArgs _dragDeltaEvent)
        {
            var deltaVertical = Math.Min(_dragDeltaEvent.VerticalChange, Control!.ActualHeight - Control.MinHeight);
            LayoutElement!.DesiredHeight = Control.ActualHeight - deltaVertical;
        }

        private void ResizeLeft(DragDeltaEventArgs _dragDeltaEvent)
        {
            var deltaHorizontal = Math.Min(_dragDeltaEvent.HorizontalChange, Control!.ActualWidth - Control.MinWidth);
            LayoutElement!.DesiredWidth = Control.ActualWidth - deltaHorizontal;
        }

        private void ResizeBottom(DragDeltaEventArgs _dragDeltaEvent)
        {
            var deltaVertical = Math.Min(-_dragDeltaEvent.VerticalChange, Control!.ActualHeight - Control.MinHeight);
            LayoutElement!.DesiredHeight = Control.ActualHeight - deltaVertical;
        }
    }

    public enum ResizeDirection
    {
        TopLeft,
        Left,
        BottomLeft,
        Bottom,
        BottomRight,
        Right,
        TopRight,
        Top
    }
}
