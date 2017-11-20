using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cImportHistoryTest and is intended
    ///to contain all cImportHistoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cImportHistoryTest
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
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cESRTrustObject.DeleteTrust();
            cImportHistoryObject.DeleteImportHistory();

            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion


        /// <summary>
        ///A test to save import history to the database
        ///</summary>
        [TestMethod()]
        public void AddImportHistoryTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            int logID = clsLogging.saveLog(0, LogType.ESROutboundImport, 4, 4);

            cImportHistory target = new cImportHistory(accountid);
            cImportHistoryItem historyItem = new cImportHistoryItem(0, template.TemplateID, logID, DateTime.Now, ImportHistoryStatus.Success, ApplicationType.ESROutboundImport, 0, DateTime.UtcNow, null);
            
            int ID = target.saveHistory(historyItem);
            
            Assert.IsTrue(ID > 0);

            cGlobalVariables.HistoryID = ID;
            cImportHistoryItem actual = target.getHistoryById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(historyItem.ImportID, actual.ImportID);
            Assert.AreEqual(historyItem.LogId, actual.LogId);
            Assert.AreEqual(historyItem.ImportStatus, actual.ImportStatus);
            Assert.AreEqual(historyItem.applicationType, actual.applicationType);
            Assert.AreEqual(historyItem.ImportedDate.ToString(), actual.ImportedDate.ToString());
            Assert.AreEqual(historyItem.ImportDataID, actual.ImportDataID);
        }

        /// <summary>
        ///A test to edit import history in the database
        ///</summary>
        [TestMethod()]
        public void EditImportHistoryTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cImportHistoryItem historyItem = cImportHistoryObject.CreateImportHistory();
          
            cImportHistory target = new cImportHistory(accountid);

            int ID = target.saveHistory(new cImportHistoryItem(historyItem.HistoryId, historyItem.ImportID, historyItem.LogId, historyItem.ImportedDate, ImportHistoryStatus.Success_With_Errors, ApplicationType.ExcelImport, historyItem.ImportDataID, historyItem.CreatedOn, historyItem.ModifiedOn));

            Assert.IsTrue(ID > 0);

            cImportHistoryItem actual = target.getHistoryById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(historyItem.ImportID, actual.ImportID);
            Assert.AreEqual(historyItem.LogId, actual.LogId);
            //Changed
            Assert.AreEqual(ImportHistoryStatus.Success_With_Errors, actual.ImportStatus);
            //Changed
            Assert.AreEqual(ApplicationType.ExcelImport, actual.applicationType);
            Assert.AreEqual(historyItem.ImportedDate.ToString(), actual.ImportedDate.ToString());
            Assert.AreEqual(historyItem.ImportDataID, actual.ImportDataID);
        }

        /// <summary>
        ///A test to save import history to the database as a delegate
        ///</summary>
        [TestMethod()]
        public void AddImportHistoryAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            int logID = clsLogging.saveLog(0, LogType.ESROutboundImport, 4, 4);

            cImportHistory target = new cImportHistory(accountid);
            cImportHistoryItem historyItem = new cImportHistoryItem(0, template.TemplateID, logID, DateTime.Now, ImportHistoryStatus.Success, ApplicationType.ESROutboundImport, 0, DateTime.UtcNow, null);

            int ID = target.saveHistory(historyItem);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.HistoryID = ID;
            cImportHistoryItem actual = target.getHistoryById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(historyItem.ImportID, actual.ImportID);
            Assert.AreEqual(historyItem.LogId, actual.LogId);
            Assert.AreEqual(historyItem.ImportStatus, actual.ImportStatus);
            Assert.AreEqual(historyItem.applicationType, actual.applicationType);
            Assert.AreEqual(historyItem.ImportedDate.ToString(), actual.ImportedDate.ToString());
            Assert.AreEqual(historyItem.ImportDataID, actual.ImportDataID);
        }

        /// <summary>
        ///A test to edit import history in the database as a delegate
        ///</summary>
        [TestMethod()]
        public void EditImportHistoryAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            cImportHistoryItem historyItem = cImportHistoryObject.CreateImportHistory();

            cImportHistory target = new cImportHistory(accountid);

            int ID = target.saveHistory(new cImportHistoryItem(historyItem.HistoryId, historyItem.ImportID, historyItem.LogId, historyItem.ImportedDate, ImportHistoryStatus.Success_With_Errors, ApplicationType.ExcelImport, historyItem.ImportDataID, historyItem.CreatedOn, historyItem.ModifiedOn));

            Assert.IsTrue(ID > 0);

            cImportHistoryItem actual = target.getHistoryById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(historyItem.ImportID, actual.ImportID);
            Assert.AreEqual(historyItem.LogId, actual.LogId);
            //Changed
            Assert.AreEqual(ImportHistoryStatus.Success_With_Errors, actual.ImportStatus);
            //Changed
            Assert.AreEqual(ApplicationType.ExcelImport, actual.applicationType);
            Assert.AreEqual(historyItem.ImportedDate.ToString(), actual.ImportedDate.ToString());
            Assert.AreEqual(historyItem.ImportDataID, actual.ImportDataID);
        }

        /// <summary>
        ///A test to get history by a valid Id
        ///</summary>
        [TestMethod()]
        public void GetHistoryByAValidIdTest()
        {
            cImportHistoryItem expected = cImportHistoryObject.CreateImportHistory();
            cImportHistory target = new cImportHistory(cGlobalVariables.AccountID);
            
            cImportHistoryItem actual = target.getHistoryById(cGlobalVariables.HistoryID);
            
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to delete the import history with a valid ID
        ///</summary>
        [TestMethod()]
        public void DeleteHistoryWithAValidIDTest()
        {
            cImportHistoryObject.CreateImportHistory();
            cImportHistory target = new cImportHistory(cGlobalVariables.AccountID);
            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportHistoryItem item = target.getHistoryById(cGlobalVariables.HistoryID);

            clsLogging.deleteLog(item.LogId);

            target.deleteHistory(cGlobalVariables.HistoryID);
            
            //System.Threading.Thread.Sleep(1000);
            //target = new cImportHistory(cGlobalVariables.AccountID);
            
            Assert.IsNull(target.getHistoryById(cGlobalVariables.HistoryID));
        }

        /// <summary>
        ///A test to delete the import history with an invalid ID
        ///</summary>
        [TestMethod()]
        public void DeleteHistoryWithAnInvalidIDTest()
        {
            cImportHistoryObject.CreateImportHistory();
            cImportHistory target = new cImportHistory(cGlobalVariables.AccountID);
            
            target.deleteHistory(0);
            
            Assert.IsNotNull(target.getHistoryById(cGlobalVariables.HistoryID));
        }

        /// <summary>
        ///A test to delete the import history with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteHistoryWithAValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;
            cImportHistoryObject.CreateImportHistory();
            cImportHistory target = new cImportHistory(cGlobalVariables.AccountID);

            cLogging clsLogging = new cLogging(cGlobalVariables.AccountID);
            cImportHistoryItem item = target.getHistoryById(cGlobalVariables.HistoryID);

            clsLogging.deleteLog(item.LogId);

            target.deleteHistory(cGlobalVariables.HistoryID);
            
            //System.Threading.Thread.Sleep(1000);
            //target = new cImportHistory(cGlobalVariables.AccountID);
            
            Assert.IsNull(target.getHistoryById(cGlobalVariables.HistoryID));
        }

        /// <summary>
        ///A test for CacheItems
        ///</summary>
        [TestMethod()]       
        public void CacheItemsTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("ihistory" + AccountID.ToString());

            cImportHistoryObject.CreateImportHistory();

            Dictionary<int, cImportHistoryItem> expected = (Dictionary<int, cImportHistoryItem>)Cache["ihistory" + AccountID.ToString()];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("ihistory" + AccountID.ToString());
        }

        /// <summary>
        ///A test for cImportHistory Constructor
        ///</summary>
        [TestMethod()]
        public void cImportHistoryConstructorTest()
        {
            cImportHistory target = new cImportHistory(cGlobalVariables.AccountID);
            Assert.IsNotNull(target); ;
        }
    }
}
