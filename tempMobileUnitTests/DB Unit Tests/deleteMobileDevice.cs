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
    public class deleteMobileDevice : DatabaseTestClass
    {

        public deleteMobileDevice()
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
        public void dbo_deleteMobileDeviceSuccess()
        {
            DatabaseTestActions testActions = this.dbo_deleteMobileDeviceSuccessData;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(deleteMobileDevice));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInitialMobileDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_deleteMobileDeviceSuccess_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkDeleteRowCount;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkDeleteEntryInAuditLog;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkCreationOfRequestorEmployee;
            this.dbo_deleteMobileDeviceSuccessData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkInitialMobileDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_deleteMobileDeviceSuccess_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkDeleteRowCount = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkDeleteEntryInAuditLog = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkCreationOfRequestorEmployee = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkEmployeeCreated);
            testInitializeAction.Conditions.Add(checkCreationOfRequestorEmployee);
            testInitializeAction.Conditions.Add(checkInitialMobileDeviceCreated);
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
            // checkInitialMobileDeviceCreated
            // 
            checkInitialMobileDeviceCreated.ColumnNumber = 1;
            checkInitialMobileDeviceCreated.Enabled = true;
            checkInitialMobileDeviceCreated.ExpectedValue = "1";
            checkInitialMobileDeviceCreated.Name = "checkInitialMobileDeviceCreated";
            checkInitialMobileDeviceCreated.NullExpected = false;
            checkInitialMobileDeviceCreated.ResultSet = 3;
            checkInitialMobileDeviceCreated.RowNumber = 1;
            // 
            // dbo_deleteMobileDeviceSuccess_TestAction
            // 
            dbo_deleteMobileDeviceSuccess_TestAction.Conditions.Add(checkDeleteRowCount);
            dbo_deleteMobileDeviceSuccess_TestAction.Conditions.Add(checkDeleteEntryInAuditLog);
            resources.ApplyResources(dbo_deleteMobileDeviceSuccess_TestAction, "dbo_deleteMobileDeviceSuccess_TestAction");
            // 
            // checkDeleteRowCount
            // 
            checkDeleteRowCount.ColumnNumber = 1;
            checkDeleteRowCount.Enabled = true;
            checkDeleteRowCount.ExpectedValue = "1";
            checkDeleteRowCount.Name = "checkDeleteRowCount";
            checkDeleteRowCount.NullExpected = false;
            checkDeleteRowCount.ResultSet = 1;
            checkDeleteRowCount.RowNumber = 1;
            // 
            // checkDeleteEntryInAuditLog
            // 
            checkDeleteEntryInAuditLog.Enabled = true;
            checkDeleteEntryInAuditLog.Name = "checkDeleteEntryInAuditLog";
            checkDeleteEntryInAuditLog.ResultSet = 2;
            // 
            // dbo_deleteMobileDeviceSuccessData
            // 
            this.dbo_deleteMobileDeviceSuccessData.PosttestAction = null;
            this.dbo_deleteMobileDeviceSuccessData.PretestAction = null;
            this.dbo_deleteMobileDeviceSuccessData.TestAction = dbo_deleteMobileDeviceSuccess_TestAction;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // checkCreationOfRequestorEmployee
            // 
            checkCreationOfRequestorEmployee.ColumnNumber = 1;
            checkCreationOfRequestorEmployee.Enabled = true;
            checkCreationOfRequestorEmployee.ExpectedValue = "1";
            checkCreationOfRequestorEmployee.Name = "checkCreationOfRequestorEmployee";
            checkCreationOfRequestorEmployee.NullExpected = false;
            checkCreationOfRequestorEmployee.ResultSet = 2;
            checkCreationOfRequestorEmployee.RowNumber = 1;
            // 
            // deleteMobileDevice
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

        private DatabaseTestActions dbo_deleteMobileDeviceSuccessData;
    }
}
