using System;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface IWidgetListControl
    {
        event EventHandler<IWidgetTemplate?> SelectedWidgetTemplateChanged;

        IWidgetTemplate? SelectedWidgetTemplate { get; }
    }
}
