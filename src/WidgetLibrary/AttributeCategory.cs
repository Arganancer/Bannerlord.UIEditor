using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class AttributeCategory
    {
        #region Properties

        public string Name { get; }
        public Type Owner { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }

        #endregion

        #region Constructors

        public AttributeCategory(string _name, Type _owner, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Owner = _owner;
            Attributes = _attributes;
        }

        #endregion
    }
}
