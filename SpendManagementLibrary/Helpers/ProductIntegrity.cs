namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class ProductIntegrity
    {
        public enum CompileMode
        {
            Debug,
            Release
        }

        public static CompileMode IsDebugCompilation()
        {
            if (AppDomain.CurrentDomain.BaseDirectory.Contains("\\Debug"))
            {
                return CompileMode.Debug;
            }

            string[] filesInBin = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin\\", "Spend Management.dll");
            List<string> sortedList = filesInBin.OrderBy(x => x).ToList();

            foreach (string dll in sortedList)
            {
                if (File.Exists(dll) == true)
                {
                    Assembly assembly = System.Reflection.Assembly.LoadFile(dll);
                    if (assembly != null)
                    {
                        foreach (object att in assembly.GetCustomAttributes(false))
                        {
                            if (att.GetType() == System.Type.GetType("System.Diagnostics.DebuggableAttribute"))
                            {
                                if (((System.Diagnostics.DebuggableAttribute)att).IsJITTrackingEnabled)
                                {
                                    return CompileMode.Debug;
                                }
                            }
                        }
                    }
                }
            }

            return CompileMode.Release;
        }

        /// <summary>
        /// Retrieve a list of all dlls and versions in the bin folder for the running application
        /// </summary>
        /// <returns>
        /// The System.Collections.Generic.SortedList`2[TKey -&gt; System.String, TValue -&gt; System.String] containing accounts and the state of dependencies on them. 
        /// </returns>
        public static SortedList<string, string> GetAssemblyVersions()
        {
            // Get all DLL files within the bin folder for the running application.
            string[] filesInBin = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin\\", "*.dll");

            SortedList<string, string> dllVersions = new SortedList<string, string>();

            // read assembly information
            foreach (AssemblyName assembly in filesInBin.Select(AssemblyName.GetAssemblyName).Where(assembly => assembly != null))
            {
                dllVersions.Add(assembly.Name, assembly.FullName);
            }

            return dllVersions;
        }
    }
}
