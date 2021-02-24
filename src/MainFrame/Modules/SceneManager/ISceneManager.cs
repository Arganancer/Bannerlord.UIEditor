using System;
using System.Xml;

namespace Bannerlord.UIEditor.MainFrame
{
    public interface ISceneManager
    {
        event EventHandler<DrawableWidgetViewModel?>? RootWidgetChanged;
        DrawableWidgetViewModel? RootWidget { get; set; }
        XmlDocument ToXml();
    }
}
