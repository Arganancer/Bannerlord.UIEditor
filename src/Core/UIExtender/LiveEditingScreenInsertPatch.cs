using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Bannerlord.UIEditor.Core.UIExtender
{
    [PrefabExtension("LiveEditingScreen", "descendant::Prefab")]
    public class LiveEditingScreenInsertPatch : PrefabExtensionInsertPatch
    {
        public static XmlDocument? LiveEditingScreenXmlDocument { get; set; }

        [PrefabExtensionXmlDocument]
        public XmlDocument LiveEditingScreenDocument => LiveEditingScreenXmlDocument!;

        public LiveEditingScreenInsertPatch()
        {
            LiveEditingScreenXmlDocument = new XmlDocument();
            LiveEditingScreenXmlDocument.LoadXml(
                @"<Prefab>
    <Constants>
    </Constants>
    <Variables>
    </Variables>
    <VisualDefinitions>
    </VisualDefinitions>
    <Window>
        <Widget />
    </Window>
</Prefab>");
        }

        public override InsertType Type => InsertType.Replace;
    }
}
