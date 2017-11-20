using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for costCentreBreakdownTest and is intended
    ///to contain all costCentreBreakdownTest Unit Tests
    ///</summary>
    [TestClass()]
    public class costCentreBreakdownTest
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
        ///A test for ReadOnly
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void ReadOnlyTest()
        {
            costCentreBreakdown target = new costCentreBreakdown(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ReadOnly;
            Assert.AreEqual(expected, actual); // should be false by default

            target.ReadOnly = expected;
            actual = target.ReadOnly;
            Assert.AreEqual(expected, actual); // should be false if set to default

            expected = true;
            target.ReadOnly = expected;
            actual = target.ReadOnly;
            Assert.AreEqual(expected, actual); // should be true if set to true

        }

        /// <summary>
        ///A test for ModalExtenderId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void ModalExtenderIdTest()
        {
            costCentreBreakdown target = new costCentreBreakdown(); // TODO: Initialize to an appropriate value
            string expected = null; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ModalExtenderId;
            Assert.AreEqual(expected, actual);

            expected = "someID";
            target.ModalExtenderId = expected;
            actual = target.ModalExtenderId;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HideButtons
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void HideButtonsTest()
        {
            costCentreBreakdown target = new costCentreBreakdown(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.HideButtons;
            Assert.AreEqual(expected, actual); // should be false by default

            target.HideButtons = expected;
            actual = target.HideButtons;
            Assert.AreEqual(expected, actual);

            expected = true;
            target.HideButtons = expected;
            actual = target.HideButtons;
            Assert.AreEqual(expected, actual);
        }

        ///// <summary>
        /////A test for AddCostCentreBreakdownRow
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void AddCostCentreBreakdownRowTest()
        //{
        //    costCentreBreakdown target = new costCentreBreakdown();
        //    int itemId = 0;
        //    Nullable<int> dep = null;
        //    Nullable<int> cc = null;
        //    Nullable<int> pc = null;
        //    int percent = 0;
        //    target.AddCostCentreBreakdownRow(itemId, dep, cc, pc, percent);
        //}

        /// <summary>
        ///A test for UserControlDisplayType
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void UserControlDisplayTypeTest()
        {
            costCentreBreakdown target = new costCentreBreakdown();
            Nullable<UserControlType> expected = UserControlType.None;
            Nullable<UserControlType> actual;

            actual = target.UserControlDisplayType;
            Assert.AreEqual(expected, actual);

            expected = UserControlType.None;
            target.UserControlDisplayType = expected;
            actual = target.UserControlDisplayType;
            Assert.AreEqual(expected, actual);

            expected = UserControlType.Inline;
            target.UserControlDisplayType = expected;
            actual = target.UserControlDisplayType;
            Assert.AreEqual(expected, actual);

            expected = UserControlType.Modal;
            target.UserControlDisplayType = expected;
            actual = target.UserControlDisplayType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EmptyValuesEnabled
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void EmptyValuesEnabledTest()
        {
            costCentreBreakdown target = new costCentreBreakdown();
            bool expected = false;
            bool actual;
            actual = target.EmptyValuesEnabled;
            Assert.AreEqual(expected, actual); // default should be false

            target.EmptyValuesEnabled = expected;
            actual = target.EmptyValuesEnabled;
            Assert.AreEqual(expected, actual);

            expected = true;
            target.EmptyValuesEnabled = expected;
            actual = target.EmptyValuesEnabled;
            Assert.AreEqual(expected, actual);
        }

        ///// <summary>
        /////A test for Page_Init
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //[DeploymentItem("Spend Management.dll")]
        //public void Page_InitTest()
        //{
        //    costCentreBreakdown_Accessor target = new costCentreBreakdown_Accessor(); // TODO: Initialize to an appropriate value
        //    object sender = null; // TODO: Initialize to an appropriate value
        //    EventArgs e = null; // TODO: Initialize to an appropriate value
        //    target.Page_Init(sender, e);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for GetCostCentreBreakdown
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetCostCentreBreakdownTest()
        {
            object[] itemArrayFromJS = new object[] { new object[] { new object[] { null, null, null, 100 } } };

            cCcbItemArray expected = new cCcbItemArray();
            //expected.itemArray.Add(new cCcbItem(1, new SpendManagementLibrary.cDepartment(0, string.Empty, string.Empty, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, null), new SpendManagementLibrary.cCostCode(0, string.Empty, string.Empty, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, null), new SpendManagementLibrary.cProjectCode(0, string.Empty, string.Empty, false, false, DateTime.Now, cGlobalVariables.EmployeeID, null, null, null), 100));
            expected.itemArray.Add(new cCcbItem(0, null, null, null, 100));

            cCcbItemArray actual;
            actual = costCentreBreakdown.GetCostCentreBreakdown(itemArrayFromJS);
            //Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.itemArray[0].costCodeID, actual.itemArray[0].costCodeID);
            Assert.AreEqual(expected.itemArray[0].departmentID, actual.itemArray[0].departmentID);
            Assert.AreEqual(expected.itemArray[0].percentageSplit, actual.itemArray[0].percentageSplit);
            Assert.AreEqual(expected.itemArray[0].projectCodeID, actual.itemArray[0].projectCodeID);
            Assert.AreEqual(expected.itemArray[0].relatedItemID, actual.itemArray[0].relatedItemID);
        }

        ///// <summary>
        /////A test for AddCostCentreBreakdownRow
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void AddCostCentreBreakdownRowTest1()
        //{
        //    costCentreBreakdown target = new costCentreBreakdown(); // TODO: Initialize to an appropriate value
        //    Nullable<int> dep = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    Nullable<int> cc = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    Nullable<int> pc = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    int percent = 0; // TODO: Initialize to an appropriate value
        //    target.AddCostCentreBreakdownRow(dep, cc, pc, percent);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for AddCostCentreBreakdownRow
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void AddCostCentreBreakdownRowTest()
        //{
        //    costCentreBreakdown target = new costCentreBreakdown(); // TODO: Initialize to an appropriate value
        //    int itemId = 0; // TODO: Initialize to an appropriate value
        //    Nullable<int> dep = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    Nullable<int> cc = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    Nullable<int> pc = new Nullable<int>(); // TODO: Initialize to an appropriate value
        //    int percent = 0; // TODO: Initialize to an appropriate value
        //    target.AddCostCentreBreakdownRow(itemId, dep, cc, pc, percent);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for costCentreBreakdown Constructor
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void costCentreBreakdownConstructorTest()
        //{
        //    costCentreBreakdown target = new costCentreBreakdown();
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}
    }
}
