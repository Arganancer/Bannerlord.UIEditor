using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Gauntlet;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// TODO:
    /// - Add interactive UIEditor canvas.
    ///   - UIEditor Canvas Elements: https://stackoverflow.com/questions/21635892/how-to-set-x-y-coordinates-of-wpf-canvas-children-through-code/21637725
    /// 
    /// - Allow elements to be resized/moved at runtime: https://www.codeproject.com/Questions/639806/WPF-Resizable-usercontrols-inside-a-window
    /// </summary>
    public partial class MainWindow : IModule, INotifyPropertyChanged
    {
        #region Properties

        public bool Disposed { get; private set; }

        public ObservableCollection<IWidgetTemplate> WidgetTemplates { get; }

        public IWidgetTemplate? SelectedWidgetTemplate
        {
            get => m_SelectedWidgetTemplate;
            set
            {
                if (m_SelectedWidgetTemplate != value)
                {
                    m_SelectedWidgetTemplate = value;
                    if (m_SelectedWidgetTemplate is not null)
                    {
                        if (GauntletManager?.UIContext is not null)
                        {
                            SelectedWidget = m_WidgetManager!.CreateWidget(GauntletManager.UIContext, m_SelectedWidgetTemplate);
                        }
                    }
                    else
                    {
                        SelectedWidget = null;
                    }

                    OnPropertyChanged();
                }
            }
        }

        public UIEditorWidget? SelectedWidget
        {
            get => m_SelectedWidget;
            private set
            {
                if (m_SelectedWidget != value)
                {
                    m_SelectedWidget = value;
                    OnPropertyChanged();
                }
            }
        }

        private IPublicContainer PublicContainer { get; set; } = null!;

        private IWidgetManager? WidgetManager
        {
            get => m_WidgetManager;
            set
            {
                if (m_WidgetManager != value)
                {
                    if (WidgetTemplates.Count > 0)
                    {
                        Dispatcher.Invoke(() => WidgetTemplates.Clear());
                    }

                    m_WidgetManager = value;
                    if (m_WidgetManager is not null)
                    {
                        foreach (IWidgetTemplate widgetTemplate in m_WidgetManager.WidgetTemplates)
                        {
                            Dispatcher.Invoke(() => WidgetTemplates.Add(widgetTemplate));
                        }
                    }
                }
            }
        }

        private IGauntletManager? GauntletManager { get; set; }

        #endregion

        #region Fields

        private IWidgetManager? m_WidgetManager;
        private IWidgetTemplate? m_SelectedWidgetTemplate;
        private UIEditorWidget? m_SelectedWidget;

        private Rectangle m_Background = null!;
        private Rectangle m_ViewableArea = null!;

        private Point m_LastMousePos;
        private bool m_IsPanning;

        private double m_CurrentZoomScale = 1.0;

        #endregion

        #region Constructors

        public MainWindow()
        {
            WidgetTemplates = new ObservableCollection<IWidgetTemplate>();
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region IConnectedObject Members

        public event EventHandler<IConnectedObject>? Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!Disposed)
            {
                OnDisposing();
            }

            Disposed = true;
        }

        #endregion

        #region IModule Members

        public void Create(IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;

            InitializeCanvas();

            // TODO: Remove. Test code.
            Rectangle topLeftRect = new()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.Black),
                Width = 50,
                Height = 50
            };

            Canvas.SetLeft(topLeftRect, -490);
            Canvas.SetTop(topLeftRect, -490);
            UIEditorCanvas.Children.Add(topLeftRect);

            Rectangle bottomRightRect = new()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.Black),
                Width = 50,
                Height = 50
            };

            Canvas.SetLeft(bottomRightRect, 2360);
            Canvas.SetTop(bottomRightRect, 1520);
            UIEditorCanvas.Children.Add(bottomRightRect);
        }

        public void Load()
        {
            PublicContainer.ConnectToModule<IWidgetManager>(this,
                _widgetManager => WidgetManager = _widgetManager,
                _ => WidgetManager = null);

            PublicContainer.ConnectToModule<IGauntletManager>(this,
                _gauntletManager => GauntletManager = _gauntletManager,
                _ => GauntletManager = null);
        }

        public void Unload()
        {
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Protected Methods

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        #endregion

        #region Private Methods

        private void InitializeCanvas()
        {
            m_Background = new Rectangle
            {
                Stroke = new SolidColorBrush(new Color {A = 255, R = 200, G = 200, B = 200}),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(new Color {A = 255, R = 200, G = 200, B = 200}),
                Width = 2920,
                Height = 2080
            };

            Canvas.SetLeft(m_Background, -500);
            Canvas.SetTop(m_Background, -500);
            UIEditorCanvas.Children.Add(m_Background);

            m_ViewableArea = new Rectangle
            {
                Stroke = new SolidColorBrush(new Color {A = 255, R = 180, G = 180, B = 180}),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(new Color {A = 255, R = 195, G = 195, B = 195}),
                Width = 1920,
                Height = 1080
            };

            Canvas.SetLeft(m_ViewableArea, 0);
            Canvas.SetTop(m_ViewableArea, 0);
            UIEditorCanvas.Children.Add(m_ViewableArea);
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

        #endregion
    }
}
