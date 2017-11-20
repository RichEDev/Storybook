using ESR_File_Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Configuration;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cN3InboundTransfersTest and is intended
    ///to contain all cN3InboundTransfersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cN3InboundTransfersTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Start(sConnectionString.ToString());
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cN3InboundTransfers.InboundTransferInProgress = false;

            cFTPClient clsClient = new cFTPClient();
            cSecureData clsSecure = new cSecureData();
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            cESRTrust trust = clsTrusts.GetESRTrustByID(cGlobalVariables.NHSTrustID);

            if (trust != null)
            {
                clsClient.DeleteFileFromFTP(trust.FTPAddress, trust.FTPUsername, clsSecure.Decrypt(trust.FTPPassword), "UnitTest.dat");

                cESRTrustObject.DeleteServiceTrust();
            }
            cAutomatedTransferObject.DeleteInboundFileFromDatabase();
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Stop(sConnectionString.ToString());
        }
        
        #endregion

        /// <summary>
        ///A test for UploadInboundFiles where the in progress flag is set to false
        ///</summary>
        [TestMethod()]
        public void UploadInboundFilesWhereInProgressFlagIsFalseTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();

            cN3InboundTransfers target = new cN3InboundTransfers();
            cN3InboundTransfers.InboundTransferInProgress = false;
            FinancialExportStatus status = FinancialExportStatus.CollectedForUploadToESR;
            target.UploadInboundFiles();

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT Status FROM InboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.InboundDataID);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    status = (FinancialExportStatus)reader.GetByte(0);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.AreEqual(FinancialExportStatus.UploadToESRSucceeded, status);
        }

        /// <summary>
        ///A test for UploadInboundFiles where the in progress flag is set to true
        ///</summary>
        [TestMethod()]
        public void UploadInboundFilesWhereInProgressFlagIsTrueTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();
            cN3InboundTransfers target = new cN3InboundTransfers();
            cN3InboundTransfers.InboundTransferInProgress = true;
            FinancialExportStatus status = FinancialExportStatus.CollectedForUploadToESR;
            target.UploadInboundFiles();

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT Status FROM InboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.InboundDataID);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    status = (FinancialExportStatus)reader.GetByte(0);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.AreEqual(FinancialExportStatus.CollectedForUploadToESR, status);
        }

        /// <summary>
        ///A test for UpdateInboundFileStatus
        ///</summary>
        [TestMethod()]
        public void UpdateInboundFileStatusTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();
            cN3InboundTransfers target = new cN3InboundTransfers();

            FinancialExportStatus status = FinancialExportStatus.CollectedForUploadToESR;
            target.UpdateInboundFileStatus(FinancialExportStatus.UploadToESRSucceeded, cGlobalVariables.InboundDataID);

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT Status FROM InboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.InboundDataID);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    status = (FinancialExportStatus)reader.GetByte(0);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.AreEqual(FinancialExportStatus.UploadToESRSucceeded, status);
        }

        /// <summary>
        ///A test for GetInboundFilesForUpload
        ///</summary>
        [TestMethod()]
        public void GetInboundFilesForUploadTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();
            cN3InboundTransfers target = new cN3InboundTransfers();
            List<cESRFileInfo> actual = target.GetInboundFilesForUpload();

            Assert.IsTrue(actual.Count > 0);

            foreach (cESRFileInfo info in actual)
            {
                if (info.DataID == cGlobalVariables.InboundDataID)
                {
                    Assert.AreEqual("UnitTest.dat", actual[0].fileName);
                    Assert.AreEqual(cGlobalVariables.NHSTrustID, actual[0].TrustID);
                    break;
                }
            }
        }

        /// <summary>
        ///A test for cN3InboundTransfers Constructor
        ///</summary>
        [TestMethod()]
        public void cN3InboundTransfersConstructorTest()
        {
            cN3InboundTransfers target = new cN3InboundTransfers();
            Assert.IsNotNull(target);
        }
    }
}
