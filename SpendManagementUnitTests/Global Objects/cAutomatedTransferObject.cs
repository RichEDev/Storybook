using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using System.Configuration;
using SELFileTransferService;
using System.Data.SqlClient;
using ESR_File_Service;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cAutomatedTransferObject
    {
        /// <summary>
        /// Create global static automated transfer
        /// </summary>
        /// <returns></returns>
        public static cAutomatedTransfer CreateAutomatedTransfer()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();
            cAutomatedTransfer transfer = new cAutomatedTransfer(cGlobalVariables.AccountID, 999, AutomatedTransferType.ESRInbound , 0, 0);
            return transfer;
        }

        /// <summary>
        /// Create a global static Inbound file record in the database
        /// </summary>
        public static void CreateInboundFileRecord()
        {
            cInboundTransfers target = new cInboundTransfers();
            cAutomatedTransfer transfer = cAutomatedTransferObject.CreateAutomatedTransfer();
            byte[] FileData = new byte[5];
            FinancialExportStatus status = FinancialExportStatus.CollectedForUploadToESR;
            
            target.SaveInboundFile(transfer, "UnitTest.dat", FileData, status);

            #region Set the Global Inbound Data ID Variable

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID FROM InboundFileData WHERE FileName = @FileName";
            smData.sqlexecute.Parameters.AddWithValue("@Filename", "UnitTest.dat");

            int DataID = 0;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            cGlobalVariables.InboundDataID = DataID;

            #endregion

        }

        /// <summary>
        /// Delete the Inbound File record from the database
        /// </summary>
        public static void DeleteInboundFileFromDatabase()
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "DELETE FROM InboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.InboundDataID);
            smData.ExecuteSQL(strSQL);
        }

        /// <summary>
        /// Create a global static Outbound file record in the database
        /// </summary>
        public static void CreateOutboundFileRecord()
        {
            cN3OutboundTransfers clsN3Outbound = new cN3OutboundTransfers();
            byte[] data = cImportTemplateObject.CreateDummyESROutboundFileInfo();
            clsN3Outbound.SaveOutboundFile(cGlobalVariables.NHSTrustID, data, "UnitTestOutbound.dat");

            #region Set the Global Inbound Data ID Variable

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID FROM OutboundFileData WHERE FileName = @FileName";
            smData.sqlexecute.Parameters.AddWithValue("@Filename", "UnitTestOutbound.dat");

            int DataID = 0;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            cGlobalVariables.OutboundDataID = DataID;

            #endregion
        }

        /// <summary>
        /// Delete the Outbound File record from the database
        /// </summary>
        public static void DeleteOutboundFileFromDatabase()
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "DELETE FROM OutboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.OutboundDataID);
            smData.ExecuteSQL(strSQL);
        }
    }
}
