using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface ICanvasEditorControl
    {
        Canvas Canvas { get; }
        Dispatcher Dispatcher { get; }
        double ViewableAreaWidth { get; }
        double ViewableAreaHeight { get; }
        double CurrentZoomScale { get; }
        event EventHandler<MouseEventArgs>? CanvasEditorMouseMove;
        event EventHandler<MouseButtonEventArgs>? CanvasEditorMouseUp;
    }
}
