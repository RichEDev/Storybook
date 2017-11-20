using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCcbItemArrayTest and is intended
    ///to contain all cCcbItemArrayTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCcbItemArrayTest
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
        ///A test for itemArray
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void itemArrayTest()
        {
            cCcbItemArray target = new cCcbItemArray(); // TODO: Initialize to an appropriate value
            List<cCcbItem> actual;
            actual = target.itemArray;
            Assert.IsTrue(actual.GetType() == typeof(List<cCcbItem>));
        }

        /// <summary>
        ///A test for cCcbItemArray Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void cCcbItemArrayConstructorTest1()
        {
            cCcbItemArray target = new cCcbItemArray();
            if (target.GetType() != typeof(cCcbItemArray))
            {
                Assert.Fail("Constructor did not return an object of the correct type!");
            }
        }

        /// <summary>
        ///A test for cCcbItemArray Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\_FILES\\_WebProjects\\_VisualStudio\\_Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void cCcbItemArrayConstructorTest()
        {
            object[] codesArr = new object[] { 0, 0, 0, 100 };
            object[] percArr = new object[] { codesArr };
            object[] objArr = new object[] { percArr };
            
            object[] ccbItemArrayData = objArr; // TODO: Initialize to an appropriate value
            cCcbItemArray target = new cCcbItemArray(ccbItemArrayData);
            Assert.IsTrue(target.itemArray.Count == 1);
        }
    }
}
