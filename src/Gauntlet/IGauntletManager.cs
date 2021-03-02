using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.Gauntlet
{
    public interface IGauntletManager
    {
        UIEditorGauntletScreen UIEditorGauntletScreen { get; }
        UIContext? UIContext { get; }
    }
}
