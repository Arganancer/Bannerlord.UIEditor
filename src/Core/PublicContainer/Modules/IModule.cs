namespace Bannerlord.UIEditor.Core
{
    public interface IModule : IConnectedObject
    {
        #region Public Methods

        /// <summary>
        ///     Used to register the <see cref="IModule" /> into the given <see cref="PublicContainer" />.<br />
        ///     Any other initialization should also be done here.
        /// </summary>
        void Create(IPublicContainer _publicContainer);

        /// <summary>
        ///     Used to connect to other objects in the <see cref="PublicContainer" />.
        /// </summary>
        void Load();

        /// <summary>
        ///     Used to sever links with any objects in the <see cref="PublicContainer" />, such as unsubscribing from events.
        /// </summary>
        void Unload();

        #endregion
    }
}
