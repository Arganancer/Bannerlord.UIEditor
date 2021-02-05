using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public class WidgetViewModel : INotifyPropertyChanged
    {
        #region Properties

        public IReadOnlyList<WidgetViewModel> Children => m_Children;

        public bool IsVisible
        {
            get => m_IsVisible;
            set
            {
                if (m_IsVisible != value)
                {
                    m_IsVisible = value;
                    foreach (WidgetViewModel child in m_Children)
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
                    Panel.SetZIndex(m_Rectangle, ZIndex);
                    foreach (WidgetViewModel child in m_Children)
                    {
                        child.ZIndex = m_ZIndex + 1;
                    }
                }
            }
        }

        public int X
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

        public int Y
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

        public int Width
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

        public int Height
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

        protected bool ParentIsHidden
        {
            get => m_ParentIsHidden;
            set
            {
                if (m_ParentIsHidden != value)
                {
                    m_ParentIsHidden = value;
                    foreach (WidgetViewModel child in m_Children)
                    {
                        child.ParentIsHidden = ParentIsHidden || !IsVisible;
                    }

                    UpdateVisibility();
                }
            }
        }

        #endregion

        #region Fields

        private readonly Canvas m_Canvas;

        private readonly List<WidgetViewModel> m_Children;

        private int m_X;
        private int m_Y;
        private int m_Width;
        private int m_Height;
        private bool m_IsVisible;
        private int m_ZIndex;
        private Rectangle m_Rectangle;
        private bool m_ParentIsHidden;

        #endregion

        #region Constructors

        public WidgetViewModel(UIEditorWidget _widget, Canvas _canvas, int _zIndex)
        {
            IsVisible = true;
            ParentIsHidden = false;

            m_Canvas = _canvas;
            m_Rectangle = new Rectangle();
            m_Children = new List<WidgetViewModel>();
            Widget = _widget;
            ZIndex = _zIndex;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Public Methods

        public void AddChildren(int _index, params WidgetViewModel[] _children)
        {
            foreach (WidgetViewModel child in _children)
            {
                child.ParentIsHidden = !IsVisible || ParentIsHidden;
            }

            m_Children.InsertRange(_index, _children);
        }

        public void RemoveChildren(params WidgetViewModel[] _children)
        {
            foreach (WidgetViewModel child in _children)
            {
                m_Children.Remove(child);
            }
        }

        #endregion

        #region Protected Methods

        protected void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        #endregion

        #region Private Methods

        private void UpdateVisibility()
        {
            var rectangleInCanvas = m_Canvas.Children.Contains(m_Rectangle);
            if ((ParentIsHidden || !IsVisible) && rectangleInCanvas)
            {
                m_Canvas.Children.Remove(m_Rectangle);
            }
            else if (!rectangleInCanvas)
            {
                m_Canvas.Children.Add(m_Rectangle);
            }
        }

        private void UpdateRectangle()
        {
            m_Rectangle.Width = Width;
            m_Rectangle.Height = Height;
            Canvas.SetLeft(m_Rectangle, X);
            Canvas.SetTop(m_Rectangle, Y);
        }

        #endregion
    }
}
