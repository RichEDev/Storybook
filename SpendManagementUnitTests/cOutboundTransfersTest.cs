using SELFileTransferService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cOutboundTransfersTest and is intended
    ///to contain all cOutboundTransfersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cOutboundTransfersTest
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
            cAutomatedTransferObject.DeleteOutboundFileFromDatabase();
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Stop(sConnectionString.ToString());
        }
        
        #endregion


        /// <summary>
        ///A test for cOutboundTransfers Constructor
        ///</summary>
        [TestMethod()]
        public void cOutboundTransfersConstructorTest()
        {
            cOutboundTransfers target = new cOutboundTransfers();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for UpdateOutboundFileStatus
        ///</summary>
        [TestMethod()]
        public void UpdateOutboundFileStatusTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();
            cAutomatedTransferObject.CreateOutboundFileRecord();
            cOutboundTransfers target = new cOutboundTransfers();

            OutboundStatus status = OutboundStatus.Complete;
            target.UpdateOutboundFileStatus(cGlobalVariables.OutboundDataID, status);

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT Status FROM OutboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.OutboundDataID);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    status = (OutboundStatus)reader.GetByte(0);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.AreEqual(OutboundStatus.Complete, status);
        }

        /// <summary>
        ///A test for processing Outbound files into expenses where the NHS Trust is not valid
        ///</summary>
        [TestMethod()]
        public void ProcessOutboundFilesWhereAnNHSTrustDoesNotExistOrIsNotValidTest()
        {
            cAutomatedTransferObject.CreateOutboundFileRecord();
            cOutboundTransfers target = new cOutboundTransfers();

            OutboundStatus status = OutboundStatus.Complete;
            target.processOutboundFiles();

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT Status FROM OutboundFileData WHERE DataID = @DataID";
            smData.sqlexecute.Parameters.AddWithValue("@DataID", cGlobalVariables.OutboundDataID);

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    status = (OutboundStatus)reader.GetByte(0);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            Assert.AreEqual(OutboundStatus.AwaitingImport, status);
        }

        /// <summary>
        ///A test for getOutboundFiles where 0 is passed into the method to get all files
        ///</summary>
        [TestMethod()]
        public void GetOutboundFilesPassingInZeroTest()
        {
            cAutomatedTransferObject.CreateOutboundFileRecord();
            cOutboundTransfers target = new cOutboundTransfers();
            
            List<cESRFileInfo> actual = target.getOutboundFiles(0);
            Assert.IsTrue(actual.Count > 0);
            Assert.AreEqual("UnitTestOutbound.dat", actual[0].fileName);
            Assert.AreEqual(cGlobalVariables.NHSTrustID, actual[0].TrustID);
        }

        /// <summary>
        ///A test for getOutboundFiles where a valid Data ID is passed into the method to get the specific file
        ///</summary>
        [TestMethod()]
        public void GetOutboundFilesPassingInValidIDTest()
        {
            cAutomatedTransferObject.CreateOutboundFileRecord();
            cOutboundTransfers target = new cOutboundTransfers();

            List<cESRFileInfo> actual = target.getOutboundFiles(cGlobalVariables.OutboundDataID);
            Assert.IsTrue(actual.Count > 0);
            Assert.AreEqual("UnitTestOutbound.dat", actual[0].fileName);
            Assert.AreEqual(cGlobalVariables.NHSTrustID, actual[0].TrustID);
        }
    }
}
