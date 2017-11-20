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
    public class authenticateMobileDevice : DatabaseTestClass
    {

        public authenticateMobileDevice()
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
        public void dbo_authenticateMobileDeviceWithArchivedEmployee()
        {
            DatabaseTestActions testActions = this.dbo_authenticateMobileDeviceWithArchivedEmployeeData;
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
        public void dbo_authenticateMobileDeviceWithNonActivatedEmployee()
        {
            DatabaseTestActions testActions = this.dbo_authenticateMobileDeviceWithNonActivatedEmployeeData;
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
        public void dbo_authenticateMobileDeviceForUnknownEmployee()
        {
            DatabaseTestActions testActions = this.dbo_authenticateMobileDeviceForUnknownEmployeeData;
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
        public void dbo_authenticateMobileDeviceSuccess()
        {
            DatabaseTestActions testActions = this.dbo_authenticateMobileDeviceSuccessData;
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
        public void dbo_authenticateMobileDeviceFail()
        {
            DatabaseTestActions testActions = this.dbo_authenticateMobileDeviceFailData;
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
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(authenticateMobileDevice));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeInactiveCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceForUnknownEmployee_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkForUnknownEmployee;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceSuccess_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkForSuccess;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceFail_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkMobileDeviceAuthenticated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testInitializeAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition mobileDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeArchived;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkForEmployeeArchivedCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeDeactivated;
            this.dbo_authenticateMobileDeviceWithArchivedEmployeeData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_authenticateMobileDeviceWithNonActivatedEmployeeData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_authenticateMobileDeviceForUnknownEmployeeData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_authenticateMobileDeviceSuccessData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_authenticateMobileDeviceFailData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeInactiveCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_authenticateMobileDeviceForUnknownEmployee_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkForUnknownEmployee = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_authenticateMobileDeviceSuccess_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkForSuccess = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_authenticateMobileDeviceFail_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkMobileDeviceAuthenticated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            mobileDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeArchived = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkForEmployeeArchivedCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeDeactivated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction
            // 
            dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction.Conditions.Add(checkEmployeeInactiveCode);
            resources.ApplyResources(dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction, "dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction");
            // 
            // checkEmployeeInactiveCode
            // 
            checkEmployeeInactiveCode.ColumnNumber = 1;
            checkEmployeeInactiveCode.Enabled = true;
            checkEmployeeInactiveCode.ExpectedValue = "-3";
            checkEmployeeInactiveCode.Name = "checkEmployeeInactiveCode";
            checkEmployeeInactiveCode.NullExpected = false;
            checkEmployeeInactiveCode.ResultSet = 1;
            checkEmployeeInactiveCode.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceForUnknownEmployee_TestAction
            // 
            dbo_authenticateMobileDeviceForUnknownEmployee_TestAction.Conditions.Add(checkForUnknownEmployee);
            resources.ApplyResources(dbo_authenticateMobileDeviceForUnknownEmployee_TestAction, "dbo_authenticateMobileDeviceForUnknownEmployee_TestAction");
            // 
            // checkForUnknownEmployee
            // 
            checkForUnknownEmployee.ColumnNumber = 1;
            checkForUnknownEmployee.Enabled = true;
            checkForUnknownEmployee.ExpectedValue = "-4";
            checkForUnknownEmployee.Name = "checkForUnknownEmployee";
            checkForUnknownEmployee.NullExpected = false;
            checkForUnknownEmployee.ResultSet = 1;
            checkForUnknownEmployee.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceSuccess_TestAction
            // 
            dbo_authenticateMobileDeviceSuccess_TestAction.Conditions.Add(checkForSuccess);
            resources.ApplyResources(dbo_authenticateMobileDeviceSuccess_TestAction, "dbo_authenticateMobileDeviceSuccess_TestAction");
            // 
            // checkForSuccess
            // 
            checkForSuccess.ColumnNumber = 1;
            checkForSuccess.Enabled = true;
            checkForSuccess.ExpectedValue = "0";
            checkForSuccess.Name = "checkForSuccess";
            checkForSuccess.NullExpected = false;
            checkForSuccess.ResultSet = 1;
            checkForSuccess.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceFail_TestAction
            // 
            dbo_authenticateMobileDeviceFail_TestAction.Conditions.Add(checkMobileDeviceAuthenticated);
            resources.ApplyResources(dbo_authenticateMobileDeviceFail_TestAction, "dbo_authenticateMobileDeviceFail_TestAction");
            // 
            // checkMobileDeviceAuthenticated
            // 
            checkMobileDeviceAuthenticated.ColumnNumber = 1;
            checkMobileDeviceAuthenticated.Enabled = true;
            checkMobileDeviceAuthenticated.ExpectedValue = "1";
            checkMobileDeviceAuthenticated.Name = "checkMobileDeviceAuthenticated";
            checkMobileDeviceAuthenticated.NullExpected = false;
            checkMobileDeviceAuthenticated.ResultSet = 1;
            checkMobileDeviceAuthenticated.RowNumber = 1;
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkEmployeeCreated);
            testInitializeAction.Conditions.Add(mobileDeviceCreated);
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
            // mobileDeviceCreated
            // 
            mobileDeviceCreated.Enabled = true;
            mobileDeviceCreated.Name = "mobileDeviceCreated";
            mobileDeviceCreated.ResultSet = 2;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction
            // 
            dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction.Conditions.Add(checkEmployeeArchived);
            resources.ApplyResources(dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction, "dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction");
            // 
            // checkEmployeeArchived
            // 
            checkEmployeeArchived.ColumnNumber = 1;
            checkEmployeeArchived.Enabled = true;
            checkEmployeeArchived.ExpectedValue = "True";
            checkEmployeeArchived.Name = "checkEmployeeArchived";
            checkEmployeeArchived.NullExpected = false;
            checkEmployeeArchived.ResultSet = 1;
            checkEmployeeArchived.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction
            // 
            dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction.Conditions.Add(checkForEmployeeArchivedCode);
            resources.ApplyResources(dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction, "dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction");
            // 
            // checkForEmployeeArchivedCode
            // 
            checkForEmployeeArchivedCode.ColumnNumber = 1;
            checkForEmployeeArchivedCode.Enabled = true;
            checkForEmployeeArchivedCode.ExpectedValue = "-2";
            checkForEmployeeArchivedCode.Name = "checkForEmployeeArchivedCode";
            checkForEmployeeArchivedCode.NullExpected = false;
            checkForEmployeeArchivedCode.ResultSet = 1;
            checkForEmployeeArchivedCode.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction
            // 
            dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction.Conditions.Add(checkEmployeeDeactivated);
            resources.ApplyResources(dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction, "dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction");
            // 
            // checkEmployeeDeactivated
            // 
            checkEmployeeDeactivated.ColumnNumber = 1;
            checkEmployeeDeactivated.Enabled = true;
            checkEmployeeDeactivated.ExpectedValue = "False";
            checkEmployeeDeactivated.Name = "checkEmployeeDeactivated";
            checkEmployeeDeactivated.NullExpected = false;
            checkEmployeeDeactivated.ResultSet = 1;
            checkEmployeeDeactivated.RowNumber = 1;
            // 
            // dbo_authenticateMobileDeviceWithArchivedEmployeeData
            // 
            this.dbo_authenticateMobileDeviceWithArchivedEmployeeData.PosttestAction = null;
            this.dbo_authenticateMobileDeviceWithArchivedEmployeeData.PretestAction = dbo_authenticateMobileDeviceWithArchivedEmployee_PretestAction;
            this.dbo_authenticateMobileDeviceWithArchivedEmployeeData.TestAction = dbo_authenticateMobileDeviceWithArchivedEmployee_TestAction;
            // 
            // dbo_authenticateMobileDeviceWithNonActivatedEmployeeData
            // 
            this.dbo_authenticateMobileDeviceWithNonActivatedEmployeeData.PosttestAction = null;
            this.dbo_authenticateMobileDeviceWithNonActivatedEmployeeData.PretestAction = dbo_authenticateMobileDeviceWithNonActivatedEmployee_PretestAction;
            this.dbo_authenticateMobileDeviceWithNonActivatedEmployeeData.TestAction = dbo_authenticateMobileDeviceWithNonActivatedEmployee_TestAction;
            // 
            // dbo_authenticateMobileDeviceForUnknownEmployeeData
            // 
            this.dbo_authenticateMobileDeviceForUnknownEmployeeData.PosttestAction = null;
            this.dbo_authenticateMobileDeviceForUnknownEmployeeData.PretestAction = null;
            this.dbo_authenticateMobileDeviceForUnknownEmployeeData.TestAction = dbo_authenticateMobileDeviceForUnknownEmployee_TestAction;
            // 
            // dbo_authenticateMobileDeviceSuccessData
            // 
            this.dbo_authenticateMobileDeviceSuccessData.PosttestAction = null;
            this.dbo_authenticateMobileDeviceSuccessData.PretestAction = null;
            this.dbo_authenticateMobileDeviceSuccessData.TestAction = dbo_authenticateMobileDeviceSuccess_TestAction;
            // 
            // dbo_authenticateMobileDeviceFailData
            // 
            this.dbo_authenticateMobileDeviceFailData.PosttestAction = null;
            this.dbo_authenticateMobileDeviceFailData.PretestAction = null;
            this.dbo_authenticateMobileDeviceFailData.TestAction = dbo_authenticateMobileDeviceFail_TestAction;
            // 
            // authenticateMobileDevice
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

        private DatabaseTestActions dbo_authenticateMobileDeviceWithArchivedEmployeeData;
        private DatabaseTestActions dbo_authenticateMobileDeviceWithNonActivatedEmployeeData;
        private DatabaseTestActions dbo_authenticateMobileDeviceForUnknownEmployeeData;
        private DatabaseTestActions dbo_authenticateMobileDeviceSuccessData;
        private DatabaseTestActions dbo_authenticateMobileDeviceFailData;
    }
}
