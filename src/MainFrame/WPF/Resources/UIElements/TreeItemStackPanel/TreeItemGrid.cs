using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bannerlord.UIEditor.MainFrame
{
    public class TreeItemGrid : Grid
    {
        public static DependencyProperty TopBorderHighlightProperty = DependencyProperty.Register("TopBorderHighlight", typeof( bool ), typeof( TreeItemGrid ));
        public static DependencyProperty BottomBorderHighlightProperty = DependencyProperty.Register("BottomBorderHighlight", typeof( bool ), typeof( TreeItemGrid ));
        public static DependencyProperty InsertDistanceToleranceProperty = DependencyProperty.Register("InsertDistanceTolerance", typeof( int ), typeof( TreeItemGrid ));

        private bool m_BorderCanHighlight = false;

        public bool TopBorderHighlight
        {
            get => (bool)GetValue(TopBorderHighlightProperty);
            set
            {
                if (!m_BorderCanHighlight)
                {
                    value = m_BorderCanHighlight;
                }
                SetValue(TopBorderHighlightProperty, value);
                if (value)
                {
                    BottomBorderHighlight = false;
                }
            }
        }

        public bool BottomBorderHighlight
        {
            get => (bool)GetValue(BottomBorderHighlightProperty);
            set
            {
                if (!m_BorderCanHighlight)
                {
                    value = m_BorderCanHighlight;
                }
                SetValue(BottomBorderHighlightProperty, value);
                if (value)
                {
                    TopBorderHighlight = false;
                }
            }
        }

        public int InsertDistanceTolerance
        {
            get => (int)GetValue(InsertDistanceToleranceProperty);
            set => SetValue(InsertDistanceToleranceProperty, value);
        }

        static TreeItemGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( TreeItemGrid ), new FrameworkPropertyMetadata(typeof( TreeItemGrid )));
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            m_BorderCanHighlight = true;
            base.OnDragEnter(e);
        }

        protected override void OnDrop(DragEventArgs _e)
        {
            m_BorderCanHighlight = false;
            SetNoBorderHighlights();
            base.OnDrop(_e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            m_BorderCanHighlight = false;
            SetNoBorderHighlights();
            base.OnDragLeave(e);
        }

        public void SetNoBorderHighlights()
        {
            BottomBorderHighlight = false;
            TopBorderHighlight = false;
        }
    }
}
