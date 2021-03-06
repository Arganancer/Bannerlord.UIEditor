﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public delegate void WidgetAttributeChangedEventHandler(UIEditorWidgetAttribute _sender, string _attributeName, object? _value);

    public class UIEditorWidgetAttribute : INotifyPropertyChanged
    {
        public event WidgetAttributeChangedEventHandler? PropertyChangedWithValue;
        public Type Type { get; }
        public string TypeName => Type.ToString();
        public object? DefaultValue { get; }
        public string Name { get; }
        public bool IsDefaultValue => Value?.Equals(DefaultValue) ?? DefaultValue is null;
        public string ValueAsString => Value?.ToString() ?? "";

        public object? Value
        {
            get => m_Value;
            set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public Type DeclaringType { get; }
        public bool IsReadonly { get; set; }
        private object? m_Value;

        public UIEditorWidgetAttribute(Type _type, object? _defaultValue, string _name, object? _value, Type _declaringType)
        {
            Type = _type;
            DefaultValue = _defaultValue;
            Name = _name;
            Value = _value;
            DeclaringType = _declaringType;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? _propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
            PropertyChangedWithValue?.Invoke(this, Name, Value);
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
