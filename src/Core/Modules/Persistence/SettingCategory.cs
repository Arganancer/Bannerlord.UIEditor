using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Bannerlord.ButterLib.Common.Extensions;

namespace Bannerlord.UIEditor.Core
{
    public class SettingCategory : ISettingCategory
    {
        public event EventHandler? ChangesPending;

        public static SettingCategory? Deserialize(XmlNode _settingCategoryNode, TypeDictionary _typeDictionary)
        {
            var id = _settingCategoryNode.Attributes?["id"]?.Value;
            if (id is null)
            {
                return null;
            }

            SettingCategory settingCategory = new(id);
            foreach (XmlNode childNode in _settingCategoryNode.ChildNodes.OfType<XmlNode>())
            {
                var setting = ValueSetting.Deserialize(childNode, _typeDictionary);
                if (setting is null)
                {
                    continue;
                }

                settingCategory.m_Settings.TryAdd(setting.Id, setting);
            }

            return settingCategory;
        }

        public string Id { get; }

        private readonly ConcurrentDictionary<string, ValueSetting> m_Settings = new();

        public SettingCategory(string _id)
        {
            Id = _id;
        }

        /// <inheritdoc/>
        public T? GetSetting<T>(string _name, T? _defaultValue)
        {
            if (!m_Settings.TryGetValue(_name, out ValueSetting setting))
            {
                if (_defaultValue is null)
                {
                    return default;
                }

                setting = new ValueSetting(_name, _defaultValue);
                m_Settings.TryAdd(_name, setting);
                OnChangesPending();
            }

            return ConvertToType<T>(setting);
        }

        /// <inheritdoc/>
        public T? GetSetting<T>(string _name)
        {
            if (!m_Settings.TryGetValue(_name, out var setting) || setting?.Value is null)
            {
                return default;
            }

            return ConvertToType<T>(setting);
        }

        /// <inheritdoc/>
        public void SetSetting<T>(string _name, T? _setting)
        {
            if (m_Settings.TryGetValue(_name, out var setting))
            {
                if (setting?.Value is not null)
                {
                    CanConvert(typeof( T ), setting.SettingType, true);
                }
            }

            if (_setting is null)
            {
                m_Settings.TryRemove(_name, out ValueSetting _);
            }
            else
            {
                m_Settings.AddOrUpdate(_name, new ValueSetting(_name, _setting), (_, _existingSettings) =>
                {
                    _existingSettings.Value = _setting;
                    return _existingSettings;
                });

                OnChangesPending();
            }
        }

        public void Serialize(XmlNode _parentNode, TypeDictionary _typeDictionary)
        {
            XmlElement settingCategoryElement = _parentNode.OwnerDocument!.CreateElement("SettingCategory");
            settingCategoryElement.SetAttribute("id", Id);
            _parentNode.AppendChild(settingCategoryElement);

            foreach (var (_, setting) in m_Settings)
            {
                setting?.Serialize(settingCategoryElement, _typeDictionary);
            }
        }

        protected virtual void OnChangesPending()
        {
            ChangesPending?.Invoke(this, EventArgs.Empty);
        }

        private static T ConvertToType<T>(ValueSetting _valueSetting)
        {
            Type desiredType = typeof( T );

            CanConvert(desiredType, _valueSetting.SettingType, true);

            return (T)_valueSetting.Value!;
        }

        private static bool CanConvert(Type _from, Type _to, bool _throwIfFalse = false)
        {
            if (!_to.IsAssignableFrom(_from))
            {
                if (!TypeDescriptor.GetConverter(_from).CanConvertTo(_to))
                {
                    if (_throwIfFalse)
                    {
                        throw new InvalidCastException($"Type {_from} cannot be implicitly cast to type {_to}");
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
