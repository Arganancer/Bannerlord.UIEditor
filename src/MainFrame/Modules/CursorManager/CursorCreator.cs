using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;
using Size = System.Windows.Size;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference: https://web.archive.org/web/20150504171203/http://tech.pro:80/tutorial/751/wpf-tutorial-how-to-use-custom-cursors
    /// </summary>
    public static class CursorCreator
    {
        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo _icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr _hIcon, ref IconInfo _pIconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr _hIcon);

        public static Cursor CreateCursor(CursorViewbox _cursorViewbox)
        {
            var xHotSpot = _cursorViewbox.HorizontalHotspotAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => (int)(_cursorViewbox.Width * 0.5f),
                HorizontalAlignment.Right => (int)_cursorViewbox.Width,
                HorizontalAlignment.Stretch => 0,
                _ => throw new ArgumentOutOfRangeException()
            };

            var yHotSpot = _cursorViewbox.VerticalHotspotAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => (int)(_cursorViewbox.Height * 0.5f),
                VerticalAlignment.Bottom => (int)_cursorViewbox.Height,
                VerticalAlignment.Stretch => 0,
                _ => throw new ArgumentOutOfRangeException()
            };

            return CreateCursor(_cursorViewbox, xHotSpot, yHotSpot);
        }

        public static Cursor CreateCursor(UIElement _element, int _xHotSpot, int _yHotSpot)
        {
            _element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _element.Arrange(new Rect(0, 0, _element.DesiredSize.Width,
                _element.DesiredSize.Height));

            RenderTargetBitmap rtb = new((int)_element.DesiredSize.Width,
                (int)_element.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(_element);

            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            MemoryStream ms = new();
            encoder.Save(ms);

            Bitmap bmp = new(ms);

            ms.Close();
            ms.Dispose();

            Cursor cur = InternalCreateCursor(bmp, _xHotSpot, _yHotSpot);

            bmp.Dispose();

            return cur;
        }

        private static Cursor InternalCreateCursor(Bitmap _bmp, int _xHotSpot, int _yHotSpot)
        {
            var tmp = new IconInfo();
            GetIconInfo(_bmp.GetHicon(), ref tmp);
            tmp.XHotspot = _xHotSpot;
            tmp.YHotspot = _yHotSpot;
            tmp.FIcon = false;

            var ptr = CreateIconIndirect(ref tmp);
            SafeFileHandle handle = new(ptr, false);
            return CursorInteropHelper.Create(handle);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IconInfo
        {
            public bool FIcon;
            public int XHotspot;
            public int YHotspot;
            public IntPtr HbmMask;
            public IntPtr HbmColor;
        }
    }
}
