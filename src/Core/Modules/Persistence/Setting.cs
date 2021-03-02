using System.Xml;

namespace Bannerlord.UIEditor.Core
{
    public abstract class Setting
    {
        public string Id { get; }
        public abstract void Serialize(XmlNode _parentNode, TypeDictionary _typeDictionary);

        protected Setting(string _id)
        {
            Id = _id;
        }
    }
}
