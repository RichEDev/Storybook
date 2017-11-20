using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for aeCarsTest and is intended
    ///to contain all aeCarsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class aeCarsTest
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
        ///A test for aeCars Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void aeCarsConstructorTest()
        {
            aeCars target = new aeCars();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void InitializeComponentTest()
        {
            aeCars_Accessor target = new aeCars_Accessor(); // TODO: Initialize to an appropriate value
            target.InitializeComponent();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnInit
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void OnInitTest()
        {
            aeCars_Accessor target = new aeCars_Accessor(); // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.OnInit(e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Page_Load
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        [DeploymentItem("Spend Management.dll")]
        public void Page_LoadTest()
        {
            aeCars_Accessor target = new aeCars_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.Page_Load(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AccountID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void AccountIDTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.AccountID = expected;
            actual = target.AccountID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Action
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void ActionTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            aeCarPageAction expected = new aeCarPageAction(); // TODO: Initialize to an appropriate value
            aeCarPageAction actual;
            target.Action = expected;
            actual = target.Action;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CarID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void CarIDTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.CarID = expected;
            actual = target.CarID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EmployeeAdmin
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void EmployeeAdminTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.EmployeeAdmin = expected;
            actual = target.EmployeeAdmin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EmployeeID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void EmployeeIDTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.EmployeeID = expected;
            actual = target.EmployeeID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for HideButtons
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void HideButtonsTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.HideButtons = expected;
            actual = target.HideButtons;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for inModalPopup
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void inModalPopupTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.inModalPopup = expected;
            actual = target.inModalPopup;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for isAeExpenses
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void isAeExpensesTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.isAeExpenses = expected;
            actual = target.isAeExpenses;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for isPoolCar
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void isPoolCarTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.isPoolCar = expected;
            actual = target.isPoolCar;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReturnURL
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void ReturnURLTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ReturnURL = expected;
            actual = target.ReturnURL;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendEmailWhenNewCarAdded
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SendEmailWhenNewCarAddedTest()
        {
            aeCars target = new aeCars(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.SendEmailWhenNewCarAdded = expected;
            actual = target.SendEmailWhenNewCarAdded;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
