using System.Collections.Generic;
using System.Reflection;

namespace Bannerlord.UIEditor.Core.UIExtender
{
    public class UIExtenderManager : Module, IUIExtenderManager
    {
        private readonly Dictionary<string, UIExtenderEx.UIExtender> m_Extenders = new();

        public void Enable(string _moduleName, Assembly _assembly)
        {
            if (!m_Extenders.TryGetValue(_moduleName, out UIExtenderEx.UIExtender uiExtender))
            {
                uiExtender = new UIExtenderEx.UIExtender(_moduleName);
                uiExtender.Register(_assembly);
                m_Extenders.Add(_moduleName, uiExtender);
            }

            uiExtender.Enable();
        }


        public void Disable(string _moduleName)
        {
            if (m_Extenders.TryGetValue(_moduleName, out UIExtenderEx.UIExtender uiExtender))
            {
                uiExtender.Disable();
            }
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<IUIExtenderManager>();
        }
    }
}
