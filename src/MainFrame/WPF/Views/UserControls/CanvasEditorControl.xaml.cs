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
    /// TODO: AddIcon left/top pixel rulers.
    /// </summary>
    public partial class CanvasEditorControl : ConnectedUserControl, ICanvasEditorControl
    {
        public Canvas Canvas => UIEditorCanvas;
        public double ViewableAreaWidth => m_ViewableArea.Width;
        public double ViewableAreaHeight => m_ViewableArea.Height;

        private Rectangle m_Background = null!;
        private Rectangle m_ViewableArea = null!;

        private Point m_LastMousePos;
        private bool m_IsPanning;

        private double m_CurrentZoomScale = 1.0;

        public CanvasEditorControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
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
            SolidColorBrush backgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#121212"))!;
            m_Background = new Rectangle
            {
                StrokeThickness = 0,
                Fill = backgroundColor,
                Width = 2920,
                Height = 2080
            };

            Canvas.SetLeft(m_Background, -500);
            Canvas.SetTop(m_Background, -500);
            UIEditorCanvas.Children.Add(m_Background);
            Panel.SetZIndex(m_Background, -2);

            m_ViewableArea = new Rectangle
            {
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1a1a1a"))!,
                StrokeThickness = 1,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#141414"))!,
                Width = 1920,
                Height = 1080
            };

            Canvas.SetLeft(m_ViewableArea, 0);
            Canvas.SetTop(m_ViewableArea, 0);
            UIEditorCanvas.Children.Add(m_ViewableArea);
            Panel.SetZIndex(m_ViewableArea, -1);
        }

        private void UIEditorCanvas_OnMouseMove(object _sender, MouseEventArgs _mouseMoveEvent)
        {
            base.OnMouseMove(_mouseMoveEvent);
            if (m_IsPanning)
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
            m_CurrentZoomScale = _mouseWheelEvent.Delta > 0 ? m_CurrentZoomScale * scaleStep : m_CurrentZoomScale / scaleStep;

            var borderPosition = _mouseWheelEvent.GetPosition(CanvasBorder);
            var canvasPosition = _mouseWheelEvent.GetPosition(UIEditorCanvas);

            ClampZoomScale();

            CanvasTranslateTransform.X = (-canvasPosition.X + ((borderPosition.X - CanvasBorder.BorderThickness.Left) / m_CurrentZoomScale)) * m_CurrentZoomScale;
            CanvasTranslateTransform.Y = (-canvasPosition.Y + ((borderPosition.Y - CanvasBorder.BorderThickness.Left) / m_CurrentZoomScale)) * m_CurrentZoomScale;

            ClampPosition();
        }

        private void ClampZoomScale()
        {
            var canvasSize = UIEditorCanvas.RenderSize;
            var minZoomScale = Math.Max(1d / (2920d / canvasSize.Height), 1d / (2080d / canvasSize.Width));
            if (m_CurrentZoomScale < minZoomScale)
            {
                m_CurrentZoomScale = minZoomScale;
            }

            if (Math.Abs(CanvasScaleTransform.ScaleX - m_CurrentZoomScale) > 0.00000001)
            {
                CanvasScaleTransform.ScaleX = m_CurrentZoomScale;
                CanvasScaleTransform.ScaleY = m_CurrentZoomScale;
            }
        }

        private void ClampPosition()
        {
            var canvasSize = UIEditorCanvas.RenderSize;
            var currentX = CanvasTranslateTransform.X / m_CurrentZoomScale;
            var currentY = CanvasTranslateTransform.Y / m_CurrentZoomScale;
            if (currentX > 500)
            {
                CanvasTranslateTransform.X = 500 * m_CurrentZoomScale;
            }
            else if (-currentX + (canvasSize.Width / m_CurrentZoomScale) > 2420)
            {
                CanvasTranslateTransform.X = -((2420 * m_CurrentZoomScale) - canvasSize.Width);
            }

            if (currentY > 500)
            {
                CanvasTranslateTransform.Y = 500 * m_CurrentZoomScale;
            }
            else if (-currentY + (canvasSize.Height / m_CurrentZoomScale) > 1580)
            {
                CanvasTranslateTransform.Y = -((1580 * m_CurrentZoomScale) - canvasSize.Height);
            }
        }
    }
}
