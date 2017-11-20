namespace EsrNhsHubFileTransferService.Service
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Timers;

    using EsrNhsHubFileTransferService.Code.CommandChannel;
    using EsrNhsHubFileTransferService.Code.Helpers;
    using EsrNhsHubFileTransferService.Code.Transfers;

    /// <summary>
    ///     Main windows service class
    /// </summary>
    public partial class EsrNhsHubWindowsService : ServiceBase
    {
        #region Fields

        private ServiceHost host;
        private ServiceHost commandHost;

        private readonly Timer timer = new Timer(300000);
        private Timer immediateTimer;
        
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Standard constructor
        /// </summary>
        public EsrNhsHubWindowsService()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Standard onStart method, sets up the global vars, then timers, then starts the services
        /// </summary>
        /// <param name="args">Parameters</param>
        protected override void OnStart(string[] args)
        {
            Log.ImportantInformation("Service starting");

            this.SetupGlobals();

            this.SetupTimer();

            this.StartHosts();

            this.ProcessImmediately();
        }

        /// <summary>
        ///     Standard onStop method
        /// </summary>
        protected override void OnStop()
        {
            Log.ImportantInformation("Service stopping");

            this.StopHosts();
        }

        /// <summary>
        ///     Prepares the Global vars from the config file or defaults
        /// </summary>
        private void SetupGlobals()
        {
            try
            {
                GlobalVariables.StartImmediately = bool.Parse(ConfigurationManager.AppSettings["StartImmediately"] ?? "false");
                GlobalVariables.ExtendedLogging = bool.Parse(ConfigurationManager.AppSettings["ExtendedLogging"] ?? "false");
                GlobalVariables.PollIntervalMilliseconds = double.Parse(ConfigurationManager.AppSettings["PollIntervalMilliseconds"] ?? "1800000");
                GlobalVariables.DeleteFilesXDaysAfterProcessed = int.Parse(ConfigurationManager.AppSettings["DeleteFilesXDaysAfterProcessed"] ?? "7");
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst setting up GlobalVariables - {0}", exception.Message);
                throw;
            }
        }

        /// <summary>
        ///     The repeating timer based on the config poll interval time
        /// </summary>
        private void SetupTimer()
        {
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
            this.timer.Interval = GlobalVariables.PollIntervalMilliseconds;
            this.timer.Elapsed += this.TimerElapsed;
        }

        /// <summary>
        ///     Sets up the file and command service hosts
        /// </summary>
        private void StartHosts()
        {
            if (this.host != null)
            {
                this.host.Close();
            }
            if (this.commandHost != null)
            {
                this.commandHost.Close();
            }

            try
            {
                this.host = new ServiceHost(typeof(EsrNhsHubWindowsService));
                this.host.Open();
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst creating/opening windows servicehost - {0}", exception.Message);
                throw;
            }

            try
            {
                this.commandHost = new ServiceHost(typeof(EsrNhsHubCommands));
                this.commandHost.Open();
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst creating/opening commands servicehost - {0}", exception.Message);
                throw;
            }
        }

        /// <summary>
        ///     Stops the file and command service hosts
        /// </summary>
        private void StopHosts()
        {
            if (this.host != null)
            {
                this.host.Close();
            }
            if (this.commandHost != null)
            {
                this.commandHost.Close();
            }
            this.host = null;
            this.commandHost = null;
        }

        /// <summary>
        ///     A run-once timer to process files at start up
        /// </summary>
        private void ProcessImmediately()
        {
            if (!GlobalVariables.StartImmediately)
            {
                return;
            }

            Log.Information("TransferFiles at startup is enabled");

            this.immediateTimer = new Timer(1000) { AutoReset = false, Enabled = true };
            this.immediateTimer.Elapsed += this.TimerElapsed;
        }

        /// <summary>
        ///     Trigger the transfers
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Parameters</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Information("Service timer has elapsed");

            this.TransferFiles();
        }

        /// <summary>
        ///     Thread-safe start of the transfers processing
        /// </summary>
        private void TransferFiles()
        {
            Log.Information("Beginning TransferFiles");

            lock (SyncRoot)
            {
                Log.Information("SyncRoot not locked - beginning transfers");

                Transfers.Begin();
            }
        }

        #endregion
    }
}