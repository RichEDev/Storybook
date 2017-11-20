using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Timers;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using SpendManagementLibrary.ESRTransferServiceClasses;
using SpendManagementLibrary;


namespace ESR_File_Service
{
    public partial class SELN3Service : ServiceBase
    {
        /// <summary>
        /// Timer interval
        /// </summary>
        double nTimerInterval;
        /// <summary>
        /// Timer process
        /// </summary>
        System.Timers.Timer tmr = new System.Timers.Timer();

        public SELN3Service()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start the ESR File Service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                // changed to using an encrypted password in the web/app/exe config
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());

                nTimerInterval = Convert.ToDouble(ConfigurationManager.AppSettings["WebServicePollTime"]);
                tmr.Interval = nTimerInterval;
                tmr.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
                tmr.Enabled = true;

                cN3OutboundTransfers.OutboundCheckInProgress = false;
                cN3InboundTransfers.InboundTransferInProgress = false;
                cN3InboundTransfers clsTransfers = new cN3InboundTransfers();
                clsTransfers.StartInboundMonitor();
            }
            catch (Exception ex)
            {
                cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, CreateErrorTextString(ex), null, null));
            }
        }

        /// <summary>
        /// Stop the ESR Transfer Service
        /// </summary>
        protected override void OnStop()
        {
            cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceStopped, AutomatedTransferType.None, 0, "", false, "", null, null));
        }

        /// <summary>
        /// Create the log item body for any errors
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string CreateErrorTextString(Exception ex)
        {
            string message = "\n";

            message += "**Message**\n";
            message += ex.Message;
            message += "\n\n";
            message += "**Stack trace**\n";
            message += ex.StackTrace;

            return message;
        }
        
        /// <summary>
        /// Timer event that checks for any Outbound files on an FTP Site. This is checked every time the timer elapsed time is reached 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                cN3OutboundTransfers clsOutbound = new cN3OutboundTransfers();
                clsOutbound.MonitorOutboundFTPSites();
                cN3InboundTransfers clsInbound = new cN3InboundTransfers();
                clsInbound.UploadInboundFiles();
            }
            catch (Exception ex)
            {
                cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, CreateErrorTextString(ex), null, null));
            }
        }      
    }
}
