using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Data;
using SpendManagementUnitTests.Global_Objects;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cFinancialExportsTest and is intended
    ///to contain all cFinancialExportsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cFinancialExportsTest
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
            cFinancialExportObject.DeleteFinancialExport();

            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion

        /// <summary>
        ///A test for GetExportById by a valid ID
        ///</summary>
        [TestMethod()]
        public void GetExportByAValidIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cFinancialExport expected = cFinancialExportObject.CreateFinancialExport(); ;

            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = target.getExportById(cGlobalVariables.ExportID);

            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetExportById by an invalid ID
        ///</summary>
        [TestMethod()]
        public void GetExportByAnInvalidIdTest()
        {
            int accountid = cGlobalVariables.AccountID;

            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = target.getExportById(0);

            Assert.IsNull(actual);
        }


        /// <summary>
        ///A test for GetESRExport by a valid NHS Trust ID
        ///</summary>
        [TestMethod()]
        public void GetESRExportByAValidNHSTrustIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cFinancialExport expected = cFinancialExportObject.CreateFinancialExport(); ;

            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = target.getESRExport(cGlobalVariables.NHSTrustID);

            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetESRExport by an invalid NHS Trust ID
        ///</summary>
        [TestMethod()]
        public void GetESRExportByAnInvalidNHSTrustIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cFinancialExport expected = cFinancialExportObject.CreateFinancialExport(); ;

            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = target.getESRExport(0);

            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for deleteFinancialExport with a valid ID
        ///</summary>
        [TestMethod()]
        public void DeleteFinancialExportWithAValidIDTest()
        {
            cFinancialExportObject.CreateFinancialExport();
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
           
            target.deleteFinancialExport(cGlobalVariables.ExportID);
            //target = new cFinancialExports(accountid);
            //System.Threading.Thread.Sleep(1000);
            Assert.IsNull(target.getExportById(cGlobalVariables.ExportID));
        }

        /// <summary>
        ///A test for deleteFinancialExport with an invalid ID
        ///</summary>
        [TestMethod()]
        public void DeleteFinancialExportWithAnInvalidIDTest()
        {
            cFinancialExportObject.CreateFinancialExport();
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
           
            target.deleteFinancialExport(0);
            Assert.IsNotNull(target.getExportById(cGlobalVariables.ExportID));
        }

        /// <summary>
        ///A test for deleteFinancialExport with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteFinancialExportWithAValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cFinancialExportObject.CreateFinancialExport();
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
           
            target.deleteFinancialExport(cGlobalVariables.ExportID);
            //target = new cFinancialExports(accountid);
            //System.Threading.Thread.Sleep(1000);
            Assert.IsNull(target.getExportById(cGlobalVariables.ExportID));
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        [TestMethod()]
        public void CacheListTest()
        {
            int accountid = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("financialexports" + accountid);

            cFinancialExportObject.CreateFinancialExport(); ;

            //The method is called in the blow constructor
            cFinancialExports target = new cFinancialExports(accountid);
            SortedList<int, cFinancialExport> expected = (SortedList<int, cFinancialExport>)Cache["financialexports" + accountid];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("financialexports" + accountid);
        }

        /// <summary>
        ///A test for addFinancialExport to add a financial export to the database where all properties have a value set
        ///</summary>
        [TestMethod()]
        public void AddFinancialExportTest()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport export = new cFinancialExport(0, cGlobalVariables.AccountID, FinancialApplication.ESR, new Guid("4a960142-c5b8-40fe-8eca-b2f7101f329c"), false, cGlobalVariables.EmployeeID, DateTime.UtcNow, 0, DateTime.Now, 4, cGlobalVariables.NHSTrustID);
            cFinancialExport actual = null;
            
            int ID = target.addFinancialExport(export);
            cGlobalVariables.ExportID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.getExportById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(export.application, actual.application);
            Assert.AreEqual(export.reportid, actual.reportid);
            Assert.AreEqual(export.automated, actual.automated);
            Assert.AreEqual(1, actual.curexportnum);
            Assert.AreEqual(0, actual.exporttype);
            Assert.AreEqual(export.NHSTrustID, actual.NHSTrustID);

        }

        /// <summary>
        ///A test for updateFinancialExport to update a financial export in the database where all properties have a value set
        ///</summary>
        [TestMethod()]
        public void UpdateFinancialExportTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cFinancialExport export = cFinancialExportObject.CreateFinancialExport();
            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = null;

            target.updateFinancialExport(new cFinancialExport(export.financialexportid, cGlobalVariables.AccountID, FinancialApplication.CustomReport, export.reportid, export.automated, export.createdby, export.createdon, export.curexportnum, export.lastexportdate, 3, 0), cGlobalVariables.EmployeeID);
            //target = new cFinancialExports(accountid);
            //System.Threading.Thread.Sleep(1000);
            actual = target.getExportById(cGlobalVariables.ExportID);

            Assert.IsNotNull(actual);

            //Changed
            Assert.AreEqual(FinancialApplication.CustomReport, actual.application);
            Assert.AreEqual(export.reportid, actual.reportid);
            Assert.AreEqual(export.automated, actual.automated);
            Assert.AreEqual(export.curexportnum, actual.curexportnum);
            //Changed
            Assert.AreEqual(3, actual.exporttype);
            //Changed
            Assert.AreEqual(0, actual.NHSTrustID);

        }

        /// <summary>
        ///A test for addFinancialExport to add a financial export to the database as a delegate
        ///</summary>
        [TestMethod()]
        public void AddFinancialExportAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cESRTrustObject.CreateESRTrustGlobalVariable();
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport export = new cFinancialExport(0, cGlobalVariables.AccountID, FinancialApplication.ESR, new Guid("4a960142-c5b8-40fe-8eca-b2f7101f329c"), false, cGlobalVariables.EmployeeID, DateTime.UtcNow, 0, DateTime.Now, 4, cGlobalVariables.NHSTrustID);
            cFinancialExport actual = null;

            int ID = target.addFinancialExport(export);
            cGlobalVariables.ExportID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.getExportById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(export.application, actual.application);
            Assert.AreEqual(export.reportid, actual.reportid);
            Assert.AreEqual(export.automated, actual.automated);
            Assert.AreEqual(1, actual.curexportnum);
            Assert.AreEqual(0, actual.exporttype);
            Assert.AreEqual(export.NHSTrustID, actual.NHSTrustID);

        }

        /// <summary>
        ///A test for updateFinancialExport to update a financial export in the database as a delegate
        ///</summary>
        [TestMethod()]
        public void UpdateFinancialExportAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            cFinancialExport export = cFinancialExportObject.CreateFinancialExport();
            cFinancialExports target = new cFinancialExports(accountid);
            cFinancialExport actual = null;

            target.updateFinancialExport(new cFinancialExport(export.financialexportid, cGlobalVariables.AccountID, FinancialApplication.CustomReport, export.reportid, export.automated, export.createdby, export.createdon, export.curexportnum, export.lastexportdate, 3, 0), cGlobalVariables.EmployeeID);
            //target = new cFinancialExports(accountid);
            //System.Threading.Thread.Sleep(1000);
            actual = target.getExportById(cGlobalVariables.ExportID);

            Assert.IsNotNull(actual);

            //Changed
            Assert.AreEqual(FinancialApplication.CustomReport, actual.application);
            Assert.AreEqual(export.reportid, actual.reportid);
            Assert.AreEqual(export.automated, actual.automated);
            Assert.AreEqual(export.curexportnum, actual.curexportnum);
            //Changed
            Assert.AreEqual(3, actual.exporttype);
            //Changed
            Assert.AreEqual(0, actual.NHSTrustID);

        }

        /// <summary>
        ///A test for cFinancialExports Constructor
        ///</summary>
        [TestMethod()]
        public void cFinancialExportsConstructorTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cFinancialExports target = new cFinancialExports(accountid);
            Assert.IsNotNull(target);
        }
    }
}
