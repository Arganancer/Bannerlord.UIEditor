using System;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface ILayoutElement
    {
        event EventHandler<double> DesiredWidthChanged;
        event EventHandler<double> DesiredHeightChanged;

        double DesiredWidth { get; set; }
        double DesiredHeight { get; set; }

        void DiscreteSetDesiredSize(double _desiredWidth, double _desiredHeight);

        Control Control { get; }
        Dock CurrentDock { get; }

        Orientation Orientation { get; set; }

        void RefreshResizerBorders();
    }
}
