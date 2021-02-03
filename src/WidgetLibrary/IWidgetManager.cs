using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public interface IWidgetManager
    {
        #region Properties

        IReadOnlyList<IWidgetTemplate> WidgetTemplates { get; }

        #endregion

        #region Public Methods

        UIEditorWidget CreateWidget(UIContext _context, IWidgetTemplate _widgetTemplate);
        void LoadAssembly(Assembly _assembly);

        #endregion
    }
}
