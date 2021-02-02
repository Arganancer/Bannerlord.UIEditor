using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace Bannerlord.UIEditor.Main.Patches
{
    [HarmonyPatch(typeof( InitialMenuVM ))]
    public class InitialMenuVMPatches
    {
        #region Harmony Patches

        [HarmonyPatch(MethodType.Constructor, new Type[0]), HarmonyPostfix, MethodImpl(MethodImplOptions.NoInlining)]
        // ReSharper disable once InconsistentNaming
        public static void ConstructorPostfix(MBBindingList<InitialMenuOptionVM> ____menuOptions)
        {
            if (____menuOptions.FirstOrDefault(_x => string.Equals(_x?.NameText, "UIEditor")) is null)
            {
                ____menuOptions.Add(new InitialMenuOptionVM(new InitialStateOption(
                    "UIEditorOption",
                    new TextObject("UIEditor"),
                    9990,
                    UIEditorLauncher.Instance.LaunchUIEditor,
                    false
                )));
            }
        }

        #endregion
    }
}
