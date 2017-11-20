using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Schema.UnitTesting;
using Microsoft.Data.Schema.UnitTesting.Conditions;

namespace tempMobileUnitTests
{
    [TestClass()]
    public class dbo_pairMobileDevice : DatabaseTestClass
    {

        public dbo_pairMobileDevice()
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
        public void dbo_pairMobileDeviceReuseExistingKey()
        {
            DatabaseTestActions testActions = this.dbo_pairMobileDeviceReuseExistingKeyData;
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
        public void dbo_pairMobileDevicePairingKeyInUse()
        {
            DatabaseTestActions testActions = this.dbo_pairMobileDevicePairingKeyInUseData;
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
        public void dbo_pairMobileDeviceSuccess()
        {
            DatabaseTestActions testActions = this.dbo_pairMobileDeviceSuccessData;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dbo_pairMobileDevice));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_pairMobileDevicePairingKeyInUse_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition CheckForKeyInUseCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_pairMobileDeviceSuccess_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition CheckSuccessfulPairingCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_pairMobileDevicePairingKeyInUse_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition CheckMobileDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_pairMobileDeviceReuseExistingKey_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkCurrentMobileDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_pairMobileDeviceReuseExistingKey_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition CheckForReuseCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            this.dbo_pairMobileDeviceReuseExistingKeyData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_pairMobileDevicePairingKeyInUseData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_pairMobileDeviceSuccessData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_pairMobileDevicePairingKeyInUse_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            CheckForKeyInUseCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_pairMobileDeviceSuccess_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            CheckSuccessfulPairingCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_pairMobileDevicePairingKeyInUse_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            CheckMobileDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            dbo_pairMobileDeviceReuseExistingKey_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkCurrentMobileDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            dbo_pairMobileDeviceReuseExistingKey_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            CheckForReuseCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
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
            // dbo_pairMobileDevicePairingKeyInUse_TestAction
            // 
            dbo_pairMobileDevicePairingKeyInUse_TestAction.Conditions.Add(CheckForKeyInUseCode);
            resources.ApplyResources(dbo_pairMobileDevicePairingKeyInUse_TestAction, "dbo_pairMobileDevicePairingKeyInUse_TestAction");
            // 
            // CheckForKeyInUseCode
            // 
            CheckForKeyInUseCode.ColumnNumber = 1;
            CheckForKeyInUseCode.Enabled = true;
            CheckForKeyInUseCode.ExpectedValue = "-1";
            CheckForKeyInUseCode.Name = "CheckForKeyInUseCode";
            CheckForKeyInUseCode.NullExpected = false;
            CheckForKeyInUseCode.ResultSet = 1;
            CheckForKeyInUseCode.RowNumber = 1;
            // 
            // dbo_pairMobileDeviceSuccess_TestAction
            // 
            dbo_pairMobileDeviceSuccess_TestAction.Conditions.Add(CheckSuccessfulPairingCode);
            resources.ApplyResources(dbo_pairMobileDeviceSuccess_TestAction, "dbo_pairMobileDeviceSuccess_TestAction");
            // 
            // CheckSuccessfulPairingCode
            // 
            CheckSuccessfulPairingCode.ColumnNumber = 1;
            CheckSuccessfulPairingCode.Enabled = true;
            CheckSuccessfulPairingCode.ExpectedValue = "0";
            CheckSuccessfulPairingCode.Name = "CheckSuccessfulPairingCode";
            CheckSuccessfulPairingCode.NullExpected = false;
            CheckSuccessfulPairingCode.ResultSet = 1;
            CheckSuccessfulPairingCode.RowNumber = 1;
            // 
            // dbo_pairMobileDevicePairingKeyInUse_PretestAction
            // 
            dbo_pairMobileDevicePairingKeyInUse_PretestAction.Conditions.Add(CheckMobileDeviceCreated);
            resources.ApplyResources(dbo_pairMobileDevicePairingKeyInUse_PretestAction, "dbo_pairMobileDevicePairingKeyInUse_PretestAction");
            // 
            // CheckMobileDeviceCreated
            // 
            CheckMobileDeviceCreated.Enabled = true;
            CheckMobileDeviceCreated.Name = "CheckMobileDeviceCreated";
            CheckMobileDeviceCreated.ResultSet = 1;
            // 
            // dbo_pairMobileDeviceReuseExistingKey_PretestAction
            // 
            dbo_pairMobileDeviceReuseExistingKey_PretestAction.Conditions.Add(checkCurrentMobileDeviceCreated);
            resources.ApplyResources(dbo_pairMobileDeviceReuseExistingKey_PretestAction, "dbo_pairMobileDeviceReuseExistingKey_PretestAction");
            // 
            // checkCurrentMobileDeviceCreated
            // 
            checkCurrentMobileDeviceCreated.Enabled = true;
            checkCurrentMobileDeviceCreated.Name = "checkCurrentMobileDeviceCreated";
            checkCurrentMobileDeviceCreated.ResultSet = 1;
            // 
            // dbo_pairMobileDeviceReuseExistingKey_TestAction
            // 
            dbo_pairMobileDeviceReuseExistingKey_TestAction.Conditions.Add(CheckForReuseCode);
            resources.ApplyResources(dbo_pairMobileDeviceReuseExistingKey_TestAction, "dbo_pairMobileDeviceReuseExistingKey_TestAction");
            // 
            // CheckForReuseCode
            // 
            CheckForReuseCode.ColumnNumber = 1;
            CheckForReuseCode.Enabled = true;
            CheckForReuseCode.ExpectedValue = "0";
            CheckForReuseCode.Name = "CheckForReuseCode";
            CheckForReuseCode.NullExpected = false;
            CheckForReuseCode.ResultSet = 1;
            CheckForReuseCode.RowNumber = 1;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_pairMobileDeviceReuseExistingKeyData
            // 
            this.dbo_pairMobileDeviceReuseExistingKeyData.PosttestAction = null;
            this.dbo_pairMobileDeviceReuseExistingKeyData.PretestAction = dbo_pairMobileDeviceReuseExistingKey_PretestAction;
            this.dbo_pairMobileDeviceReuseExistingKeyData.TestAction = dbo_pairMobileDeviceReuseExistingKey_TestAction;
            // 
            // dbo_pairMobileDevicePairingKeyInUseData
            // 
            this.dbo_pairMobileDevicePairingKeyInUseData.PosttestAction = null;
            this.dbo_pairMobileDevicePairingKeyInUseData.PretestAction = dbo_pairMobileDevicePairingKeyInUse_PretestAction;
            this.dbo_pairMobileDevicePairingKeyInUseData.TestAction = dbo_pairMobileDevicePairingKeyInUse_TestAction;
            // 
            // dbo_pairMobileDeviceSuccessData
            // 
            this.dbo_pairMobileDeviceSuccessData.PosttestAction = null;
            this.dbo_pairMobileDeviceSuccessData.PretestAction = null;
            this.dbo_pairMobileDeviceSuccessData.TestAction = dbo_pairMobileDeviceSuccess_TestAction;
            // 
            // dbo_pairMobileDevice
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

        private DatabaseTestActions dbo_pairMobileDeviceReuseExistingKeyData;
        private DatabaseTestActions dbo_pairMobileDevicePairingKeyInUseData;
        private DatabaseTestActions dbo_pairMobileDeviceSuccessData;
    }
}
