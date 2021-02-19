using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.UIEditor.Core
{
    internal class ModuleItem
    {
        public event EventHandler<ModuleItemKey>? CanBeDestroyed;

        public Type Type { get; }

        public string Name { get; }

        public object? Module
        {
            get => m_Module;
            set
            {
                if (m_Module != value)
                {
                    if (m_Module is not null)
                    {
                        OnUnregistering(m_Module);
                    }

                    m_Module = value;
                    if (m_Module is not null)
                    {
                        OnRegistered(m_Module);
                    }
                    else if (ConnectedObjects.Count == 0)
                    {
                        OnCanBeDestroyed();
                    }
                }
            }
        }

        public Token Token { get; }

        public Dictionary<IConnectedObject, (Action<object> onModuleRegistered, Action<object> onModuleUnregistering)> ConnectedObjects { get; }

        private object? m_Module;

        public ModuleItem(
            Type _type,
            Token _token,
            object? _module,
            string _name)
        {
            ConnectedObjects = new Dictionary<IConnectedObject, (Action<object> onModuleRegistered, Action<object> onModuleUnregistering)>();
            Type = _type;
            Name = _name;
            Module = _module;
            Token = _token;
        }

        public void AddConnectedObject(IConnectedObject _subscriber, Action<object> _onModuleRegistered, Action<object> _onModuleUnregistering)
        {
            _subscriber.Disposing += OnSubscriberDisposing;
            ConnectedObjects.Add(_subscriber, (_onModuleRegistered, _onModuleUnregistering));
        }

        protected virtual void OnCanBeDestroyed()
        {
            CanBeDestroyed?.Invoke(this, new ModuleItemKey(Name, Type));
        }

        private void OnRegistered(object _module)
        {
            foreach (Action<object> onModuleRegistered in ConnectedObjects.Values.Select(_x => _x.onModuleRegistered))
            {
                onModuleRegistered(_module);
            }
        }

        private void OnUnregistering(object _module)
        {
            foreach (Action<object> onModuleUnregistering in ConnectedObjects.Values.Select(_x => _x.onModuleUnregistering))
            {
                onModuleUnregistering(_module);
            }
        }

        private void OnSubscriberDisposing(object? _sender, IConnectedObject _subscriber)
        {
            _subscriber.Disposing -= OnSubscriberDisposing;
            ConnectedObjects.Remove(_subscriber);
            if (ConnectedObjects.Count == 0 && Module is null)
            {
                OnCanBeDestroyed();
            }
        }
    }
}
