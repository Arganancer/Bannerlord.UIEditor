using System.Windows;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame.Resources
{
    public class WidgetRectangle : ContentControl
    {
        public static DependencyProperty IsWidgetFocusedProperty = DependencyProperty.Register("IsWidgetFocused", typeof( bool ), typeof( WidgetRectangle ));

        public bool IsWidgetFocused
        {
            get => (bool)GetValue(IsWidgetFocusedProperty);
            set => SetValue(IsWidgetFocusedProperty, value);
        }

        static WidgetRectangle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( WidgetRectangle ), new FrameworkPropertyMetadata(typeof( WidgetRectangle )));
        }
    }
}
