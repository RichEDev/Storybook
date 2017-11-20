using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCcbItemTest and is intended
    ///to contain all cCcbItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCcbItemTest
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
        ///A test for relatedItemID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void relatedItemIDTest()
        {
            cCcbItem target = new cCcbItem(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.relatedItemID;
            Assert.AreEqual(0, actual);
        }

        /// <summary>
        ///A test for projectCodeID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void projectCodeIDTest()
        {
            cCcbItem target = new cCcbItem(); // TODO: Initialize to an appropriate value
            cProjectCode actual;
            actual = target.projectCodeID;
            Assert.AreEqual(null, actual);
        }

        /// <summary>
        ///A test for percentageSplit
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void percentageSplitTest()
        {
            cCcbItem target = new cCcbItem(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.percentageSplit;
            Assert.AreEqual(0, actual);
        }

        /// <summary>
        ///A test for departmentID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void departmentIDTest()
        {
            cCcbItem target = new cCcbItem(); // TODO: Initialize to an appropriate value
            cDepartment actual;
            actual = target.departmentID;
            Assert.AreEqual(null, actual);
        }

        /// <summary>
        ///A test for costCodeID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void costCodeIDTest()
        {
            cCcbItem target = new cCcbItem(); // TODO: Initialize to an appropriate value
            cCostCode actual;
            actual = target.costCodeID;
            Assert.AreEqual(null, actual);
        }

        /// <summary>
        ///A test for cCcbItem Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void cCcbItemConstructorTest1()
        {
            int id = 0;
            cDepartment dep = null;
            cCostCode cc = null;
            cProjectCode pc = null;
            int perc = 0;
            cCcbItem target = new cCcbItem(id, dep, cc, pc, perc);
            Assert.AreEqual(null, target.costCodeID);
            Assert.AreEqual(null, target.departmentID);
            Assert.AreEqual(0, target.percentageSplit);
            Assert.AreEqual(null, target.projectCodeID);
            Assert.AreEqual(0, target.relatedItemID);
        }

        ///// <summary>
        /////A test for cCcbItem Constructor
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void cCcbItemConstructorTest()
        //{
        //    cCcbItem target = new cCcbItem();
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}
    }
}
