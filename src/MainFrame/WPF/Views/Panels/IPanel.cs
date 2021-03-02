using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface IPanel
    {
        string PanelName { get; }

        ISettingCategory SettingCategory { get; set; }
    }
}
