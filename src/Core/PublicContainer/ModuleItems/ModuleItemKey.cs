using System;

namespace Bannerlord.UIEditor.Core
{
    internal readonly struct ModuleItemKey
    {
        #region Public Properties

        public string Name { get; }
        public string InterfaceFullName { get; }

        #endregion

        #region Private Fields

        private readonly int m_HashCode;

        #endregion

        #region Constructors

        public ModuleItemKey(string _name, Type _interface)
        {
            Name = _name;
            InterfaceFullName = _interface.FullName!;
            m_HashCode = HashCode.Combine(Name, InterfaceFullName);
        }

        #endregion

        #region ValueType Members

        public override int GetHashCode() => m_HashCode;

        public override string ToString() => $"Name:{Name} Type:{InterfaceFullName}";

        #endregion
    }
}
