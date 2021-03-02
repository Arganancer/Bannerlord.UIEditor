using System;
using System.Diagnostics;
using System.Xml;
using HarmonyLib;

namespace Bannerlord.UIEditor.Core
{
    public class ValueSetting : Setting

    {
        public static ValueSetting? Deserialize(XmlNode _node, TypeDictionary _typeDictionary)
        {
            try
            {
                var id = _node.Attributes?["id"]?.Value;
                var value = _node.Attributes?["value"]?.Value;
                var type = _node.Attributes?["type"]?.Value;
                if (id is null || value is null || type is null)
                {
                    return null;
                }

                var settingType = _typeDictionary.GetTypeFromKey(type);
                object convertedValue = settingType.IsEnum ? Enum.Parse(settingType, value) : Convert.ChangeType(value, settingType);
                return new ValueSetting(id, convertedValue, settingType);
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                Trace.WriteLine(e.ToString());
                return null;
            }
        }

        public object Value { get; set; }
        public Type SettingType { get; }

        public ValueSetting(string _id, object _value) : this(_id, _value, _value.GetType())
        {
        }

        public ValueSetting(string _id, object _value, Type _type) : base(_id)
        {
            Value = _value;
            SettingType = _type;
        }

        public override void Serialize(XmlNode _parentNode, TypeDictionary _typeDictionary)
        {
            XmlElement settingElement = _parentNode.OwnerDocument!.CreateElement("Setting");
            settingElement.SetAttribute("id", Id);
            settingElement.SetAttribute("value", Value.ToString());
            settingElement.SetAttribute("type", _typeDictionary.GetKeyFromType(SettingType));
            _parentNode.AppendChild(settingElement);
        }
    }
}
