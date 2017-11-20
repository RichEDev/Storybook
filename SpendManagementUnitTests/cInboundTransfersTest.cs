using SELFileTransferService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;
using System.Configuration;
using System.Data.SqlClient;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInboundTransfersTest and is intended
    ///to contain all cInboundTransfersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInboundTransfersTest
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
            cAutomatedTransferObject.DeleteInboundFileFromDatabase();
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Stop(sConnectionString.ToString());
        }
        
        #endregion


        /// <summary>
        ///A test for UpdateInboundFileStatus
        ///</summary>
        [TestMethod()]
        public void UpdateInboundFileStatusTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();
            cInboundTransfers target = new cInboundTransfers();
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
        ///A test for SaveInboundFile
        ///</summary>
        [TestMethod()]
        public void SaveInboundFileTest()
        {
            cAutomatedTransferObject.CreateInboundFileRecord();
            byte[] FileData = new byte[0];
            FinancialExportStatus status = FinancialExportStatus.CollectedForUploadToESR;

            #region Get the information from the database

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT DataID, FileData, FileName, NHSTrustID, FinancialExportID, Status, ExportHistoryID FROM InboundFileData WHERE FileName = @FileName";
            smData.sqlexecute.Parameters.AddWithValue("@FileName", "UnitTest.dat");

            string FileName = "";
            int TrustID = 1;
            int DataID = 0;
            int FinancialExportID = 1;
            int ExportHistoryID = 1;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    DataID = reader.GetInt32(0);
                    TrustID = reader.GetInt32(3);

                    if (reader.IsDBNull(1) == true)
                    {
                        FileData = new byte[0];
                    }
                    else
                    {
                        FileData = (byte[])reader.GetSqlBinary(1);
                    }

                    FileName = reader.GetString(2);

                    FinancialExportID = reader.GetInt32(4);

                    status = (FinancialExportStatus)reader.GetByte(5);

                    ExportHistoryID = reader.GetInt32(6);
                }

                reader.Close();
            }
            smData.sqlexecute.Parameters.Clear();

            #endregion

            cGlobalVariables.InboundDataID = DataID;
            Assert.IsTrue(DataID >0);
            Assert.AreEqual(cGlobalVariables.NHSTrustID, TrustID);
            Assert.AreEqual(FinancialExportStatus.CollectedForUploadToESR, status);
            Assert.AreEqual("UnitTest.dat", FileName);
            Assert.AreEqual(0, FinancialExportID);
            Assert.AreEqual(0, ExportHistoryID);

        } 
    }
}
