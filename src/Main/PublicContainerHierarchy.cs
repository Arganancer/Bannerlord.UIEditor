using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bannerlord.ButterLib.Common.Extensions;
using Bannerlord.UIEditor.AppContext;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Gauntlet;
using Bannerlord.UIEditor.MainFrame;
using Bannerlord.UIEditor.WidgetLibrary;
using Module = Bannerlord.UIEditor.Core.Module;

namespace Bannerlord.UIEditor.Main
{
    public sealed class PublicContainerHierarchy
    {
        public static PublicContainerHierarchy Instance => LazyInstance.Value;
        private static readonly Lazy<PublicContainerHierarchy> LazyInstance = new(() => new PublicContainerHierarchy());

        private readonly Dictionary<string, PublicContainerInfo> m_PublicContainerInfos;

        private PublicContainerHierarchy()
        {
            m_PublicContainerInfos = new Dictionary<string, PublicContainerInfo>
            {
                {
                    "Main", new PublicContainerInfo("Main", null, new List<Assembly>
                    {
                        typeof( UIEditorLauncher ).Assembly, // Main
                        Assembly.Load(typeof( IPublicContainer ).Assembly.FullName), // Core
                        Assembly.Load(typeof( IApplicationManager ).Assembly.FullName) // AppContext
                    })
                },
                {
                    "UIEditor", new PublicContainerInfo("UIEditor", "Main", new List<Assembly>
                    {
                        Assembly.Load(typeof( IMainWindow ).Assembly.FullName), // MainFrame
                        Assembly.Load(typeof( IWidgetManager ).Assembly.FullName), // WidgetLibrary
                        Assembly.Load(typeof( IGauntletManager ).Assembly.FullName) // Gauntlet
                    })
                }
            };

            foreach (var (_, publicContainerInfo) in m_PublicContainerInfos)
            {
                foreach (Assembly assembly in publicContainerInfo.DefaultAssemblies)
                {
                    ExtractAssemblyModules(assembly, publicContainerInfo);
                }
            }
        }

        public PublicContainerInfo GetPublicContainerInfo(string _name)
        {
            return m_PublicContainerInfos[_name];
        }

        private void ExtractAssemblyModules(Assembly _assembly, PublicContainerInfo _defaultPublicContainer)
        {
            List<Type> types = _assembly.SafeGetTypes(_type => !_type.IsAbstract && !_type.IsInterface && typeof( Module ).IsAssignableFrom(_type)).ToList();
            foreach (Type type in types)
            {
                var moduleAttribute = type.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttribute is null)
                {
                    _defaultPublicContainer.AddType(type);
                }
                else
                {
#if STANDALONE_EDITOR
                    if (moduleAttribute.LoadWhenStandalone)
                    {
                        if (moduleAttribute.PublicContainerName is null)
                        {
                            _defaultPublicContainer.AddType(type);
                        }
                        else
                        {
                            m_PublicContainerInfos[moduleAttribute.PublicContainerName].AddType(type);
                        }
                    }
#else
                    if (moduleAttribute.LoadWhenSubModule)
                    {
                        if (moduleAttribute.PublicContainerName is null)
                        {
                            _defaultPublicContainer.AddType(type);
                        }
                        else
                        {
                            m_PublicContainerInfos[moduleAttribute.PublicContainerName].AddType(type);
                        }
                    }
#endif
                }
            }
        }
    }
}
