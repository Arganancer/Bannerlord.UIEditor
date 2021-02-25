using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Bannerlord.ButterLib.Common.Extensions;
using HarmonyLib;
using TaleWorlds.Engine;
using Path = System.IO.Path;

namespace Bannerlord.UIEditor.Core
{
    public class SettingsManager : Module, ISettingsManager
    {
        public static SettingsManager? Instance { get; private set; }

        public static readonly string ModDirectory = Path.Combine(Utilities.GetConfigsPath(), "UIEditor");
        public static readonly string UIEditorSettingsFileName = "UIEditor.settings";

        private readonly ConcurrentDictionary<string, Setting> m_Settings = new();

        /// <summary>
        /// Returns the value of the setting that corresponds to the given <paramref name="_name"/>.<br/>
        /// If the setting is not set, creates the setting and sets its value to <paramref name="_defaultValue"/>, then returns <paramref name="_defaultValue"/>.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
        public T? GetSetting<T>(string _name, T? _defaultValue)
        {
            if (!m_Settings.TryGetValue(_name, out Setting setting))
            {
                if (_defaultValue is null)
                {
                    return default;
                }

                setting = new Setting(_name, _defaultValue);
                m_Settings.TryAdd(_name, setting);
                SaveSettings();
            }

            return ConvertToType<T>(setting);
        }

        /// <summary>
        /// Returns the value of the setting that corresponds to the given <paramref name="_name"/>.<br/>
        /// Returns null if the setting is not set.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
        public T? GetSetting<T>(string _name)
        {
            if (!m_Settings.TryGetValue(_name, out var setting) || setting?.Value is null)
            {
                return default;
            }

            return ConvertToType<T>(setting);
        }

        /// <summary>
        /// If the setting exists, updates its value, else creates the setting and sets its value.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
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
                m_Settings.TryRemove(_name, out Setting _);
            }
            else
            {
                m_Settings.AddOrUpdate(_name, new Setting(_name, _setting), (_, _existingSettings) =>
                {
                    _existingSettings.Value = _setting;
                    return _existingSettings;
                });

                SaveSettings();
            }
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            Instance = this;
            LoadSettings();

            RegisterModule<ISettingsManager>();
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(ModDirectory);

                XmlDocument xmlDocument = new();

                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);
                XmlElement uiEditorConfig = xmlDocument.CreateElement("UIEditorConfig");
                xmlDocument.AppendChild(uiEditorConfig);

                foreach (var (_, setting) in m_Settings)
                {
                    if (setting is null)
                    {
                        continue;
                    }

                    SerializeObjectAndAppendAsChildOfNode(uiEditorConfig, setting);
                }

                xmlDocument.Save(Path.Combine(ModDirectory, UIEditorSettingsFileName));
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
            }
        }

        private void LoadSettings()
        {
            string fullPath = Path.Combine(ModDirectory, UIEditorSettingsFileName);
            if (!File.Exists(fullPath))
            {
                return;
            }

            try
            {
                m_Settings.Clear();

                XmlDocument xmlDocument = new();
                xmlDocument.Load(fullPath);

                if (xmlDocument.DocumentElement is null)
                {
                    return;
                }

                foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes.OfType<XmlNode>())
                {
                    Setting? setting = Setting.Deserialize(childNode);
                    if(setting is null)
                    {
                        continue;
                    }
                    m_Settings.TryAdd(setting.Id, setting);
                }
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
            }
        }

        private static void SerializeObjectAndAppendAsChildOfNode(XmlNode _node, Setting _setting)
        {
            XmlSerializer serializer = new(typeof( Setting ));
            using XmlWriter writer = _node.CreateNavigator().AppendChild();
            writer.WriteWhitespace("");
            XmlSerializerNamespaces nameSpaces = new();
            nameSpaces.Add(_node.GetNamespaceOfPrefix(_node.NamespaceURI), _node.NamespaceURI);
            serializer.Serialize(writer, _setting, nameSpaces);
        }

        private static T ConvertToType<T>(Setting _setting)
        {
            Type desiredType = typeof( T );

            CanConvert(desiredType, _setting.SettingType, true);

            return (T)_setting.Value!;
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
