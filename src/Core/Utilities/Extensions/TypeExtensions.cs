using System;
using System.Linq;

namespace Bannerlord.UIEditor.Core
{
    public static class TypeExtensions
    {
        public static T? GetCustomAttribute<T>(this Type _type) where T : Attribute
        {
            return _type.GetCustomAttributes(typeof( T ), false).FirstOrDefault() as T;
        }
    }
}
