using System;
using System.Reflection;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    internal delegate UIEditorWidget CreateWidget(WidgetFactory _widgetFactory, UIContext _context);

    internal class WidgetTemplate : IWidgetTemplate
    {
        public Type WidgetType { get; }
        public Assembly Owner { get; }
        public CreateWidget CreateInstance { get; }

        public string Name => WidgetType.Name;

        public WidgetTemplate(Type _widgetType, Assembly _owner, CreateWidget _createInstance)
        {
            WidgetType = _widgetType;
            Owner = _owner;
            CreateInstance = _createInstance;
        }
    }
}
