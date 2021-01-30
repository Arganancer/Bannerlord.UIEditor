using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIEditor.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.UIEditor.Main
{
	public class UIEditorSubModule : MBSubModuleBase
	{
		private ModuleCoordinator m_ModuleCoordinator = null!;

        protected override void OnSubModuleLoad()
        {
            List<Assembly> startupAssemblies = new()
            {
                Assembly.Load(typeof(ModuleCoordinator).Assembly.FullName!),
            };

            m_ModuleCoordinator = new ModuleCoordinator(startupAssemblies);
            m_ModuleCoordinator.Start();

            base.OnSubModuleLoad();
        }
	}
}
