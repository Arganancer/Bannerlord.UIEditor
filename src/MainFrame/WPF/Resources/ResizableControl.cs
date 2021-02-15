using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference: https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/master/OneCodeTeam/How%20to%20Resize%20WPF%20panel%20on%20Runtime/%5BC%23%5D-How%20to%20Resize%20WPF%20panel%20on%20Runtime/C%23/RuntimeResizablePanel/ResizablePanel.cs
    /// </summary>
    public class ResizableControl : ContentControl
    {
        #region Consts/Statics

        public static DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof( Brush ), typeof( ResizableControl ));
        public static DependencyProperty BorderSizeProperty = DependencyProperty.Register("BorderSize", typeof( double ), typeof( ResizableControl ));

        #endregion

        #region Properties

        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public double BorderSize
        {
            get => (double)GetValue(BorderSizeProperty);
            set => SetValue(BorderSizeProperty, value);
        }

        #endregion

        #region Constructors

        static ResizableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( ResizableControl ), new FrameworkPropertyMetadata(typeof( ResizableControl )));
        }

        #endregion
    }
}
