using System.Configuration;

namespace SpendManagementLibrary
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Global variables as defined in the web config and loaded in the global.asax
    /// </summary>
    public class GlobalVariables
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GlobalVariables"/> class. 
        /// Constructor for GlobalVariables
        /// </summary>
        /// <param name="configType">
        /// </param>
      public GlobalVariables(ApplicationType configType)
        {
            MetabaseConnectionString = GetConnectionString("metabase");
            GlobalAddressDatabaseConnectionString = GetConnectionString("globaladdresses");

            // web application no longer reading from web config and page not created yet in global.asax, so no http object can be passed here.
            if (configType == ApplicationType.Service)
            {
                DefaultModule = (Modules)int.Parse(GetAppSetting("active_module"));
            }

            int reportServicePath;

            int.TryParse(GetAppSetting("ReportsServicePort"), out reportServicePath);
            ReportServicePort = reportServicePath;
            CompiledMode = ProductIntegrity.IsDebugCompilation();

            if (configType == ApplicationType.Web)
            {
                StaticContentLibrary = GetAppSetting("StaticLibraryPath");

                if (StaticContentLibrary.Substring(StaticContentLibrary.Length - 1, 1) == "/")
                {
                    StaticContentLibrary = StaticContentLibrary.Substring(0, StaticContentLibrary.Length - 1);
                }

                StaticContentFolderPath = GetAppSetting("StaticLibraryFolderLocation");

                if (StaticContentFolderPath.Substring(StaticContentFolderPath.Length - 1, 1) == "/")
                {
                    StaticContentFolderPath = StaticContentFolderPath.Substring(0, StaticContentFolderPath.Length - 1);
                }

                LogonPageImagesPath = GetAppSetting("LogonPageImagesPath");
                if (LogonPageImagesPath.EndsWith("/"))
                {
                    LogonPageImagesPath = LogonPageImagesPath.TrimEnd('/');
                }
            }

            ////Configuration related to DVLA Look up licence check API 
            LicenceCheckConsentPortalLiveUrl = GetAppSetting("LicenceCheckConsentPortalLiveUrl");
            LicenceCheckConsentPortalDemoUrl = GetAppSetting("LicenceCheckConsentPortalDemoUrl");
            LicenceCheckPortalAccessMode = GetAppSetting("LicenceCheckPortalAccessMode");
            LicenceCheckPortalAccessUserName = GetAppSetting("LicenceCheckPortalAccessUserName");
            LicenceCheckPortalAccessPassword = GetAppSetting("LicenceCheckPortalAccessPassword");

            GreenLightInfoPage = GetAppSetting("GreenLightInfoPage");
        }

        /// <summary>
        /// The type of application calling this class
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        public enum ApplicationType
        {
            /// <summary>
            /// A website/web application
            /// </summary>
            Web,

            /// <summary>
            /// A windows service/console
            /// </summary>
            Service
        }

        /// <summary>
        /// Gets or sets the global address database connection string for the application
        /// </summary>
        public static string GlobalAddressDatabaseConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the metabase connection string for this website
        /// </summary>
        public static string MetabaseConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the default Module to use
        /// </summary>
        public static Modules DefaultModule { get; set; }

        /// <summary>
        /// Gets or sets the Report Service port number
        /// </summary>
        public static int ReportServicePort { get; set; }

        /// <summary>
        /// Gets or sets the location of our static content
        /// </summary>
        public static string StaticContentLibrary { get; set; }

    	/// <summary>
		/// Gets or sets the folder location of our static content
    	/// </summary>
    	public static string StaticContentFolderPath { get; set; }

    	/// <summary>
        /// Gets or sets the location of our logon page images
        /// </summary>
        public static string LogonPageImagesPath { get; set; }

        /// <summary>
        /// Gets or sets the compilation mode
        /// </summary>
        public static ProductIntegrity.CompileMode CompiledMode { get; set; }

        /// <summary>
        /// Gets or sets the URL used for GreenLight information adverts
        /// </summary>
        public static string GreenLightInfoPage { get; set; }
        
        /// <summary>
        /// Gets or sets the Demo URL used for Licence Check Consent Portal Access
        /// </summary>
        public static string LicenceCheckConsentPortalDemoUrl { get; set; }

        /// <summary>
        /// Gets or sets the  Live URL used for Licence Check Consent Portal Access
        /// </summary>
        public static string LicenceCheckConsentPortalLiveUrl { get; set; }

        /// <summary>
        /// Gets or sets the Mode of Licence Check Portal access (Test/Live)
        /// </summary>
        public static string LicenceCheckPortalAccessMode { get; set; }

        /// <summary>
        /// Gets or sets the  user name for the account to access Licence Check Api
        /// </summary>
        public static string LicenceCheckPortalAccessUserName { get; set; }

        /// <summary>
        /// Gets or sets the Password for the account to access Licence Check Api
        /// </summary>
        public static string LicenceCheckPortalAccessPassword { get; set; }

        #region static data
        /// <summary>
        /// Gets the default mail server if no user is logged in
        /// </summary>
        public static string DefaultEmailServerHostname
        {
            get { return "127.0.0.1"; }
        }

        /// <summary>
        /// Gets the default error email to address.
        /// </summary>
        public static string DefaultErrorEmailToAddress
        {
            get { return "errors@selenity.com"; }
        }

        /// <summary>
        /// Gets the default error email from address.
        /// </summary>
        public static string DefaultErrorEmailFromAddress
        {
            get { return "admin@sel-expenses.com"; }
        }

        /// <summary>
        /// Gets the default modal grid 
        /// </summary>
        public static int DefaultModalGridPageSize
        {
            get { return 10; }
        }

        #endregion

        #region testing - static data

        /// <summary>
        /// Gets the default testing error email to address.
        /// </summary>
        public static string DefaultTestingErrorEmailToAddress
        {
            get { return "internalerrors@selenity.com"; }
        }

        /// <summary>
        /// Gets the default testing error email from address.
        /// </summary>
        public static string DefaultTestingErrorEmailFromAddress
        {
            get { return "errors@sel-expenses.com"; }
        }

        /// <summary>
        /// Gets the default testing error email hostname.
        /// </summary>
        public static string DefaultTestingErrorEmailHostname
        {
            get { return "oxygen"; }
        }

        /// <summary>
        /// Gets the default Application instance name from the .config
        /// </summary>
        public static string DefaultApplicationInstanceName
        {
            get
            {           
                string applicationInstanceName = ConfigurationManager.AppSettings["ApplicationInstanceName"];
               
                if (applicationInstanceName != null)
                {
                    return applicationInstanceName;
                }
                else
                {
                    return "Expenses";
                }
            }
        }
        #endregion

        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
        private static string GetConnectionString(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// The get app setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// The get app setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
        public static bool GetAppSettingAsBoolean(string key)
        {
            return String.Compare(GetAppSetting(key), "true", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
