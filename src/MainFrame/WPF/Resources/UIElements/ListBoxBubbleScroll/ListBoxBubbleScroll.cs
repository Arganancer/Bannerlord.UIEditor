using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bannerlord.UIEditor.MainFrame
{
    public class ListBoxBubbleScroll : ListBox
    {
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs _e)
        {
            if (!_e.Handled)
            {
                _e.Handled = true;
                var eventArg = new MouseWheelEventArgs(_e.MouseDevice, _e.Timestamp, _e.Delta)
                {
                    RoutedEvent = MouseWheelEvent, 
                    Source = this
                };
                UIElement? parent = Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
