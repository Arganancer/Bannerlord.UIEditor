using System;
using System.Diagnostics;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidgetAttribute
    {
        #region Properties

        public Type Type { get; }
        public object? DefaultValue { get; }
        public string Name { get; }
        public object? Value { get; set; }
        public Type DeclaringType { get; }

        public string ValueAsString
        {
            get
            {
                try
                {
                    return Value?.ToString() ?? "";
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    return "";
                }
            }
        }

        #endregion

        #region Constructors

        public UIEditorWidgetAttribute(Type _type, object? _defaultValue, string _name, object? _value, Type _declaringType)
        {
            Type = _type;
            DefaultValue = _defaultValue;
            Name = _name;
            Value = _value;
            DeclaringType = _declaringType;
        }

        #endregion
    }
}
