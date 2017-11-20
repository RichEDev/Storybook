using System;
using System.Reflection;

namespace SpendManagement.AutomatedTests.Runtime.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CompiledScript
    {
        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly Assembly _assembly;

        /// <summary>
        /// 
        /// </summary>
        private LanguageType _sourceLanguage;

        /// <summary>
        /// 
        /// </summary>
        private ExecutionContext _executionContext;

        /// <summary>
        /// 
        /// </summary>
        private bool _cached;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public LanguageType SourceLanguage
        {
            get
            {
                return this._sourceLanguage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Cached
        {
            get
            {
                return this._cached;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExecutionContext ExecutionContext
        {
            get
            {
                if (this._executionContext == null)
                {
                    this._executionContext = new ExecutionContext(this);
                }

                return this._executionContext;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// 
        /// </summary>
        internal Assembly Assembly
        {
            get
            {
                return this._assembly;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void FlushCache()
        {
            throw new NotImplementedException("FlushCache");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cache()
        {
            throw new NotImplementedException("Cache");
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        internal CompiledScript(Assembly assembly)
            : this(assembly, LanguageType.MSIL)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="language"></param>
        internal CompiledScript(Assembly assembly, LanguageType language)
        {
            this._assembly = assembly;
            this._sourceLanguage = language;
        }

        #endregion
    }
}
