using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class AttributeCategory
    {
        public event WidgetAttributeChangedEventHandler? PropertyChanged;
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

        private bool m_IsReadonly;

        public AttributeCategory(string _name, Type _owner, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Owner = _owner;
            Attributes = _attributes;
            foreach (UIEditorWidgetAttribute attribute in _attributes)
            {
                attribute.PropertyChanged += OnPropertyChanged;
            }
        }

        protected virtual void OnPropertyChanged(UIEditorWidgetAttribute _sender, string _attributeName, object? _value)
        {
            PropertyChanged?.Invoke(_sender, _attributeName, _value);
        }
    }
}
