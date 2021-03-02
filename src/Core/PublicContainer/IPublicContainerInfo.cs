using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    public interface IPublicContainerInfo
    {
        string Name { get; }
        string? Parent { get; }
        IReadOnlyList<Type> Types { get; }
        IEnumerable<IModule> InstantiateModules();
    }
}
