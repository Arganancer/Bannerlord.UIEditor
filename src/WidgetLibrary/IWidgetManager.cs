using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public interface IWidgetManager
    {
        IReadOnlyList<IWidgetTemplate> WidgetTemplates { get; }

        UIEditorWidget CreateWidget(UIContext _context, IWidgetTemplate _widgetTemplate);
        void LoadAssembly(Assembly _assembly);
    }
}
