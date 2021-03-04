using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bannerlord.UIEditor.MainFrame
{
    public static class CursorHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Win32Point
        {
            public int X;
            public int Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point _pt);

        public static Point GetCurrentCursorPosition(Visual _relativeTo)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return _relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        public static bool IsCursorInside(FrameworkElement _relativeTo)
        {
            Point relativePoint = GetCurrentCursorPosition(_relativeTo);
            return relativePoint.X >= 0 && relativePoint.X <= _relativeTo.ActualWidth && relativePoint.Y >= 0 && relativePoint.Y <= _relativeTo.ActualHeight;
        }
    }
}
