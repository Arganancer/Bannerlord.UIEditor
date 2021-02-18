using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Bannerlord.ButterLib.Common.Extensions;

namespace Bannerlord.UIEditor.MainFrame
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

        private Grid m_ResizerGrid = null!;
        private Dictionary<ResizeDirection, Resizer> Borders = new();

        public ResizableControl()
        {
            BorderColor = Brushes.Transparent;
            BorderSize = 5;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        static ResizableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof( ResizableControl ), new FrameworkPropertyMetadata(typeof( ResizableControl )));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_ResizerGrid = (Template.FindName("ResizerGrid", this) as Grid)!;
            foreach (ResizeDirection resizeDirection in Enum.GetValues(typeof( ResizeDirection )))
            {
                Borders.Add(resizeDirection, (Resizer)Template.FindName(Enum.GetName(typeof( ResizeDirection ), resizeDirection)!, this));
            }

            AdjustBorders();
        }

        public void AdjustBorders()
        {
            DockPanel[] ancestors = this.GetVisualAncestorsOfType<DockPanel>()?.ToArray()!;
            DockPanel parent = ancestors[0];

            Dictionary<Dock, bool> enabledSides = new() {{Dock.Left, false}, {Dock.Bottom, false}, {Dock.Right, false}, {Dock.Top, false}};

            var dockPosition = DockPanel.GetDock(this);
            var onlyInnerBorders = ancestors.Length > 1;

            if (dockPosition == Dock.Left)
            {
                if (onlyInnerBorders && HasDockedChildAtPosition(Dock.Right, parent))
                {
                    enabledSides[Dock.Right] = true;
                }

                if (!onlyInnerBorders)
                {
                    enabledSides[Dock.Right] = true;
                }
            }

            if (dockPosition == Dock.Right)
            {
                if (!onlyInnerBorders)
                {
                    enabledSides[Dock.Left] = true;
                }
            }

            if (dockPosition == Dock.Top)
            {
                if (onlyInnerBorders && HasDockedChildAtPosition(Dock.Bottom, parent))
                {
                    enabledSides[Dock.Bottom] = true;
                }

                if (!onlyInnerBorders)
                {
                    enabledSides[Dock.Bottom] = true;
                }
            }

            if (dockPosition == Dock.Bottom)
            {
                if (!onlyInnerBorders)
                {
                    enabledSides[Dock.Top] = true;
                }
            }

            foreach (var (dock, isEnabled) in enabledSides)
            {
                switch (dock)
                {
                    case Dock.Left:
                        m_ResizerGrid.ColumnDefinitions[0].Width = new GridLength(isEnabled ? BorderSize : 0);
                        break;
                    case Dock.Top:
                        m_ResizerGrid.RowDefinitions[0].Height = new GridLength(isEnabled ? BorderSize : 0);
                        break;
                    case Dock.Right:
                        m_ResizerGrid.ColumnDefinitions[2].Width = new GridLength(isEnabled ? BorderSize : 0);
                        break;
                    case Dock.Bottom:
                        m_ResizerGrid.RowDefinitions[2].Height = new GridLength(isEnabled ? BorderSize : 0);
                        break;
                }
            }

            foreach (var (direction, resizer) in Borders)
            {
                resizer.IsEnabled = direction switch
                {
                    ResizeDirection.TopLeft => enabledSides[Dock.Top] && enabledSides[Dock.Left],
                    ResizeDirection.Left => enabledSides[Dock.Left],
                    ResizeDirection.BottomLeft => enabledSides[Dock.Bottom] && enabledSides[Dock.Left],
                    ResizeDirection.Bottom => enabledSides[Dock.Bottom],
                    ResizeDirection.BottomRight => enabledSides[Dock.Bottom] && enabledSides[Dock.Right],
                    ResizeDirection.Right => enabledSides[Dock.Right],
                    ResizeDirection.TopRight => enabledSides[Dock.Top] && enabledSides[Dock.Right],
                    ResizeDirection.Top => enabledSides[Dock.Top],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            bool HasDockedChildAtPosition(Dock _position, DockPanel _dockPanel)
            {
                return _dockPanel.Children.OfType<ResizableControl>().Any(_x => DockPanel.GetDock(_x) == _position);
            }
        }
    }
}
