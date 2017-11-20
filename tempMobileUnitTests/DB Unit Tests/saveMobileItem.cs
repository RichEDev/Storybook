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
    public class saveMobileItem : DatabaseTestClass
    {

        public saveMobileItem()
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
        public void dbo_saveMobileItemWithInvalidReason()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileItemWithInvalidReasonData;
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
        public void dbo_saveMobileItemWithInvalidCurrency()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileItemWithInvalidCurrencyData;
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
        public void dbo_saveMobileItemWithInvalidSubcat()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileItemWithInvalidSubcatData;
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
        public void dbo_saveMobileItemSuccess()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileItemSuccessData;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(saveMobileItem));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInitialEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkInitialMobileDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkExpenseCategoryCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkSubcatCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkReasonCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileItemWithInvalidCurrency_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInvalidCurrencyCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileItemWithInvalidSubcat_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInvalidSubcatCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileItemSuccess_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkMobileItemSavedSuccessfully;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileItemWithInvalidReason_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInvalidReasonCode;
            this.dbo_saveMobileItemWithInvalidReasonData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_saveMobileItemWithInvalidCurrencyData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_saveMobileItemWithInvalidSubcatData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_saveMobileItemSuccessData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkInitialEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkInitialMobileDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            checkExpenseCategoryCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkSubcatCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkReasonCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            dbo_saveMobileItemWithInvalidCurrency_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkInvalidCurrencyCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileItemWithInvalidSubcat_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkInvalidSubcatCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileItemSuccess_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkMobileItemSavedSuccessfully = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileItemWithInvalidReason_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkInvalidReasonCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkInitialEmployeeCreated);
            testInitializeAction.Conditions.Add(checkInitialMobileDeviceCreated);
            testInitializeAction.Conditions.Add(checkExpenseCategoryCreated);
            testInitializeAction.Conditions.Add(checkSubcatCreated);
            testInitializeAction.Conditions.Add(checkReasonCreated);
            resources.ApplyResources(testInitializeAction, "testInitializeAction");
            // 
            // checkInitialEmployeeCreated
            // 
            checkInitialEmployeeCreated.ColumnNumber = 1;
            checkInitialEmployeeCreated.Enabled = true;
            checkInitialEmployeeCreated.ExpectedValue = "1";
            checkInitialEmployeeCreated.Name = "checkInitialEmployeeCreated";
            checkInitialEmployeeCreated.NullExpected = false;
            checkInitialEmployeeCreated.ResultSet = 1;
            checkInitialEmployeeCreated.RowNumber = 1;
            // 
            // checkInitialMobileDeviceCreated
            // 
            checkInitialMobileDeviceCreated.Enabled = true;
            checkInitialMobileDeviceCreated.Name = "checkInitialMobileDeviceCreated";
            checkInitialMobileDeviceCreated.ResultSet = 2;
            // 
            // checkExpenseCategoryCreated
            // 
            checkExpenseCategoryCreated.ColumnNumber = 1;
            checkExpenseCategoryCreated.Enabled = true;
            checkExpenseCategoryCreated.ExpectedValue = "1";
            checkExpenseCategoryCreated.Name = "checkExpenseCategoryCreated";
            checkExpenseCategoryCreated.NullExpected = false;
            checkExpenseCategoryCreated.ResultSet = 3;
            checkExpenseCategoryCreated.RowNumber = 1;
            // 
            // checkSubcatCreated
            // 
            checkSubcatCreated.ColumnNumber = 1;
            checkSubcatCreated.Enabled = true;
            checkSubcatCreated.ExpectedValue = "1";
            checkSubcatCreated.Name = "checkSubcatCreated";
            checkSubcatCreated.NullExpected = false;
            checkSubcatCreated.ResultSet = 4;
            checkSubcatCreated.RowNumber = 1;
            // 
            // checkReasonCreated
            // 
            checkReasonCreated.ColumnNumber = 1;
            checkReasonCreated.Enabled = true;
            checkReasonCreated.ExpectedValue = "1";
            checkReasonCreated.Name = "checkReasonCreated";
            checkReasonCreated.NullExpected = false;
            checkReasonCreated.ResultSet = 5;
            checkReasonCreated.RowNumber = 1;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_saveMobileItemWithInvalidCurrency_TestAction
            // 
            dbo_saveMobileItemWithInvalidCurrency_TestAction.Conditions.Add(checkInvalidCurrencyCode);
            resources.ApplyResources(dbo_saveMobileItemWithInvalidCurrency_TestAction, "dbo_saveMobileItemWithInvalidCurrency_TestAction");
            // 
            // checkInvalidCurrencyCode
            // 
            checkInvalidCurrencyCode.ColumnNumber = 1;
            checkInvalidCurrencyCode.Enabled = true;
            checkInvalidCurrencyCode.ExpectedValue = "-6";
            checkInvalidCurrencyCode.Name = "checkInvalidCurrencyCode";
            checkInvalidCurrencyCode.NullExpected = false;
            checkInvalidCurrencyCode.ResultSet = 1;
            checkInvalidCurrencyCode.RowNumber = 1;
            // 
            // dbo_saveMobileItemWithInvalidSubcat_TestAction
            // 
            dbo_saveMobileItemWithInvalidSubcat_TestAction.Conditions.Add(checkInvalidSubcatCode);
            resources.ApplyResources(dbo_saveMobileItemWithInvalidSubcat_TestAction, "dbo_saveMobileItemWithInvalidSubcat_TestAction");
            // 
            // checkInvalidSubcatCode
            // 
            checkInvalidSubcatCode.ColumnNumber = 1;
            checkInvalidSubcatCode.Enabled = true;
            checkInvalidSubcatCode.ExpectedValue = "-7";
            checkInvalidSubcatCode.Name = "checkInvalidSubcatCode";
            checkInvalidSubcatCode.NullExpected = false;
            checkInvalidSubcatCode.ResultSet = 1;
            checkInvalidSubcatCode.RowNumber = 1;
            // 
            // dbo_saveMobileItemSuccess_TestAction
            // 
            dbo_saveMobileItemSuccess_TestAction.Conditions.Add(checkMobileItemSavedSuccessfully);
            resources.ApplyResources(dbo_saveMobileItemSuccess_TestAction, "dbo_saveMobileItemSuccess_TestAction");
            // 
            // checkMobileItemSavedSuccessfully
            // 
            checkMobileItemSavedSuccessfully.ColumnNumber = 1;
            checkMobileItemSavedSuccessfully.Enabled = true;
            checkMobileItemSavedSuccessfully.ExpectedValue = "1";
            checkMobileItemSavedSuccessfully.Name = "checkMobileItemSavedSuccessfully";
            checkMobileItemSavedSuccessfully.NullExpected = false;
            checkMobileItemSavedSuccessfully.ResultSet = 1;
            checkMobileItemSavedSuccessfully.RowNumber = 1;
            // 
            // dbo_saveMobileItemWithInvalidReason_TestAction
            // 
            dbo_saveMobileItemWithInvalidReason_TestAction.Conditions.Add(checkInvalidReasonCode);
            resources.ApplyResources(dbo_saveMobileItemWithInvalidReason_TestAction, "dbo_saveMobileItemWithInvalidReason_TestAction");
            // 
            // checkInvalidReasonCode
            // 
            checkInvalidReasonCode.ColumnNumber = 1;
            checkInvalidReasonCode.Enabled = true;
            checkInvalidReasonCode.ExpectedValue = "-5";
            checkInvalidReasonCode.Name = "checkInvalidReasonCode";
            checkInvalidReasonCode.NullExpected = false;
            checkInvalidReasonCode.ResultSet = 1;
            checkInvalidReasonCode.RowNumber = 1;
            // 
            // dbo_saveMobileItemWithInvalidReasonData
            // 
            this.dbo_saveMobileItemWithInvalidReasonData.PosttestAction = null;
            this.dbo_saveMobileItemWithInvalidReasonData.PretestAction = null;
            this.dbo_saveMobileItemWithInvalidReasonData.TestAction = dbo_saveMobileItemWithInvalidReason_TestAction;
            // 
            // dbo_saveMobileItemWithInvalidCurrencyData
            // 
            this.dbo_saveMobileItemWithInvalidCurrencyData.PosttestAction = null;
            this.dbo_saveMobileItemWithInvalidCurrencyData.PretestAction = null;
            this.dbo_saveMobileItemWithInvalidCurrencyData.TestAction = dbo_saveMobileItemWithInvalidCurrency_TestAction;
            // 
            // dbo_saveMobileItemWithInvalidSubcatData
            // 
            this.dbo_saveMobileItemWithInvalidSubcatData.PosttestAction = null;
            this.dbo_saveMobileItemWithInvalidSubcatData.PretestAction = null;
            this.dbo_saveMobileItemWithInvalidSubcatData.TestAction = dbo_saveMobileItemWithInvalidSubcat_TestAction;
            // 
            // dbo_saveMobileItemSuccessData
            // 
            this.dbo_saveMobileItemSuccessData.PosttestAction = null;
            this.dbo_saveMobileItemSuccessData.PretestAction = null;
            this.dbo_saveMobileItemSuccessData.TestAction = dbo_saveMobileItemSuccess_TestAction;
            // 
            // saveMobileItem
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

        private DatabaseTestActions dbo_saveMobileItemWithInvalidReasonData;
        private DatabaseTestActions dbo_saveMobileItemWithInvalidCurrencyData;
        private DatabaseTestActions dbo_saveMobileItemWithInvalidSubcatData;
        private DatabaseTestActions dbo_saveMobileItemSuccessData;
    }
}
