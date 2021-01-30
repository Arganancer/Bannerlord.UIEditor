using System;

namespace Bannerlord.UIEditor.Core
{
    public interface IPublicContainer
    {
        #region Public Properties

        string Name { get; }

        IPublicContainer? Parent { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called from the <see cref="IModule.Create"/> method.<br/>
        /// Use this to register an <see cref="IModule"/> component in the public container.<br/><br/>
        /// Throws an <see cref="ArgumentException"/> when the generic type (T) is not an interface.
        /// </summary>
        /// <typeparam name="T">The interface of the module you wish to register.</typeparam>
        /// <param name="_module">The module to register.</param>
        /// <param name="_moduleName">
        /// (Optional) The name of the registering module.<br/>
        /// If only a single instance of the registering module's interface will ever be registered at once, no need to name it.
        /// If multiple modules registering as the given interface can registered simultaneously, register each instance with
        /// a distinct and representative name.
        /// </param>
        Token RegisterModule<T>(IModule _module, string _moduleName = "") where T : class;

        void UnregisterModule(Token _token);

        /// <summary>
        /// If called from an <see cref="IModule"/>, should be called inside the <see cref="IModule.Load"/> method.
        /// Returns a registered module that implements the specified interface.<br/>
        /// Use this when the target module is supposed to be registered at the time of the call.<br/>
        /// Otherwise, use <see cref="ConnectToModule{T}"/>.<br/><br/>
        /// Throws a <see cref="NullReferenceException"/> when the requested module is not registered in the public container.<br/>
        /// Throws an <see cref="ArgumentException"/> when the generic type (T) is not an interface.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when the requested module is not registered in the public container.</exception>
        /// <exception cref="ArgumentException">Thrown when the generic type (T) is not an interface.</exception>
        /// <typeparam name="T">The interface of the type of module you wish to get.</typeparam>
        /// <param name="_moduleName">Name of the module to return. Leave blank to get the first module of the specified type.</param>
        T? GetModule<T>(string _moduleName = "") where T : class;

        /// <summary>
        /// If called from an <see cref="IModule"/>, should be called inside the <see cref="IModule.Load"/> method.
        /// Connects the given <paramref name="_connectedObject"/> to a module specified by the given interface (T).<br/>
        /// Use this when the target module can be registered and unregistered repeatedly during the life-cycle of the connected object.<br/>
        /// Otherwise, use <see cref="GetModule{T}"/>.<br/><br/>
        /// Throws an <see cref="ArgumentException"/> when the generic type (T) is not an interface.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the generic type (T) is not an interface.</exception>
        /// <typeparam name="T">The interface of the type of module you wish to connect to.</typeparam>
        /// <param name="_connectedObject">The object you wish to connect. Ideally the IConnectedObject in question should be connecting itself via a "this".</param>
        /// <param name="_onConnectedModuleRegistered">The method to call when the targeted module is registered.</param>
        /// <param name="_onConnectedModuleUnregistering">The method to call when the targeted module is unregistering itself.</param>
        /// <param name="_moduleName">Name of the module to connect to. Leave blank to connect to the first module of the specified type.</param>
        void ConnectToModule<T>(
            IConnectedObject _connectedObject,
            Action<T> _onConnectedModuleRegistered,
            Action<T> _onConnectedModuleUnregistering,
            string _moduleName = "") where T : class;

        #endregion
    }
}
