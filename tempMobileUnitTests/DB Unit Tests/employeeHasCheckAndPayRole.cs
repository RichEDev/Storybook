using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Schema.UnitTesting;
using Microsoft.Data.Schema.UnitTesting.Conditions;

namespace tempMobileUnitTests.DB_Unit_Tests
{
    [TestClass()]
    public class employeeHasCheckAndPayRole : DatabaseTestClass
    {

        public employeeHasCheckAndPayRole()
        {
            InitializeComponent();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            base.InitializeTest();
        }
        [TestCleanup()]
        public void TestCleanup()
        {
            base.CleanupTest();
        }

        [TestMethod()]
        public void dbo_employeeHasCheckAndPayFalse()
        {
            DatabaseTestActions testActions = this.dbo_employeeHasCheckAndPayFalseData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            ExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            // Execute the test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
            ExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            // Execute the post-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
            ExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
        }
        [TestMethod()]
        public void dbo_employeeHasCheckAndPayTrue()
        {
            DatabaseTestActions testActions = this.dbo_employeeHasCheckAndPayTrueData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            ExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            // Execute the test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
            ExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            // Execute the post-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
            ExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
        }


        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testInitializeAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_employeeHasCheckAndPayTrue_TestAction;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_employeeHasCheckAndPayFalse_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(employeeHasCheckAndPayRole));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition CheckEmployeeHasNoCheckAndPay;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition CheckEmployeeHasCheckAndPay;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_employeeHasCheckAndPayTrue_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkAccessRoleCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkRoleElementCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkEmpAccessRoleCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_employeeHasCheckAndPayTrue_PosttestAction;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            this.dbo_employeeHasCheckAndPayFalseData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_employeeHasCheckAndPayTrueData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_employeeHasCheckAndPayTrue_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            dbo_employeeHasCheckAndPayFalse_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            CheckEmployeeHasNoCheckAndPay = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            CheckEmployeeHasCheckAndPay = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_employeeHasCheckAndPayTrue_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkAccessRoleCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkRoleElementCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            checkEmpAccessRoleCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            dbo_employeeHasCheckAndPayTrue_PosttestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            // 
            // dbo_employeeHasCheckAndPayFalseData
            // 
            this.dbo_employeeHasCheckAndPayFalseData.PosttestAction = null;
            this.dbo_employeeHasCheckAndPayFalseData.PretestAction = null;
            this.dbo_employeeHasCheckAndPayFalseData.TestAction = dbo_employeeHasCheckAndPayFalse_TestAction;
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkEmployeeCreated);
            resources.ApplyResources(testInitializeAction, "testInitializeAction");
            // 
            // checkEmployeeCreated
            // 
            checkEmployeeCreated.ColumnNumber = 1;
            checkEmployeeCreated.Enabled = true;
            checkEmployeeCreated.ExpectedValue = "1";
            checkEmployeeCreated.Name = "checkEmployeeCreated";
            checkEmployeeCreated.NullExpected = false;
            checkEmployeeCreated.ResultSet = 1;
            checkEmployeeCreated.RowNumber = 1;
            // 
            // dbo_employeeHasCheckAndPayTrueData
            // 
            this.dbo_employeeHasCheckAndPayTrueData.PosttestAction = dbo_employeeHasCheckAndPayTrue_PosttestAction;
            this.dbo_employeeHasCheckAndPayTrueData.PretestAction = dbo_employeeHasCheckAndPayTrue_PretestAction;
            this.dbo_employeeHasCheckAndPayTrueData.TestAction = dbo_employeeHasCheckAndPayTrue_TestAction;
            // 
            // dbo_employeeHasCheckAndPayTrue_TestAction
            // 
            dbo_employeeHasCheckAndPayTrue_TestAction.Conditions.Add(CheckEmployeeHasCheckAndPay);
            resources.ApplyResources(dbo_employeeHasCheckAndPayTrue_TestAction, "dbo_employeeHasCheckAndPayTrue_TestAction");
            // 
            // dbo_employeeHasCheckAndPayFalse_TestAction
            // 
            dbo_employeeHasCheckAndPayFalse_TestAction.Conditions.Add(CheckEmployeeHasNoCheckAndPay);
            resources.ApplyResources(dbo_employeeHasCheckAndPayFalse_TestAction, "dbo_employeeHasCheckAndPayFalse_TestAction");
            // 
            // CheckEmployeeHasNoCheckAndPay
            // 
            CheckEmployeeHasNoCheckAndPay.ColumnNumber = 1;
            CheckEmployeeHasNoCheckAndPay.Enabled = true;
            CheckEmployeeHasNoCheckAndPay.ExpectedValue = "False";
            CheckEmployeeHasNoCheckAndPay.Name = "CheckEmployeeHasNoCheckAndPay";
            CheckEmployeeHasNoCheckAndPay.NullExpected = false;
            CheckEmployeeHasNoCheckAndPay.ResultSet = 1;
            CheckEmployeeHasNoCheckAndPay.RowNumber = 1;
            // 
            // CheckEmployeeHasCheckAndPay
            // 
            CheckEmployeeHasCheckAndPay.ColumnNumber = 1;
            CheckEmployeeHasCheckAndPay.Enabled = true;
            CheckEmployeeHasCheckAndPay.ExpectedValue = "True";
            CheckEmployeeHasCheckAndPay.Name = "CheckEmployeeHasCheckAndPay";
            CheckEmployeeHasCheckAndPay.NullExpected = false;
            CheckEmployeeHasCheckAndPay.ResultSet = 1;
            CheckEmployeeHasCheckAndPay.RowNumber = 1;
            // 
            // dbo_employeeHasCheckAndPayTrue_PretestAction
            // 
            dbo_employeeHasCheckAndPayTrue_PretestAction.Conditions.Add(checkAccessRoleCreated);
            dbo_employeeHasCheckAndPayTrue_PretestAction.Conditions.Add(checkRoleElementCreated);
            dbo_employeeHasCheckAndPayTrue_PretestAction.Conditions.Add(checkEmpAccessRoleCreated);
            resources.ApplyResources(dbo_employeeHasCheckAndPayTrue_PretestAction, "dbo_employeeHasCheckAndPayTrue_PretestAction");
            // 
            // checkAccessRoleCreated
            // 
            checkAccessRoleCreated.ColumnNumber = 1;
            checkAccessRoleCreated.Enabled = true;
            checkAccessRoleCreated.ExpectedValue = "1";
            checkAccessRoleCreated.Name = "checkAccessRoleCreated";
            checkAccessRoleCreated.NullExpected = false;
            checkAccessRoleCreated.ResultSet = 1;
            checkAccessRoleCreated.RowNumber = 1;
            // 
            // checkRoleElementCreated
            // 
            checkRoleElementCreated.Enabled = true;
            checkRoleElementCreated.Name = "checkRoleElementCreated";
            checkRoleElementCreated.ResultSet = 2;
            // 
            // checkEmpAccessRoleCreated
            // 
            checkEmpAccessRoleCreated.Enabled = true;
            checkEmpAccessRoleCreated.Name = "checkEmpAccessRoleCreated";
            checkEmpAccessRoleCreated.ResultSet = 3;
            // 
            // dbo_employeeHasCheckAndPayTrue_PosttestAction
            // 
            resources.ApplyResources(dbo_employeeHasCheckAndPayTrue_PosttestAction, "dbo_employeeHasCheckAndPayTrue_PosttestAction");
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // employeeHasCheckAndPayRole
            // 
            this.TestCleanupAction = testCleanupAction;
            this.TestInitializeAction = testInitializeAction;
        }

        #endregion


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        #endregion

        private DatabaseTestActions dbo_employeeHasCheckAndPayFalseData;
        private DatabaseTestActions dbo_employeeHasCheckAndPayTrueData;
    }
}
