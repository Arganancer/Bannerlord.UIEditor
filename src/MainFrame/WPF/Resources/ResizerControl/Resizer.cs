using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference: https://channel9.msdn.com/Blogs/OneCode/How-to-resize-WPF-panel-at-Runtime
    /// </summary>
    public class Resizer : Thumb
    {
        public static DependencyProperty ThumbDirectionProperty = DependencyProperty.Register("ThumbDirection", typeof( ResizeDirection ), typeof( Resizer ));

        public ResizeDirection ThumbDirection
        {
            get => (ResizeDirection)GetValue(ThumbDirectionProperty);
            set => SetValue(ThumbDirectionProperty, value);
        }

        static Resizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( Resizer ), new FrameworkPropertyMetadata(typeof( Resizer )));
        }

        public Resizer()
        {
            DragDelta += Resizer_DragDelta;
        }

        private void Resizer_DragDelta(object _sender, DragDeltaEventArgs _dragDeltaEvent)
        {
            if (DataContext is Control designerItem)
            {
                //if (double.IsNaN(designerItem.Width))
                //{
                //    designerItem.Width = designerItem.ActualWidth;
                //}

                //if (double.IsNaN(designerItem.Height))
                //{
                //    designerItem.Height = designerItem.ActualHeight;
                //}

                switch (ThumbDirection)
                {
                    case ResizeDirection.TopLeft:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Left:
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.BottomLeft:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Bottom:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.BottomRight:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Right:
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.TopRight:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Top:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        break;
                }
            }

            _dragDeltaEvent.Handled = true;
        }

        private static void ResizeRight(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(-_dragDeltaEvent.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
            _designerItem.Width -= deltaHorizontal;
        }

        private static void ResizeTop(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(_dragDeltaEvent.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
            _designerItem.Height -= deltaHorizontal;
        }

        private static void ResizeLeft(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(_dragDeltaEvent.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
            _designerItem.Width -= deltaHorizontal;
        }

        private static void ResizeBottom(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(-_dragDeltaEvent.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
            _designerItem.Height -= deltaHorizontal;
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
