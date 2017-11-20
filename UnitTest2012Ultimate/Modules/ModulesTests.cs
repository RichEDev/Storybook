using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spend_Management;
using SpendManagementLibrary;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MoQweb.classes;
using System.Web;

namespace UnitTest2012Ultimate
{
    /// <summary>

    /// </summary>
    [TestClass]
    public class ModulesTests
    {
        private string username;
        private string password;
        private int accountID;
        private int employeeID;
        private int delegateID;
        private int subAccountID;
        private int activeModule;

        #region Modules Tests


        /// <summary>
        /// A test for GetModuleByID
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Modules"), TestMethod()]
        public void GetModuleByIdTest()
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

		/// <summary>
		/// A test for GetThemeFromModule: PurchaseOrders, SpendManagement, ESR &amp; expenses modules should return ExpensesNewTheme
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetThemeFromModulePurchaseOrdersReturnsDefaultTheme()
		{
			const string FrameworkTheme = "FrameworkTheme";
		    const string ExpensesTheme = "ExpensesThemeNew";
		    var info = HostManager.GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == (int)Modules.PurchaseOrders);
            string actual = HostManager.GetTheme(Modules.PurchaseOrders);
		    Assert.AreEqual(info == null ? ExpensesTheme : FrameworkTheme, actual);
		}

		/// <summary>
		/// A test for GetThemeFromModule: contracts module should return FrameworkTheme or ExpensesTheme if no entry
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetThemeFromModuleContractsReturnsFrameworkTheme()
		{
            const string FrameworkTheme = "FrameworkTheme";
            const string ExpensesTheme = "ExpensesThemeNew";
            var info = HostManager.GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == (int)Modules.contracts);
            string actual = HostManager.GetTheme(Modules.contracts);
            Assert.AreEqual(info != null ? FrameworkTheme : ExpensesTheme, actual);
		}

		/// <summary>
		/// A test for GetThemeFromModule, SmartDiligence should return SmartDTheme
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetThemeFromModuleSmartDiligenceReturnsSmartDTheme()
		{
            const string SmartDTheme = "SmartDTheme";
            const string ExpensesTheme = "ExpensesThemeNew";
            var info = HostManager.GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == (int)Modules.SmartDiligence);
            var actual = HostManager.GetTheme(Modules.SmartDiligence);
            Assert.AreEqual(info != null ? SmartDTheme : ExpensesTheme, actual);
		}

		/// <summary>
		/// A test for GetThemeFromModule, Greenlight module should return GreenlightTheme
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetThemeFromModuleGreenlightReturnsGreenlightTheme()
		{
            const string GreenLightTheme = "GreenLightTheme";
            const string ExpensesTheme = "ExpensesThemeNew";
            var info = HostManager.GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == (int)Modules.Greenlight);
            var actual = HostManager.GetTheme(Modules.Greenlight);
            Assert.AreEqual(info != null ? GreenLightTheme : ExpensesTheme, actual);
		}

        /// <summary>
        /// A test for GetThemeFromModule, GreenlightWorkforce module should return GreenlightWorkforceTheme
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
        public void GetThemeFromModuleGreenlightWorkforceReturnsGreenlightWorkforceTheme()
        {
            const string GreenLightWorkforceTheme = "GreenLightWorkforceTheme";
            const string ExpensesTheme = "ExpensesThemeNew";
            var info = HostManager.GlobalHostInformation.Values.FirstOrDefault(x => x.ModuleId == (int)Modules.GreenlightWorkforce);
            var actual = HostManager.GetTheme(Modules.GreenlightWorkforce);
            Assert.AreEqual(info != null ? GreenLightWorkforceTheme : ExpensesTheme, actual);
        }

		/// <summary>
		/// A test for GetDefaultHomepageForModule, the projects page should be returned for the smart diligence module.
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleSmartDReturnsProjectPage()
		{
			const string TARGET = "~/shared/viewentities.aspx?entityid=33&viewid=16";
			const Modules MODULE = Modules.SmartDiligence;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

		/// <summary>
		/// A test for GetDefaultHomepageForModule where the spend management module is passed, the default homepage is expected
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleSpendManagementReturnsStandardHomepage()
		{
			const string TARGET = "~/home.aspx";
			const Modules MODULE = Modules.SpendManagement;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

		/// <summary>
		/// A test for GetDefaultHomepageForModule where the expenses module is passed, the default homepage is expected
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleExpensesReturnsStandardHomepage()
		{
			const string TARGET = "~/home.aspx";
			const Modules MODULE = Modules.expenses;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

		/// <summary>
		/// A test for GetDefaultHomepageForModule where the framework module is passed, the default homepage is expected
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleFrameworkReturnsStandardHomepage()
		{
			const string TARGET = "~/home.aspx";
			const Modules MODULE = Modules.contracts;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

		/// <summary>
		/// A test for GetDefaultHomepageForModule where the corporate diligence module is passed, the default homepage is expected
		/// </summary>
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleCorporateDiligenceReturnsStandardHomepage()
		{
			const string TARGET = "~/home.aspx";
			const Modules MODULE = Modules.CorporateDiligence;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

		/// <summary>
		/// A test for GetDefaultHomepageForModule where an out of range module is passed, an error is generated.
		/// </summary>
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		[TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
		public void GetDefaultHomepageForModuleEsrModuleGeneratesError()
		{
			const string TARGET = "~/home.aspx";
			const Modules MODULE = Modules.ESR;
			var homepage = cModules.GetDefaultHomepageForModule(MODULE);
			Assert.AreEqual(TARGET, homepage);
		}

        #endregion


        [TestInitialize]
        public void TestInitialize()
        {
            GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            GlobalVariables.DefaultModule = (Modules)int.Parse(ConfigurationManager.AppSettings["active_module"].ToString());

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                // Starts the sql dependency on "metabase" connection string
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());
            }

            username = string.Empty;
            password = string.Empty;
            accountID = GlobalTestVariables.AccountId;
            employeeID = GlobalTestVariables.EmployeeId;
            delegateID = GlobalTestVariables.DelegateId;
            subAccountID = GlobalTestVariables.SubAccountId;
            activeModule = GlobalTestVariables.ActiveModuleId;

        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
    }
}
