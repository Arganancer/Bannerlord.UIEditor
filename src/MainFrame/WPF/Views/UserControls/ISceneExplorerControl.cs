using System;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface ISceneExplorerControl
    {
        event EventHandler<WidgetViewModel?>? SelectedWidgetChanged;
    }
}
