namespace SELFileTransferService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Net.Mail;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;
    using System.ServiceProcess;
    using System.Text;
    using Common.Logging;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    using SpendManagementLibrary.ESRTransferServiceClasses;

    public partial class SELFileTransferService : ServiceBase
    {
        /// <summary>
        /// Timer interval
        /// </summary>
        double nTimerInterval;

        /// <summary>
        /// Timer process
        /// </summary>
        readonly System.Timers.Timer tmr = new System.Timers.Timer();

        /// <summary>
        /// A private instance of <see cref="EmailSender"/>
        /// </summary>
        private EmailSender _sender;

        /// <summary>
        /// A private instance of <see cref="Remoting"/>
        /// </summary>
        private Remoting _remoting;

        /// <summary>
        /// True if the Logs are being emailed.
        /// </summary>
        private static bool logEmailsRunning;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<SELFileTransferService>().GetLogger();

        public SELFileTransferService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Debug("SELFileTransferService is starting.");
            this.CreateObjects();
            try
            {
                // changed to using an encrypted password in the web/app/exe config
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());

                int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"Port {port}");
                }

                Hashtable props = new Hashtable { { "name", "FileTransferChan" }, { "port", port.ToString() } };

                BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider
                    { TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full };

                ChannelServices.RegisterChannel(new HttpChannel(props, clientProvider, serverProvider), false);
                WellKnownServiceTypeEntry wkste = new WellKnownServiceTypeEntry(typeof(cESRTransferSvc), "ESRTransfer.rem", WellKnownObjectMode.SingleCall);
                RemotingConfiguration.ApplicationName = "ESRTransfers";
                RemotingConfiguration.RegisterWellKnownServiceType(wkste);

                this.nTimerInterval = Convert.ToDouble(ConfigurationManager.AppSettings["WebServicePollTime"]);
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"Web Service Poll Time {this.nTimerInterval}");
                }
                this.tmr.Interval = this.nTimerInterval;
                this.tmr.Elapsed += this.tmr_Elapsed;
                this.tmr.Enabled = true;

                // Set the Outbound import progress flag to false
                cOutboundTransfers.OutboundImportInProgress = false;
                cInboundTransfers.InboundTransfersProcessing = false;
            }
            catch (Exception ex)
            {
                this.SendErrorEmail(ex);
            }
        }

        protected override void OnStop()
        {
            Log.Debug("SELFileTransferService is stopping.");
            try
            {
                // Send email on stop of the service
                
                string errorsAddress = ConfigurationManager.AppSettings["ErrorEmailAddress"];
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"Error Email Address {errorsAddress}");
                }
                string subject = "ESR Bi-directional Service on " + Environment.MachineName + " has stopped";
                string body = "The ESR Bi-directional Service Service on " + Environment.MachineName + " has stopped";

                
                this._sender.SendEmail(new MailMessage("admin@sel-expenses.com", errorsAddress, subject, body));
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex.Message} {ex.StackTrace}");
            }

        }

        private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Log.Info("Timer Elapsed - Starting checks");
                IESR esr = this._remoting.GetServiceInstance();

                cOutboundTransfers clsOutboundTransfers = new cOutboundTransfers(this._remoting);

                Log.Info("Processing outbound files");

                clsOutboundTransfers.ProcessOutboundFiles(esr, this);

                // Check for any inbound files
                cInboundTransfers clsTransfers = new cInboundTransfers(this._remoting);

                Log.Info("Get inbound updated statuses");

                clsTransfers.GetInboundUpdatedStatuses(esr);

                Log.Info("Check for inbound files");

                clsTransfers.CheckForInboundFiles(esr);
                
                // Check for any updated trusts
                Log.Info("Get updated trust details");

                this.GetUpdatedTrustDetails(esr);

                Log.Info("Save Trust update date");
                

                // Update the last check date for the trust details from the server
                this.SaveTrustUpdateDate(esr.GetUpdateServerUTCDateTime());

                Log.Info("Check and send transfer log emails");


                CheckAndSendTransferLogEmails(this._sender);

                esr = null;
            }
            catch (Exception ex)
            {
                this.SendErrorEmail(ex);
            }
        }

        /// <summary>
        /// Send email on error in the Windows service
        /// </summary>
        /// <param name="ex"></param>
        public void SendErrorEmail(Exception ex)
        {
            try
            {
                #region Send error email on any application error

                string errorsAddress = ConfigurationManager.AppSettings["ErrorEmailAddress"];

                string subject = "Error occured in the ESR Bi-directional Service Service on " + Environment.MachineName;
                string message = "An error has occurred on the ESR Bi-directional Service on " + Environment.MachineName + "\n";

                message += "Server: " + Environment.MachineName + "\n";
                message += "\n";
                message += "**Error Information**\n";
                message += "\n";
                message += "**Message**\n";
                message += ex.Message;
                message += "\n\n";
                message += "**Stack trace**\n";
                message += ex.StackTrace;

                MailMessage msg = new MailMessage("admin@sel-expenses.com", errorsAddress, subject, message);
                Log.Warn($"SendErrorEmail -  {subject} - {message}");
                this._sender.SendEmail(msg);
                #endregion
            }
            catch (Exception e)
            {
                Log.Warn($"SendErrorEmail -  {e.Message}  {e.StackTrace}");
            }
        }

        /// <summary>
        /// Get any updated trust details from expenses2009 so they can be updated for the ESR File Transfers
        /// </summary>
        public void GetUpdatedTrustDetails(IESR esr)
        {
            Log.Debug("Get Updated Trust Details");

            try
            {
                cNHSTrusts clsTrusts =
                    new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                List<TrustUpdateInfo> lstUpdatedTrusts = esr.GetUpdatedTrusts(this.GetLastTrustUpdateCheckTime());

                if (lstUpdatedTrusts.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"{lstUpdatedTrusts[0].lstTrusts.Count} trust records found.");
                    }

                    foreach (TrustUpdateInfo updateInfo in lstUpdatedTrusts)
                    {
                        foreach (cESRTrust trust in updateInfo.lstTrusts)
                        {
                            int trustID =
                                clsTrusts.GetTrustIDByExpTrustIDAndAccountID(trust.TrustID, updateInfo.AccountID);
                            if (Log.IsDebugEnabled)
                            {
                                Log.Debug($"Saving trust {trust.TrustName}, id {trustID}.");
                            }
                            clsTrusts.SaveTrust(
                                new cESRTrust(trustID, trust.TrustID, updateInfo.AccountID, trust.TrustName,
                                    trust.TrustVPD, trust.FTPAddress, trust.FTPUsername, trust.FTPPassword,
                                    trust.Archived, trust.CreatedOn, trust.ModifiedOn), updateInfo.AccountID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Exception - " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get the date the last time trusts were checked for updates 
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastTrustUpdateCheckTime()
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            const string SQL = "SELECT LastTrustUpdateDateCheck FROM globalProperties";
            DateTime updateDate = new DateTime(1900, 01, 01);

            using (SqlDataReader reader = smData.GetReader(SQL))
            {
                while (reader.Read())
                {
                    updateDate = reader.GetDateTime(reader.GetOrdinal("LastTrustUpdateDateCheck"));
                }

                reader.Close();
            }

            return updateDate;
        }

        /// <summary>
        /// Save the server web application server date and time to the ESRFileTransfer database
        /// </summary>
        /// <param name="date"></param>
        public void SaveTrustUpdateDate(DateTime date)
        {
            using (var smData = new DatabaseConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString))
            {
                smData.sqlexecute.Parameters.AddWithValue("@Date", date);
                smData.ExecuteProc("dbo.saveTrustUpdateDate");
                smData.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Gets all Inbound and Outbound transfer log items and emails them to the specified reserved email group
        /// </summary>
        public static void CheckAndSendTransferLogEmails(EmailSender emailSender)
        {
            if (logEmailsRunning)
            {
                Log.Debug("Check and Send Transfer Log Emails already running.");
                return;
            }

            Log.Debug("Check and Send Transfer Log Emails");

            logEmailsRunning = true;
            var clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            var clsTransfer = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            var sbBody = new StringBuilder();
            var emailAddress = string.Empty;
            var currentTrustId = -1;
            var info = string.Empty;
            var logItems = new List<cESRTransferLogItem>();
            string subject = string.Empty;

            var lstLogItems = clsTransfer.GetTransferLogItemsToEmail();

            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Found {lstLogItems.Count} messages to email.");
            }

            const string BrandName = "Expenses";

            foreach (cESRTransferLogItem logItem in lstLogItems.Values)
            {
                cESRTrust trust = clsTrusts.GetESRTrustByID(logItem.NHSTrustID);

                if (trust != null)
                {
                    if (trust.TrustID != currentTrustId)
                    {
                        if (subject != string.Empty || sbBody.ToString() != string.Empty)
                        {
                            SendEmail(emailSender, emailAddress, subject, sbBody, clsTransfer, logItems);
                        }

                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug($"Compiling messages for trust {trust.TrustName}.");
                        }

                        info =
                            $"**Trust Information**\nServer: {Environment.MachineName}\nTrust Name: {trust.TrustName}\nTrust VPD: {trust.TrustVPD}\nTrust ID: {trust.TrustID}\n\n\n";
                        currentTrustId = trust.TrustID;
                        logItems = new List<cESRTransferLogItem>();
                        subject = $"SEL File Transfer service - Processing for {trust.TrustName}";
                        sbBody.Clear();
                        sbBody.Append(info);
                    }
                    

                    emailAddress = ConfigurationManager.AppSettings["LogEmailAddress"];

                    #region Create Email subject and body based on the transfer type and log item type

                    string message;

                    switch (logItem.TransferType)
                    {
                        case AutomatedTransferType.ESRInbound:

                            switch (logItem.LogItemType)
                            {

                                case TransferLogItemType.InboundFileProcessedSuccessfully:
                                    message =
                                        $"Inbound File {logItem.Filename} has Processed Successfully to {Environment.MachineName} and is awaiting upload to the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;

                                case TransferLogItemType.InboundFileProcessingFailed:
                                    message =
                                        $"Inbound File {logItem.Filename} has Failed to Process to {Environment.MachineName} and will not upload to the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;

                                case TransferLogItemType.InboundFileUploadedSuccessfully:
                                    message =
                                        $"Inbound File {logItem.Filename} has successfully uploaded to the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    sbBody.Append(logItem.LogItemBody);
                                    break;

                                case TransferLogItemType.InboundFileUploadFailed:
                                    message =
                                        $"Inbound File {logItem.Filename} has failed to upload to the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    sbBody.Append(logItem.LogItemBody);
                                    break;
                            }

                            break;
                        case AutomatedTransferType.ESROutbound:

                            switch (logItem.LogItemType)
                            {
                                case TransferLogItemType.OutboundFileDownloaded:
                                    message =
                                        $"Outbound File {logItem.Filename} has been Successfully Downloaded from the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    sbBody.Append(logItem.LogItemBody);
                                    break;

                                case TransferLogItemType.OutboundFileDownloadedWithoutData:
                                    message =
                                        $"Outbound File {logItem.Filename} has been Downloaded from the NHS Hub but it has no data at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    sbBody.Append(logItem.LogItemBody);
                                    break;

                                case TransferLogItemType.OutboundFileDownloadFailed:
                                    message =
                                        $"Outbound File {logItem.Filename} has failed to downloaded from the NHS Hub at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    sbBody.Append(logItem.LogItemBody);
                                    break;

                                case TransferLogItemType.OutboundFileImportStarted:
                                    message =
                                        $"Outbound File {logItem.Filename} Import has started to process into {BrandName} at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;

                                case TransferLogItemType.OutboundFileImportedSuccessfully:
                                    message =
                                        $"Outbound File {logItem.Filename} has been Successfully Imported into {BrandName} at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;

                                case TransferLogItemType.OutboundFileImportFailed:
                                    message =
                                        $"Outbound File {logItem.Filename} Import into {BrandName} has failed  at {logItem.CreatedOn}.";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;

                                case TransferLogItemType.OutboundFileValidationFailed:
                                    message =
                                        $"Outbound File {logItem.Filename} Import Validation has failed as the Outbound import file format is incorrect.  at {logItem.CreatedOn}";
                                    sbBody.Append(message);
                                    Log.Info(message);
                                    break;
                            }

                            break;
                    }

                    sbBody.Append(Environment.NewLine);
                    logItems.Add(logItem);

                    #endregion
                }
                else
                {
                    emailAddress = ConfigurationManager.AppSettings["ErrorEmailAddress"];

                    switch (logItem.LogItemType)
                    {
                        case TransferLogItemType.ESRServiceStopped:
                            subject = "ESR File Service on the N3 computer has stopped";
                            sbBody.Append($"Server: {Environment.MachineName}\n\n");
                            sbBody.Append($"The ESR File Service on the N3 computer has stopped at {logItem.CreatedOn}");
                            Log.Info($"The ESR File Service on the N3 computer has stopped at {logItem.CreatedOn}");
                            logItems.Add(logItem);
                            break;
                        case TransferLogItemType.ESRServiceErrored:
                            subject = "Error has occured on the ESR File Service on the N3 computer";
                            sbBody.Append($"Server: {Environment.MachineName}\n\n");
                            sbBody.Append($"An Error has occured on the ESR File Service on the N3 computer at {logItem.CreatedOn}");
                            sbBody.Append(logItem.LogItemBody);
                            Log.Info($"An Error has occured on the ESR File Service on the N3 computer \n {logItem.LogItemBody}");
                            logItems.Add(logItem);
                            break;
                        default:
                            UpdateLogStatus(clsTransfer, new List<cESRTransferLogItem>{logItem});
                            break;
                    }
                }
            }

            if (subject != string.Empty || sbBody.ToString() != string.Empty)
            {
                SendEmail(emailSender, emailAddress, subject, sbBody, clsTransfer, logItems);
            }

            logEmailsRunning = false;
            sbBody = null;
            lstLogItems = null;
            Log.Debug("Check and Send Transfer Log Emails - Complete.");
        }

        /// <summary>
        /// Create any instance objects required.
        /// </summary>
        private void CreateObjects()
        {
            string emailServer = ConfigurationManager.AppSettings["EmailServer"];
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Email server {emailServer}");
            }
            this._sender = new EmailSender(emailServer);
            this._remoting = new Remoting();
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="sender">An instance of <see cref="EmailSender"/></param>
        /// <param name="emailAddress">The email address to send the mail to.</param>
        /// <param name="subject">The subject line of the email</param>
        /// <param name="body">The body of the email</param>
        /// <param name="transferLogging">An instance of <see cref="cESRTransferLogging"/></param>
        /// <param name="logItems">A <see cref="List{T}"/> of <seealso cref="cESRTransferLogItem"/> to process as an email</param>
        private static void SendEmail(EmailSender sender, string emailAddress, string subject, StringBuilder body,
            cESRTransferLogging transferLogging, List<cESRTransferLogItem> logItems)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Send email to {emailAddress} - {subject}.");
            }

            if (sender.SendEmail(new MailMessage("admin@sel-expenses.com", emailAddress, subject, body.ToString())))
            {
                UpdateLogStatus(transferLogging, logItems);
            }
        }

        /// <summary>
        /// Set the email status of the transfer log items to sent
        /// </summary>
        /// <param name="transferLogging">An instance of <see cref="cESRTransferLogging"/></param>
        /// <param name="logItems">A <see cref="List{T}"/> of <seealso cref="cESRTransferLogItem"/> to update</param>
        private static void UpdateLogStatus(cESRTransferLogging transferLogging, List<cESRTransferLogItem> logItems)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Update Log Status for {logItems.Count} messages.");
            }
            foreach (var esrTransferLogItem in logItems)
            {
                esrTransferLogItem.EmailSent = true;
                transferLogging.SaveLogItem(esrTransferLogItem);
            }
        }
    }
}
