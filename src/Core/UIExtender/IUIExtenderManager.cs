using System.Reflection;

namespace Bannerlord.UIEditor.Core.UIExtender
{
    public interface IUIExtenderManager
    {
        void Enable(string _moduleName, Assembly _assembly);
        void Disable(string _moduleName);
    }
}
