using System;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Data.SqlClient;
using System.Configuration;
using SpendManagementLibrary;

namespace Expenses_Reports
{
    using BusinessLogic.Modules;

    public partial class Service1 : ServiceBase
    {
        private static bool _enableLogging;

        /// <summary>
        /// Service constructor
        /// </summary>
        public Service1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Service Start method
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
             try
            {
                _enableLogging = Convert.ToBoolean(ConfigurationManager.AppSettings["enableLogging"]);
                // Set global variables for use in SML classes
                GlobalVariables.ReportServicePort = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                DiagLog("OnStart", $"  Port - {GlobalVariables.ReportServicePort}");
                
                GlobalVariables.DefaultModule = (Modules) int.Parse(ConfigurationManager.AppSettings["defaultModule"]);
                DiagLog("OnStart", $"Module - {GlobalVariables.DefaultModule}" );
                GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ToString();
                DiagLog("OnStart", $"Connection String - {GlobalVariables.MetabaseConnectionString}" );
                // changed to using an encrypted password in the web/app/exe config
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString =
                    new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);

                if (!new cAccounts().CacheList(false))
                {
                    throw new InvalidOperationException("Accounts cache has not been populated.");
                }

                ChannelServices.RegisterChannel(new TcpChannel(GlobalVariables.ReportServicePort));
                WellKnownServiceTypeEntry wkste = new WellKnownServiceTypeEntry(typeof (cReportsSvc), "reports.rem",
                    WellKnownObjectMode.SingleCall);
                RemotingConfiguration.ApplicationName = "ExpensesReports";
                RemotingConfiguration.RegisterWellKnownServiceType(wkste);

                HostManager.SetHostInformation();
            }
             catch (Exception e)
             {
                 DiagLog("OnStart", e.Message);
                 if (e.InnerException != null)
                 {
                     DiagLog("OnStart - Inner exception", e.InnerException.Message);
                 }
             }
        }

        protected override void OnStop()
        {
            try
            {
                cEmails clsEmails = new cEmails();
                clsEmails.SendOnStopEmail();
            }
            catch (Exception e)
            {
                DiagLog("OnStop", e.Message);
                if (e.InnerException != null)
                {
                    DiagLog("OnStop - Inner exception", e.InnerException.Message);
                }
            }
        }

        /// <summary>
        /// Outputs a message to the EventLog but only if enableLogging is set to "true"
        /// </summary>
        /// <param name="methodName">
        /// The method Name.
        /// </param>
        /// <param name="methodMessage">
        /// The method Message.
        /// </param>
        public static void DiagLog(string methodName, string methodMessage = "")
        {
            if (!_enableLogging)
            {
                return;
            }

            string methodInfo = methodMessage == string.Empty ? "Standard Message" : methodMessage;

            cEventlog.LogEntry(string.Format("ReportEngine : cReportsSvc : {0} : {1}", methodName, methodInfo));
        }
    }
}
