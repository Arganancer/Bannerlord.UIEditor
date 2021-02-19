using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidgetAttribute
    {
        public Type Type { get; }
        public string TypeName => Type.ToString();
        public object? DefaultValue { get; }
        public string Name { get; }
        public object? Value { get; set; }
        public Type DeclaringType { get; }
        public bool IsReadonly { get; set; }

        public UIEditorWidgetAttribute(Type _type, object? _defaultValue, string _name, object? _value, Type _declaringType)
        {
            Type = _type;
            DefaultValue = _defaultValue;
            Name = _name;
            Value = _value;
            DeclaringType = _declaringType;
        }
    }

    public class UIEditorWidgetAttributeCollection : UIEditorWidgetAttribute
    {
        public ObservableCollection<object> Collection { get; }

        public UIEditorWidgetAttributeCollection(Type _type, object? _defaultValue, string _name, object? _value, Type _declaringType, IEnumerable<object> _collection) :
            base(_type, _defaultValue, _name, _value, _declaringType)
        {
            Collection = new ObservableCollection<object>(_collection);
        }
    }
}
