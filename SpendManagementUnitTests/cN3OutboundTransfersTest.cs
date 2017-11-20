using ESR_File_Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using System.Configuration;
using System.Data.SqlClient;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cN3OutboundTransfersTest and is intended
    ///to contain all cN3OutboundTransfersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cN3OutboundTransfersTest
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
            cESRTrustObject.DeleteServiceTrust();
            cN3OutboundTransfers.OutboundCheckInProgress = false;
            cAutomatedTransferObject.DeleteOutboundFileFromDatabase();
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Stop(sConnectionString.ToString());
        }
        
        #endregion


        /// <summary>
        ///A test for cN3OutboundTransfers Constructor
        ///</summary>
        [TestMethod()]
        public void cN3OutboundTransfersConstructorTest()
        {
            cN3OutboundTransfers target = new cN3OutboundTransfers();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for GetOutboundFileDetailsFromDB
        ///</summary>
        [TestMethod()]
        public void GetOutboundFileDetailsFromDBTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();
            cAutomatedTransferObject.CreateOutboundFileRecord();
            cN3OutboundTransfers target = new cN3OutboundTransfers();
            
            List<cOutboundFileDetails> actual = target.GetOutboundFileDetailsFromDB(cGlobalVariables.NHSTrustID);

            foreach (cOutboundFileDetails details in actual)
            {
                Assert.AreEqual("UnitTestOutbound.dat", actual[0].FileName);
            }
        }

        /// <summary>
        ///A test for MonitorOutboundFTPSites where the in progress flag is true
        ///</summary>
        [TestMethod()]
        public void MonitorOutboundFTPSitesWhereInProgressFlagTrueTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();

            cN3OutboundTransfers target = new cN3OutboundTransfers();
            cN3OutboundTransfers.OutboundCheckInProgress = true;
            target.MonitorOutboundFTPSites();

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID, FileData, NHSTrustID, FileName, Status FROM OutboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.OutboundDataID);

            string FileName = "";
            int TrustID = 1;
            int DataID = 0;
            OutboundStatus status = OutboundStatus.None;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                    TrustID = reader.GetInt32(2);
                    FileName = reader.GetString(3);
                    status = (OutboundStatus)reader.GetByte(4);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.IsTrue(DataID == 0);
            
        }

        /// <summary>
        ///A test for MonitorOutboundFTPSites where the in progress flag is false
        ///</summary>
        [TestMethod()]
        public void MonitorOutboundFTPSitesWhereInProgressFlagFalseTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();
            cN3OutboundTransfers target = new cN3OutboundTransfers();
            cN3OutboundTransfers.OutboundCheckInProgress = false;
            target.MonitorOutboundFTPSites();

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID, FileData, NHSTrustID, FileName, Status FROM OutboundFileData WHERE FileName = @FileName";
            smData.sqlexecute.Parameters.AddWithValue("@FileName", "GO_123_ASG_20100317_UnitTest_31812374.DAT");

            string FileName = "";
            int TrustID = 1;
            int DataID = 0;
            OutboundStatus status = OutboundStatus.None;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                    TrustID = reader.GetInt32(2);
                    FileName = reader.GetString(3);
                    status = (OutboundStatus)reader.GetByte(4);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.IsTrue(DataID > 0);
            cGlobalVariables.OutboundDataID = DataID;
            Assert.AreEqual(cGlobalVariables.NHSTrustID, TrustID);
            Assert.AreEqual("GO_123_ASG_20100317_UnitTest_31812374.DAT", FileName);
            Assert.AreEqual(OutboundStatus.AwaitingImport, status);
        }

        /// <summary>
        ///A test for SaveOutboundFile
        ///</summary>
        [TestMethod()]
        public void SaveOutboundFileTest()
        {
            byte[] data = cImportTemplateObject.CreateDummyESROutboundFileInfo();
            cN3OutboundTransfers target = new cN3OutboundTransfers();
            
            target.SaveOutboundFile(cGlobalVariables.NHSTrustID, data, "UnitTestOutbound.dat");

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID, FileData, NHSTrustID, FileName, Status FROM OutboundFileData WHERE FileName = @FileName";
            smData.sqlexecute.Parameters.AddWithValue("@FileName", "UnitTestOutbound.dat");

            string FileName = "";
            int TrustID = 1;
            int DataID = 0;
            OutboundStatus status = OutboundStatus.None;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                    TrustID = reader.GetInt32(2);
                    FileName = reader.GetString(3);
                    status = (OutboundStatus)reader.GetByte(4);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            cGlobalVariables.OutboundDataID = DataID;
            Assert.IsTrue(DataID > 0);
            Assert.AreEqual(cGlobalVariables.NHSTrustID, TrustID);
            Assert.AreEqual("UnitTestOutbound.dat", FileName);
            Assert.AreEqual(OutboundStatus.AwaitingImport, status);
        }
    }
}
