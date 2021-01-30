using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bannerlord.UIEditor.Core
{
    public class ModuleCoordinator
    {
        #region Public Properties

        public IPublicContainer PublicContainer => m_PublicContainer;

        #endregion

        #region Private Fields

        private readonly List<Assembly> m_StartupAssemblies;
        private readonly PublicContainer m_PublicContainer;
        private readonly List<IModule> m_Modules;

        #endregion

        #region Constructors

        public ModuleCoordinator(List<Assembly> _startupAssemblies)
        {
            m_StartupAssemblies = _startupAssemblies;
            m_Modules = new List<IModule>();
            m_PublicContainer = new PublicContainer("Main");
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            Initialize();
            foreach (IModule module in m_Modules)
            {
                module.Create(m_PublicContainer);
            }

            foreach (IModule module in m_Modules)
            {
                module.Load();
            }
        }

        public void Stop()
        {
            foreach (IModule module in m_Modules)
            {
                module.Unload();
            }
        }

        public void Initialize()
        {
            Type[] types = m_StartupAssemblies.SelectMany(_assembly => _assembly.GetTypes())
                .Where(_type => !_type.IsAbstract && !_type.IsInterface && typeof( IModule ).IsAssignableFrom(_type)).ToArray();
            m_Modules.AddRange(InstantiateAndGetBaseModules(types.Where(_type => typeof( System.Reflection.Module ).IsAssignableFrom(_type))));
        }

        #endregion

        #region Private Methods

        private static IEnumerable<IModule> InstantiateAndGetBaseModules(IEnumerable<Type> _baseModuleTypes)
        {
            List<IModule> modules = new();
            foreach (Type baseModuleType in _baseModuleTypes)
            {
                modules.Add((IModule)Activator.CreateInstance(baseModuleType)!);
            }

            return modules;
        }

        #endregion
    }
}
