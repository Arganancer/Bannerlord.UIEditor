using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public interface IWidgetManager
    {
        event EventHandler<bool> IsWorkingChanged;

        bool IsWorking { get; }

        IReadOnlyList<IWidgetCategory> WidgetTemplateCategories { get; }

        UIEditorWidget CreateWidget(UIContext _context, IWidgetTemplate _widgetTemplate);

        void LoadAssembly(Assembly _assembly);
    }
}
