using System.Windows;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame
{
    public class CursorViewbox : Viewbox
    {
        public static DependencyProperty VerticalHotspotAlignmentProperty = DependencyProperty.Register("VerticalHotspotAlignment", typeof( VerticalAlignment ), typeof( CursorViewbox ));
        public static DependencyProperty HorizontalHotspotAlignmentProperty = DependencyProperty.Register("HorizontalHotspotAlignment", typeof( VerticalAlignment ), typeof( CursorViewbox ));

        public VerticalAlignment VerticalHotspotAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalHotspotAlignmentProperty);
            set => SetValue(VerticalHotspotAlignmentProperty, value);
        }

        public HorizontalAlignment HorizontalHotspotAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalHotspotAlignmentProperty);
            set => SetValue(HorizontalHotspotAlignmentProperty, value);
        }

        static CursorViewbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( CursorViewbox ), new FrameworkPropertyMetadata(typeof( CursorViewbox )));
        }
    }
}
