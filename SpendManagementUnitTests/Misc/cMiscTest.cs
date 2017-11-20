using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementUnitTests.Global_Objects;
using SpendManagementLibrary;
using System.Configuration;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cMiscTest and is intended
    ///to contain all cMiscTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cMiscTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cSubAccountObject.CreateSubAccount();
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cSubAccountObject.DeleteSubAccount();
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion


        /// <summary>
        ///A test for getCurrentUser() with all session variables set
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cMiscGetCurrentUserWithAllSessionVariablesSet()
        {
            cAccountSubAccounts target = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountSubAccount subAcc = target.getSubAccountById(cGlobalVariables.SubAccountID);
            
            System.Web.HttpContext.Current.Session["SubAccountID"] = subAcc.SubAccountID;
            System.Web.HttpContext.Current.Session["SMModule"] = Modules.SpendManagement;

            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;
            CurrentUser user =  cMisc.getCurrentUser();
            Assert.AreEqual(cGlobalVariables.AccountID, user.AccountID);
            Assert.AreEqual(cGlobalVariables.EmployeeID, user.EmployeeID);
            Assert.AreEqual(Modules.SpendManagement, user.CurrentActiveModule);
            Assert.AreEqual(subAcc.SubAccountID, user.CurrentSubAccountId);
            Assert.IsNotNull(user.Employee);
            Assert.IsNotNull(user.Account);
            Assert.IsNotNull(user.clsDelegate);

            System.Web.HttpContext.Current.Session["SubAccountID"] = null;
            System.Web.HttpContext.Current.Session["SMModule"] = null;
            System.Web.HttpContext.Current.Session["myid"] = null;
        }

        /// <summary>
        ///A test for getCurrentUser with all session variables set to null
        ///</summary>       
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cMiscGetCurrentUserWithAllSessionVariableSetToNull()
        {
            cAccountSubAccounts target = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountSubAccount subAcc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            System.Web.HttpContext.Current.Session["SubAccountID"] = null;
            System.Web.HttpContext.Current.Session["SMModule"] = null;
            System.Web.HttpContext.Current.Session["myid"] = null;

            int activeModule = -1;
            Modules tmpModule = Modules.SpendManagement;
            if (System.Configuration.ConfigurationManager.AppSettings["active_module"] != null)
            {
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["active_module"], out activeModule))
                {
                    tmpModule = (Modules)activeModule;
                }
            }

            CurrentUser user = cMisc.getCurrentUser();
            Assert.AreEqual(cGlobalVariables.AccountID, user.AccountID);
            Assert.AreEqual(cGlobalVariables.EmployeeID, user.EmployeeID);
            Assert.IsNotNull(user.Employee);
            Assert.AreEqual(tmpModule, user.CurrentActiveModule);
            Assert.AreEqual(user.Employee.defaultSubAccountID, user.CurrentSubAccountId);
            Assert.IsNotNull(user.Account);
            Assert.IsNull(user.clsDelegate);
        }

        /// <summary>
        ///A test for getCurrentUser(string identity) with all session variables set
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cMiscGetCurrentUserOverloadWithAllSessionVariablesSet()
        {
            cAccountSubAccounts target = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountSubAccount subAcc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            System.Web.HttpContext.Current.Session["SubAccountID"] = subAcc.SubAccountID;
            System.Web.HttpContext.Current.Session["SMModule"] = Modules.expenses;
            
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            CurrentUser user = cMisc.getCurrentUser(cGlobalVariables.AccountID.ToString() + "," + cGlobalVariables.EmployeeID.ToString());
            Assert.AreEqual(cGlobalVariables.AccountID, user.AccountID);
            Assert.AreEqual(cGlobalVariables.EmployeeID, user.EmployeeID);
            Assert.AreEqual(Modules.expenses, user.CurrentActiveModule);
            Assert.AreEqual(subAcc.SubAccountID, user.CurrentSubAccountId);
            Assert.IsNotNull(user.Employee);
            Assert.IsNotNull(user.Account);
            Assert.IsNotNull(user.clsDelegate);

            System.Web.HttpContext.Current.Session["SubAccountID"] = null;
            System.Web.HttpContext.Current.Session["SMModule"] = null;
            System.Web.HttpContext.Current.Session["myid"] = null;
        }

        /// <summary>
        ///A test for getCurrentUser(string identity) with all session variables set to null
        ///</summary>       
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cMiscGetCurrentUserOverloadWithAllSessionVariableSetToNull()
        {
            cAccountSubAccounts target = new cAccountSubAccounts(cGlobalVariables.AccountID);
            cAccountSubAccount subAcc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            System.Web.HttpContext.Current.Session["SubAccountID"] = null;
            System.Web.HttpContext.Current.Session["SMModule"] = null;
            System.Web.HttpContext.Current.Session["myid"] = null;

            int activeModule = -1;
            Modules tmpModule = Modules.SpendManagement;
            if (System.Configuration.ConfigurationManager.AppSettings["active_module"] != null)
            {
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["active_module"], out activeModule))
                {
                    tmpModule = (Modules)activeModule;
                }
            }

            CurrentUser user = cMisc.getCurrentUser();
            Assert.AreEqual(cGlobalVariables.AccountID, user.AccountID);
            Assert.AreEqual(cGlobalVariables.EmployeeID, user.EmployeeID);
            Assert.IsNotNull(user.Employee);
            Assert.AreEqual(tmpModule, user.CurrentActiveModule);
            Assert.AreEqual(user.Employee.defaultSubAccountID, user.CurrentSubAccountId);
            Assert.IsNotNull(user.Account);
            Assert.IsNull(user.clsDelegate);
        }
    }
}
