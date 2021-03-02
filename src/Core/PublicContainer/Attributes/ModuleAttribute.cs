using System;

namespace Bannerlord.UIEditor.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Attribute to decorate <see cref="Module"/> classes when their startup information is more complex than the default.<br/>
        /// By default, a <see cref="Module"/> not decorated with a <see cref="ModuleAttribute"/> will be considered to have both <see cref="LoadWhenStandalone"/>
        /// and <see cref="LoadWhenSubModule"/> set to true, and it will be associated to the default public container for its Assembly.
        /// </summary>
        /// <param name="_publicContainerName">The name of the public container the module wishes to load into.
        /// Leave null to associate the module to the default public container for its Assembly.</param>
        /// <param name="_loadWhenStandalone">If true, loads the module normally when the UIEditor is launched as a standalone exe.</param>
        /// <param name="_loadWhenSubModule">If true, loads the module normally when the UIEditor is launched as a Bannerlord SubModule.</param>
        public ModuleAttribute(string? _publicContainerName = null, bool _loadWhenStandalone = true, bool _loadWhenSubModule = true)
        {
            PublicContainerName = _publicContainerName;
            LoadWhenStandalone = _loadWhenStandalone;
            LoadWhenSubModule = _loadWhenSubModule;
        }

        /// <summary>
        /// The name of the public container the module wishes to load into.
        /// </summary>
        public string? PublicContainerName { get; }

        /// <summary>
        /// If true, loads the module normally when the UIEditor is launched as a standalone exe.
        /// </summary>
        public bool LoadWhenStandalone { get; }

        /// <summary>
        /// If true, loads the module normally when the UIEditor is launched as a Bannerlord SubModule.
        /// </summary>
        public bool LoadWhenSubModule { get; }
    }
}
