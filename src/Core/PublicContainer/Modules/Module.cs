using System;
using System.Collections.Generic;

namespace Bannerlord.UIEditor.Core
{
    /// <inheritdoc />
    public abstract class Module : IModule
    {
        #region Protected Properties

        protected IPublicContainer PublicContainer { get; private set; } = null!;

        protected bool Disposed { get; private set; }

        #endregion

        #region Private Fields

        private Token m_RegistrationToken;

        private readonly List<Token> m_SubModuleRegistrationTokens = new();

        #endregion

        #region Constructors

        protected Module()
        {
            Disposed = false;
        }

        #endregion

        #region IConnectedObject Members

        public event EventHandler<IConnectedObject>? Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion

        #region IModule Members

        /// <inheritdoc />
        public virtual void Create(IPublicContainer _publicContainer)
        {
            PublicContainer = _publicContainer;
        }

        /// <inheritdoc />
        public virtual void Load()
        {
        }

        /// <inheritdoc />
        public virtual void Unload()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This register is meant to be used when a module creates and wishes to register instances of its own submodules
        /// that do not register themselves for whatever reason.<br/>
        /// SubModules registered this way have their lifetime associated to the module that registered them. When this module
        /// is disposed (see <see cref="Dispose"/>), its submodules are also unregistered and destroyed.<br/><br/>
        /// <inheritdoc cref="IPublicContainer.RegisterModule{T}"/><br/><br/>
        /// See <seealso cref="IPublicContainer.RegisterModule{T}"/> for more info.
        /// </summary>
        protected virtual void RegisterModule<TInterface>(IModule _instance, string _name = "") where TInterface : class
        {
            var token = PublicContainer.RegisterModule<TInterface>(_instance, _name);
            m_SubModuleRegistrationTokens.Add(token);
        }

        /// <summary>
        /// If only a single instance of this module will ever exist, no need to name it.
        /// If multiple instances can exist and be registered simultaneously, register each instance with
        /// a distinct and representative name.
        /// </summary>
        protected virtual void RegisterModule<TInterface>(string _name = "") where TInterface : class
        {
            RegisterModule<TInterface>(this, _name);
        }

        protected virtual void Dispose(bool _disposing)
        {
            // Execute if resources have not already been disposed.
            if (!Disposed)
            {
                if (_disposing)
                {
                    OnDisposing();
                    for (var i = 0; i < m_SubModuleRegistrationTokens.Count; i++)
                    {
                        var token = m_SubModuleRegistrationTokens[i];
                        if (token.IsValid())
                        {
                            PublicContainer.UnregisterModule(token);
                            m_SubModuleRegistrationTokens[i] = Token.CreateInvalid();
                        }
                    }

                    if (m_RegistrationToken.IsValid())
                    {
                        PublicContainer.UnregisterModule(m_RegistrationToken);
                        m_RegistrationToken = Token.CreateInvalid();
                    }
                }
            }

            Disposed = true;
        }

        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, this);
        }

        #endregion
    }
}
