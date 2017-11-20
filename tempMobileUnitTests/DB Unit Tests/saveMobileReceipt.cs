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
    public class saveMobileReceipt : DatabaseTestClass
    {

        public saveMobileReceipt()
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
        public void dbo_saveMobileReceipt()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileReceiptData;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(saveMobileReceipt));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkCategoryCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkSubcatCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkReasonCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkMobileItemCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileReceipt_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkReceiptDataSaved;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            this.dbo_saveMobileReceiptData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkCategoryCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkSubcatCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkReasonCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkMobileItemCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileReceipt_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkReceiptDataSaved = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkEmployeeCreated);
            testInitializeAction.Conditions.Add(checkCategoryCreated);
            testInitializeAction.Conditions.Add(checkSubcatCreated);
            testInitializeAction.Conditions.Add(checkReasonCreated);
            testInitializeAction.Conditions.Add(checkMobileItemCreated);
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
            // checkCategoryCreated
            // 
            checkCategoryCreated.ColumnNumber = 1;
            checkCategoryCreated.Enabled = true;
            checkCategoryCreated.ExpectedValue = "1";
            checkCategoryCreated.Name = "checkCategoryCreated";
            checkCategoryCreated.NullExpected = false;
            checkCategoryCreated.ResultSet = 2;
            checkCategoryCreated.RowNumber = 1;
            // 
            // checkSubcatCreated
            // 
            checkSubcatCreated.ColumnNumber = 1;
            checkSubcatCreated.Enabled = true;
            checkSubcatCreated.ExpectedValue = "1";
            checkSubcatCreated.Name = "checkSubcatCreated";
            checkSubcatCreated.NullExpected = false;
            checkSubcatCreated.ResultSet = 3;
            checkSubcatCreated.RowNumber = 1;
            // 
            // checkReasonCreated
            // 
            checkReasonCreated.ColumnNumber = 1;
            checkReasonCreated.Enabled = true;
            checkReasonCreated.ExpectedValue = "1";
            checkReasonCreated.Name = "checkReasonCreated";
            checkReasonCreated.NullExpected = false;
            checkReasonCreated.ResultSet = 4;
            checkReasonCreated.RowNumber = 1;
            // 
            // checkMobileItemCreated
            // 
            checkMobileItemCreated.ColumnNumber = 1;
            checkMobileItemCreated.Enabled = true;
            checkMobileItemCreated.ExpectedValue = "1";
            checkMobileItemCreated.Name = "checkMobileItemCreated";
            checkMobileItemCreated.NullExpected = false;
            checkMobileItemCreated.ResultSet = 5;
            checkMobileItemCreated.RowNumber = 1;
            // 
            // dbo_saveMobileReceipt_TestAction
            // 
            dbo_saveMobileReceipt_TestAction.Conditions.Add(checkReceiptDataSaved);
            resources.ApplyResources(dbo_saveMobileReceipt_TestAction, "dbo_saveMobileReceipt_TestAction");
            // 
            // checkReceiptDataSaved
            // 
            checkReceiptDataSaved.Enabled = true;
            checkReceiptDataSaved.Name = "checkReceiptDataSaved";
            checkReceiptDataSaved.ResultSet = 1;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_saveMobileReceiptData
            // 
            this.dbo_saveMobileReceiptData.PosttestAction = null;
            this.dbo_saveMobileReceiptData.PretestAction = null;
            this.dbo_saveMobileReceiptData.TestAction = dbo_saveMobileReceipt_TestAction;
            // 
            // saveMobileReceipt
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

        private DatabaseTestActions dbo_saveMobileReceiptData;
    }
}
