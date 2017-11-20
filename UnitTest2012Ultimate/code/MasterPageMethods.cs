namespace UnitTest2012Ultimate.Code
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// The master page methods.
    /// </summary>
    [TestClass]
    public class MasterPageMethods
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// The get module enum returns spend management module with blank host.
        /// </summary>
        [TestMethod]
        public void GetModuleEnumReturnsSpendManagementModuleWithBlankHost()
        {
            string host = string.Empty;
            Modules result = cMasterPageMethods.GetModuleEnum(host);
            Assert.AreEqual(Modules.SpendManagement, result);
        }

        /// <summary>
        /// At present, testing mode sets the current user to module spend management, getCurrentUser will need to be developed further after the logon page completion.
        /// </summary>
        [TestMethod]
        public void GetModuleEnumReturnsSpendManagementWithAnyOtherHostInTestMode()
        {
            const string Host = "greenlight.sel-expenses.com";
            Modules result = cMasterPageMethods.GetModuleEnum(Host);
            Assert.AreEqual(Modules.SpendManagement, result);
        }
    }
}
