using System;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface IWidgetListControl
    {
        #region Events and Delegates

        event EventHandler<IWidgetTemplate?> SelectedWidgetTemplateChanged;

        #endregion

        #region Properties

        IWidgetTemplate? SelectedWidgetTemplate { get; }

        #endregion
    }
}
