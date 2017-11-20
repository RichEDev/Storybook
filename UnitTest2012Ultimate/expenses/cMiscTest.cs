namespace UnitTest2012Ultimate.expenses
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spend_Management;

    /// <summary>
    /// The c misc test.
    /// </summary>
    [TestClass]
    public class cMiscTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
        }

        #endregion

        /// <summary>
        /// The c misc escape linebreaks nothing to escape.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void CMiscEscapeLinebreaksNothingToEscape()
        {
            const string INVALUE = "A test string with nothing to escape.";
            string outValue = cMisc.EscapeLinebreaks(INVALUE);

            Assert.AreEqual(INVALUE, outValue);
        }

        /// <summary>
        /// The c misc escape linebreaks already escaped.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void CMiscEscapeLinebreaksAlreadyEscaped()
        {
            const string INVALUE = "A test \\n string that\'s\\ralready escaped\\r\\n.";
            string outValue = cMisc.EscapeLinebreaks(INVALUE);

            Assert.AreEqual(INVALUE, outValue);
        }

        /// <summary>
        /// The c misc escape linebreaks escape new line.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void CMiscEscapeLinebreaksEscapeNewLine()
        {
            const string INVALUE = "A test \nstring that needs \n\nescaping";
            string outValue = cMisc.EscapeLinebreaks(INVALUE);

            Assert.AreEqual(outValue, "A test \\nstring that needs \\n\\nescaping");
        }

        /// <summary>
        /// The c misc escape linebreaks escape carriage return.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void CMiscEscapeLinebreaksEscapeCarriageReturn()
        {
            const string INVALUE = "A test \rstring that needs \r\rescaping";
            string outValue = cMisc.EscapeLinebreaks(INVALUE);

            Assert.AreEqual(outValue, "A test \\rstring that needs \\r\\rescaping");
        }

        /// <summary>
        /// The c misc escape linebreaks escape all.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void CMiscEscapeLinebreaksEscapeAll()
        {
            const string INVALUE = "A test \n\rstring that needs \rescaping\n";
            string outValue = cMisc.EscapeLinebreaks(INVALUE);

            Assert.AreEqual(outValue, "A test \\n\\rstring that needs \\rescaping\\n");
        }

        /// <summary>
        /// The set user values from context returns false in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsValidUserInTestMode()
        {
            CurrentUser actual = cMisc.GetCurrentUser();

            Assert.IsNotNull(actual, "There should be a valid user in test mode");
        }

        /// <summary>
        /// The get current user returns correct account id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsCorrectAccountIdInTestMode()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int actual = user.AccountID;
            int expected = GlobalTestVariables.AccountId;
            Assert.AreEqual(expected, actual, "AccountId does not match GlobalTestVariables value.");
        }

        /// <summary>
        /// The get current user returns correct employee id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsCorrectEmployeeInTestMode()
        {
            HelperMethods.ClearTestDelegateID();
            CurrentUser user = cMisc.GetCurrentUser();
            int actual = user.EmployeeID;
            int expected = GlobalTestVariables.EmployeeId;
            Assert.AreEqual(expected, actual, "EmployeeId does not match GlobalTestVariables value.");
        }

        /// <summary>
        /// The get current user returns correct employee id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserIsNotDelegateInTestMode()
        {
            HelperMethods.ClearTestDelegateID();
            CurrentUser user = cMisc.GetCurrentUser();
            bool actual = user.isDelegate;
            Assert.AreEqual(false, actual, "User should not be a delegate in test mode.");
            Assert.IsNull(user.Delegate);
        }

        /// <summary>
        /// The get current user returns subaccountid > 0 in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserHasSubAccountMinus1InTestMode()
        {
            HelperMethods.ClearTestDelegateID();
            CurrentUser user = cMisc.GetCurrentUser();
            int actual = user.CurrentSubAccountId;
            Assert.IsTrue(actual > 0, "User should have subaccount refreshed in test mode.");
        }

        /// <summary>
        /// The get current user returns default module of spend management in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserHasSpendManagementModuleInTestMode()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            Modules actual = user.CurrentActiveModule;
            Assert.AreEqual(Modules.SpendManagement, actual, "User should have default module of SpendManagement in test mode.");
        }

        /// <summary>
        /// The get current user returns a valid user in test mode (service) passing an identity to avoid httpContext checks.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsValidUserInTestModeWithIdentityPassed()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            Assert.IsNotNull(user, "A valid user should be returned when forcing the identity");
        }

        /// <summary>
        /// The get current user returns correct account id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsCorrectAccountIdInTestModeForcedIdentity()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            int actual = user.AccountID;
            int expected = GlobalTestVariables.AccountId;
            Assert.AreEqual(expected, actual, "AccountId does not match GlobalTestVariables value.");
        }

        /// <summary>
        /// The get current user returns correct employee id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserReturnsCorrectEmployeeInTestModeForcedIdentity()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            int actual = user.EmployeeID;
            int expected = GlobalTestVariables.EmployeeId;
            Assert.AreEqual(expected, actual, "EmployeeId does not match GlobalTestVariables value.");
        }

        /// <summary>
        /// The get current user returns correct employee id in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserIsNotDelegateInTestModeForcedIdentity()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            bool actual = user.isDelegate;
            Assert.AreEqual(false, actual, "User should not be a delegate in test mode.");
            Assert.IsNull(user.Delegate);
        }

        /// <summary>
        /// The get current user returns subaccountid > 0 in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserHasSubAccountMinus1InTestModeForcedIdentity()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            int actual = user.CurrentSubAccountId;
            Assert.IsTrue(actual > 0, "User should have subaccount refreshed in test mode.");
        }

        /// <summary>
        /// The get current user returns default module of spend management in test mode.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Expenses"), TestCategory("Misc")]
        public void GetCurrentUserHasSpendManagementModuleInTestModeForcedIdentity()
        {
            string identity = string.Format("{0},{1}", GlobalTestVariables.AccountId, GlobalTestVariables.EmployeeId);
            CurrentUser user = cMisc.GetCurrentUser(identity);
            Modules actual = user.CurrentActiveModule;
            Assert.AreEqual(Modules.SpendManagement, actual, "User should have default module of SpendManagement in test mode.");
        }
    }
}
