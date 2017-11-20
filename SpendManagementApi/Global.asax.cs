namespace SpendManagementApi
{
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Http.Filters;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using SpendManagementApi.Bootstrap;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// The API application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static Container container;

        /// <summary>
        /// Performed on startup.
        /// </summary>
        protected void Application_Start()
        {
            // Loads global variables
            new GlobalVariables(GlobalVariables.ApplicationType.Service);

            container = Bootstrapper.Bootstrap();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                // Starts the sql dependency on "metabase" connection string
                var crypt = new cSecureData();
                var connectionString = new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
                connectionString.Password = crypt.Decrypt(connectionString.Password);
                SqlDependency.Start(connectionString.ToString());
            }

            // Caching of data
            new cAccounts().CacheList();
            HostManager.SetHostInformation();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutomapperConfig.Configure();
        }
        
        /// <summary>
        /// Handles any errors in the application and logs them.
        /// </summary>
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            cEventlog.LogEntry(exception.Message, true, EventLogEntryType.Error);
        }
    }
}
