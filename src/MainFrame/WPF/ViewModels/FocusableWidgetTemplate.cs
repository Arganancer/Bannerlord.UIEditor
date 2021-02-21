using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame
{
    public class FocusableWidgetTemplate : IFocusable
    {
        public IWidgetTemplate WidgetTemplate { get; }
        public string Name => WidgetTemplate.Name;
        public bool IsFocused { get; set; }

        public FocusableWidgetTemplate(IWidgetTemplate _widgetTemplate)
        {
            WidgetTemplate = _widgetTemplate;
        }
    }
}
