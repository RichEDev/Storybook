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
    public class saveMobileDevice : DatabaseTestClass
    {

        public saveMobileDevice()
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
        public void dbo_saveMobileDeviceSuccess()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileDeviceSuccessData;
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
        public void dbo_saveMobileDeviceDuplicate()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileDeviceDuplicateData;
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
        public void dbo_saveMobileDeviceEdited()
        {
            DatabaseTestActions testActions = this.dbo_saveMobileDeviceEditedData;
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
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileDeviceDuplicate_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(saveMobileDevice));
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkForDuplicateReturnCode;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileDeviceEdited_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkUpdateSuccessful;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkDeviceNameEdited;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkDeviceTypeEdited;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkPairingKeyEdited;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkSerialKeyEdited;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkEditAuditLogEntryExists;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testInitializeAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkRequestorEmployeeCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction testCleanupAction;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileDeviceSuccess_TestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkMobileDeviceSaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkEmployeeSaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkDeviceTypeIdSaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkDeviceNameSaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkPairingKeySaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkSerialKeySaved;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkCreatedByCorrect;
            Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition checkInsertAuditLogEntryExists;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileDeviceDuplicate_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkInitialDeviceCreated;
            Microsoft.Data.Schema.UnitTesting.DatabaseTestAction dbo_saveMobileDeviceEdited_PretestAction;
            Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition checkPreEditDeviceCreated;
            this.dbo_saveMobileDeviceSuccessData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_saveMobileDeviceDuplicateData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            this.dbo_saveMobileDeviceEditedData = new Microsoft.Data.Schema.UnitTesting.DatabaseTestActions();
            dbo_saveMobileDeviceDuplicate_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkForDuplicateReturnCode = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileDeviceEdited_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkUpdateSuccessful = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkDeviceNameEdited = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkDeviceTypeEdited = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkPairingKeyEdited = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkSerialKeyEdited = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkEditAuditLogEntryExists = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            testInitializeAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkRequestorEmployeeCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            testCleanupAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            dbo_saveMobileDeviceSuccess_TestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkMobileDeviceSaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkEmployeeSaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkDeviceTypeIdSaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkDeviceNameSaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkPairingKeySaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkSerialKeySaved = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkCreatedByCorrect = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            checkInsertAuditLogEntryExists = new Microsoft.Data.Schema.UnitTesting.Conditions.NotEmptyResultSetCondition();
            dbo_saveMobileDeviceDuplicate_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkInitialDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            dbo_saveMobileDeviceEdited_PretestAction = new Microsoft.Data.Schema.UnitTesting.DatabaseTestAction();
            checkPreEditDeviceCreated = new Microsoft.Data.Schema.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // dbo_saveMobileDeviceDuplicate_TestAction
            // 
            dbo_saveMobileDeviceDuplicate_TestAction.Conditions.Add(checkForDuplicateReturnCode);
            resources.ApplyResources(dbo_saveMobileDeviceDuplicate_TestAction, "dbo_saveMobileDeviceDuplicate_TestAction");
            // 
            // checkForDuplicateReturnCode
            // 
            checkForDuplicateReturnCode.ColumnNumber = 1;
            checkForDuplicateReturnCode.Enabled = true;
            checkForDuplicateReturnCode.ExpectedValue = "-1";
            checkForDuplicateReturnCode.Name = "checkForDuplicateReturnCode";
            checkForDuplicateReturnCode.NullExpected = false;
            checkForDuplicateReturnCode.ResultSet = 1;
            checkForDuplicateReturnCode.RowNumber = 1;
            // 
            // dbo_saveMobileDeviceEdited_TestAction
            // 
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkUpdateSuccessful);
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkDeviceNameEdited);
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkDeviceTypeEdited);
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkPairingKeyEdited);
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkSerialKeyEdited);
            dbo_saveMobileDeviceEdited_TestAction.Conditions.Add(checkEditAuditLogEntryExists);
            resources.ApplyResources(dbo_saveMobileDeviceEdited_TestAction, "dbo_saveMobileDeviceEdited_TestAction");
            // 
            // checkUpdateSuccessful
            // 
            checkUpdateSuccessful.ColumnNumber = 1;
            checkUpdateSuccessful.Enabled = true;
            checkUpdateSuccessful.ExpectedValue = "1";
            checkUpdateSuccessful.Name = "checkUpdateSuccessful";
            checkUpdateSuccessful.NullExpected = false;
            checkUpdateSuccessful.ResultSet = 1;
            checkUpdateSuccessful.RowNumber = 1;
            // 
            // checkDeviceNameEdited
            // 
            checkDeviceNameEdited.ColumnNumber = 1;
            checkDeviceNameEdited.Enabled = true;
            checkDeviceNameEdited.ExpectedValue = "UnitTestDevice_Edited";
            checkDeviceNameEdited.Name = "checkDeviceNameEdited";
            checkDeviceNameEdited.NullExpected = false;
            checkDeviceNameEdited.ResultSet = 2;
            checkDeviceNameEdited.RowNumber = 1;
            // 
            // checkDeviceTypeEdited
            // 
            checkDeviceTypeEdited.ColumnNumber = 2;
            checkDeviceTypeEdited.Enabled = true;
            checkDeviceTypeEdited.ExpectedValue = "3";
            checkDeviceTypeEdited.Name = "checkDeviceTypeEdited";
            checkDeviceTypeEdited.NullExpected = false;
            checkDeviceTypeEdited.ResultSet = 2;
            checkDeviceTypeEdited.RowNumber = 1;
            // 
            // checkPairingKeyEdited
            // 
            checkPairingKeyEdited.ColumnNumber = 3;
            checkPairingKeyEdited.Enabled = true;
            checkPairingKeyEdited.ExpectedValue = "00999-88888-111111";
            checkPairingKeyEdited.Name = "checkPairingKeyEdited";
            checkPairingKeyEdited.NullExpected = false;
            checkPairingKeyEdited.ResultSet = 2;
            checkPairingKeyEdited.RowNumber = 1;
            // 
            // checkSerialKeyEdited
            // 
            checkSerialKeyEdited.ColumnNumber = 4;
            checkSerialKeyEdited.Enabled = true;
            checkSerialKeyEdited.ExpectedValue = "be21d28605cf201291c4a2ad0ae93a5f0d141bbb";
            checkSerialKeyEdited.Name = "checkSerialKeyEdited";
            checkSerialKeyEdited.NullExpected = false;
            checkSerialKeyEdited.ResultSet = 2;
            checkSerialKeyEdited.RowNumber = 1;
            // 
            // checkEditAuditLogEntryExists
            // 
            checkEditAuditLogEntryExists.Enabled = true;
            checkEditAuditLogEntryExists.Name = "checkEditAuditLogEntryExists";
            checkEditAuditLogEntryExists.ResultSet = 3;
            // 
            // testInitializeAction
            // 
            testInitializeAction.Conditions.Add(checkEmployeeCreated);
            testInitializeAction.Conditions.Add(checkRequestorEmployeeCreated);
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
            // checkRequestorEmployeeCreated
            // 
            checkRequestorEmployeeCreated.ColumnNumber = 1;
            checkRequestorEmployeeCreated.Enabled = true;
            checkRequestorEmployeeCreated.ExpectedValue = "1";
            checkRequestorEmployeeCreated.Name = "checkRequestorEmployeeCreated";
            checkRequestorEmployeeCreated.NullExpected = false;
            checkRequestorEmployeeCreated.ResultSet = 2;
            checkRequestorEmployeeCreated.RowNumber = 1;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_saveMobileDeviceSuccess_TestAction
            // 
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkMobileDeviceSaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkEmployeeSaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkDeviceTypeIdSaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkDeviceNameSaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkPairingKeySaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkSerialKeySaved);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkCreatedByCorrect);
            dbo_saveMobileDeviceSuccess_TestAction.Conditions.Add(checkInsertAuditLogEntryExists);
            resources.ApplyResources(dbo_saveMobileDeviceSuccess_TestAction, "dbo_saveMobileDeviceSuccess_TestAction");
            // 
            // checkMobileDeviceSaved
            // 
            checkMobileDeviceSaved.ColumnNumber = 1;
            checkMobileDeviceSaved.Enabled = true;
            checkMobileDeviceSaved.ExpectedValue = "1";
            checkMobileDeviceSaved.Name = "checkMobileDeviceSaved";
            checkMobileDeviceSaved.NullExpected = false;
            checkMobileDeviceSaved.ResultSet = 1;
            checkMobileDeviceSaved.RowNumber = 1;
            // 
            // checkEmployeeSaved
            // 
            checkEmployeeSaved.ColumnNumber = 1;
            checkEmployeeSaved.Enabled = true;
            checkEmployeeSaved.ExpectedValue = "1";
            checkEmployeeSaved.Name = "checkEmployeeSaved";
            checkEmployeeSaved.NullExpected = false;
            checkEmployeeSaved.ResultSet = 2;
            checkEmployeeSaved.RowNumber = 1;
            // 
            // checkDeviceTypeIdSaved
            // 
            checkDeviceTypeIdSaved.ColumnNumber = 1;
            checkDeviceTypeIdSaved.Enabled = true;
            checkDeviceTypeIdSaved.ExpectedValue = "4";
            checkDeviceTypeIdSaved.Name = "checkDeviceTypeIdSaved";
            checkDeviceTypeIdSaved.NullExpected = false;
            checkDeviceTypeIdSaved.ResultSet = 3;
            checkDeviceTypeIdSaved.RowNumber = 1;
            // 
            // checkDeviceNameSaved
            // 
            checkDeviceNameSaved.ColumnNumber = 2;
            checkDeviceNameSaved.Enabled = true;
            checkDeviceNameSaved.ExpectedValue = "UnitTestDevice";
            checkDeviceNameSaved.Name = "checkDeviceNameSaved";
            checkDeviceNameSaved.NullExpected = false;
            checkDeviceNameSaved.ResultSet = 3;
            checkDeviceNameSaved.RowNumber = 1;
            // 
            // checkPairingKeySaved
            // 
            checkPairingKeySaved.ColumnNumber = 3;
            checkPairingKeySaved.Enabled = true;
            checkPairingKeySaved.ExpectedValue = "00999-88888-222222";
            checkPairingKeySaved.Name = "checkPairingKeySaved";
            checkPairingKeySaved.NullExpected = false;
            checkPairingKeySaved.ResultSet = 3;
            checkPairingKeySaved.RowNumber = 1;
            // 
            // checkSerialKeySaved
            // 
            checkSerialKeySaved.ColumnNumber = 4;
            checkSerialKeySaved.Enabled = true;
            checkSerialKeySaved.ExpectedValue = "be21d28605cf201291c4a2ad0ae93a5f0d141aaa";
            checkSerialKeySaved.Name = "checkSerialKeySaved";
            checkSerialKeySaved.NullExpected = false;
            checkSerialKeySaved.ResultSet = 3;
            checkSerialKeySaved.RowNumber = 1;
            // 
            // checkCreatedByCorrect
            // 
            checkCreatedByCorrect.ColumnNumber = 1;
            checkCreatedByCorrect.Enabled = true;
            checkCreatedByCorrect.ExpectedValue = "1";
            checkCreatedByCorrect.Name = "checkCreatedByCorrect";
            checkCreatedByCorrect.NullExpected = false;
            checkCreatedByCorrect.ResultSet = 4;
            checkCreatedByCorrect.RowNumber = 1;
            // 
            // checkInsertAuditLogEntryExists
            // 
            checkInsertAuditLogEntryExists.Enabled = true;
            checkInsertAuditLogEntryExists.Name = "checkInsertAuditLogEntryExists";
            checkInsertAuditLogEntryExists.ResultSet = 5;
            // 
            // dbo_saveMobileDeviceDuplicate_PretestAction
            // 
            dbo_saveMobileDeviceDuplicate_PretestAction.Conditions.Add(checkInitialDeviceCreated);
            resources.ApplyResources(dbo_saveMobileDeviceDuplicate_PretestAction, "dbo_saveMobileDeviceDuplicate_PretestAction");
            // 
            // checkInitialDeviceCreated
            // 
            checkInitialDeviceCreated.ColumnNumber = 1;
            checkInitialDeviceCreated.Enabled = true;
            checkInitialDeviceCreated.ExpectedValue = "1";
            checkInitialDeviceCreated.Name = "checkInitialDeviceCreated";
            checkInitialDeviceCreated.NullExpected = false;
            checkInitialDeviceCreated.ResultSet = 1;
            checkInitialDeviceCreated.RowNumber = 1;
            // 
            // dbo_saveMobileDeviceEdited_PretestAction
            // 
            dbo_saveMobileDeviceEdited_PretestAction.Conditions.Add(checkPreEditDeviceCreated);
            resources.ApplyResources(dbo_saveMobileDeviceEdited_PretestAction, "dbo_saveMobileDeviceEdited_PretestAction");
            // 
            // checkPreEditDeviceCreated
            // 
            checkPreEditDeviceCreated.ColumnNumber = 1;
            checkPreEditDeviceCreated.Enabled = true;
            checkPreEditDeviceCreated.ExpectedValue = "1";
            checkPreEditDeviceCreated.Name = "checkPreEditDeviceCreated";
            checkPreEditDeviceCreated.NullExpected = false;
            checkPreEditDeviceCreated.ResultSet = 1;
            checkPreEditDeviceCreated.RowNumber = 1;
            // 
            // dbo_saveMobileDeviceSuccessData
            // 
            this.dbo_saveMobileDeviceSuccessData.PosttestAction = null;
            this.dbo_saveMobileDeviceSuccessData.PretestAction = null;
            this.dbo_saveMobileDeviceSuccessData.TestAction = dbo_saveMobileDeviceSuccess_TestAction;
            // 
            // dbo_saveMobileDeviceDuplicateData
            // 
            this.dbo_saveMobileDeviceDuplicateData.PosttestAction = null;
            this.dbo_saveMobileDeviceDuplicateData.PretestAction = dbo_saveMobileDeviceDuplicate_PretestAction;
            this.dbo_saveMobileDeviceDuplicateData.TestAction = dbo_saveMobileDeviceDuplicate_TestAction;
            // 
            // dbo_saveMobileDeviceEditedData
            // 
            this.dbo_saveMobileDeviceEditedData.PosttestAction = null;
            this.dbo_saveMobileDeviceEditedData.PretestAction = dbo_saveMobileDeviceEdited_PretestAction;
            this.dbo_saveMobileDeviceEditedData.TestAction = dbo_saveMobileDeviceEdited_TestAction;
            // 
            // saveMobileDevice
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

        private DatabaseTestActions dbo_saveMobileDeviceSuccessData;
        private DatabaseTestActions dbo_saveMobileDeviceDuplicateData;
        private DatabaseTestActions dbo_saveMobileDeviceEditedData;
    }
}
