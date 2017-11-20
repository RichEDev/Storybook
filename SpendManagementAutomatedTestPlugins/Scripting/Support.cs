using System;
using System.CodeDom.Compiler;

namespace SpendManagement.AutomatedTests.Runtime.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="initializerObject"></param>
    internal delegate void AsyncRunner(Object initializerObject);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    internal delegate CompiledScript AsyncCompiler(string data, CompileType type);

    /// <summary>
    /// 
    /// </summary>
    public enum LanguageType : byte
    {
        /// <summary>
        /// 
        /// </summary>
        CSharp,

        /// <summary>
        /// 
        /// </summary>
        JavaScript,

        /// <summary>
        /// 
        /// </summary>
        VisualBasic,

        /// <summary>
        /// 
        /// </summary>
        MSIL
    }

    /// <summary>
    /// 
    /// </summary>
    internal enum CompileType : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Source,

        /// <summary>
        /// 
        /// </summary>
        File
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScriptCompileException : SystemException
    {
        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly int errorCode;

        /// <summary>
        /// 
        /// </summary>
        private readonly string[] errorList;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ErrorList
        {
            get
            {
                return this.errorList;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal ScriptCompileException(string message, CompilerErrorCollection errors)
            : this(message, errors, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        internal ScriptCompileException(string message, CompilerErrorCollection errors, Exception innerException)
            : base(message, innerException)
        {
            this.errorList = new string[errors.Count];

            for (int x = 0; x < errors.Count; x++)
            {
                this.errorList[x] = String.Format("0x{0:x} > {1} - {2}", errors[x].Line, errors[x].ErrorNumber, errors[x].ErrorText);
            }
        }

        #endregion
    }
}