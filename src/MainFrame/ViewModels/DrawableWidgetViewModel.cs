using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Resources;
using Bannerlord.UIEditor.MainFrame.Resources.Resizer;
using Bannerlord.UIEditor.WidgetLibrary;
using TaleWorlds.GauntletUI;
using Canvas = System.Windows.Controls.Canvas;
using HorizontalAlignment = TaleWorlds.GauntletUI.HorizontalAlignment;
using VerticalAlignment = TaleWorlds.GauntletUI.VerticalAlignment;

namespace Bannerlord.UIEditor.MainFrame
{
    public class DrawableWidgetViewModel : WidgetViewModel
    {
        private event BoundAttributePropertyChangedEventHandler? BoundAttributePropertyChanged;

        private delegate void BoundAttributePropertyChangedEventHandler(string _propertyName, object? _value);

        private const float Tolerance = 0.0000000001f;
        public DrawableWidgetViewModel? Parent { get; protected set; }
        public ObservableCollection<DrawableWidgetViewModel> Children { get; set; } = new();

        public bool IsDirty { get; private set; }

        public bool IsVisible
        {
            get => m_IsVisible;
            set
            {
                if (m_IsVisible != value)
                {
                    m_IsVisible = value;
                    foreach (DrawableWidgetViewModel child in Children)
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
                    SetDirty();
                    foreach (DrawableWidgetViewModel child in Children)
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
                if (Math.Abs(m_X - value) > Tolerance)
                {
                    m_X = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public float Y
        {
            get => m_Y;
            set
            {
                if (Math.Abs(m_Y - value) > Tolerance)
                {
                    m_Y = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public float Width
        {
            get => m_Width;
            set
            {
                if (Math.Abs(m_Width - value) > Tolerance)
                {
                    m_Width = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public float Height
        {
            get => m_Height;
            set
            {
                if (Math.Abs(m_Height - value) > Tolerance)
                {
                    m_Height = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        protected float ActualX { get; set; }

        protected float ActualY { get; set; }

        protected float ActualWidth { get; set; }
        protected float ActualHeight { get; set; }

        protected bool ParentIsHidden
        {
            get => m_ParentIsHidden;
            set
            {
                if (m_ParentIsHidden != value)
                {
                    m_ParentIsHidden = value;
                    foreach (DrawableWidgetViewModel child in Children)
                    {
                        child.ParentIsHidden = ParentIsHidden || !IsVisible;
                    }

                    UpdateVisibility();
                }
            }
        }

        protected WidgetRectangle? Rectangle
        {
            get => m_Rectangle;
            set
            {
                if (m_Rectangle != value)
                {
                    if (m_Rectangle is not null)
                    {
                        m_Rectangle.MouseMove -= OnMouseMove;
                        m_Rectangle.MouseDown -= OnMouseDown;
                    }

                    m_Rectangle = value;
                    if (m_Rectangle is not null)
                    {
                        m_Rectangle.MouseMove += OnMouseMove;
                        m_Rectangle.MouseDown += OnMouseDown;
                    }
                }
            }
        }

        protected float SuggestedWidth
        {
            get => m_SuggestedWidth;
            set
            {
                if (Math.Abs(m_SuggestedWidth - value) < Tolerance)
                {
                    return;
                }

                m_SuggestedWidth = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float SuggestedHeight
        {
            get => m_SuggestedHeight;
            set
            {
                if (Math.Abs(m_SuggestedHeight - value) < Tolerance)
                {
                    return;
                }

                m_SuggestedHeight = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MarginTop
        {
            get => m_MarginTop;
            set
            {
                if (Math.Abs(m_MarginTop - value) < Tolerance)
                {
                    return;
                }

                m_MarginTop = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MarginLeft
        {
            get => m_MarginLeft;
            set
            {
                if (Math.Abs(m_MarginLeft - value) < Tolerance)
                {
                    return;
                }

                m_MarginLeft = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MarginBottom
        {
            get => m_MarginBottom;
            set
            {
                if (Math.Abs(m_MarginBottom - value) < Tolerance)
                {
                    return;
                }

                m_MarginBottom = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MarginRight
        {
            get => m_MarginRight;
            set
            {
                if (Math.Abs(m_MarginRight - value) < Tolerance)
                {
                    return;
                }

                m_MarginRight = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected VerticalAlignment VerticalAlignment
        {
            get => m_VerticalAlignment;
            set
            {
                if (m_VerticalAlignment == value)
                {
                    return;
                }

                m_VerticalAlignment = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected HorizontalAlignment HorizontalAlignment
        {
            get => m_HorizontalAlignment;
            set
            {
                if (m_HorizontalAlignment == value)
                {
                    return;
                }

                m_HorizontalAlignment = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected SizePolicy WidthSizePolicy
        {
            get => m_WidthSizePolicy;
            set
            {
                if (m_WidthSizePolicy == value)
                {
                    return;
                }

                m_WidthSizePolicy = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected SizePolicy HeightSizePolicy
        {
            get => m_HeightSizePolicy;
            set
            {
                if (m_HeightSizePolicy == value)
                {
                    return;
                }

                m_HeightSizePolicy = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MaxWidth
        {
            get => m_MaxWidth;
            set
            {
                if (Math.Abs(m_MaxWidth - value) < Tolerance)
                {
                    return;
                }

                m_MaxWidth = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MaxHeight
        {
            get => m_MaxHeight;
            set
            {
                if (Math.Abs(m_MaxHeight - value) < Tolerance)
                {
                    return;
                }

                m_MaxHeight = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MinWidth
        {
            get => m_MinWidth;
            set
            {
                if (Math.Abs(m_MinWidth - value) < Tolerance)
                {
                    return;
                }

                m_MinWidth = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float MinHeight
        {
            get => m_MinHeight;
            set
            {
                if (Math.Abs(m_MinHeight - value) < Tolerance)
                {
                    return;
                }

                m_MinHeight = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float PositionXOffset
        {
            get => m_PositionXOffset;
            set
            {
                if (Math.Abs(m_PositionXOffset - value) < Tolerance)
                {
                    return;
                }

                m_PositionXOffset = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        protected float PositionYOffset
        {
            get => m_PositionYOffset;
            set
            {
                if (Math.Abs(m_PositionYOffset - value) < Tolerance)
                {
                    return;
                }

                m_PositionYOffset = value;
                SetDirty();
                OnBoundAttributePropertyChanged(value);
            }
        }

        private float ParentActualWidth => (float)(Parent?.ActualWidth ?? m_CanvasEditorControl!.ViewableAreaWidth);
        private float ParentActualHeight => (float)(Parent?.ActualHeight ?? m_CanvasEditorControl!.ViewableAreaHeight);
        private float ParentActualX => Parent?.ActualX ?? 0;
        private float ParentActualY => Parent?.ActualY ?? 0;

        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;
        private bool m_IsVisible;
        private int m_ZIndex;
        private WidgetRectangle? m_Rectangle;
        private bool m_ParentIsHidden;
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

        private ResizeDirection? m_ResizeDirection;

        private bool m_IsMovingWidget;

        private PreMoveState m_PreMoveState;

        private readonly ICanvasEditorControl? m_CanvasEditorControl;
        private readonly IFocusManager m_FocusManager;
        private bool m_WidgetPropertyChanged;

        public DrawableWidgetViewModel(string _name,
            UIEditorWidget _widget,
            ICanvasEditorControl? _canvasEditorControl,
            IPublicContainer _publicContainer) : base(_name, _widget, _publicContainer)
        {
            IsReadonly = false;
            m_CanvasEditorControl = _canvasEditorControl;
            m_CanvasEditorControl?.Dispatcher.Invoke(() => { Rectangle = new WidgetRectangle(); });
            m_FocusManager = PublicContainer.GetModule<IFocusManager>();

            IsVisible = true;
            ParentIsHidden = false;

            ZIndex = 0;

            Widget.PropertyChanged += OnWidgetPropertyChanged;

            BindWidgetAttributes();
            UpdateVisibility();
            SetDirty();
        }

        public override bool IsFocused
        {
            get => base.IsFocused;
            set
            {
                if (Rectangle is not null)
                {
                    m_CanvasEditorControl?.Canvas.Dispatcher.Invoke(() => Rectangle.IsWidgetFocused = value);
                }

                base.IsFocused = value;
            }
        }

        protected override void OnDisposing(IConnectedObject _e)
        {
            m_CanvasEditorControl?.Canvas.Children.Remove(Rectangle);
            Widget.PropertyChanged -= OnWidgetPropertyChanged;
            Rectangle = null;
            Dispose();
        }

        public virtual void ToXml(XmlNode _parent)
        {
            XmlElement widgetElement = _parent.OwnerDocument!.CreateElement(Widget.Name);
            foreach (UIEditorWidgetAttribute attribute in Widget.Attributes.Where(_a => !_a.IsDefaultValue))
            {
                widgetElement.SetAttribute(attribute.Name, attribute.ValueAsString);
            }

            XmlNode nextParent = _parent.AppendChild(widgetElement);
            if (Children.Any())
            {
                nextParent = nextParent.AppendChild(nextParent.OwnerDocument!.CreateElement("Children"));
            }

            foreach (var child in Children)
            {
                child.ToXml(nextParent);
            }
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public bool WidgetExistsInParents(DrawableWidgetViewModel _widgetViewModel)
        {
            return Parent is not null && (Parent.Equals(_widgetViewModel) || Parent.WidgetExistsInParents(_widgetViewModel));
        }

        public void AddChildren(int _index, params DrawableWidgetViewModel[] _children)
        {
            foreach (DrawableWidgetViewModel child in _children)
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
                foreach (DrawableWidgetViewModel child in _children)
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

        public int GetIndexOfChild(DrawableWidgetViewModel _child)
        {
            return Children.IndexOf(_child);
        }

        public void AddChildren(params DrawableWidgetViewModel[] _children)
        {
            AddChildren(int.MaxValue, _children);
        }

        public void RemoveChildren(params DrawableWidgetViewModel[] _children)
        {
            foreach (DrawableWidgetViewModel childToRemove in _children)
            {
                Children.Remove(childToRemove);
            }

            foreach (DrawableWidgetViewModel child in Children)
            {
                child.RemoveChildren(_children);
            }
        }

        public bool Update()
        {
            var updated = m_WidgetPropertyChanged;
            m_WidgetPropertyChanged = false;
            if (IsDirty)
            {
                IsDirty = false;
                updated = true;
                UpdateRectangle();
            }

            foreach (var child in Children)
            {
                updated = updated || child.Update();
            }

            UpdateCursor(m_ResizeDirection);

            return updated;
        }


        protected void RefreshWidget()
        {
            RefreshSize();
            RefreshPosition();
        }

        protected void RefreshSize()
        {
            switch (WidthSizePolicy)
            {
                case SizePolicy.Fixed:
                    ActualWidth = Math.Max(MinWidth, MaxWidth == 0 ? SuggestedWidth : Math.Min(MaxWidth, SuggestedWidth));
                    Width = ActualWidth + MarginLeft + MarginRight;
                    break;
                case SizePolicy.StretchToParent:
                {
                    var actualWidth = ParentActualWidth - MarginLeft - MarginRight;
                    ActualWidth = Math.Max(MinWidth, MaxWidth == 0 ? actualWidth : Math.Min(MaxWidth, actualWidth));
                    Width = ParentActualWidth;
                    break;
                }
                case SizePolicy.CoverChildren:
                {
                    var minX = float.MaxValue;
                    var maxX = float.MinValue;
                    foreach (DrawableWidgetViewModel child in Children)
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
                    var actualHeight = ParentActualHeight - MarginTop - MarginBottom;
                    ActualHeight = Math.Max(MinHeight, MaxHeight == 0 ? actualHeight : Math.Min(MaxHeight, actualHeight));
                    Height = ParentActualHeight;
                    break;
                }
                case SizePolicy.CoverChildren:
                {
                    var minY = float.MaxValue;
                    var maxY = float.MinValue;
                    foreach (DrawableWidgetViewModel child in Children)
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
            if (WidthSizePolicy == SizePolicy.StretchToParent)
            {
                X = ParentActualX;
                ActualX = X + MarginLeft;
            }
            else
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        X = ParentActualX;
                        ActualX = X + MarginLeft;
                        break;
                    case HorizontalAlignment.Center:
                        X = (int)(ParentActualX + (ParentActualWidth * 0.5f) - (ActualWidth * 0.5f));
                        ActualX = (int)(X + (MarginLeft * 0.5f) - (MarginRight * 0.5f));
                        break;
                    case HorizontalAlignment.Right:
                        X = ParentActualX + ParentActualWidth - ActualWidth;
                        ActualX = X - MarginRight;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            X += PositionXOffset;
            ActualX += PositionXOffset;

            if (HeightSizePolicy == SizePolicy.StretchToParent)
            {
                Y = ParentActualY;
                ActualY = Y + MarginTop;
            }
            else
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        Y = ParentActualY;
                        ActualY = Y + MarginTop;
                        break;
                    case VerticalAlignment.Center:
                        Y = (int)(ParentActualY + (ParentActualHeight * 0.5f) - (ActualHeight * 0.5f));
                        ActualY = (int)(Y + (MarginTop * 0.5f) - (MarginBottom * 0.5f));
                        break;
                    case VerticalAlignment.Bottom:
                        Y = ParentActualY + ParentActualHeight - ActualHeight;
                        ActualY = Y - MarginBottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Y += PositionYOffset;
            ActualY += PositionYOffset;
        }

        private void OnWidgetPropertyChanged(UIEditorWidgetAttribute _sender, string _attributeName, object? _value)
        {
            m_WidgetPropertyChanged = true;
        }

        private void OnBoundAttributePropertyChanged(object? _value, [CallerMemberName] string? _propertyName = null)
        {
            BoundAttributePropertyChanged?.Invoke(_propertyName!, _value);
        }

        private void OnMouseDown(object _sender, MouseButtonEventArgs _e)
        {
            if (_e.ChangedButton == MouseButton.Left)
            {
                m_FocusManager.SetFocus(this);
                m_ResizeDirection = GetResizeDirection(_e.GetPosition(m_Rectangle));
                if (m_ResizeDirection is null)
                {
                    m_PreMoveState = new PreMoveState(_e.GetPosition(m_CanvasEditorControl!.Canvas), PositionXOffset, PositionYOffset, MarginLeft, MarginRight, MarginBottom, MarginTop);
                    m_IsMovingWidget = true;
                }

                m_CanvasEditorControl!.Canvas.CaptureMouse();
                m_CanvasEditorControl.CanvasEditorMouseMove += OnCanvasEditorMouseMove;
                m_CanvasEditorControl.CanvasEditorMouseUp += OnCanvasEditorMouseUp;
            }
        }

        private void OnCanvasEditorMouseUp(object _sender, MouseButtonEventArgs _e)
        {
            if (_e.ChangedButton == MouseButton.Left)
            {
                m_IsMovingWidget = false;
                m_ResizeDirection = null;
                m_CanvasEditorControl!.Canvas.ReleaseMouseCapture();
                m_CanvasEditorControl.CanvasEditorMouseMove -= OnCanvasEditorMouseMove;
                m_CanvasEditorControl.CanvasEditorMouseUp -= OnCanvasEditorMouseUp;

                SuggestedHeight = Math.Max(0, SuggestedHeight);
                SuggestedWidth = Math.Max(0, SuggestedWidth);
            }
        }

        private void OnCanvasEditorMouseMove(object _sender, MouseEventArgs _e)
        {
            UpdateCursor(m_ResizeDirection);
            if (m_ResizeDirection is not null)
            {
                ResizeWidget(_e);
            }
            else if (m_IsMovingWidget)
            {
                MoveWidget(_e);
            }
        }

        private void MoveWidget(MouseEventArgs _e)
        {
            var mousePosition = _e.GetPosition(m_CanvasEditorControl!.Canvas);
            switch (WidthSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            MarginLeft = (float)(m_PreMoveState.MarginLeft + mousePosition.X - m_PreMoveState.MousePosition.X);
                            break;
                        case HorizontalAlignment.Center:
                            PositionXOffset = (int)(m_PreMoveState.PositionXOffset + mousePosition.X - m_PreMoveState.MousePosition.X);
                            break;
                        case HorizontalAlignment.Right:
                            MarginRight = (float)(m_PreMoveState.MarginRight - (mousePosition.X - m_PreMoveState.MousePosition.X));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                case SizePolicy.CoverChildren:
                    PositionXOffset = (int)(m_PreMoveState.PositionXOffset + mousePosition.X - m_PreMoveState.MousePosition.X);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (HeightSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            MarginTop = (int)(m_PreMoveState.MarginTop + mousePosition.Y - m_PreMoveState.MousePosition.Y);
                            break;
                        case VerticalAlignment.Center:
                            PositionYOffset = (int)(m_PreMoveState.PositionYOffset + mousePosition.Y - m_PreMoveState.MousePosition.Y);
                            break;
                        case VerticalAlignment.Bottom:
                            MarginBottom = (int)(m_PreMoveState.MarginBottom - (mousePosition.Y - m_PreMoveState.MousePosition.Y));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                case SizePolicy.CoverChildren:
                    PositionYOffset = (int)(m_PreMoveState.PositionYOffset + mousePosition.Y - m_PreMoveState.MousePosition.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMouseMove(object _sender, MouseEventArgs _e)
        {
            if (m_ResizeDirection is null)
            {
                var mousePosition = _e.GetPosition(m_Rectangle);
                UpdateCursor(GetResizeDirection(mousePosition));
            }
        }

        private void ResizeWidget(MouseEventArgs _e)
        {
            var mousePosition = _e.GetPosition(m_CanvasEditorControl!.Canvas);
            switch (m_ResizeDirection)
            {
                case ResizeDirection.TopLeft:
                    ResizeTop(mousePosition);
                    ResizeLeft(mousePosition);
                    break;
                case ResizeDirection.Left:
                    ResizeLeft(mousePosition);
                    break;
                case ResizeDirection.BottomLeft:
                    ResizeBottom(mousePosition);
                    ResizeLeft(mousePosition);
                    break;
                case ResizeDirection.Bottom:
                    ResizeBottom(mousePosition);
                    break;
                case ResizeDirection.BottomRight:
                    ResizeBottom(mousePosition);
                    ResizeRight(mousePosition);
                    break;
                case ResizeDirection.Right:
                    ResizeRight(mousePosition);
                    break;
                case ResizeDirection.TopRight:
                    ResizeTop(mousePosition);
                    ResizeRight(mousePosition);
                    break;
                case ResizeDirection.Top:
                    ResizeTop(mousePosition);
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResizeBottom(Point _mousePosition)
        {
            switch (HeightSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            SuggestedHeight = (float)(_mousePosition.Y - ActualY);
                            break;
                        case VerticalAlignment.Center:
                            SuggestedHeight = (float)(_mousePosition.Y - ActualY);
                            break;
                        case VerticalAlignment.Bottom:
                            var newHeight = (float)(_mousePosition.Y - ActualY);
                            var difference = newHeight - SuggestedHeight;
                            SuggestedHeight = newHeight;
                            MarginBottom -= difference;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                    MarginBottom = (float)(ParentActualY + ParentActualHeight - _mousePosition.Y);
                    break;
                case SizePolicy.CoverChildren:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResizeRight(Point _mousePosition)
        {
            switch (WidthSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            SuggestedWidth = (float)(_mousePosition.X - ActualX);
                            break;
                        case HorizontalAlignment.Center:
                            SuggestedWidth = (float)(_mousePosition.X - ActualX);
                            break;
                        case HorizontalAlignment.Right:
                            var newWidth = (float)(_mousePosition.X - ActualX);
                            var difference = newWidth - SuggestedWidth;
                            SuggestedWidth = newWidth;
                            MarginRight -= difference;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                    MarginRight = (float)(ParentActualX + ParentActualWidth - _mousePosition.X);
                    break;
                case SizePolicy.CoverChildren:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResizeLeft(Point _mousePosition)
        {
            switch (WidthSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Right:
                            SuggestedWidth = (float)(ParentActualX + ParentActualWidth - _mousePosition.X - MarginRight);
                            break;
                        case HorizontalAlignment.Center:
                            SuggestedWidth = ((float)(ParentActualX + ParentActualWidth - _mousePosition.X) * 2) - ParentActualWidth;
                            break;
                        case HorizontalAlignment.Left:
                            var newWidth = (float)(SuggestedWidth + ActualX - _mousePosition.X);
                            var difference = newWidth - SuggestedWidth;
                            SuggestedWidth = newWidth;
                            MarginLeft -= difference;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                    MarginLeft = (float)(_mousePosition.X - ParentActualX);
                    break;
                case SizePolicy.CoverChildren:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ResizeTop(Point _mousePosition)
        {
            switch (HeightSizePolicy)
            {
                case SizePolicy.Fixed:
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            SuggestedHeight = (float)(ParentActualY + ParentActualHeight - _mousePosition.Y - MarginBottom);
                            break;
                        case VerticalAlignment.Center:
                            SuggestedHeight = ((float)(ParentActualY + ParentActualHeight - _mousePosition.Y) * 2) - ParentActualHeight;
                            break;
                        case VerticalAlignment.Top:
                            var newHeight = (float)(SuggestedHeight + ActualY - _mousePosition.Y);
                            var difference = newHeight - SuggestedHeight;
                            SuggestedHeight = newHeight;
                            MarginTop -= difference;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SizePolicy.StretchToParent:
                    MarginTop = (float)(_mousePosition.Y - ParentActualY);
                    break;
                case SizePolicy.CoverChildren:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCursor(ResizeDirection? _resizeDirection)
        {
            switch (_resizeDirection)
            {
                case null:
                    if (m_IsMovingWidget)
                    {
                        CursorManager.Instance!.SetCursor(CursorIcon.MoveIcon);
                    }

                    break;
                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    CursorManager.Instance!.SetCursor(CursorIcon.SizeNWSEIcon);
                    break;
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    CursorManager.Instance!.SetCursor(CursorIcon.SizeWEIcon);
                    break;
                case ResizeDirection.BottomLeft:
                case ResizeDirection.TopRight:
                    CursorManager.Instance!.SetCursor(CursorIcon.SizeNESWIcon);
                    break;
                case ResizeDirection.Bottom:
                case ResizeDirection.Top:
                    CursorManager.Instance!.SetCursor(CursorIcon.SizeNSIcon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ResizeDirection? GetResizeDirection(Point _mousePosition)
        {
            var resizeEdgeTolerance = 8 / m_CanvasEditorControl!.CurrentZoomScale;
            if (_mousePosition.X < resizeEdgeTolerance && _mousePosition.Y < resizeEdgeTolerance) return ResizeDirection.TopLeft;
            if (_mousePosition.X > m_Rectangle!.Width - resizeEdgeTolerance && _mousePosition.Y > m_Rectangle!.Height - resizeEdgeTolerance) return ResizeDirection.BottomRight;
            if (_mousePosition.X < resizeEdgeTolerance && _mousePosition.Y > m_Rectangle!.Height - resizeEdgeTolerance) return ResizeDirection.BottomLeft;
            if (_mousePosition.X > m_Rectangle!.Width - resizeEdgeTolerance && _mousePosition.Y < resizeEdgeTolerance) return ResizeDirection.TopRight;
            if (_mousePosition.X < resizeEdgeTolerance) return ResizeDirection.Left;
            if (_mousePosition.X > m_Rectangle!.Width - resizeEdgeTolerance) return ResizeDirection.Right;
            if (_mousePosition.Y < resizeEdgeTolerance) return ResizeDirection.Top;
            if (_mousePosition.Y > m_Rectangle!.Height - resizeEdgeTolerance) return ResizeDirection.Bottom;

            return null;
        }

        private void BindWidgetAttribute(string _attributeName, Action<object?> _onPropertyChanged)
        {
            var widgetAttribute = Widget.SubscribeToAttribute(_attributeName, _onPropertyChanged);
            BoundAttributePropertyChanged += (_propertyName, _value) =>
            {
                if (_propertyName.Equals(_attributeName))
                {
                    widgetAttribute.Value = _value;
                }
            };
        }

        private void BindWidgetAttributes()
        {
            BindWidgetAttribute("MarginTop", _value => MarginTop = Convert.ToSingle(_value));
            BindWidgetAttribute("MarginLeft", _value => MarginLeft = Convert.ToSingle(_value));
            BindWidgetAttribute("MarginBottom", _value => MarginBottom = Convert.ToSingle(_value));
            BindWidgetAttribute("MarginRight", _value => MarginRight = Convert.ToSingle(_value));
            BindWidgetAttribute("VerticalAlignment", _value => VerticalAlignment = (VerticalAlignment)_value!);
            BindWidgetAttribute("HorizontalAlignment", _value => HorizontalAlignment = (HorizontalAlignment)_value!);
            BindWidgetAttribute("SuggestedWidth", _value => SuggestedWidth = Convert.ToSingle(_value));
            BindWidgetAttribute("SuggestedHeight", _value => SuggestedHeight = Convert.ToSingle(_value));
            BindWidgetAttribute("WidthSizePolicy", _value => WidthSizePolicy = (SizePolicy)_value!);
            BindWidgetAttribute("HeightSizePolicy", _value => HeightSizePolicy = (SizePolicy)_value!);
            BindWidgetAttribute("MaxWidth", _value => MaxWidth = Convert.ToSingle(_value));
            BindWidgetAttribute("MaxHeight", _value => MaxHeight = Convert.ToSingle(_value));
            BindWidgetAttribute("MinWidth", _value => MinWidth = Convert.ToSingle(_value));
            BindWidgetAttribute("MinHeight", _value => MinHeight = Convert.ToSingle(_value));
            BindWidgetAttribute("PositionXOffset", _value => PositionXOffset = Convert.ToSingle(_value));
            BindWidgetAttribute("PositionYOffset", _value => PositionYOffset = Convert.ToSingle(_value));
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

            foreach (DrawableWidgetViewModel child in Children)
            {
                child.UpdateVisibility();
            }
        }

        private void UpdateRectangle(bool _updateChildren = true, bool _updateParent = true)
        {
            if (Rectangle is null)
            {
                return;
            }

            if (_updateChildren && (WidthSizePolicy == SizePolicy.CoverChildren || HeightSizePolicy == SizePolicy.CoverChildren))
            {
                foreach (DrawableWidgetViewModel child in Children)
                {
                    child.RefreshSize();
                }
            }

            RefreshWidget();
            if (_updateParent && Parent is not null && (Parent.WidthSizePolicy == SizePolicy.CoverChildren || Parent.HeightSizePolicy == SizePolicy.CoverChildren))
            {
                Parent.UpdateRectangle(false);
            }

            m_CanvasEditorControl?.Dispatcher.Invoke(() =>
            {
                Canvas.SetLeft(Rectangle, ActualX);
                Canvas.SetTop(Rectangle, ActualY);
                Rectangle.Width = ActualWidth;
                Rectangle.Height = ActualHeight;
                Panel.SetZIndex(Rectangle, ZIndex);
            });

            if (_updateChildren)
            {
                foreach (DrawableWidgetViewModel child in Children)
                {
                    child.UpdateRectangle(_updateChildren, false);
                }
            }

            IsDirty = false;
        }

        private struct PreMoveState
        {
            public Point MousePosition { get; }
            public float PositionXOffset { get; }
            public float PositionYOffset { get; }
            public float MarginLeft { get; }
            public float MarginRight { get; }
            public float MarginBottom { get; }
            public float MarginTop { get; }

            public PreMoveState(Point _mousePosition, float _positionXOffset, float _positionYOffset, float _marginLeft, float _marginRight, float _marginBottom, float _marginTop)
            {
                MousePosition = _mousePosition;
                MarginLeft = _marginLeft;
                MarginRight = _marginRight;
                MarginBottom = _marginBottom;
                MarginTop = _marginTop;
                PositionXOffset = _positionXOffset;
                PositionYOffset = _positionYOffset;
            }
        }
    }
}
