using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.UIEditor.Core
{
    public sealed class PublicContainer : IPublicContainer
    {
        #region Private Fields

        private readonly Dictionary<string, Dictionary<ModuleItemKey, ModuleItem>> m_Modules;
        private readonly Dictionary<Token, ModuleItemKey> m_ModulesByToken;
        private readonly PublicContainer? m_Parent;

        #endregion

        #region Constructors

        public PublicContainer(string _name, PublicContainer? _parent = null)
        {
            m_Parent = _parent;
            Name = _name;
            m_Modules = new Dictionary<string, Dictionary<ModuleItemKey, ModuleItem>>();
            m_ModulesByToken = new Dictionary<Token, ModuleItemKey>();
        }

        #endregion

        #region IPublicContainer Members

        public string Name { get; }
        public IPublicContainer? Parent => m_Parent;

        public Token RegisterModule<T>(IModule _module, string _moduleName) where T : class
        {
            return InternalRegisterModule<T>(_module, _moduleName, false).Token;
        }

        public void UnregisterModule(Token _token)
        {
            if (!_token.IsValid())
            {
                throw new ArgumentException("Token was invalid.");
            }

            lock (m_Modules)
            {
                if (m_ModulesByToken.TryGetValue(_token, out var key))
                {
                    if (m_Modules.TryGetValue(key.InterfaceFullName, out var moduleItems) &&
                        moduleItems.TryGetValue(key, out var moduleItem))
                    {
                        moduleItem.Module = null;
                    }
                    else
                    {
                        throw new Exception("ModuleItem was not found.");
                    }
                }
                else
                {
                    if (Parent != null)
                    {
                        Parent.UnregisterModule(_token);
                    }
                    else
                    {
                        throw new Exception("ModuleItem was not found.");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public T GetModule<T>(string _moduleName = "") where T : class
        {
            var moduleItem = GetModuleItem<T>(_moduleName);
            if (moduleItem?.Module is null)
            {
                throw new NullReferenceException($"Module of type {typeof( T ).Name}{(string.IsNullOrWhiteSpace(_moduleName) ? "" : $" with name {_moduleName}")} was not registered in PublicContainer {Name}");
            }

            return (moduleItem.Module as T)!;
        }

        /// <inheritdoc/>
        public void ConnectToModule<T>(
            IConnectedObject _connectedObject,
            Action<T> _onConnectedModuleRegistered,
            Action<T> _onConnectedModuleUnregistering,
            string _moduleName = "") where T : class
        {
            ModuleItem moduleItem = GetModuleItem<T>(_moduleName) ?? InternalRegisterModule<T>(null, _moduleName, true);

            void OnConnectedModuleRegistered(object _module) => _onConnectedModuleRegistered((_module as T)!);
            void OnConnectedModuleUnregistering(object _module) => _onConnectedModuleUnregistering((_module as T)!);
            moduleItem.AddConnectedObject(_connectedObject, OnConnectedModuleRegistered, OnConnectedModuleUnregistering);
        }

        #endregion

        #region Private Methods

        private void OnModuleItemCanBeDestroyed(object? _sender, ModuleItemKey _key)
        {
            if (m_Modules.TryGetValue(_key.InterfaceFullName, out var moduleItems) &&
                moduleItems.Remove(_key, out var moduleItem))
            {
                m_ModulesByToken.Remove(moduleItem.Token);
                if (moduleItems.Count == 0)
                {
                    m_Modules.Remove(_key.InterfaceFullName);
                }
            }
            else
            {
                throw new Exception("ModuleItem was not found.");
            }
        }

        private ModuleItem InternalRegisterModule<T>(IModule? _module, string _moduleName, bool _createPlaceholder) where T : class
        {
            Type moduleType = typeof( T );

            if (_moduleName == null)
            {
                throw new ArgumentNullException(nameof( _moduleName ));
            }

            if (_module == null || !_createPlaceholder)
            {
                throw new ArgumentNullException(nameof( _module ));
            }

            if (!moduleType.IsInterface)
            {
                throw new Exception($"{moduleType.Name} needs to be an interface.");
            }

            lock (m_Modules)
            {
                ModuleItemKey key = new(_moduleName, moduleType);
                if (!m_Modules.TryGetValue(key.InterfaceFullName, out var moduleItems))
                {
                    moduleItems = new Dictionary<ModuleItemKey, ModuleItem>();
                    m_Modules.Add(key.InterfaceFullName, moduleItems);
                }

                if (moduleItems.TryGetValue(key, out var moduleItem))
                {
                    if (moduleItem.Module is null)
                    {
                        if (_createPlaceholder)
                        {
                            return moduleItem;
                        }

                        // ModuleItem existed as a placeholder due to ConnectedObjects pre-subscribing.
                        moduleItem.Module = _module;
                        return moduleItem;
                    }

                    throw new Exception($"A component with the name [{_moduleName}] of type [{moduleType.FullName}] " +
                                        "is already registered in the public container.");
                }

                if (!_createPlaceholder && !moduleType.IsInstanceOfType(_module))
                {
                    throw new Exception($"The object {_module.GetType().FullName} is not a {moduleType.FullName}");
                }

                moduleItem = new ModuleItem(moduleType, Token.Create(), _module, _moduleName);
                moduleItem.CanBeDestroyed += OnModuleItemCanBeDestroyed;
                moduleItems.Add(key, moduleItem);
                m_ModulesByToken.Add(moduleItem.Token, key);

                return moduleItem;
            }
        }

        private ModuleItem? GetModuleItem<T>(string _moduleName) where T : class
        {
            Type moduleType = typeof( T );

            if (!moduleType.IsInterface)
            {
                throw new ArgumentException($"{nameof( GetModuleItem )}: {moduleType.Name} needs to be an interface.");
            }

            return string.IsNullOrWhiteSpace(_moduleName) ? GetModuleItemFromInterfaceFullName<T>(moduleType.Name) : GetModuleItemFromKey<T>(new ModuleItemKey(_moduleName, moduleType));
        }

        private ModuleItem? GetModuleItemFromInterfaceFullName<T>(string _interfaceFullName) where T : class
        {
            lock (m_Modules)
            {
                if (!m_Modules.TryGetValue(_interfaceFullName, out var moduleItems))
                {
                    return m_Parent?.GetModuleItemFromInterfaceFullName<T>(_interfaceFullName);
                }

                var moduleItem = moduleItems.Select(_x => _x.Value)
                    .FirstOrDefault(_x => _x.Module is T || _x.Type == typeof( T ));

                return moduleItem;
            }
        }

        private ModuleItem? GetModuleItemFromKey<T>(ModuleItemKey _key) where T : class
        {
            lock (m_Modules)
            {
                if (!m_Modules.TryGetValue(_key.InterfaceFullName, out var moduleItems)
                    || !moduleItems.TryGetValue(_key, out var moduleItem))
                {
                    return m_Parent?.GetModuleItemFromKey<T>(_key);
                }

                if (!(moduleItem.Module is T || moduleItem.Type == typeof( T )))
                {
                    return null;
                }

                return moduleItem;
            }
        }

        #endregion
    }
}
