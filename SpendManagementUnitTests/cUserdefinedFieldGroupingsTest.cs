using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cUserdefinedFieldGroupingsTest and is intended
    ///to contain all cUserdefinedFieldGroupingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cUserdefinedFieldGroupingsTest
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
        ///A test for cUserdefinedFieldGroupings Constructor
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_cUserdefinedFieldGroupingsConstructor()
        {
            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_CreateDropDown()
        {
            cUserdefinedFieldGrouping oGrouping = cUserDefinedFieldGroupingObject.CreateObject();
            cTable table = cUserDefinedFieldGroupingObject.GetUDFGroupingTable();

            int accountid = cGlobalVariables.AccountID; 
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            try
            {
                List<ListItem> actual;
                actual = target.CreateDropDown(table.TableID);

                ListItem expected = new ListItem(oGrouping.GroupName, oGrouping.UserdefinedGroupID.ToString());

                Assert.IsTrue(actual.Count > 0);
                Assert.IsTrue(actual.Contains(expected));
            }
            finally
            {
                target.DeleteGrouping(oGrouping.UserdefinedGroupID, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for deleteGrouping
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_deleteGrouping_validGroup()
        {
            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            try
            {
                int userdefinedgroupid = cUserDefinedFieldGroupingObject.CreateID();
                int employeeid = cGlobalVariables.EmployeeID;
                target.DeleteGrouping(userdefinedgroupid, employeeid);

                cUserdefinedFieldGrouping actual = null;
                actual = target.GetGroupingByID(userdefinedgroupid);

                Assert.IsNull(actual);
            }
            finally { }
        }

        /// <summary>
        ///A test for getGroupingByAssocTable
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_getGroupingByAssocTable_validTableWithGroups()
        {
            cUserdefinedFieldGrouping oGroup = cUserDefinedFieldGroupingObject.CreateObject();
            Guid tableid = cUserDefinedFieldGroupingObject.GetUDFGroupingTable().TableID;

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            try
            {
                Dictionary<int, cUserdefinedFieldGrouping> actual;
                actual = target.GetGroupingByAssocTable(tableid);

                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.ContainsKey(oGroup.UserdefinedGroupID));
                cCompareAssert.AreEqual(oGroup, actual[oGroup.UserdefinedGroupID]);
            }
            finally
            {
                target.DeleteGrouping(oGroup.UserdefinedGroupID, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for getGroupingByAssocTable
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_getGroupingByAssocTable_invalidTable()
        {
            Dictionary<int, cUserdefinedFieldGrouping> expected = new Dictionary<int, cUserdefinedFieldGrouping>();

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            try
            {
                Dictionary<int, cUserdefinedFieldGrouping> actual;
                actual = target.GetGroupingByAssocTable(new Guid());

                Assert.IsTrue(actual.Count == 0);
                cCompareAssert.AreEqual(expected, actual);
            }
            catch
            {
            }
        }

        /// <summary>
        ///A test for getGroupingByID
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_getGroupingByID_validID()
        {
            cUserdefinedFieldGrouping expected = cUserDefinedFieldGroupingObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            try
            {
                int id = expected.UserdefinedGroupID;

                cUserdefinedFieldGrouping actual;
                actual = target.GetGroupingByID(id);

                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                target.DeleteGrouping(expected.UserdefinedGroupID, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for getGroupingByID
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_getGroupingByID_invalidID()
        {
            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            int id = -36;

            cUserdefinedFieldGrouping actual;
            actual = target.GetGroupingByID(id);

            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for saveGrouping
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_saveGrouping_valid()
        {
            cUserdefinedFieldGrouping grouping = cUserDefinedFieldGroupingObject.ValidTemplate();

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            int actual = 0;

            try
            {
                actual = target.SaveGrouping(grouping);

                Assert.IsTrue(actual > 0);
            }
            finally
            {
                target.DeleteGrouping(actual, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for saveGrouping
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_saveGrouping_validUpdating()
        {
            cUserdefinedFieldGrouping grouping = cUserDefinedFieldGroupingObject.ValidTemplate();

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            int actual = 0;
            try
            {
                actual = target.SaveGrouping(grouping);
                Assert.IsTrue(actual > 0);

                cUserdefinedFieldGrouping grouping2 = target.GetGroupingByID(actual);
                int actual2 = target.SaveGrouping(grouping2);

                grouping = target.GetGroupingByID(actual);

                Assert.AreEqual(actual, actual2);

                Assert.IsNull(grouping2.ModifiedOn);
                Assert.IsNull(grouping2.ModifiedBy);
                Assert.IsTrue(grouping.ModifiedOn > grouping2.CreatedOn);
                Assert.AreEqual(grouping.ModifiedBy, grouping2.CreatedBy);

                cCompareAssert.AreEqual(grouping2, grouping, new List<string> { "ModifiedBy", "ModifiedOn" });
            }
            finally
            {
                target.DeleteGrouping(actual, cGlobalVariables.EmployeeID);
            }
        }

        /// <summary>
        ///A test for saveGrouping
        ///</summary>
        [TestMethod()]
        public void cUserdefinedFieldGrouping_saveGrouping_invalidNoTable()
        {
            cUserdefinedFieldGrouping grouping = cUserDefinedFieldGroupingObject.InvalidTemplate();

            int accountid = cGlobalVariables.AccountID;
            cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid);

            int actual = 0;

            try
            {
                actual = target.SaveGrouping(grouping);

                Assert.IsTrue(actual == -1);
            }
            finally
            {
                if (actual > 0)
                {
                    target.DeleteGrouping(actual, cGlobalVariables.EmployeeID);
                }
            }
        }

        ///// <summary>
        /////A test for cacheKey
        /////</summary>
        //[TestMethod()]
        //public void cUserdefinedFieldGrouping_cacheKey_default()
        //{
        //    cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(cGlobalVariables.AccountID);

        //    string expected = "userdefinedfieldgroupings" + cGlobalVariables.AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString();
        //    string actual = target.CacheKey;

        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////A test for sortListByTable
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration1/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void sortListByTableTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings_Accessor target = new cUserdefinedFieldGroupings_Accessor(param0); // TODO: Initialize to an appropriate value
        //    Guid tableid = new Guid(); // TODO: Initialize to an appropriate value
        //    SortedList<string, cUserdefinedFieldGrouping> expected = null; // TODO: Initialize to an appropriate value
        //    SortedList<string, cUserdefinedFieldGrouping> actual;
        //    actual = target.sortListByTable(tableid);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for AccountID
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration1/sm")]
        //public void AccountIDTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid); // TODO: Initialize to an appropriate value
        //    int actual;
        //    actual = target.AccountID;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for getGrid
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration1/sm")]
        //public void getGridTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid); // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.getGrid();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for InitialiseData
        /////</summary>
        //[TestMethod()]
        //public void InitialiseDataTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings target = new cUserdefinedFieldGroupings(accountid); // TODO: Initialize to an appropriate value
        //    target.InitialiseData();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for getFilterCategories
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration1/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void getFilterCategoriesTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings_Accessor target = new cUserdefinedFieldGroupings_Accessor(param0); // TODO: Initialize to an appropriate value
        //    int groupingId = 0; // TODO: Initialize to an appropriate value
        //    Dictionary<int, List<int>> expected = null; // TODO: Initialize to an appropriate value
        //    Dictionary<int, List<int>> actual;
        //    actual = target.getFilterCategories(groupingId);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for CacheList
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration1/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void CacheListTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cUserdefinedFieldGroupings_Accessor target = new cUserdefinedFieldGroupings_Accessor(param0); // TODO: Initialize to an appropriate value
        //    SortedList<int, cUserdefinedFieldGrouping> expected = null; // TODO: Initialize to an appropriate value
        //    SortedList<int, cUserdefinedFieldGrouping> actual;
        //    actual = target.CacheList();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
