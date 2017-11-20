using System.Configuration;
using System.Data.SqlClient;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate
{
    using System;

    /// <summary>
    /// Class to simulate the application code done when the IIS web app starts/stops
    ///  - Will need to be kept in sync with the global.asax from expenses/framework
    /// </summary>
    public class GlobalAsax
    {	
        /// <summary>
        /// Dummy App start
        /// </summary>
		public static void Application_Start()
		{
            // Set global variables for use in SML classes
            GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            GlobalVariables.DefaultModule = GlobalTestVariables.ActiveModule;
            GlobalVariables.StaticContentFolderPath = GlobalTestVariables.StaticLibraryFolderLocation;
            cEventlog.LogEntry("Unit Tests Starting" + GlobalVariables.DefaultModule);

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                // Starts the sql dependency on "metabase" connection string
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());
            }

            // Caching of data
            bool cAccountsCached = new cAccounts().CacheList();
            cTables clsTables = new cTables();
            cFields clsFields = new cFields();
            HostManager.SetHostInformation();
		}

        /// <summary>
        /// Dummy App end
        /// </summary>
		public static void Application_End()
		{
            cEventlog.LogEntry("Unit Tests Shutting down");
            try
            {
                cAccounts clsAccounts = new cAccounts();
                clsAccounts.ToggleDependencies(cAccounts.ToggleDependancy.Stop);
            }
            catch { }
		}
    }
}
