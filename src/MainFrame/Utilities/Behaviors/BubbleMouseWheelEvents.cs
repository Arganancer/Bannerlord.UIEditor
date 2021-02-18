using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference: https://stackoverflow.com/questions/1585462/bubbling-scroll-events-from-a-listview-to-its-parent
    /// </summary>
    public sealed class BubbleMouseWheelEvents : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= PreviewMouseWheel;
            base.OnDetaching();
        }

        private void PreviewMouseWheel(object _sender, MouseWheelEventArgs _e)
        {
            _e.Handled = true;
            var e2 = new MouseWheelEventArgs(_e.MouseDevice, _e.Timestamp, _e.Delta) {RoutedEvent = UIElement.MouseWheelEvent};
            AssociatedObject.RaiseEvent(e2);
        }
    }
}
