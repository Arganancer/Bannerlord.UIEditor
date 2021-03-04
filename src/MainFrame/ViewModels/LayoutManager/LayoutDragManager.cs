using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public delegate void LayoutDragDropEnabledChangedEventHandler(bool _isDragDropEnabled);

    public class LayoutDragManager : Module, ILayoutDragManager
    {
        public bool IsPanelBeingDragged
        {
            get => m_IsPanelBeingDragged;
            private set
            {
                if (m_IsPanelBeingDragged != value)
                {
                    m_IsPanelBeingDragged = value;
                    OnLayoutDragDropEnabledChanged(m_IsPanelBeingDragged);
                }
            }
        }

        private bool m_IsPanelBeingDragged;
        public event LayoutDragDropEnabledChangedEventHandler? LayoutDragDropEnabledChanged;

        public void StartDragging()
        {
            IsPanelBeingDragged = true;
        }

        public void StopDragging()
        {
            IsPanelBeingDragged = false;
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            RegisterModule<ILayoutDragManager>();
        }

        protected virtual void OnLayoutDragDropEnabledChanged(bool _isDragDropEnabled)
        {
            LayoutDragDropEnabledChanged?.Invoke(_isDragDropEnabled);
        }
    }
}
