using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bannerlord.UIEditor.Core
{
    public class PublicContainerInfo : IPublicContainerInfo
    {
        public string Name { get; }
        public string? Parent { get; }
        public IReadOnlyList<Type> Types => m_Types;

        public IReadOnlyList<Assembly> DefaultAssemblies => m_Assemblies;

        private Func<IEnumerable<IModule>>? m_CachedInstantiator;

        /// <summary>
        /// The <see cref="Module"/> types associated to the public container defined by this PublicContainerInfo.<br/>
        /// Note that even if a Module doesn't register itself inside of the PublicContainer, its life-cycle is still linked to the PublicContainer.
        /// </summary>
        private readonly List<Type> m_Types;

        private List<Assembly> m_Assemblies;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_name">The name of the PublicContainer to add. Must be unique, and cannot be null or whitespace.</param>
        /// <param name="_parentName">
        /// The name of the PublicContainer to add as a parent.
        /// Leave empty to add the new PublicContainer as a direct descendent of the Main PublicContainer.<br/>
        /// Other than the Main PublicContainer, every PublicContainer must have a valid Parent.<br/>
        /// The new PublicContainer's life-cycle is associated to its parent. If the parent PublicContainer is removed, all of its children will also be removed.
        /// </param>
        public PublicContainerInfo(string _name, string? _parentName, List<Assembly> _defaultAssemblies)
        {
            Name = _name;
            Parent = _parentName;
            m_Assemblies = _defaultAssemblies;
            m_Types = new List<Type>();
        }

        public IEnumerable<IModule> InstantiateModules()
        {
            m_CachedInstantiator ??= CreateModulesInstantiator();

            return m_CachedInstantiator();
        }

        public void AddType(Type _type)
        {
            m_Types.Add(_type);
            if (m_CachedInstantiator is not null)
            {
                m_CachedInstantiator = null;
            }
        }

        private Func<IEnumerable<IModule>> CreateModulesInstantiator()
        {
            IEnumerable<Func<IModule>> moduleInstantiators = from moduleType in Types
                select (Func<IModule>)Expression.Lambda(Expression.Convert(Expression.New(moduleType), typeof( IModule ))).Compile();

            return () => moduleInstantiators.Select(_x => _x());
        }
    }
}
