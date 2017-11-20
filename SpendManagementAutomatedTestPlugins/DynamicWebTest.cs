using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.WebTesting;
using SpendManagement.AutomatedTests.Runtime.Plugins;
using SpendManagement.AutomatedTests.Runtime.Scripting;

namespace SpendManagement.AutomatedTests.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DynamicWebTest : WebTest
    {
        /// <summary>
        /// 
        /// </summary>
        private ViewStateHandler ViewStatePlugin = new ViewStateHandler();

        /// <summary>
        /// 
        /// </summary>
        private Compiler ExtensionScriptCompiler = Compiler.CreateJavaScriptCompiler();

        /// <summary>
        /// 
        /// </summary>
        public DynamicWebTest()
        {
            // Assign plugin event handlers
            this.PreRequest += new EventHandler<PreRequestEventArgs>(this.ViewStatePlugin.PreRequest);
        }
    }
}
