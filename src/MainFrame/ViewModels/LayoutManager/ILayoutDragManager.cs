namespace Bannerlord.UIEditor.MainFrame
{
    public interface ILayoutDragManager
    {
        event LayoutDragDropEnabledChangedEventHandler? LayoutDragDropEnabledChanged;
        bool IsPanelBeingDragged { get; }
        void StartDragging();
        void StopDragging();
    }
}
