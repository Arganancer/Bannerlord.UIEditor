using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.UIEditor.Core
{
    public class ModuleCoordinator
    {
        public IPublicContainer MainPublicContainer => m_MainPublicContainer;

        private PublicContainer m_MainPublicContainer = null!;
        private readonly Dictionary<PublicContainer, List<IModule>> m_PublicContainers;

        public ModuleCoordinator()
        {
            m_PublicContainers = new Dictionary<PublicContainer, List<IModule>>();
        }

        /// <summary>
        /// Creates the Main PublicContainer.<br/>
        /// See <seealso cref="AddPublicContainer"/> for more info on how a PublicContainer is created.
        /// </summary>
        /// <param name="_publicContainerInfo" >
        /// The info of the Main <see cref="PublicContainer"/>. The <see cref="Module"/>s in this PublicContainer will have
        /// their life-cycles attached to the lifetime of the entire solution, and so should be chosen carefully/accordingly.
        /// </param>
        public void Start(IPublicContainerInfo _publicContainerInfo)
        {
            m_MainPublicContainer = (PublicContainer)AddPublicContainer(_publicContainerInfo);
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
        /// in <see cref="IPublicContainerInfo.Types"/>, then calls all of the <see cref="Module.Create"/>, followed by all of the
        /// <see cref="Module.Load"/> methods of every module in the PublicContainer (whether those modules register themselves in the public container or not).<br/>
        /// Throws an <see cref="ArgumentException"/> if the <paramref name="_publicContainerInfo.Name"/> param is null or whitespace, or if another PublicContainer
        /// with the same name already exists.<br/>
        /// <seealso cref="PublicContainerInfo"/>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="_publicContainerInfo.Name"/> param is null or whitespace, or if another PublicContainer
        /// with the same name already exists.
        /// </exception>
        public IPublicContainer AddPublicContainer(IPublicContainerInfo _publicContainerInfo)
        {
            if (m_PublicContainers.Any(_p => _p.Key.Name == _publicContainerInfo.Name))
            {
                throw new ArgumentException($"A PublicContainer with the name {_publicContainerInfo.Name} already exist.");
            }

            if (string.IsNullOrWhiteSpace(_publicContainerInfo.Name))
            {
                throw new ArgumentException("A PublicContainer's name cannot be null or white space.");
            }

            var parentPublicContainer = m_PublicContainers.FirstOrDefault(_x => string.Equals(_x.Key.Name, _publicContainerInfo.Parent)).Key;
            if (parentPublicContainer is null && !string.Equals("Main", _publicContainerInfo.Name))
            {
                throw new ArgumentException($"Tried to create a PublicContainer as a child of a non-existent PublicContainer. Name: {_publicContainerInfo.Name} | Parent Name: {_publicContainerInfo.Parent}");
            }

            PublicContainer publicContainer = new(_publicContainerInfo.Name, parentPublicContainer);
            List<IModule> modules = _publicContainerInfo.InstantiateModules().ToList();
            m_PublicContainers.Add(publicContainer, modules);

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
            List<KeyValuePair<PublicContainer, List<IModule>>> childrenToRemove = new();
            foreach (var child in m_PublicContainers.Where(_x => _x.Key.Parent == _publicContainer.Key))
            {
                childrenToRemove.Add(child);
            }

            foreach (var child in childrenToRemove)
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
    }
}
