using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference for interop window management: https://engy.us/blog/2020/01/01/implementing-a-custom-window-title-bar-in-wpf/
    /// </summary>
    public partial class MainWindow : ConnectedWindow
    {
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr _handle, uint _flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr _hMonitor, ref MonitorInfo _lpmi);

        public static IntPtr HookProc(IntPtr _hwnd, int _msg, IntPtr _wParam, IntPtr _lParam, ref bool _handled)
        {
            if (_msg == WmGetMinMaxInfo)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
                var mmi = (MinMaxInfo)Marshal.PtrToStructure(_lParam, typeof( MinMaxInfo ));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                var monitor = MonitorFromWindow(_hwnd, MonitorDefaultToNearest);

                if (monitor != IntPtr.Zero)
                {
                    var monitorInfo = new MonitorInfo {cbSize = Marshal.SizeOf(typeof( MonitorInfo ))};

                    GetMonitorInfo(monitor, ref monitorInfo);
                    var rcWorkArea = monitorInfo.rcWork;
                    var rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, _lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WmGetMinMaxInfo = 0x0024;
        private const uint MonitorDefaultToNearest = 0x00000002;

        public MainWindow()
        {
            InitializeComponent();
        }

        public override void Load()
        {
            base.Load();

            PublicContainer.GetModule<ICursorManager>().Initialize(this);
        }

        protected override void OnSourceInitialized(EventArgs _e)
        {
            base.OnSourceInitialized(_e);
            ((HwndSource)PresentationSource.FromVisual(this))!.AddHook(HookProc);
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect(int _left, int _top, int _right, int _bottom)
            {
                Left = _left;
                Top = _top;
                Right = _right;
                Bottom = _bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            public int cbSize;
            public Rect rcMonitor;
            public Rect rcWork;
            public uint dwFlags;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public Point(int _x, int _y)
            {
                X = _x;
                Y = _y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInfo
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }
    }
}
