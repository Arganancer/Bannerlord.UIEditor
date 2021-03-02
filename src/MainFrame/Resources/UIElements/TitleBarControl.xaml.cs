using System;
using System.Windows;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame.Resources
{
    /// <summary>
    /// Interaction logic for TitleBarControl.xaml
    /// Reference for window state management: https://engy.us/blog/2020/01/01/implementing-a-custom-window-title-bar-in-wpf/
    /// </summary>
    public partial class TitleBarControl : UserControl
    {
        private Window Window { get; set; } = null!;

        public TitleBarControl()
        {
            InitializeComponent();
            Loaded += (_sender, _args) =>
            {
                Window = Window.GetWindow(this)!;
                RefreshMaximizeRestoreButton();
                Window.StateChanged += OnWindowStateChanged;
            };
            DataContext = this;
        }

        private void OnWindowStateChanged(object _sender, EventArgs _e)
        {
            RefreshMaximizeRestoreButton();
        }

        private void OnMinimizeButtonClick(object _sender, RoutedEventArgs _e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRestoreButtonClick(object _sender, RoutedEventArgs _e)
        {
            Window.WindowState = Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnCloseButtonClick(object _sender, RoutedEventArgs _e)
        {
            Window.Close();
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (Window.WindowState == WindowState.Maximized)
            {
                maximizeButton.Visibility = Visibility.Collapsed;
                restoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                maximizeButton.Visibility = Visibility.Visible;
                restoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void DebugRefreshButton_OnClick(object _sender, RoutedEventArgs _e)
        {
            ((MainWindow)Window).PublicContainer.GetModule<ILayoutManager>().Refresh();
        }
    }
}
