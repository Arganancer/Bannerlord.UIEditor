using System.Windows;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface ICursorManager
    {
        void Initialize(MainWindow _mainWindow);
        void SetCursor(CursorIcon _cursorIcon );
    }
}
