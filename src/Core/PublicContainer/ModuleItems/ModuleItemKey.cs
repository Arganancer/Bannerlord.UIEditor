using System;

namespace Bannerlord.UIEditor.Core
{
    internal readonly struct ModuleItemKey
    {
        public string Name { get; }
        public string InterfaceFullName { get; }

        private readonly int m_HashCode;

        public ModuleItemKey(string _name, Type _interface)
        {
            Name = _name;
            InterfaceFullName = _interface.FullName!;
            m_HashCode = HashCode.Combine(Name, InterfaceFullName);
        }

        public override int GetHashCode() => m_HashCode;

        public override string ToString() => $"Name:{Name} Type:{InterfaceFullName}";
    }
}
