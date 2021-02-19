using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class AttributeCategory
    {
        private bool m_IsReadonly;
        public string Name { get; }
        public Type Owner { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }

        public bool IsReadonly
        {
            get => m_IsReadonly;
            set
            {
                if (m_IsReadonly != value)
                {
                    m_IsReadonly = value;
                    foreach (UIEditorWidgetAttribute attribute in Attributes)
                    {
                        attribute.IsReadonly = m_IsReadonly;
                    }
                }
            }
        }

        public AttributeCategory(string _name, Type _owner, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Owner = _owner;
            Attributes = _attributes;
        }
    }
}
