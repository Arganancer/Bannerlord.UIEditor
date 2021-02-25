using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using HarmonyLib;

namespace Bannerlord.UIEditor.Core
{
    [XmlType("Setting")]
    public class Setting
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("value")]
        public string ValueAsString
        {
            get => Value.ToString();
            set => Value = Convert.ChangeType(value, SettingType);
        }

        [XmlAttribute("type")]
        public string TypeName
        {
            get => SettingType.AssemblyQualifiedName!;
            set => SettingType = Type.GetType(value)!;
        }

        [XmlIgnore]
        public Type SettingType { get; set; }

        [XmlIgnore]
        public object Value { get; set; }

        public Setting()
        {
            Id = "";
            SettingType = null!;
            Value = null!;
        }

        public Setting(string _id, object _value)
        {
            Id = _id;
            Value = _value;
            SettingType = _value.GetType();
        }
        public Setting(string _id, object _value, Type _type)
        {
            Id = _id;
            Value = _value;
            SettingType = _type;
        }

        public static Setting? Deserialize(XmlNode _node)
        {
            try
            {
                var value = _node.Attributes?["value"]?.Value;
                var type = _node.Attributes?["type"]?.Value;
                var id = _node.Attributes?["id"]?.Value;
                if (id is null || value is null || type is null)
                {
                    return null;
                }

                var settingType = Type.GetType(type)!;
                object convertedValue = settingType.IsEnum ? Enum.Parse(settingType, value) : Convert.ChangeType(value, settingType);
                return new Setting(id, convertedValue, settingType);
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
