using System;
using System.Reflection;

namespace SpendManagement.AutomatedTests.Runtime.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ExecutionContext
    {
        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private CompiledScript _script;

        /// <summary>
        /// 
        /// </summary>
        private AsyncRunner asyncRunner;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public CompiledScript Script
        {
            get
            {
                return this._script;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void Run(Object initializerObject)
        {
            foreach (Type type in this.Script.Assembly.GetTypes())
            {
                Type interfaceType = type.GetInterface(typeof(IScript).ToString());

                if (interfaceType != null)
                {
                    Object script = Activator.CreateInstance(type);

                    if (script != null)
                    {
                        type.InvokeMember("Initialize", BindingFlags.InvokeMethod, null, script, new object[] { });
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IAsyncResult BeginRun(Object initializerObject, AsyncCallback callback, Object state)
        {
            return this.asyncRunner.BeginInvoke(initializerObject, callback, state);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndRun(IAsyncResult result)
        {
            this.asyncRunner.EndInvoke(result);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        internal ExecutionContext(CompiledScript script)
        {
            this.asyncRunner = new AsyncRunner(this.Run);
            this._script = script;
        }

        #endregion
    }
}
