using System;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidgetAttribute
    {
        #region Properties

        public Type Type { get; }
        public object? DefaultValue { get; }
        public string Name { get; }
        public object? Value { get; set; }

        #endregion

        #region Constructors

        public UIEditorWidgetAttribute(Type _type, object? _defaultValue, string _name, object? _value)
        {
            Type = _type;
            DefaultValue = _defaultValue;
            Name = _name;
            Value = _value;
        }

        #endregion
    }
}
