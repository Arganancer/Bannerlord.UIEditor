using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    /// <summary>
    /// Reference: https://channel9.msdn.com/Blogs/OneCode/How-to-resize-WPF-panel-at-Runtime
    /// </summary>
    public class Resizer : Thumb
    {
        public static DependencyProperty CursorIconProperty = DependencyProperty.Register("CursorIcon", typeof( CursorIcon ), typeof( ResizableControl ));
        public static DependencyProperty ThumbDirectionProperty = DependencyProperty.Register("ThumbDirection", typeof( ResizeDirection ), typeof( Resizer ));

        public ResizeDirection ThumbDirection
        {
            get => (ResizeDirection)GetValue(ThumbDirectionProperty);
            set => SetValue(ThumbDirectionProperty, value);
        }

        public CursorIcon CursorIcon
        {
            get => (CursorIcon)GetValue(CursorIconProperty);
            set
            {
                SetValue(CursorIconProperty, value);
                Cursor = CursorManager.Instance!.GetCursor(value);
            }
        }

        private string m_SettingsNamePrefix = null!;
        private SettingsManager m_SettingsManager;

        static Resizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( Resizer ), new FrameworkPropertyMetadata(typeof( Resizer )));
        }

        public Resizer()
        {
            m_SettingsManager = SettingsManager.Instance!;
            DragDelta += Resizer_DragDelta;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_SettingsNamePrefix = this.GetVisualAncestorOfType<ResizableControl>()!.Name;

            if (DataContext is Control designerItem)
            {
                var width = m_SettingsManager.GetSetting<double?>($"{m_SettingsNamePrefix}_Width");
                if (width is not null)
                {
                    designerItem.Width = (double)width;
                }

                var height = m_SettingsManager.GetSetting<double?>($"{m_SettingsNamePrefix}_Height");
                if (height is not null)
                {
                    designerItem.Height = (double)height;
                }
            }
        }

        private void Resizer_DragDelta(object _sender, DragDeltaEventArgs _dragDeltaEvent)
        {
            if (DataContext is Control designerItem)
            {
                switch (ThumbDirection)
                {
                    case ResizeDirection.TopLeft:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Left:
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.BottomLeft:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        ResizeLeft(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Bottom:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.BottomRight:
                        ResizeBottom(_dragDeltaEvent, designerItem);
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Right:
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.TopRight:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        ResizeRight(_dragDeltaEvent, designerItem);
                        break;
                    case ResizeDirection.Top:
                        ResizeTop(_dragDeltaEvent, designerItem);
                        break;
                }
            }

            _dragDeltaEvent.Handled = true;
        }

        private void ResizeRight(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(-_dragDeltaEvent.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
            _designerItem.Width -= deltaHorizontal;
            m_SettingsManager.SetSetting($"{m_SettingsNamePrefix}_Width", _designerItem.Width);
        }

        private void ResizeTop(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(_dragDeltaEvent.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
            _designerItem.Height -= deltaHorizontal;
            m_SettingsManager.SetSetting($"{m_SettingsNamePrefix}_Height", _designerItem.Height);
        }

        private void ResizeLeft(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(_dragDeltaEvent.HorizontalChange, _designerItem.ActualWidth - _designerItem.MinWidth);
            _designerItem.Width -= deltaHorizontal;
            m_SettingsManager.SetSetting($"{m_SettingsNamePrefix}_Width", _designerItem.Width);
        }

        private void ResizeBottom(DragDeltaEventArgs _dragDeltaEvent, Control _designerItem)
        {
            var deltaHorizontal = Math.Min(-_dragDeltaEvent.VerticalChange, _designerItem.ActualHeight - _designerItem.MinHeight);
            _designerItem.Height -= deltaHorizontal;
            m_SettingsManager.SetSetting($"{m_SettingsNamePrefix}_Height", _designerItem.Height);
        }
    }

    public enum ResizeDirection
    {
        TopLeft,
        Left,
        BottomLeft,
        Bottom,
        BottomRight,
        Right,
        TopRight,
        Top
    }
}
