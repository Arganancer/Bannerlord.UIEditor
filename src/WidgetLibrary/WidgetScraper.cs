using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Bannerlord.UIEditor.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    internal static class WidgetScraper
    {
        internal static IEnumerable<WidgetTemplate> ScrapeAssembly(Assembly _assembly)
        {
            IEnumerable<Type> widgetTypes = _assembly.SafeGetTypes(_type => !_type.IsAbstract && typeof( Widget ).IsAssignableFrom(_type));
            return widgetTypes.Select(CreateWidgetTemplateFromType);
        }

        internal static WidgetTemplate CreateWidgetTemplateFromType(Type _type)
        {
            List<Func<object, UIEditorWidgetAttribute>> attributeInstantiators = (
                from propertyInfo in _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                where propertyInfo.GetCustomAttribute(typeof( EditorAttribute )) is not null
                let getter = propertyInfo.GetMethod
                where getter is not null
                select CreateAttributeInstantiator(_type, getter, propertyInfo)).ToList();

            UIEditorWidget CreateWidget(WidgetFactory _widgetFactory, UIContext _context)
            {
                Widget instance = _widgetFactory.CreateBuiltinWidget(_context, _type.Name);
                return new UIEditorWidget(_type.Name, (from createAttribute in attributeInstantiators select createAttribute(instance)).ToList());
            }

            return new WidgetTemplate(_type, _type.Assembly, CreateWidget);
        }

        private static Func<object, UIEditorWidgetAttribute> CreateAttributeInstantiator(Type _widgetType, MethodInfo _getter, PropertyInfo _propertyInfo)
        {
            ParameterExpression instanceParam = Expression.Parameter(typeof( object ));
            Expression returnExpression = Expression.Call(Expression.Convert(instanceParam, _widgetType), _getter);
            if (!_propertyInfo.PropertyType.IsClass)
            {
                returnExpression = Expression.Convert(returnExpression, typeof( object ));
            }

            Func<object, object> getPropertyValue = (Func<object, object>)Expression.Lambda(returnExpression, instanceParam).Compile();
            if (_propertyInfo.PropertyType.IsEnum)
            {
                return _instance =>
                {
                    var defaultPropertyValue = getPropertyValue(_instance);
                    return new UIEditorWidgetAttributeCollection(_getter.ReturnType, defaultPropertyValue, _propertyInfo.Name, defaultPropertyValue, _propertyInfo.DeclaringType!, Enum.GetValues(_propertyInfo.PropertyType).Cast<Enum>());
                };
            }

            return _instance =>
            {
                var defaultPropertyValue = getPropertyValue(_instance);
                return new UIEditorWidgetAttribute(_getter.ReturnType, defaultPropertyValue, _propertyInfo.Name, defaultPropertyValue, _propertyInfo.DeclaringType!);
            };
        }
    }
}
