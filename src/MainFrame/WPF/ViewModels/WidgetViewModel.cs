using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public class WidgetViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WidgetViewModel> Children { get; set; }

        public string Name
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
                    Panel.SetZIndex(m_Rectangle, ZIndex);
                    foreach (WidgetViewModel child in Children)
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

        public bool IsReadonly
        {
            get => m_IsReadonly;
            set
            {
                if (m_IsReadonly != value)
                {
                    m_IsReadonly = value;
                    OnPropertyChanged();
                }
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

        private readonly ICanvasEditorControl m_CanvasEditorControl;

        private int m_X;
        private int m_Y;
        private int m_Width;
        private int m_Height;
        private bool m_IsVisible;
        private int m_ZIndex;
        private Rectangle m_Rectangle;
        private bool m_ParentIsHidden;
        private string m_Name;
        private bool m_IsReadonly;

        public WidgetViewModel(string _name, UIEditorWidget _widget, ICanvasEditorControl _canvasEditorControl)
        {
            Children = new ObservableCollection<WidgetViewModel>();
            m_CanvasEditorControl = _canvasEditorControl;
            m_CanvasEditorControl.Dispatcher.Invoke(() => { m_Rectangle = new Rectangle(); });

            Name = _name;
            IsVisible = true;
            ParentIsHidden = false;

            Widget = _widget;
            ZIndex = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddChildren(int _index, params WidgetViewModel[] _children)
        {
            foreach (WidgetViewModel child in _children)
            {
                m_CanvasEditorControl.Dispatcher.Invoke(() =>
                {
                    child.ParentIsHidden = !IsVisible || ParentIsHidden;
                    child.ZIndex = ZIndex + 1;
                });
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
            foreach (WidgetViewModel child in _children)
            {
                Children.Remove(child);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
        }

        private void UpdateVisibility()
        {
            m_CanvasEditorControl.Dispatcher.Invoke(() =>
            {
                var rectangleInCanvas = m_CanvasEditorControl.Canvas.Children.Contains(m_Rectangle);
                if ((ParentIsHidden || !IsVisible) && rectangleInCanvas)
                {
                    m_CanvasEditorControl.Canvas.Children.Remove(m_Rectangle);
                }
                else if (!rectangleInCanvas)
                {
                    m_CanvasEditorControl.Canvas.Children.Add(m_Rectangle);
                }
            });
        }

        private void UpdateRectangle()
        {
            m_CanvasEditorControl.Dispatcher.Invoke(() =>
            {
                m_Rectangle.Width = Width;
                m_Rectangle.Height = Height;
                Canvas.SetLeft(m_Rectangle, X);
                Canvas.SetTop(m_Rectangle, Y);
            });
        }
    }
}
