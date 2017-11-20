using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for poolcarsTest and is intended
    ///to contain all poolcarsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class poolcarsTest
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
        ///A test for poolcars Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void poolcarsConstructorTest()
        {
            poolcars target = new poolcars();
            Assert.Inconclusive("TODO: Implement code to verify target");
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
            poolcars_Accessor target = new poolcars_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.Page_Load(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Master
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root-help_support\\Spend Management_root-help_support\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void MasterTest()
        {
            poolcars target = new poolcars(); // TODO: Initialize to an appropriate value
            smTemplate actual;
            actual = target.Master;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
