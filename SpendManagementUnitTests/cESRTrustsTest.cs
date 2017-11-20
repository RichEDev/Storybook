using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System;
using SpendManagementUnitTests.Global_Objects;
using System.Web.Caching;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cESRTrustsTest and is intended
    ///to contain all cESRTrustsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cESRTrustsTest
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
        public void ESRTrustTestCleanup()
        {
            cESRTrustObject.DeleteTrust();

            //Set the delegate to null for the current user
            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion

        /// <summary>
        ///Add a trust with a name that currently doesn't exist
        ///Make sure all properties have a value set
        ///Check the values of the save match the returned value from the database.
        ///Check an ID is returned
        ///</summary>
        [TestMethod()]
        public void AddTrustWithoutDuplicateExistingAndAllPropValuesSetTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = new cESRTrust(0, "Unit Test Trust" + DateTime.Now.Ticks.ToString(), "VPD", "M", "N", 1, "ftp", "UnitTestAccount", "UnitTestPassword", false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now), ",");
            cESRTrust actual = null;
            int ID = target.SaveTrust(trust);
            cGlobalVariables.NHSTrustID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual(trust.FTPAddress, actual.FTPAddress);
            Assert.AreEqual(clsSecure.Encrypt(trust.FTPPassword), actual.FTPPassword);
            Assert.AreEqual(trust.FTPUsername, actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trust.TrustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        /// This adds a new trust to the database with any properties that can have no value or be null set 
        /// </summary>
        [TestMethod()]
        public void AddTrustWithoutDuplicateExistingAndPropValuesSetToNullOrNothingTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = new cESRTrust(0, "Unit Test Trust" + DateTime.Now.Ticks.ToString(), "VPD", "M", "N", 1, "", "", "", false, null, null, ",");
            cESRTrust actual = null;
            int ID = target.SaveTrust(trust);
            cGlobalVariables.NHSTrustID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual(trust.FTPAddress, actual.FTPAddress);
            Assert.AreEqual(trust.FTPPassword, actual.FTPPassword);
            Assert.AreEqual(trust.FTPUsername, actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trust.TrustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        /// Edit a trust with a name that currently doesn't exist
        /// Make sure all properties have a value set
        /// Check the values of the save match the returned value from the database.
        /// Check an ID is returned
        /// </summary>
        [TestMethod()]
        public void EditTrustWithoutDuplicateExistingAndAllPropValuesSetTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            string trustName = "Edited Trust" + DateTime.Now.Ticks.ToString();

            int ID = target.SaveTrust(new cESRTrust(trust.TrustID, trustName, trust.TrustVPD, trust.PeriodType, trust.PeriodRun, trust.RunSequenceNumber, trust.FTPAddress, trust.FTPUsername, "UnitTestPassword", trust.Archived, trust.CreatedOn, trust.ModifiedOn, trust.DelimiterCharacter));

            Assert.IsTrue(ID > 0);

            cESRTrust actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual(trust.FTPAddress, actual.FTPAddress);
            Assert.AreEqual(trust.FTPPassword, actual.FTPPassword);
            Assert.AreEqual(trust.FTPUsername, actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        /// A trust is edited in the database with any properties that can have no value or be null set 
        /// </summary>
        [TestMethod()]
        public void EditTrustWithoutDuplicateExistingAndValuesSetToNullOrNothingTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            string trustName = "Edited Trust" + DateTime.Now.Ticks.ToString();

            int ID = target.SaveTrust(new cESRTrust(trust.TrustID, trustName, trust.TrustVPD, trust.PeriodType, trust.PeriodRun, trust.RunSequenceNumber, "", "", "", trust.Archived, null, null, trust.DelimiterCharacter));

            Assert.IsTrue(ID > 0);

            cESRTrust actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual("", actual.FTPAddress);
            Assert.AreEqual("", actual.FTPPassword);
            Assert.AreEqual("", actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        /// Try and save a trust with the same name as one already existing
        /// Check -1 is returned
        /// </summary>
        [TestMethod()]
        public void SaveTrustWithDuplicateExistingTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrust trust = new cESRTrust(0, "TestTrust" + DateTime.Now.Ticks.ToString(), "VPD", "M", "N", 1, "", "", "", false, null, null, ",");
            int ID = target.SaveTrust(trust);

            Assert.IsTrue(ID == -1);
        }

        /// <summary>
        /// Add a trust as a delegate
        /// </summary>
        [TestMethod()]
        public void AddTrustAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = new cESRTrust(0, "Unit Test Trust" + DateTime.Now.Ticks.ToString(), "VPD", "M", "N", 1, "ftp", "UnitTestAccount", "UnitTestPassword", false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now), ",");
            cESRTrust actual = null;
            
            int ID = target.SaveTrust(trust);
            cGlobalVariables.NHSTrustID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual(trust.FTPAddress, actual.FTPAddress);
            Assert.AreEqual(clsSecure.Encrypt(trust.FTPPassword), actual.FTPPassword);
            Assert.AreEqual(trust.FTPUsername, actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trust.TrustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        /// Edit a Trust as a delegate
        /// </summary>
        [TestMethod()]
        public void EditTrustAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            cSecureData clsSecure = new cSecureData();
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            string trustName = "Edited Trust" + DateTime.Now.Ticks.ToString();

            int ID = target.SaveTrust(new cESRTrust(trust.TrustID, trustName, trust.TrustVPD, trust.PeriodType, trust.PeriodRun, trust.RunSequenceNumber, trust.FTPAddress, trust.FTPUsername, "UnitTestPassword", trust.Archived, trust.CreatedOn, trust.ModifiedOn, trust.DelimiterCharacter));

            Assert.IsTrue(ID > 0);

            cESRTrust actual = target.GetESRTrustByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(trust.AccountID, actual.AccountID);
            Assert.AreEqual(trust.Archived, actual.Archived);
            Assert.AreEqual(trust.DelimiterCharacter, actual.DelimiterCharacter);
            Assert.AreEqual(trust.FTPAddress, actual.FTPAddress);
            Assert.AreEqual(trust.FTPPassword, actual.FTPPassword);
            Assert.AreEqual(trust.FTPUsername, actual.FTPUsername);
            Assert.AreEqual(trust.PeriodRun, actual.PeriodRun);
            Assert.AreEqual(trust.PeriodType, actual.PeriodType);
            Assert.AreEqual(trust.RunSequenceNumber, actual.RunSequenceNumber);
            Assert.AreEqual(trustName, actual.TrustName);
            Assert.AreEqual(trust.TrustVPD, actual.TrustVPD);
        }

        /// <summary>
        ///A test for ResetCache
        ///</summary>
        [TestMethod()]
        public void ResetCacheTest()
        {
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            int accountID = cGlobalVariables.AccountID;
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);
            target.ResetCache();
            SortedList<int, cESRTrust> expected = (SortedList<int, cESRTrust>)Cache["ESRTrustRecords" + accountID];
            Assert.IsTrue(expected.Count > 0);
        }

        /// <summary>
        ///A test for IncrementRunSequenceNumber
        ///</summary>
        [TestMethod()]
        public void IncrementRunSequenceNumberTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);
            target.IncrementRunSequenceNumber(trust.TrustID, 2);
            trust = target.GetESRTrustByID(trust.TrustID);

            Assert.AreEqual(2, trust.RunSequenceNumber);
        }

        /// <summary>
        ///A test to get the ESR Trust by a valid VPD
        ///</summary>
        [TestMethod()]
        public void GetESRTrustByVPDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            string trustVPD = "123";
            cESRTrust expected = cESRTrustObject.CreateESRTrustGlobalVariable();

            cESRTrusts target = new cESRTrusts(accountID);
            cESRTrust actual = target.GetESRTrustByVPD(trustVPD);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to get the ESR Trust by an invalid VPD
        ///</summary>
        [TestMethod()]
        public void GetESRTrustByAnInvalidVPDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrust expected = cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);

            string trustVPD = "456";
            
            cESRTrust actual = target.GetESRTrustByVPD(trustVPD);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test to get the ESR Trust record by a valid ID 
        ///</summary>
        [TestMethod()]
        public void GetESRTrustByIDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrust expected = cESRTrustObject.CreateESRTrustGlobalVariable();

            cESRTrusts target = new cESRTrusts(accountID);

            cESRTrust actual = target.GetESRTrustByID(expected.TrustID);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to get the ESR Trust record by an invalid ID 
        ///</summary>
        [TestMethod()]
        public void GetESRTrustByAnInvalidIDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);

            cESRTrust actual = target.GetESRTrustByID(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for deleting a Trust with a valid trust ID
        ///</summary>
        [TestMethod()]
        public void DeleteTrustWithAValidIDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();

            cESRTrusts target = new cESRTrusts(accountID);
            target.DeleteTrust(cGlobalVariables.NHSTrustID);

            Assert.IsNull(target.GetESRTrustByID(cGlobalVariables.NHSTrustID));
        }

        /// <summary>
        ///A test for deleting a Trust with an invalid trust ID i.e 0
        ///</summary>
        [TestMethod()]
        public void DeleteTrustWithAnInvalidIDTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();
            
            cESRTrusts target = new cESRTrusts(accountID);
            target.DeleteTrust(0);

            Assert.IsNotNull(target.GetESRTrustByID(cGlobalVariables.NHSTrustID));
        }

        /// <summary>
        ///A test for deleting a Trust with a valid trust ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteTrustWithAValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();

            cESRTrusts target = new cESRTrusts(accountID);
            target.DeleteTrust(cGlobalVariables.NHSTrustID);

            Assert.IsNull(target.GetESRTrustByID(cGlobalVariables.NHSTrustID));
        }

        /// <summary>
        ///A test for CreateESRInboundFilename
        ///</summary>
        [TestMethod()]
        public void CreateESRInboundFilenameTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);

            DateTime taxdate = DateTime.Today;
            int taxyear;
            int taxperiod;

            if (taxdate < new DateTime(DateTime.Today.Year, 04, 06))
            {
                taxperiod = DateTime.Today.Month + 8;
                taxyear = int.Parse(DateTime.Today.ToString("yy"));
            }
            else
            {
                taxperiod = DateTime.Today.Month - 3;
                taxyear = int.Parse(DateTime.Today.AddYears(1).Year.ToString().Substring(2, 2));
            }

            string expected = "TA_" + trust.TrustVPD + "_SEL_" + trust.PeriodType + trust.PeriodRun + taxyear.ToString("00") + taxperiod.ToString("00") + "_00000002.DAT";
            string actual = target.CreateESRInboundFilename(trust.TrustID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Cache any trusts stored in the database where all the trust property values are set
        /// </summary>
        [TestMethod()]
        public void CacheListTestWithAllPropValuesSet()
        {
            int accountID = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("ESRTrustRecords" + accountID);
            
            cESRTrustObject.CreateESRTrustGlobalVariable();

            //The method is called in the blow constructor
            cESRTrusts target = new cESRTrusts(accountID);
            SortedList<int, cESRTrust> expected = (SortedList<int, cESRTrust>)Cache["ESRTrustRecords" + accountID];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("ESRTrustRecords" + accountID);
        }

        /// <summary>
        /// An object with its values that can be set to null or nothing are set, this is then added to the database and 
        /// then extracted via the cache list method to test the values returned hold up
        /// </summary>
        [TestMethod()]
        public void CacheListTestWithPropValuesSetToNullOrNothing()
        {
            int accountID = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("ESRTrustRecords" + accountID);

            cESRTrustObject.CreateESRTrustGlobalVariableWithValuesThatCanBeSetToNullOrNothing();

            //The method is called in the below constructor
            cESRTrusts target = new cESRTrusts(accountID);
            SortedList<int, cESRTrust> expected = (SortedList<int, cESRTrust>)Cache["ESRTrustRecords" + accountID];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("ESRTrustRecords" + accountID);
        }

        [TestMethod()]
        public void CacheListWithNoTrustsStoredInTheDatabaseTest()
        {
            int accountID = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("ESRTrustRecords" + accountID);

            //The method is called in the blow constructor
            cESRTrusts target = new cESRTrusts(accountID);
            SortedList<int, cESRTrust> expected = (SortedList<int, cESRTrust>)Cache["ESRTrustRecords" + accountID];
            Assert.IsNull(expected);
        }

        /// <summary>
        ///A test to archive the Trust to true and false
        ///</summary>
        [TestMethod()]
        public void ArchiveTrustTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrust trust = cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);

            bool expected = false;
            bool actual;
            actual = target.GetESRTrustByID(trust.TrustID).Archived;

            expected = true;
            actual = target.ArchiveTrust(trust.TrustID);
            Assert.AreEqual(expected, actual);
            actual = target.GetESRTrustByID(trust.TrustID).Archived;
            Assert.AreEqual(expected, actual);

            expected = true;
            actual = target.ArchiveTrust(trust.TrustID);
            Assert.AreEqual(expected, actual);
            expected = false;
            actual = target.GetESRTrustByID(trust.TrustID).Archived;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateDropDownList
        ///</summary>
        [TestMethod()]
        public void CreateDropDownListTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cESRTrusts target = new cESRTrusts(accountID);
            DropDownList ddl = new DropDownList();

            target.CreateDropDownList(ref ddl);
            Assert.IsTrue(ddl.Items.Count > 0);
        }

        /// <summary>
        ///A test for cESRTrusts Constructor
        ///</summary>
        [TestMethod()]
        public void cESRTrustsConstructorTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRTrusts target = new cESRTrusts(accountID);
            Assert.IsNotNull(target);
        }
    }
}
