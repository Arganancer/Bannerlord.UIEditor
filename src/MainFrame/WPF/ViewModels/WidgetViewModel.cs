using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.WidgetLibrary;
using TaleWorlds.GauntletUI;
using Canvas = System.Windows.Controls.Canvas;

namespace Bannerlord.UIEditor.MainFrame
{
    public class WidgetViewModel : IConnectedObject, INotifyPropertyChanged, IFocusable
    {
        public WidgetViewModel? Parent { get; protected set; }
        public ObservableCollection<WidgetViewModel> Children { get; set; } = new();

        public string? Name
        {
            get => m_Name;
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsVisible
        {
            get => m_IsVisible;
            set
            {
                if (m_IsVisible != value)
                {
                    m_IsVisible = value;
                    foreach (WidgetViewModel child in Children)
                    {
                        child.ParentIsHidden = ParentIsHidden || !IsVisible;
                    }

                    UpdateVisibility();
                }
            }
        }

        public int ZIndex
        {
            get => m_ZIndex;
            set
            {
                if (m_ZIndex != value)
                {
                    m_ZIndex = value;
                    UpdateRectangle();
                    foreach (WidgetViewModel child in Children)
                    {
                        child.ZIndex = m_ZIndex + 1;
                    }
                }
            }
        }

        public float X
        {
            get => m_X;
            set
            {
                if (m_X != value)
                {
                    m_X = value;
                    UpdateRectangle();
                    OnPropertyChanged();
                }
            }
        }

        public float Y
        {
            get => m_Y;
            set
            {
                if (m_Y != value)
                {
                    m_Y = value;
                    UpdateRectangle();
                    OnPropertyChanged();
                }
            }
        }

        public float Width
        {
            get => m_Width;
            set
            {
                if (m_Width != value)
                {
                    m_Width = value;
                    UpdateRectangle();
                    OnPropertyChanged();
                }
            }
        }

        public float Height
        {
            get => m_Height;
            set
            {
                if (m_Height != value)
                {
                    m_Height = value;
                    UpdateRectangle();
                    OnPropertyChanged();
                }
            }
        }

        public UIEditorWidget Widget { get; }

        public bool IsReadonly
        {
            get => m_IsReadonly;
            set
            {
                if (m_IsReadonly != value)
                {
                    m_IsReadonly = value;
                    Widget.IsReadonly = m_IsReadonly;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsFocused
        {
            get => m_IsFocused;
            set
            {
                m_IsFocused = value;
                UpdateVisualFocus();
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => m_IsExpanded;
            set
            {
                m_IsExpanded = value;
                OnPropertyChanged();
            }
        }

        protected float ActualX { get; set; }

        protected float ActualY { get; set; }

        protected float ActualWidth { get; set; }
        protected float ActualHeight { get; set; }

        protected float SuggestedWidth
        {
            get => m_SuggestedWidth;
            set
            {
                m_SuggestedWidth = value;
                UpdateRectangle();
            }
        }

        protected float SuggestedHeight
        {
            get => m_SuggestedHeight;
            set
            {
                m_SuggestedHeight = value;
                UpdateRectangle();
            }
        }

        protected bool ParentIsHidden
        {
            get => m_ParentIsHidden;
            set
            {
                if (m_ParentIsHidden != value)
                {
                    m_ParentIsHidden = value;
                    foreach (WidgetViewModel child in Children)
                    {
                        child.ParentIsHidden = ParentIsHidden || !IsVisible;
                    }

                    UpdateVisibility();
                }
            }
        }

        protected Rectangle? Rectangle
        {
            get => m_Rectangle;
            set
            {
                if (m_Rectangle != value)
                {
                    if (m_Rectangle is not null)
                    {
                        m_Rectangle.MouseMove -= OnMouseMove;
                        m_Rectangle.MouseLeave -= OnMouseLeave;
                        m_Rectangle.MouseDown -= OnMouseDown;
                    }

                    m_Rectangle = value;
                    if (m_Rectangle is not null)
                    {
                        m_Rectangle.MouseMove += OnMouseMove;
                        m_Rectangle.MouseLeave += OnMouseLeave;
                        m_Rectangle.MouseDown += OnMouseDown;
                    }
                }
            }
        }

        protected float MarginTop
        {
            get => m_MarginTop;
            set
            {
                m_MarginTop = value;
                UpdateRectangle();
            }
        }

        protected float MarginLeft
        {
            get => m_MarginLeft;
            set
            {
                m_MarginLeft = value;
                UpdateRectangle();
            }
        }

        protected float MarginBottom
        {
            get => m_MarginBottom;
            set
            {
                m_MarginBottom = value;
                UpdateRectangle();
            }
        }

        protected float MarginRight
        {
            get => m_MarginRight;
            set
            {
                m_MarginRight = value;
                UpdateRectangle();
            }
        }

        protected VerticalAlignment VerticalAlignment
        {
            get => m_VerticalAlignment;
            set
            {
                m_VerticalAlignment = value;
                UpdateRectangle();
            }
        }

        protected HorizontalAlignment HorizontalAlignment
        {
            get => m_HorizontalAlignment;
            set
            {
                m_HorizontalAlignment = value;
                UpdateRectangle();
            }
        }

        protected SizePolicy WidthSizePolicy
        {
            get => m_WidthSizePolicy;
            set
            {
                m_WidthSizePolicy = value;
                UpdateRectangle();
            }
        }

        protected SizePolicy HeightSizePolicy
        {
            get => m_HeightSizePolicy;
            set
            {
                m_HeightSizePolicy = value;
                UpdateRectangle();
            }
        }

        protected float MaxWidth
        {
            get => m_MaxWidth;
            set
            {
                m_MaxWidth = value;
                UpdateRectangle();
            }
        }

        protected float MaxHeight
        {
            get => m_MaxHeight;
            set
            {
                m_MaxHeight = value;
                UpdateRectangle();
            }
        }

        protected float MinWidth
        {
            get => m_MinWidth;
            set
            {
                m_MinWidth = value;
                UpdateRectangle();
            }
        }

        protected float MinHeight
        {
            get => m_MinHeight;
            set
            {
                m_MinHeight = value;
                UpdateRectangle();
            }
        }

        protected float PositionXOffset
        {
            get => m_PositionXOffset;
            set
            {
                m_PositionXOffset = value;
                UpdateRectangle();
            }
        }

        protected float PositionYOffset
        {
            get => m_PositionYOffset;
            set
            {
                m_PositionYOffset = value;
                UpdateRectangle();
            }
        }

        private IPublicContainer PublicContainer { get; set; } = null!;

        private ICanvasEditorControl? m_CanvasEditorControl;

        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;
        private bool m_IsVisible;
        private int m_ZIndex;
        private Rectangle? m_Rectangle;
        private bool m_ParentIsHidden;
        private string? m_Name;
        private bool m_IsReadonly;
        private float m_MarginTop;
        private float m_MarginLeft;
        private float m_MarginBottom;
        private float m_MarginRight;
        private VerticalAlignment m_VerticalAlignment;
        private HorizontalAlignment m_HorizontalAlignment;
        private SizePolicy m_WidthSizePolicy;
        private SizePolicy m_HeightSizePolicy;
        private float m_MaxWidth;
        private float m_MaxHeight;
        private float m_MinWidth;
        private float m_MinHeight;
        private float m_PositionXOffset;
        private float m_PositionYOffset;
        private float m_SuggestedWidth;
        private float m_SuggestedHeight;
        private bool m_IsFocused;
        private bool m_IsExpanded;
        private IFocusManager m_FocusManager;

        public WidgetViewModel(string _name, UIEditorWidget _widget, ICanvasEditorControl? _canvasEditorControl, IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;
            m_FocusManager = PublicContainer.GetModule<IFocusManager>();

            Name = _name;
            Widget = _widget;
            _widget.PropertyChanged += WidgetPropertyChanged;

            if (_canvasEditorControl is null)
            {
                return;
            }

            AddToCanvas(_canvasEditorControl);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<IConnectedObject>? Disposing;

        public void Dispose()
        {
            OnDisposing(this);
        }

        public void AddToCanvas(ICanvasEditorControl _canvasEditorControl)
        {
            if (m_CanvasEditorControl is null)
            {
                m_CanvasEditorControl = _canvasEditorControl;
                m_CanvasEditorControl.Dispatcher.Invoke(() => { Rectangle = new Rectangle {Stroke = new SolidColorBrush(Colors.DeepSkyBlue), StrokeThickness = 1, Fill = new SolidColorBrush(Colors.SteelBlue)}; });

                IsVisible = true;
                ParentIsHidden = false;

                ZIndex = 0;

                UpdateVisibility();
                UpdateRectangle();
            }
        }

        public bool WidgetExistsInParents(WidgetViewModel _widgetViewModel)
        {
            return Parent is not null && (Parent.Equals(_widgetViewModel) || Parent.WidgetExistsInParents(_widgetViewModel));
        }

        public void AddChildren(int _index, params WidgetViewModel[] _children)
        {
            foreach (WidgetViewModel child in _children)
            {
                m_CanvasEditorControl?.Dispatcher.Invoke(() =>
                {
                    child.ParentIsHidden = !IsVisible || ParentIsHidden;
                    child.ZIndex = ZIndex + 1;
                });
                child.Parent = this;
            }

            if (_index >= Children.Count)
            {
                foreach (WidgetViewModel child in _children)
                {
                    Children.Add(child);
                }
            }
            else
            {
                for (var i = 0; i < _children.Length; i++)
                {
                    Children.Insert(_index + i, _children[i]);
                }
            }
        }

        public int GetIndexOfChild(WidgetViewModel _child)
        {
            return Children.IndexOf(_child);
        }

        public void AddChildren(params WidgetViewModel[] _children)
        {
            AddChildren(int.MaxValue, _children);
        }

        public void RemoveChildren(params WidgetViewModel[] _children)
        {
            foreach (WidgetViewModel childToRemove in _children)
            {
                Children.Remove(childToRemove);
            }

            foreach (WidgetViewModel child in Children)
            {
                child.RemoveChildren(_children);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        protected void RefreshWidget()
        {
            RefreshSize();
            RefreshPosition();
        }

        protected void RefreshSize()
        {
            var parentWidth = (float)(Parent?.ActualWidth ?? m_CanvasEditorControl!.ViewableAreaWidth);
            var parentHeight = (float)(Parent?.ActualHeight ?? m_CanvasEditorControl!.ViewableAreaHeight);

            switch (WidthSizePolicy)
            {
                case SizePolicy.Fixed:
                    ActualWidth = Math.Max(MinWidth, MaxWidth == 0 ? SuggestedWidth : Math.Min(MaxWidth, SuggestedWidth));
                    Width = ActualWidth + MarginLeft + MarginRight;
                    break;
                case SizePolicy.StretchToParent:
                {
                    var actualWidth = parentWidth - MarginLeft - MarginRight;
                    ActualWidth = Math.Max(MinWidth, MaxWidth == 0 ? actualWidth : Math.Min(MaxWidth, actualWidth));
                    Width = parentWidth;
                    break;
                }
                case SizePolicy.CoverChildren:
                {
                    var minX = float.MaxValue;
                    var maxX = float.MinValue;
                    foreach (WidgetViewModel child in Children)
                    {
                        if (child.X < minX)
                        {
                            minX = child.X;
                        }

                        var childRight = child.X + child.Width;
                        if (childRight > maxX)
                        {
                            maxX = childRight;
                        }
                    }

                    var actualWidth = maxX - minX;
                    ActualWidth = Math.Max(MinWidth, MaxWidth == 0 ? actualWidth : Math.Min(MaxWidth, actualWidth));
                    Width = ActualWidth + MarginLeft + MarginRight;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (HeightSizePolicy)
            {
                case SizePolicy.Fixed:
                    ActualHeight = Math.Max(MinHeight, MaxHeight == 0 ? SuggestedHeight : Math.Min(MaxHeight, SuggestedHeight));
                    Height = ActualHeight + MarginTop + MarginBottom;
                    break;
                case SizePolicy.StretchToParent:
                {
                    var actualHeight = parentHeight - MarginTop - MarginBottom;
                    ActualHeight = Math.Max(MinHeight, MaxHeight == 0 ? actualHeight : Math.Min(MaxHeight, actualHeight));
                    Height = parentHeight;
                    break;
                }
                case SizePolicy.CoverChildren:
                {
                    var minY = float.MaxValue;
                    var maxY = float.MinValue;
                    foreach (WidgetViewModel child in Children)
                    {
                        if (child.Y < minY)
                        {
                            minY = child.Y;
                        }

                        var childBottom = child.Y + child.Height;
                        if (childBottom > maxY)
                        {
                            maxY = childBottom;
                        }
                    }

                    var actualHeight = maxY - minY;
                    ActualHeight = Math.Max(MinHeight, MaxHeight == 0 ? actualHeight : Math.Min(MaxHeight, actualHeight));
                    Height = ActualHeight + MarginTop + MarginBottom;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void RefreshPosition()
        {
            var parentX = Parent?.ActualX ?? 0;
            var parentY = Parent?.ActualY ?? 0;
            var parentWidth = (float)(Parent?.ActualWidth ?? m_CanvasEditorControl!.ViewableAreaWidth);
            var parentHeight = (float)(Parent?.ActualHeight ?? m_CanvasEditorControl!.ViewableAreaHeight);

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    X = parentX;
                    ActualX = X + MarginLeft;
                    break;
                case HorizontalAlignment.Center:
                    X = (int)(parentX + (parentWidth * 0.5f) - (Width * 0.5f));
                    ActualX = (int)(X + (MarginLeft * 0.5f) + (MarginRight * 0.5f));
                    break;
                case HorizontalAlignment.Right:
                    X = parentX + parentWidth - ActualWidth;
                    ActualX = X - MarginRight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            X += PositionXOffset;
            ActualX += PositionXOffset;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Y = parentY;
                    ActualY = Y + MarginTop;
                    break;
                case VerticalAlignment.Center:
                    Y = (int)(parentY + (parentHeight * 0.5f) - (Height * 0.5f));
                    ActualY = (int)(Y + (MarginTop * 0.5f) + (MarginBottom * 0.5f));
                    break;
                case VerticalAlignment.Bottom:
                    Y = parentY + parentHeight - ActualHeight;
                    ActualY = Y - MarginBottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Y += PositionYOffset;
            ActualY += PositionYOffset;
        }

        protected virtual void OnDisposing(IConnectedObject _e)
        {
            Disposing?.Invoke(this, _e);
        }

        private void OnMouseLeave(object _sender, MouseEventArgs _e)
        {
            m_IsMouseOver = false;
            UpdateVisualFocus();
        }

        private void OnMouseDown(object _sender, MouseButtonEventArgs _e)
        {
            m_FocusManager.SetFocus(this);
        }

        private void OnMouseMove(object _sender, MouseEventArgs _e)
        {
            m_IsMouseOver = true;
            UpdateVisualFocus();
        }

        private bool m_IsMouseOver;

        private void UpdateVisualFocus()
        {
            if(m_IsMouseOver && IsFocused)
            {
                m_CanvasEditorControl?.Dispatcher.Invoke(() =>
                {
                    Rectangle!.StrokeThickness = 3;
                    Rectangle.Stroke = new SolidColorBrush(Color.FromRgb(125, 236, 250));
                    Rectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 201, 227)) { Opacity = 0.5 };
                });
            }
            else if (m_IsMouseOver)
            {
                m_CanvasEditorControl?.Dispatcher.Invoke(() =>
                {
                    Rectangle!.StrokeThickness = 2;
                    Rectangle.Stroke = new SolidColorBrush(Color.FromRgb(53, 203, 222));
                    Rectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 163, 184)) { Opacity = 0.3 };
                });
            }
            else if (IsFocused)
            {
                m_CanvasEditorControl?.Dispatcher.Invoke(() =>
                {
                    Rectangle!.StrokeThickness = 2;
                    Rectangle.Stroke = new SolidColorBrush(Color.FromRgb(92, 221, 237));
                    Rectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 183, 207)) { Opacity = 0.4 };
                });
            }
            else
            {
                m_CanvasEditorControl?.Dispatcher.Invoke(() =>
                {
                    Rectangle!.StrokeThickness = 1;
                    Rectangle.Stroke = new SolidColorBrush(Color.FromRgb(24, 184, 204));
                    Rectangle.Fill = new SolidColorBrush(Color.FromRgb(0, 147, 166)) {Opacity = 0.2};
                });
            }
        }

        /// <summary>
        /// TODO: Should probably change the way this works and subscribe to required attributes directly.
        /// Also initialize all of these properties.
        /// </summary>
        private void WidgetPropertyChanged(UIEditorWidgetAttribute _sender, string _attributeName, object? _value)
        {
            switch (_attributeName)
            {
                case "MarginTop":
                    MarginTop = Convert.ToSingle(_value);
                    break;
                case "MarginLeft":
                    MarginLeft = Convert.ToSingle(_value);
                    break;
                case "MarginBottom":
                    MarginBottom = Convert.ToSingle(_value);
                    break;
                case "MarginRight":
                    MarginRight = Convert.ToSingle(_value);
                    break;
                case "VerticalAlignment":
                    VerticalAlignment = (VerticalAlignment)_value!;
                    break;
                case "HorizontalAlignment":
                    HorizontalAlignment = (HorizontalAlignment)_value!;
                    break;
                case "SuggestedWidth":
                    SuggestedWidth = Convert.ToSingle(_value);
                    break;
                case "SuggestedHeight":
                    SuggestedHeight = Convert.ToSingle(_value);
                    break;
                case "WidthSizePolicy":
                    WidthSizePolicy = (SizePolicy)_value!;
                    break;
                case "HeightSizePolicy":
                    HeightSizePolicy = (SizePolicy)_value!;
                    break;
                case "MaxWidth":
                    MaxWidth = Convert.ToSingle(_value);
                    break;
                case "MaxHeight":
                    MaxHeight = Convert.ToSingle(_value);
                    break;
                case "MinWidth":
                    MinWidth = Convert.ToSingle(_value);
                    break;
                case "MinHeight":
                    MinHeight = Convert.ToSingle(_value);
                    break;
                case "PositionXOffset":
                    PositionXOffset = Convert.ToSingle(_value);
                    break;
                case "PositionYOffset":
                    PositionYOffset = Convert.ToSingle(_value);
                    break;
            }
        }

        private void UpdateVisibility()
        {
            m_CanvasEditorControl?.Dispatcher.Invoke(() =>
            {
                var rectangleInCanvas = m_CanvasEditorControl.Canvas.Children.Contains(Rectangle);
                if ((ParentIsHidden || !IsVisible) && rectangleInCanvas)
                {
                    m_CanvasEditorControl.Canvas.Children.Remove(Rectangle);
                }
                else if (!rectangleInCanvas && Rectangle is not null)
                {
                    m_CanvasEditorControl.Canvas.Children.Add(Rectangle);
                }
            });

            foreach (WidgetViewModel child in Children)
            {
                child.UpdateVisibility();
            }
        }

        private void UpdateRectangle()
        {
            if (Rectangle is null)
            {
                return;
            }

            // TODO: update parent if child size changes.
            if (WidthSizePolicy == SizePolicy.CoverChildren || HeightSizePolicy == SizePolicy.CoverChildren)
            {
                foreach (WidgetViewModel child in Children)
                {
                    child.RefreshSize();
                }
            }

            RefreshWidget();
            m_CanvasEditorControl?.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(Rectangle, ActualX);
                Canvas.SetTop(Rectangle, ActualY);
                Rectangle.Width = ActualWidth;
                Rectangle.Height = ActualHeight;
                Panel.SetZIndex(Rectangle, ZIndex);
            });

            foreach (WidgetViewModel child in Children)
            {
                child.UpdateRectangle();
            }
        }
    }
}
