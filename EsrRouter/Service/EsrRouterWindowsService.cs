namespace EsrRouter.Service
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Routing;
    using System.ServiceProcess;
    using System.Threading;

    public partial class EsrRouterWindowsService : ServiceBase
    {
        #region Static Fields

        public static bool EnableDiagnostics;

        #endregion

        #region Fields

        private ServiceHost host;

        #endregion

        #region Constructors and Destructors

        public EsrRouterWindowsService()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnStart(string[] args)
        {
            EventLog eventLog = new EventLog("ESR Services")
            {
                Source = "File Transfer Service - ESR Router"
            };

            string threadIds = this.CurrentThreadIdentifier();
            eventLog.WriteEntry(string.Format("{0}Service starting", threadIds));

            if (this.host != null)
            {
                this.host.Close();
            }

            // Set up any configuration variables
            EnableDiagnostics = Convert.ToBoolean(ConfigurationManager.AppSettings["enableDiagnostics"] ?? "false");

            try
            {
                // Create the servicehost that will run the relay service
                this.host = new ServiceHost(typeof(RoutingService));
                this.host.Open();
                this.host.Faulted += this.HostFaulted;
            }
            catch (Exception exception)
            {
                eventLog.WriteEntry(string.Format("{0}Error whilst creating/opening EsrRouter servicehost - {1}", threadIds, exception.Message), EventLogEntryType.Error);
                throw;
            }

            // Log the service name with all listening addresses
            eventLog.WriteEntry(string.Format("{0}{1} listening at {2}", threadIds, this.ServiceName, this.host.BaseAddresses.Aggregate(string.Empty, (current, address) => current + (" " + address.AbsoluteUri))), EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            if (this.host != null)
            {
                this.host.Close();
            }

            this.host = null;
        }

        private void HostFaulted(object sender, EventArgs e)
        {
            EventLog eventLog = new EventLog("ESR Services")
            {
                Source = "File Transfer Service - ESR Router"
            };

            eventLog.WriteEntry(string.Format("{0}{1} has faulted (File Relay), notify administrators of this problem", this.CurrentThreadIdentifier(), this.ServiceName), EventLogEntryType.Error);
        }

        private string CurrentThreadIdentifier()
        {
            return string.Format("[{0}] AppDomain.GetCurrentThreadId()\n[{1}] Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)\n\n", AppDomain.GetCurrentThreadId(), Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

    }
}