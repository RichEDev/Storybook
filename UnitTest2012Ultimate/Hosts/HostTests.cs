namespace UnitTest2012Ultimate.Hosts
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions;

    /// <summary>
    /// The tests for hosts. 
    /// </summary>
    [TestClass]
    public class HostTests
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public static int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public static int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the delegate id.
        /// </summary>
        public static int DelegateId { get; set; }

        /// <summary>
        /// Gets or sets the sub account id.
        /// </summary>
        public static int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the active module.
        /// </summary>
        public static int ActiveModule { get; set; }

        /// <summary>
        /// Called once for the whole set of tests.
        /// </summary>
        /// <param name="context">
        /// The context of the tests.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
            Username = string.Empty;
            Password = string.Empty;
            AccountId = GlobalTestVariables.AccountId;
            EmployeeId = GlobalTestVariables.EmployeeId;
            DelegateId = GlobalTestVariables.DelegateId;
            SubAccountId = GlobalTestVariables.SubAccountId;
            ActiveModule = GlobalTestVariables.DelegateId;

            // since test order is not guaranteed, we can run certain tests here immediately before others.
            GlobalHostInformationShouldNotBeNullAfterGlobalStartup();
            GlobalHostInformationShouldContainAtLeastOneEntryAfterGlobalStartup();
            EnsureFullComplimentOfHostInfoWithinCollection();
        }

        /// <summary>
        /// Happens once for cleanup for the whole set of tests.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The global host information static collection should not be null after global startup.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public static void GlobalHostInformationShouldNotBeNullAfterGlobalStartup()
        {
            Assert.IsNotNull(HostManager.GlobalHostInformation);
        }

        /// <summary>
        /// The global host information static collection should contain at least one entry after global startup.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers"), TestCategory("Hosts")]
        public static void GlobalHostInformationShouldContainAtLeastOneEntryAfterGlobalStartup()
        {
            Assert.IsTrue(HostManager.GlobalHostInformation.Count > 0); 
        }

        /// <summary>
        /// The new get address method should return the same values as the old code.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void NewGetAddressMethodReturnsSameValuesAsOldCode()
        {
            string uri = "http://imaginary.com/shared/home.aspx";
            string path = "/imaginary.com/shared/home.aspx";
            string query = "&imaginethis=no";

            string addressNew = HostManager.GetFormattedAddress(uri, path, query);
            string addressOld = uri.Replace(path, string.Empty).Replace("https://", string.Empty).Replace("http://", string.Empty);

            Assert.IsTrue(addressNew == addressOld);

            uri = "http://localhost/shared/whatever.aspx";
            path = "/localhost/shared/whatever.aspx";
            query = string.Empty;

            addressNew = HostManager.GetFormattedAddress(uri, path, query);
            addressOld = uri.Replace(path, string.Empty).Replace("https://", string.Empty).Replace("http://", string.Empty);

            Assert.IsTrue(addressNew == addressOld);
        }

        /// <summary>
        /// There should be at least one hostname for expenses.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void ThereIsAtLeastOneHostnameForExpenses()
        {
            Assert.IsTrue(HostManager.GetHosts(Modules.expenses).Count > 0);
        }

        /// <summary>
        /// The company placeholder correctly substitues the {companyID} placeholder.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void CompanyPlaceholderCorrectlySubstitues()
        {
            Assert.IsTrue(HostManager.SubstituteHostnameCompanyIdPlaceHolder("{companyID}.whateverz.com", "whatever") == "whatever.whateverz.com");
        }

        /// <summary>
        /// The correct themes are returned for host modules.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void CorrectThemesAreReturnedForHostModules()
        {
            Assert.IsTrue(HostManager.GetTheme(Modules.expenses) == "ExpensesThemeNew");
            Assert.IsTrue(HostManager.GetTheme(Modules.CorporateDiligence) == "CorporateDiligenceTheme");
            Assert.IsTrue(HostManager.GetTheme(Modules.ESR) == "ExpensesThemeNew");
            Assert.IsTrue(HostManager.GetTheme(Modules.Greenlight) == "GreenLightTheme");
            Assert.IsTrue(HostManager.GetTheme(Modules.GreenlightWorkforce) == "GreenLightWorkforceTheme");
            Assert.IsTrue(HostManager.GetTheme(Modules.PurchaseOrders) == "FrameworkTheme");
            Assert.IsTrue(HostManager.GetTheme(Modules.SmartDiligence) == "SmartDTheme");
            Assert.IsTrue(HostManager.GetTheme(Modules.SpendManagement) == "ExpensesThemeNew");
            Assert.IsTrue(HostManager.GetTheme(Modules.contracts) == "FrameworkTheme");
        }

        /// <summary>
        /// The correct module returned for localhost.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void CorrectModuleReturnedForLocalhost()
        {
            Assert.IsTrue(HostManager.GetModule("http://localhost") == Modules.expenses);
        }

        /// <summary>
        /// When localhost is passed GetHostname returns correct host.
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Hosts")]
        public void LocalhostPassedReturnsCorrectHost()
        {
            var localhost = new Host(99, "localhost", 2, "ExpensesThemeNew");
            var databaseHost = HostManager.GetHost("localhost");

            Assert.IsTrue(localhost.HostnameDescription == databaseHost.HostnameDescription);
            Assert.IsTrue(localhost.ModuleId == databaseHost.ModuleId);
            Assert.IsTrue(localhost.Theme == databaseHost.Theme);
        }

        /// <summary>
        /// This method populates the static collection of host information with a full array of module hosts to be tested. The databases will invariably contain too few or unpredictable data.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An unspecified module
        /// </exception>
        private static void EnsureFullComplimentOfHostInfoWithinCollection()
        {
            bool expensesHostExists = false;
            bool frameworkHostExists = false;
            bool purchaseHostExists = false;
            bool spendmanagementHostExists = false;
            bool smartdHostExists = false;
            bool corpHostExists = false;
            bool greenlightHostExists = false;
            bool greenlightWorkforceHostExists = false;
            bool esrHostExists = false;
            bool localHostExists = false;
            bool companyidHostExists = false;

            foreach (Host host in HostManager.GlobalHostInformation.Values)
            {
                switch ((Modules)host.ModuleId)
                {
                    case Modules.PurchaseOrders:
                        purchaseHostExists = true;
                        break;
                    case Modules.expenses:
                        expensesHostExists = true;
                        break;
                    case Modules.contracts:
                        frameworkHostExists = true;
                        break;
                    case Modules.SpendManagement:
                        spendmanagementHostExists = true;
                        break;
                    case Modules.SmartDiligence:
                        smartdHostExists = true;
                        break;
                    case Modules.ESR:
                        esrHostExists = true;
                        break;
                    case Modules.Greenlight:
                        greenlightHostExists = true;
                        break;
                    case Modules.GreenlightWorkforce:
                        greenlightWorkforceHostExists = true;
                        break;
                    case Modules.CorporateDiligence:
                        corpHostExists = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (host.HostnameDescription.Contains("localhost"))
                {
                    localHostExists = true;
                }

                if (host.HostnameDescription.Contains("{companyID}"))
                {
                    companyidHostExists = true;
                }
            }

            if (!expensesHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50000, new Host(50000, "imagine.sel-expenses.com", (int)Modules.expenses, "ExpensesThemeNew"));
            }

            if (!frameworkHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50001, new Host(50001, "imagine.sel-expenses.com", (int)Modules.contracts, "FrameworkTheme"));
            }

            if (!corpHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50002, new Host(50002, "imagine.sel-expenses.com", (int)Modules.CorporateDiligence, "CorporateDiligenceTheme"));
            }

            if (!esrHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50003, new Host(50003, "imagine.sel-expenses.com", (int)Modules.ESR, "ExpensesThemeNew"));
            }

            if (!greenlightHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50004, new Host(50004, "imagine.sel-expenses.com", (int)Modules.Greenlight, "GreenLightTheme"));
            }

            if (!purchaseHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50005, new Host(50005, "imagine.sel-expenses.com", (int)Modules.PurchaseOrders, "FrameworkTheme"));
            }

            if (!smartdHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50006, new Host(50006, "imagine.sel-expenses.com", (int)Modules.SmartDiligence, "SmartDTheme"));
            }

            if (!spendmanagementHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50007, new Host(50007, "imagine.sel-expenses.com", (int)Modules.SpendManagement, "ExpensesThemeNew"));
            }

            if (!localHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50008, new Host(50008, "localhost", (int)Modules.expenses, "ExpensesThemeNew"));
            }

            if (!companyidHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50009, new Host(50009, "{companyID}.localhost", (int)Modules.expenses, "ExpensesThemeNew"));
            }

            if (!greenlightWorkforceHostExists)
            {
                HostManager.GlobalHostInformation.TryAdd(
                    50010, new Host(50010, "imagine.sel-expenses.com", (int)Modules.GreenlightWorkforce, "GreenLightWorkforceTheme"));
            }
        }
    }
}
