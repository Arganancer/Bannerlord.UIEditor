using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.MainFrame.Gauntlet
{
    public interface IGauntletManager
    {
        UIEditorGauntletScreen UIEditorGauntletScreen { get; }
        UIContext? UIContext { get; }
    }
}
