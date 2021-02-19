using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bannerlord.UIEditor.Core
{
    public class ModuleCoordinator
    {
        public IPublicContainer MainPublicContainer => m_MainPublicContainer;

        private PublicContainer m_MainPublicContainer = null!;
        private readonly Dictionary<PublicContainer, List<IModule>> m_PublicContainers;
        private Dictionary<Assembly, Func<IEnumerable<IModule>>> m_CachedInstantiators;

        public ModuleCoordinator()
        {
            m_CachedInstantiators = new Dictionary<Assembly, Func<IEnumerable<IModule>>>();
            m_PublicContainers = new Dictionary<PublicContainer, List<IModule>>();
        }

        /// <summary>
        /// Creates the Main PublicContainer.<br/>
        /// See <seealso cref="AddPublicContainer"/> for more info on how a PublicContainer is created.
        /// </summary>
        /// <param name="_startupAssemblies" >
        /// The assemblies to associate to the Main PublicContainer. The <see cref="Module"/>s in these assemblies will have
        /// their life-cycles attached to the lifetime of the entire solution, and so should be chosen carefully/accordingly.
        /// </param>
        public void Start(List<Assembly> _startupAssemblies)
        {
            m_MainPublicContainer = (PublicContainer)AddPublicContainer("Main", "", _startupAssemblies);
        }

        /// <summary>
        /// Unloads, then disposes of all modules in all PublicContainers, then clears all PublicContainers from the ModuleCoordinator.<br/>
        /// Should only be called when the software is ending.
        /// </summary>
        public void Stop()
        {
            RemovePublicContainer(m_PublicContainers.FirstOrDefault(_x => _x.Key == m_MainPublicContainer));

            m_PublicContainers.Clear();
        }

        /// <summary>
        /// Creates a <see cref="PublicContainer"/> with the specified Name and Parent, instantiates all <see cref="Module"/> inheritors
        /// in the specified <paramref name="_assemblies"/>, then calls all of the <see cref="Module.Create"/>, followed by all of the
        /// <see cref="Module.Load"/> methods of every registered module in the PublicContainer.<br/>
        /// Leave <paramref name="_parentName"/> empty to add the new PublicContainer as a direct descendent of the Main PublicContainer.<br/><br/>
        /// Throws an <see cref="ArgumentException"/> if the <paramref name="_name"/> param is null or whitespace, or if another PublicContainer
        /// with the same name already exists.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="_name"/> param is null or whitespace, or if another PublicContainer
        /// with the same name already exists.
        /// </exception>
        /// <param name="_name">The name of the PublicContainer to add. Must be unique, and cannot be null or whitespace.</param>
        /// <param name="_parentName">
        /// The name of the PublicContainer to add as a parent.
        /// Leave empty to add the new PublicContainer as a direct descendent of the Main PublicContainer.<br/>
        /// Other than the Main PublicContainer, every PublicContainer must have a valid Parent.<br/>
        /// The new PublicContainer's life-cycle is associated to its parent. If the parent PublicContainer is removed, all of its children will also be removed.
        /// </param>
        /// <param name="_assemblies">
        /// The Assemblies in which to look for <see cref="Module"/> inheritors to instantiate and associate to the new PublicContainer.<br/>
        /// Note that even if a Module doesn't register itself inside of the PublicContainer, its life-cycle is still linked to the new PublicContainer.
        /// </param>
        /// <returns></returns>
        public IPublicContainer AddPublicContainer(string _name, string _parentName, List<Assembly> _assemblies)
        {
            if (m_PublicContainers.Any(_p => _p.Key.Name == _name))
            {
                throw new ArgumentException($"A PublicContainer with the name {_name} already exist.");
            }

            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new ArgumentException("A PublicContainer's name cannot be null or white space.");
            }

            string parentName = string.IsNullOrWhiteSpace(_parentName) ? "Main" : _parentName;
            PublicContainer parentPublicContainer = m_PublicContainers.FirstOrDefault(_x => string.Equals(_x.Key.Name, parentName)).Key;
            if (parentPublicContainer is null && !string.Equals("Main", _name))
            {
                throw new ArgumentException($"Tried to create a PublicContainer as a child of a non-existent PublicContainer. Name: {_name} | Parent Name: {_parentName}");
            }

            PublicContainer publicContainer = new(_name, parentPublicContainer);
            List<IModule> modules = InstantiateModulesInAssemblies(_assemblies);
            m_PublicContainers.Add(publicContainer, modules);

            //IEnumerable<ConnectedWindow> connectedWindowModules = modules.OfType<ConnectedWindow>().ToList();
            //if(connectedWindowModules.Any())
            //{
            //    foreach (ConnectedWindow connectedWindow in connectedWindowModules)
            //    {
            //        connectedWindow.WaitForInitialized();
            //    }
            //}

            foreach (IModule module in modules)
            {
                module.Create(publicContainer);
            }

            foreach (IModule module in modules)
            {
                module.Load();
            }

            return publicContainer;
        }

        public bool ContainsPublicContainer(string _name)
        {
            return TryGetPublicContainer(_name, out IPublicContainer _);
        }

        public bool TryGetPublicContainer(string _name, out IPublicContainer _publicContainer)
        {
            _publicContainer = m_PublicContainers.FirstOrDefault(_x => string.Equals(_name, _x.Key.Name)).Key;
            return _publicContainer is not null;
        }

        /// <summary>
        /// Calls the Dispose then Unload of all <see cref="Module"/>s associated to the <see cref="PublicContainer"/> with <paramref name="_name"/>, as
        /// well as all of the target PublicContainer's child PublicContainers.<br/><br/>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="_name"/> is null or whitespace.<br/>
        /// Throws a <see cref="NullReferenceException"/> a PublicContainer with <paramref name="_name"/> does not exist.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="_name"/> is null or whitespace.</exception>
        /// <exception cref="NullReferenceException">Throws when a PublicContainer with <paramref name="_name"/> does not exist.</exception>
        public void RemovePublicContainer(string _name)
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new ArgumentException("PublicContainer name cannot be null or whitespace");
            }

            var publicContainerToRemove = m_PublicContainers.FirstOrDefault(_x => string.Equals(_x.Key.Name, _name));
            if (publicContainerToRemove.Key is null)
            {
                throw new NullReferenceException($"A PublicContainer with name {_name} did not exist.");
            }

            RemovePublicContainer(publicContainerToRemove);
        }

        private void RemovePublicContainer(KeyValuePair<PublicContainer, List<IModule>> _publicContainer)
        {
            foreach (var child in m_PublicContainers.Where(_x => _x.Key.Parent == _publicContainer.Key))
            {
                RemovePublicContainer(child);
            }

            foreach (IModule module in _publicContainer.Value)
            {
                module.Unload();
                module.Dispose();
            }

            m_PublicContainers.Remove(_publicContainer.Key);
        }

        private List<IModule> InstantiateModulesInAssemblies(IEnumerable<Assembly> _assemblies)
        {
            List<IModule> output = new();
            foreach (Assembly assembly in _assemblies)
            {
                if (!m_CachedInstantiators.TryGetValue(assembly, out Func<IEnumerable<IModule>> moduleInstantiator))
                {
                    Type[] types = assembly.GetTypes().Where(_type => !_type.IsAbstract && !_type.IsInterface && typeof( IModule ).IsAssignableFrom(_type)).ToArray();
                    moduleInstantiator = CreateModulesInstantiator(types.Where(_type => typeof( Module ).IsAssignableFrom(_type)));
                    m_CachedInstantiators.Add(assembly, moduleInstantiator);
                }

                output.AddRange(moduleInstantiator());
            }

            return output;
        }

        private static Func<IEnumerable<IModule>> CreateModulesInstantiator(IEnumerable<Type> _moduleTypes)
        {
            IEnumerable<Func<IModule>> moduleInstantiators = from moduleType in _moduleTypes
                select (Func<IModule>)Expression.Lambda(Expression.Convert(Expression.New(moduleType), typeof( IModule ))).Compile();

            return () => moduleInstantiators.Select(_x => _x());
        }
    }
}
