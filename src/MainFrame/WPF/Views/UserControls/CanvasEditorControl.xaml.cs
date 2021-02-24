using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// TODO: UIEditor Canvas Elements: https://stackoverflow.com/questions/21635892/how-to-set-x-y-coordinates-of-wpf-canvas-children-through-code/21637725
    /// TODO: Remove magic numbers and allow changing resolution/aspect ratio.
    /// TODO: Add left/top pixel guides.
    /// </summary>
    public partial class CanvasEditorControl : ConnectedUserControl, ICanvasEditorControl
    {
        public Canvas Canvas => UIEditorCanvas;
        public double ViewableAreaWidth => m_ViewableArea.Width;
        public double ViewableAreaHeight => m_ViewableArea.Height;

        public event EventHandler<MouseEventArgs>? CanvasEditorMouseMove;
        public event EventHandler<MouseButtonEventArgs>? CanvasEditorMouseUp; 

        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }

        public int BackgroundBuffer { get; set; }

        public double CurrentZoomScale { get; private set; } = 1.0;

        private Rectangle m_Background = null!;
        private Rectangle m_ViewableArea = null!;

        private Point m_LastMousePos;
        private bool m_IsPanning;

        public CanvasEditorControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            CanvasWidth = 1920;
            CanvasHeight = 1080;
            BackgroundBuffer = 2000;

            InitializeCanvas();
            PublicContainer.RegisterModule<ICanvasEditorControl>(this);
        }

        public void AddWidget(IWidgetTemplate _widgetTemplate, Point _point)
        {
        }

        private void UIEditorCanvas_OnDragOver(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( IWidgetTemplate )))
            {
                _e.Effects = _e.Data.GetDataPresent(nameof( IWidgetTemplate )) ? DragDropEffects.Copy : DragDropEffects.None;
                _e.Handled = true;
            }
        }

        private void UIEditorCanvas_OnDrop(object _sender, DragEventArgs _e)
        {
            if (_e.Data.GetDataPresent(nameof( IWidgetTemplate )))
            {
                IWidgetTemplate widgetTemplate = (IWidgetTemplate)_e.Data.GetData(nameof( IWidgetTemplate ))!;
                AddWidget(widgetTemplate, _e.GetPosition(UIEditorCanvas));
                _e.Handled = true;
            }
        }

        private void InitializeCanvas()
        {
            SolidColorBrush backgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#121212")!;
            m_Background = new Rectangle {StrokeThickness = 0, Fill = backgroundColor, Width = CanvasWidth + BackgroundBuffer * 2, Height = CanvasHeight + BackgroundBuffer * 2 };

            Canvas.SetLeft(m_Background, -BackgroundBuffer);
            Canvas.SetTop(m_Background, -BackgroundBuffer);
            UIEditorCanvas.Children.Add(m_Background);
            Panel.SetZIndex(m_Background, -2);

            m_ViewableArea = new Rectangle
            {
                Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#1a1a1a")!,
                StrokeThickness = 1,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#141414")!,
                Width = CanvasWidth,
                Height = CanvasHeight
            };

            Canvas.SetLeft(m_ViewableArea, 0);
            Canvas.SetTop(m_ViewableArea, 0);
            UIEditorCanvas.Children.Add(m_ViewableArea);
            Panel.SetZIndex(m_ViewableArea, -1);
        }

        private void UIEditorCanvas_OnMouseMove(object _sender, MouseEventArgs _mouseMoveEvent)
        {
            base.OnMouseMove(_mouseMoveEvent);
            OnCanvasEditorMouseMove(_mouseMoveEvent);

            if ( !_mouseMoveEvent.Handled && m_IsPanning)
            {
                var position = _mouseMoveEvent.GetPosition(this);
                CanvasTranslateTransform.X += position.X - m_LastMousePos.X;
                CanvasTranslateTransform.Y += position.Y - m_LastMousePos.Y;
                m_LastMousePos = position;

                ClampPosition();
            }
        }

        private void UIEditorCanvas_OnMouseDown(object _sender, MouseButtonEventArgs _mouseDownEvent)
        {
            base.OnMouseDown(_mouseDownEvent);
            switch (_mouseDownEvent.ChangedButton)
            {
                case MouseButton.Left:
                    break;
                case MouseButton.Middle:
                    UIEditorCanvas.CaptureMouse();
                    m_LastMousePos = _mouseDownEvent.GetPosition(this);
                    m_IsPanning = true;
                    break;
                case MouseButton.Right:
                    break;
            }
        }

        private void UIEditorCanvas_OnMouseUp(object _sender, MouseButtonEventArgs _mouseUpEvent)
        {
            base.OnMouseUp(_mouseUpEvent);
            OnCanvasEditorMouseUp(_mouseUpEvent);

            if (_mouseUpEvent.Handled)
            {
                return;
            }

            switch (_mouseUpEvent.ChangedButton)
            {
                case MouseButton.Left:
                    break;
                case MouseButton.Middle:
                    UIEditorCanvas.ReleaseMouseCapture();
                    m_IsPanning = false;
                    break;
                case MouseButton.Right:
                    break;
            }
        }

        private void UIEditorCanvas_OnSizeChanged(object _sender, SizeChangedEventArgs _e)
        {
            ClampZoomScale();
            ClampPosition();
        }

        private void UIEditorCanvas_OnMouseWheel(object _sender, MouseWheelEventArgs _mouseWheelEvent)
        {
            const double scaleStep = 1.1;
            CurrentZoomScale = _mouseWheelEvent.Delta > 0 ? CurrentZoomScale * scaleStep : CurrentZoomScale / scaleStep;

            var borderPosition = _mouseWheelEvent.GetPosition(CanvasBorder);
            var canvasPosition = _mouseWheelEvent.GetPosition(UIEditorCanvas);

            ClampZoomScale();

            CanvasTranslateTransform.X = (-canvasPosition.X + ((borderPosition.X - CanvasBorder.BorderThickness.Left) / CurrentZoomScale)) * CurrentZoomScale;
            CanvasTranslateTransform.Y = (-canvasPosition.Y + ((borderPosition.Y - CanvasBorder.BorderThickness.Left) / CurrentZoomScale)) * CurrentZoomScale;

            ClampPosition();
        }

        private void ClampZoomScale()
        {
            var canvasSize = UIEditorCanvas.RenderSize;
            var minZoomScale = Math.Max(1d / ((CanvasWidth + BackgroundBuffer * 2) / canvasSize.Height), 1d / ((CanvasHeight + BackgroundBuffer * 2) / canvasSize.Width));
            if (CurrentZoomScale < minZoomScale)
            {
                CurrentZoomScale = minZoomScale;
            }

            if (Math.Abs(CanvasScaleTransform.ScaleX - CurrentZoomScale) > 0.00000001)
            {
                CanvasScaleTransform.ScaleX = CurrentZoomScale;
                CanvasScaleTransform.ScaleY = CurrentZoomScale;
            }
        }

        private void ClampPosition()
        {
            var canvasSize = UIEditorCanvas.RenderSize;
            var currentX = CanvasTranslateTransform.X / CurrentZoomScale;
            var currentY = CanvasTranslateTransform.Y / CurrentZoomScale;
            if (currentX > BackgroundBuffer)
            {
                CanvasTranslateTransform.X = BackgroundBuffer * CurrentZoomScale;
            }
            else if (-currentX + (canvasSize.Width / CurrentZoomScale) > (CanvasWidth + BackgroundBuffer))
            {
                CanvasTranslateTransform.X = -(((CanvasWidth + BackgroundBuffer) * CurrentZoomScale) - canvasSize.Width);
            }

            if (currentY > BackgroundBuffer)
            {
                CanvasTranslateTransform.Y = BackgroundBuffer * CurrentZoomScale;
            }
            else if (-currentY + (canvasSize.Height / CurrentZoomScale) > (CanvasHeight + BackgroundBuffer))
            {
                CanvasTranslateTransform.Y = -(((CanvasHeight + BackgroundBuffer) * CurrentZoomScale) - canvasSize.Height);
            }
        }

        private void OnCanvasEditorMouseMove(MouseEventArgs _e)
        {
            CanvasEditorMouseMove?.Invoke(this, _e);
        }

        private void OnCanvasEditorMouseUp(MouseButtonEventArgs _e)
        {
            CanvasEditorMouseUp?.Invoke(this, _e);
        }
    }
}
