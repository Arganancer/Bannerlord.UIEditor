using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class AttributeCategory
    {
        public string Name { get; }
        public Type Owner { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }

        public AttributeCategory(string _name, Type _owner, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Owner = _owner;
            Attributes = _attributes;
        }
    }
}
