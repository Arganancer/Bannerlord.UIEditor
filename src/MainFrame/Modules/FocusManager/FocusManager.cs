using System;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public class FocusManager : Module, IFocusManager
    {
        public IFocusable? FocusedItem
        {
            get => m_FocusedItem;
            private set
            {
                if (m_FocusedItem == value)
                {
                    return;
                }

                if (m_FocusedItem is not null)
                {
                    m_FocusedItem.IsFocused = false;
                }

                m_FocusedItem = value;
                if (m_FocusedItem is not null)
                {
                    m_FocusedItem.IsFocused = true;
                }

                OnFocusChanged(m_FocusedItem);
            }
        }

        private IFocusable? m_FocusedItem;

        public event EventHandler<IFocusable?>? FocusChanged;

        public void SetFocus(IFocusable? _focusedItem)
        {
            FocusedItem = _focusedItem;
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IFocusManager>();
        }

        private void OnFocusChanged(IFocusable? _focusedItem)
        {
            FocusChanged?.Invoke(this, _focusedItem);
        }
    }
}
