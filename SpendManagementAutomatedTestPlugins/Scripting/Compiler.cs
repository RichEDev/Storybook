using System;
using System.Reflection;
using System.CodeDom.Compiler;

namespace SpendManagement.AutomatedTests.Runtime.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Compiler : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly CodeDomProvider _engine;

        /// <summary>
        /// 
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        private LanguageType _sourceLanguage;

        /// <summary>
        /// 
        /// </summary>
        private AsyncCompiler asyncCompiler;

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

        #endregion

        #region Internal Properties

        /// <summary>
        /// 
        /// </summary>
        internal CodeDomProvider Engine
        {
            get
            {
                return this._engine;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Compiler CreateJavaScriptCompiler()
        {
            return new Compiler(LanguageType.JavaScript);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Compiler CreateVisualBasicCompiler()
        {
            return new Compiler(LanguageType.VisualBasic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Compiler CreateCSharpCompiler()
        {
            return new Compiler(LanguageType.CSharp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginCompileFromFile(string filename, AsyncCallback callback, object state)
        {
            return this.asyncCompiler.BeginInvoke(filename, CompileType.File, callback, state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public CompiledScript CompileFromFile(string filename)
        {
            return this.Compile(filename, CompileType.File);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public CompiledScript EndCompileFromFile(IAsyncResult result)
        {
            return this.asyncCompiler.EndInvoke(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginCompileFromSource(string source, AsyncCallback callback, object state)
        {
            return this.asyncCompiler.BeginInvoke(source, CompileType.Source, callback, state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CompiledScript CompileFromSource(string source)
        {
            return this.Compile(source, CompileType.Source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public CompiledScript EndCompileFromSource(IAsyncResult result)
        {
            return this.asyncCompiler.EndInvoke(result);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        ~Compiler()
        {
            this.Dispose(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Internally constructs the Compiler class with the correct
        /// Code Provider required by public constructors
        /// </summary>
        /// <param name="file"></param>
        private Compiler(LanguageType type)
        {
            switch (type)
            {
                case LanguageType.CSharp:
                    this._engine = new Microsoft.CSharp.CSharpCodeProvider();
                    break;

                case LanguageType.JavaScript:
                    this._engine = new Microsoft.JScript.JScriptCodeProvider();
                    break;

                case LanguageType.VisualBasic:
                    this._engine = new Microsoft.VisualBasic.VBCodeProvider();
                    break;

                case LanguageType.MSIL:
                    throw new NotSupportedException("MSIL is not a supported compilation type. It is runtime only");
            }

            this._sourceLanguage = type;
            this.asyncCompiler = new AsyncCompiler(this.Compile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <param name="type"></param>
        private CompiledScript Compile(string script, CompileType type)
        {
            CompilerResults compileResults;

            CompilerParameters settings = new CompilerParameters();
            settings.IncludeDebugInformation = false;
            settings.GenerateExecutable = false;
            settings.ReferencedAssemblies.Add("System.dll");
            settings.ReferencedAssemblies.Add(typeof(Compiler).Assembly.Location);

            switch (type)
            {
                case CompileType.File:
                    settings.GenerateInMemory = false;
                    compileResults = this.Engine.CompileAssemblyFromFile(settings, script);
                    break;

                case CompileType.Source:
                    settings.GenerateInMemory = true;
                    compileResults = this.Engine.CompileAssemblyFromSource(settings, script);
                    break;

                default:
                    throw new InvalidOperationException("Internal Compilation Error");
            }

            if (compileResults.Errors.Count > 0)
            {
                throw new ScriptCompileException("Errors occured compiling the script", compileResults.Errors);
            }

            return new CompiledScript(compileResults.CompiledAssembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                if (!disposing)
                {
                    this._engine.Dispose();
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
