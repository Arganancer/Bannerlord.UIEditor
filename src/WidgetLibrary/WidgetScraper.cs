using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.GauntletUI;
using Module = Bannerlord.UIEditor.Core.Module;

namespace Bannerlord.UIEditor.WidgetLibrary
{
	public class WidgetScraper : Module
	{
        public override void Load()
        {
            base.Load();
            ScrapeAssembly(typeof(Widget).Assembly);
        }

        public IEnumerable<UIEditorWidget> ScrapeAssembly(Assembly _assembly)
        {
            IEnumerable<Type> widgetTypes = _assembly.GetTypes().Where(_type => !_type.IsAbstract && typeof( Widget ).IsAssignableFrom(_type));
            return widgetTypes.Select(ConvertTypeToWidget);
        }

        private UIEditorWidget ConvertTypeToWidget(Type _type)
        {
            List<UIEditorWidgetAttribute> attributes = new();
            foreach (PropertyInfo propertyInfo in _type.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof( EditorAttribute )) is not null)
                {
                    // TODO: Find a way to get the default value of the property.
                    // Best way would most likely be to create an instance of the Widget type, then check the value of the property.
                    attributes.Add(new UIEditorWidgetAttribute(propertyInfo.PropertyType, null, propertyInfo.Name, null ));
                }
            }

            return new UIEditorWidget(_type.Name, _type.Assembly, attributes);
        }
    }
}
