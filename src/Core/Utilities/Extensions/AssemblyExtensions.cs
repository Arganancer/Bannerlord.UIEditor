using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bannerlord.UIEditor.Core
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> SafeGetTypes(this Assembly _assembly, Func<Type, bool> _predicate)
        {
            try
            {
                return _assembly.GetTypes().Where(_predicate);
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(_type => _type is not null && _predicate(_type));
            }
        }
    }
}
