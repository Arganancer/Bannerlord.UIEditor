using System;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface IFocusManager
    {
        event EventHandler<IFocusable?>? FocusChanged;
        IFocusable? FocusedItem { get; }
        void SetFocus(IFocusable? _focusedItem);
    }
}
