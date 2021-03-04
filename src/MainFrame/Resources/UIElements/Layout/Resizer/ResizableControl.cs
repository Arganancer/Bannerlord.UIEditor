using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Bannerlord.ButterLib.Common.Extensions;

namespace Bannerlord.UIEditor.MainFrame.Resources.Resizer
{
    /// <summary>
    /// Reference: https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/master/OneCodeTeam/How%20to%20Resize%20WPF%20panel%20on%20Runtime/%5BC%23%5D-How%20to%20Resize%20WPF%20panel%20on%20Runtime/C%23/RuntimeResizablePanel/ResizablePanel.cs
    /// </summary>
    public class ResizableControl : ContentControl
    {
        public static DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof( Brush ), typeof( ResizableControl ));
        public static DependencyProperty BorderSizeProperty = DependencyProperty.Register("BorderSize", typeof( double ), typeof( ResizableControl ));

        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public double BorderSize
        {
            get => (double)GetValue(BorderSizeProperty);
            set => SetValue(BorderSizeProperty, value);
        }

        public double TotalBorderWidth => m_ResizerGrid?.ColumnDefinitions.Any() ?? false
            ? m_ResizerGrid.ColumnDefinitions[0].Width.Value +
              m_ResizerGrid.ColumnDefinitions[2].Width.Value
            : 0;

        public double TotalBorderHeight => m_ResizerGrid?.RowDefinitions.Any() ?? false
            ? m_ResizerGrid.RowDefinitions[0].Height.Value +
              m_ResizerGrid.RowDefinitions[2].Height.Value
            : 0;

        public IReadOnlyDictionary<ResizeDirection, Resizer> Borders => m_Borders;

        public ILayoutElement LayoutElement
        {
            get => m_LayoutElement;
            set
            {
                if (m_LayoutElement != value)
                {
                    m_LayoutElement = value;
                    foreach (var border in m_Borders)
                    {
                        border.Value.LayoutElement = m_LayoutElement;
                    }
                }
            }
        }

        private Grid? m_ResizerGrid;
        private readonly Dictionary<ResizeDirection, Resizer> m_Borders;
        private ILayoutElement m_LayoutElement = null!;

        private Dock[]? m_EnabledSides;

        public ResizableControl()
        {
            BorderColor = Brushes.Transparent;
            BorderSize = 5;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            m_Borders = new Dictionary<ResizeDirection, Resizer>();
        }

        static ResizableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( ResizableControl ), new FrameworkPropertyMetadata(typeof( ResizableControl )));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //LayoutElement = this.GetVisualAncestorOfType<ILayoutElement>()!;

            m_ResizerGrid = (Template.FindName("ResizerGrid", this) as Grid)!;

            if (!m_Borders.Any())
            {
                foreach (ResizeDirection resizeDirection in Enum.GetValues(typeof( ResizeDirection )))
                {
                    Resizer resizer = (Resizer)Template.FindName(Enum.GetName(typeof( ResizeDirection ), resizeDirection)!, this);
                    resizer.LayoutElement = LayoutElement;
                    m_Borders.Add(resizeDirection, resizer);
                }
            }

            if (m_EnabledSides is not null)
            {
                AdjustBorders(m_EnabledSides);
            }
        }

        public void AdjustBorders(params Dock[] _enabledSides)
        {
            if (m_Borders.Count == 0 || m_ResizerGrid is null)
            {
                m_EnabledSides = _enabledSides;
                return;
            }

            m_ResizerGrid.ColumnDefinitions[0].Width = new GridLength(_enabledSides.Contains(Dock.Left) ? BorderSize : 0);
            m_ResizerGrid.RowDefinitions[0].Height = new GridLength(_enabledSides.Contains(Dock.Top) ? BorderSize : 0);
            m_ResizerGrid.ColumnDefinitions[2].Width = new GridLength(_enabledSides.Contains(Dock.Right) ? BorderSize : 0);
            m_ResizerGrid.RowDefinitions[2].Height = new GridLength(_enabledSides.Contains(Dock.Bottom) ? BorderSize : 0);

            foreach (var (direction, resizer) in m_Borders)
            {
                resizer.IsEnabled = direction switch
                {
                    ResizeDirection.TopLeft => _enabledSides.Contains(Dock.Top) && _enabledSides.Contains(Dock.Left),
                    ResizeDirection.Left => _enabledSides.Contains(Dock.Left),
                    ResizeDirection.BottomLeft => _enabledSides.Contains(Dock.Bottom) && _enabledSides.Contains(Dock.Left),
                    ResizeDirection.Bottom => _enabledSides.Contains(Dock.Bottom),
                    ResizeDirection.BottomRight => _enabledSides.Contains(Dock.Bottom) && _enabledSides.Contains(Dock.Right),
                    ResizeDirection.Right => _enabledSides.Contains(Dock.Right),
                    ResizeDirection.TopRight => _enabledSides.Contains(Dock.Top) && _enabledSides.Contains(Dock.Right),
                    ResizeDirection.Top => _enabledSides.Contains(Dock.Top),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
