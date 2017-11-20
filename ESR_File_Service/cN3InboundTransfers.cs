using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using ESR_File_Service.Database;
using SpendManagementLibrary;
using SpendManagementLibrary.ESRTransferServiceClasses;

namespace ESR_File_Service
{
    /// <summary>
    /// This class will aid with the process the inbound transfers to the NHS Hub
    /// </summary>
    public class cN3InboundTransfers
    {
        /// <summary>
        /// private property for the inbound transfer progress flag
        /// </summary>
        private static bool bInboundTransferInProgress;

        #region Properties

        /// <summary>
        /// Static boolean that says whether an ESR inbound transfer is in progress
        /// </summary>
        public static bool InboundTransferInProgress
        {
            get { return bInboundTransferInProgress; }
            set { bInboundTransferInProgress = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public cN3InboundTransfers()
        {
        }

        /// <summary>
        /// Sets an SQL Dependency on the Inbound file database table
        /// </summary>
        public void StartInboundMonitor()
        {
            cFTPClient client = new cFTPClient();

            try
            {
                DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

                string strSQL = "SELECT DataID, FileData, FileName, NHSTrustID, FinancialExportID, Status, ExportHistoryID FROM  dbo.InboundFileData";

                smData.sqlexecute.Notification = null;
                smData.sqlexecute.CommandText = strSQL;

                SqlDependency dep = new SqlDependency(smData.sqlexecute);
                dep.OnChange += new OnChangeEventHandler(dep_OnChange);
                smData.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                //Error caught here due to the dependency event handler being able to call this method
                SELN3Service clsService = new SELN3Service();
                cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, clsService.CreateErrorTextString(ex), null, null));
            }
        }

        /// <summary>
        /// This is the on change event handler for the inbound file SQL dependency. Whenever a change is made to the inbound file database
        /// table this event fires to check if any inbound files need transferring to the NHS hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dep = (SqlDependency)sender;
            dep.OnChange -= dep_OnChange;
            cFTPClient client = new cFTPClient();
            UploadInboundFiles();
            StartInboundMonitor(); 
        }

        /// <summary>
        /// Get all file details for inbound files that are ready for upload to the NHS Hub
        /// </summary>
        /// <returns>A list of inbound file details</returns>
        public List<cESRFileInfo> GetInboundFilesForUpload()
        {
            List<cESRFileInfo> lstFiles = new List<cESRFileInfo>();
            byte[] FileData;
            string FileName;
            int TrustID;
            int DataID;

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            string strSQL = "SELECT * FROM InboundFileData WHERE status = @status";
            smData.sqlexecute.Parameters.AddWithValue("@status", (byte)FinancialExportStatus.CollectedForUploadToESR);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(reader.GetOrdinal("DataID"));
                    TrustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));

                    if (reader.IsDBNull(reader.GetOrdinal("FileData")) == true)
                    {
                        FileData = new byte[0];
                    }
                    else
                    {
                        FileData = (byte[])reader.GetSqlBinary(reader.GetOrdinal("FileData"));
                    }

                    FileName = reader.GetString(reader.GetOrdinal("FileName"));

                    if (FileData.Length > 0)
                    {
                        lstFiles.Add(new cESRFileInfo(DataID, TrustID, FileName, FileData));
                    }
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            return lstFiles;
        }

        /// <summary>
        /// This will process through any inbound file required for upload to the NHS hub and upload
        /// </summary>
        public void UploadInboundFiles()
        {
            if (!InboundTransferInProgress)
            {
                try
                {
                    //Set to true so no more transfers can process while one is currently happening
                    InboundTransferInProgress = true;
                    cFTPClient ftpClient = new cFTPClient();
                    List<cESRFileInfo> lstFiles = this.GetInboundFilesForUpload();
                    using (var entities = new EsrNhsHubDatabase())
                    {
                        var ftpServerList = entities.ftpLocations.Where(f => !f.archived);

                        //Loop round all File Data Byte Arrays to send to the ESR Hub
                        foreach (cESRFileInfo fileInfo in lstFiles)
                        {
                            foreach (ftpLocation ftp in ftpServerList)
                            {
                                var status = ftpClient.UploadInboundFile(ftp, fileInfo.fileName, fileInfo.FileData, fileInfo.TrustID);
                                this.UpdateInboundFileStatus(status, fileInfo.DataID);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SELN3Service clsService = new SELN3Service();
                    cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, clsService.CreateErrorTextString(ex), null, null));
                }
                finally
                {
                    InboundTransferInProgress = false;
                }
            }
        }

        /// <summary>
        /// Update Inbound
        /// </summary>
        /// <param name="status"></param>
        /// <param name="DataID">ID of the inbound file data</param>
        public void UpdateInboundFileStatus(FinancialExportStatus status, int DataID)
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            smData.sqlexecute.Parameters.AddWithValue("@DataID", DataID);
            smData.sqlexecute.Parameters.AddWithValue("@Status", status);

            smData.ExecuteProc("dbo.UpdateInboundFileStatus");
            smData.sqlexecute.Parameters.Clear();
        }

    }
}
