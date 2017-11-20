using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SpendManagementLibrary;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Data;

namespace ESR_File_Service
{
    /// <summary>
    ///This object deals with the outbound files and the database such as getting the information from the
    ///database and saving the files to the database
    /// </summary>
    public class cN3OutboundTransfers
    {
        private static bool bOutboundCheckInProgress;

        #region Properties

        public static bool OutboundCheckInProgress
        {
            get { return bOutboundCheckInProgress; }
            set { bOutboundCheckInProgress = value; }
        }

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        public cN3OutboundTransfers()
        {
        }

        /// <summary>
        /// Save the Outbound file byte array to the database
        /// </summary>
        /// <param name="NHSTrustID"></param>
        /// <param name="FileData">byte[] of the data</param>
        /// <param name="FileName">Name of the Outbound fuile</param>
        public void SaveOutboundFile(int NHSTrustID, byte[] FileData, string FileName)
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            smData.sqlexecute.Parameters.AddWithValue("@FileData", FileData);
            smData.sqlexecute.Parameters.AddWithValue("@NHSTrustID", NHSTrustID);
            smData.sqlexecute.Parameters.AddWithValue("@FileName", FileName);
            smData.sqlexecute.Parameters.AddWithValue("@FileCreatedOn", DateTime.Now);
            smData.sqlexecute.Parameters.AddWithValue("@FileModifiedOn", DBNull.Value);
            smData.sqlexecute.Parameters.AddWithValue("@Status", OutboundStatus.AwaitingImport);
            
            smData.ExecuteProc("saveOutboundFileData");

            smData.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Get all existing outbound file details for a specific trust
        /// </summary>
        /// <param name="TrustID"></param>
        /// <returns>A list of outbound file details</returns>
        public List<cOutboundFileDetails> GetOutboundFileDetailsFromDB(int TrustID)
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            System.Data.SqlClient.SqlDataReader reader;
            List<cOutboundFileDetails> lstOutboundFiles = new List<cOutboundFileDetails>();
            string FileName;
            DateTime FileCreatedOn;
            DateTime FileModifiedOn;
            string strSQL = "SELECT FileName, FileCreatedOn, FileModifiedOn FROM OutboundFileData WHERE NHSTrustID = @TrustID";
            smData.sqlexecute.Parameters.AddWithValue("@TrustID", TrustID);
            
            using (reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    FileName = reader.GetString(reader.GetOrdinal("FileName"));

                    if (reader.IsDBNull(reader.GetOrdinal("FileCreatedOn")) == true)
                    {
                        FileCreatedOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        FileCreatedOn = reader.GetDateTime(reader.GetOrdinal("FileCreatedOn"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("FileModifiedOn")) == true)
                    {
                        FileModifiedOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        FileModifiedOn = reader.GetDateTime(reader.GetOrdinal("FileModifiedOn"));
                    }

                    lstOutboundFiles.Add(new cOutboundFileDetails(FileName, FileCreatedOn, FileModifiedOn));
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            return lstOutboundFiles;
        }

        /// <summary>
        /// This kicks off a check of all existing trust accounts to see if there are any new outbound files and if so download
        /// and store the files in the database.
        /// </summary>
        public void MonitorOutboundFTPSites()
        {
            cFTPClient clsFTClient = new cFTPClient();

            if (!OutboundCheckInProgress)
            {
                try
                {
                    OutboundCheckInProgress = true;

                    cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    cN3OutboundTransfers clsOutboundTransfers = new cN3OutboundTransfers();
                    List<cOutboundFileDetails> lstExistingOutboundFiles = null;
                    List<string> lstOutboundFiles = null;
                    cSecureData clsSecure = new cSecureData();
                    string decryptedPassword = "";
                    bool fileExists = false;
                    byte[] fileData = new byte[0];

                    foreach (cESRTrust trust in clsTrusts.ListTrusts().Values)
                    {
                        lstExistingOutboundFiles = clsOutboundTransfers.GetOutboundFileDetailsFromDB(trust.TrustID);

                        if (trust.FTPPassword != "")
                        {
                            decryptedPassword = clsSecure.Decrypt(trust.FTPPassword);
                        }

                        lstOutboundFiles = clsFTClient.GetFTPDirectoryFileList(trust.FTPAddress, trust.FTPUsername, decryptedPassword);

                        foreach (string fileName in lstOutboundFiles)
                        {
                            //Reset boolean value for every file iteration
                            fileExists = false;
                            fileData = new byte[0];

                            //Check if the outbound file already exists
                            foreach (cOutboundFileDetails fileDets in lstExistingOutboundFiles)
                            {
                                if (fileName == fileDets.FileName)
                                {
                                    fileExists = true;
                                    break;
                                }
                            }

                            if (!fileExists)
                            {
                                fileData = clsFTClient.DownloadOutboundFile(trust, fileName, decryptedPassword);

                                if (fileData.Length > 0)
                                {
                                    //Save the outbound file to the database
                                    clsOutboundTransfers.SaveOutboundFile(trust.TrustID, fileData, fileName);
                                }
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
                    //Reset the progress check
                    OutboundCheckInProgress = false;
                }
            }

        }
    }
}
