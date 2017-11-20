using ESR_File_Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Configuration;
using System.Data.SqlClient;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cFTPClientTest and is intended
    ///to contain all cFTPClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cFTPClientTest
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
            cFTPClient clsClient = new cFTPClient();
            cSecureData clsSecure = new cSecureData();
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            cESRTrust trust = clsTrusts.GetESRTrustByID(cGlobalVariables.NHSTrustID);

            if (trust != null)
            {
                clsClient.DeleteFileFromFTP(trust.FTPAddress, trust.FTPUsername, clsSecure.Decrypt(trust.FTPPassword), "UnitTest.dat");
                cESRTrustObject.DeleteServiceTrust();
            }
            // changed to using an encrypted password in the web/app/exe config
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Stop(sConnectionString.ToString());
        }
        
        #endregion


        /// <summary>
        ///A test for GetFTPDirectoryFileList
        ///</summary>
        [TestMethod()]
        public void GetFTPDirectoryFileListTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();

            cFTPClient target = new cFTPClient();
            cSecureData clsSecure = new cSecureData();
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            cESRTrust trust = clsTrusts.GetESRTrustByID(cGlobalVariables.NHSTrustID);
            List<string> actual = target.GetFTPDirectoryFileList(trust.FTPAddress, trust.FTPUsername, clsSecure.Decrypt(trust.FTPPassword));

            foreach (string str in actual)
            {
                if (str == "GO_123_ASG_20100317_UnitTest_31812374.DAT")
                {
                    Assert.AreEqual("GO_123_ASG_20100317_UnitTest_31812374.DAT", str);
                    break;
                }
            }
        }

        /// <summary>
        ///A test for DownloadOutboundFile
        ///</summary>
        [TestMethod()]
        public void DownloadOutboundFileTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();

            cFTPClient target = new cFTPClient();
            cSecureData clsSecure = new cSecureData();
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            
            cESRTrust trust = clsTrusts.GetESRTrustByID(cGlobalVariables.NHSTrustID);
           
            byte[] actual = target.DownloadOutboundFile(trust, "GO_123_ASG_20100317_UnitTest_31812374.DAT", clsSecure.Decrypt(trust.FTPPassword));
            Assert.IsTrue(actual.Length > 0);
        }

        /// <summary>
        ///A test for UploadInboundFile
        ///</summary>
        [TestMethod()]
        public void UploadInboundFileTest()
        {
            cESRTrustObject.CreateServiceESRTrustGlobalVariable();

            cFTPClient target = new cFTPClient();

            byte[] fileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            cESRTrust trust = clsTrusts.GetESRTrustByID(cGlobalVariables.NHSTrustID);

            FinancialExportStatus actual = target.UploadInboundFile(trust, "UnitTest.dat", fileData);
            Assert.AreEqual(FinancialExportStatus.UploadToESRSucceeded, actual);
        }
    }
}
