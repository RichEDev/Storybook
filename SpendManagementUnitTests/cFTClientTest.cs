using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using System.Net;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cFTClientTest and is intended
    ///to contain all cFTClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cFTClientTest
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        public void UploadFileTest_Success()
        {
            cFTPClient target = cFTPClientObject.getFTPClientObject();

            string filename = "UploadTest.txt"; 
            byte[] fileData = null; 
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            fileData = encoding.GetBytes("This is a test file created by the UploadFileTest() method in the cFTPClientTest Unit tests\n\n");
            FTP_Status expected = FTP_Status.Success;
            FTP_Status actual;
            actual = target.UploadFile(filename, fileData);
            Assert.AreEqual(expected, actual);

            // clear up after upload
            target.DeleteFileFromFTP(filename);
        }

        /// <summary>
        ///A test for UploadFile
        ///</summary>
        [TestMethod()]
        public void UploadFileTest_Fail()
        {
            cFTPClient target = cFTPClientObject.getInvalidFTPClientObject();

            string filename = "UploadTest.txt";
            byte[] fileData = null;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            fileData = encoding.GetBytes("This is a test file created by the UploadFileTest() method in the cFTPClientTest Unit tests\n\n");
            FTP_Status expected = FTP_Status.Fail;
            FTP_Status actual;
            actual = target.UploadFile(filename, fileData);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteFileFromFTP
        ///</summary>
        [TestMethod()]
        public void DeleteFileFromFTPTest()
        {
            cFTPClient target = cFTPClientObject.getFTPClientObject();

            string filename = "DeleteTest.txt";
            byte[] fileData = null;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            fileData = encoding.GetBytes("This is a test file created by the UploadFileTest() method in the cFTPClientTest Unit tests\n\n");
            
            FTP_Status actual = target.UploadFile(filename, fileData);
            Assert.IsTrue(actual == FTP_Status.Success);
            
            FTP_Status delStatus = target.DeleteFileFromFTP(filename);
            Assert.IsTrue(delStatus == FTP_Status.Success);
        }

        /// <summary>
        ///A test for cFTClient Constructor
        ///</summary>
        [TestMethod()]
        public void cFTClientConstructorTest()
        {
            cFTPClient target = cFTPClientObject.getFTPClientObject();
            Assert.IsNotNull(target);
            Assert.IsTrue(target.FTP_KeepAlive);
            Assert.IsTrue(target.FTP_Password == "123");
            Assert.IsTrue(target.FTP_Root == "");
            Assert.IsTrue(target.FTP_UseBinary);
            Assert.IsTrue(target.FTP_UsePassive);
            Assert.IsFalse(target.FTP_UseSSL);
            Assert.IsTrue(target.FTP_Username == "anonymous");
        }

        /// <summary>
        ///A test for DownloadFile
        ///</summary>
        [TestMethod()]
        public void DownloadFileTest()
        {
            cFTPClient target = cFTPClientObject.getFTPClientObject();

            string filename = "FTP_UnitTest.txt";
            byte[] actual = null; 
            actual = target.DownloadFile(filename);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Length > 0);
        }

        /// <summary>
        ///A test for GetFTPDirectoryFileList success
        ///</summary>
        [TestMethod()]
        public void GetFTPDirectoryFileListTest_Success()
        {
            cFTPClient target = cFTPClientObject.getFTPClientObject();
            List<string> actual;
            actual = target.GetFTPDirectoryFileList();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetFTPDirectoryFileList fail
        ///</summary>
        [TestMethod()]
        public void GetFTPDirectoryFileListTest_Fail()
        {
            cFTPClient target = cFTPClientObject.getInvalidFTPClientObject();
            List<string> actual;
            actual = target.GetFTPDirectoryFileList();
            Assert.IsNull(actual);
        }
    }
}
