using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cModulesTest and is intended
    ///to contain all cModulesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cModulesTest
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
        ///A test for cModules Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestCategory("Continuous Integration"), TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Projects\\expenses\\expenses-fwcatchup_iteration1\\expenses\\expenses", "/")]
        //[UrlToTest("http://localhost/iteration1/expenses/unitTests.aspx")]
        public void cModulesConstructorTest()
        {
            System.Web.Caching.Cache expectedCache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            expectedCache.Remove("spendmanagementModules");

            cModules target = new cModules();
            List<cModule> expected = (List<cModule>)expectedCache["spendmanagementModules"];

            Assert.IsTrue(expected != null);
            Assert.IsTrue(expected.Count > 0);

            expectedCache.Remove("spendmanagementModules");
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestCategory("Continuous Integration"), TestMethod()]
        public void CacheList_UpdateRefreshes()
        {
            System.Web.Caching.Cache expectedCache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            expectedCache.Remove("spendmanagementModules");

            cModules target = new cModules();
            List<cModule> expected = (List<cModule>)expectedCache["spendmanagementModules"];

            expectedCache.Remove("spendmanagementModules");
            List<cModule> actual;
            actual = target.CacheList();

            Assert.IsTrue(expected != null);
            Assert.IsTrue(expected.Count > 0);
            Assert.AreEqual(expected.Count, actual.Count);
            
            cModule mod;
            foreach (cModule me in expected)
            {
                mod = null;
                foreach (cModule ma in actual)
                {
                    if (ma.ModuleID == me.ModuleID)
                    {
                        mod = ma;
                        break;
                    }
                }
                cCompareAssert.AreEqual(me, mod);
            }

            expectedCache.Remove("spendmanagementModules");

            // should this also create a metabase entry and then test for its existance?
        }

        /// <summary>
        ///A test for GetModuleByID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestCategory("Continuous Integration"), TestMethod()]
        public void GetModuleByIDTest()
        {
            System.Web.Caching.Cache expectedCache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            expectedCache.Remove("spendmanagementModules");

            cModules target = new cModules();
            int moduleID = 2; // should be expenses
            cModule expected = cModulesObject.CreateExpensesModuleWithoutElements();
            cModule actual;
            actual = target.GetModuleByID(moduleID);

            cCompareAssert.AreEqual(expected, actual);

            expectedCache.Remove("spendmanagementModules");
        }
    }
}
