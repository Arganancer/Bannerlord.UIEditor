using System;
using System.Collections.Generic;
using System.Xml;
using Bannerlord.ButterLib.Common.Extensions;

namespace Bannerlord.UIEditor.Core
{
    public class TypeDictionary
    {
        public const string NodeName = "TypeDictionary";

        public static TypeDictionary Deserialize(XmlNode _typeDictionaryNode)
        {
            TypeDictionary typeDictionary = new();
            foreach (XmlNode typeEntryNode in _typeDictionaryNode.ChildNodes)
            {
                string key = typeEntryNode.Attributes!["key"].Value;
                string assemblyQualifiedName = typeEntryNode.Attributes!["value"].Value;
                TypeEntry typeEntry = new(key, assemblyQualifiedName, Type.GetType(assemblyQualifiedName)!);
                typeDictionary.m_Types.Add(key, typeEntry);
            }

            return typeDictionary;
        }

        private readonly Dictionary<string, TypeEntry> m_Types = new();

        public Type GetTypeFromKey(string _key)
        {
            return m_Types[_key].Type;
        }

        public string GetKeyFromType(Type _type)
        {
            if (!m_Types.TryGetValue(_type.Name, out TypeEntry typeEntry))
            {
                typeEntry = new TypeEntry(_type.Name, _type.AssemblyQualifiedName!, _type);
                m_Types.Add(_type.Name, typeEntry);
            }

            return typeEntry.Key;
        }

        public void Serialize(XmlNode _parentNode)
        {
            XmlElement typeDictionaryElement = _parentNode.OwnerDocument!.CreateElement(NodeName);
            _parentNode.AppendChild(typeDictionaryElement);

            foreach (var (key, typeEntry) in m_Types)
            {
                XmlElement typeEntryElement = typeDictionaryElement.OwnerDocument!.CreateElement("TypeEntry");
                typeEntryElement.SetAttribute("key", key);
                typeEntryElement.SetAttribute("value", typeEntry.AssemblyQualifiedName);
                typeDictionaryElement.AppendChild(typeEntryElement);
            }
        }

        public class TypeEntry
        {
            public string Key { get; }
            public string AssemblyQualifiedName { get; }
            public Type Type { get; }

            public TypeEntry(string _key, string _assemblyQualifiedName, Type _type)
            {
                Key = _key;
                AssemblyQualifiedName = _assemblyQualifiedName;
                Type = _type;
            }
        }
    }
}
