namespace MetabaseDatabaseUnitTest
{
    using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class GetModuleFromHostUrl : SqlDatabaseTestClass
    {

        public GetModuleFromHostUrl()
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

        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction EmptyResultSetWithNullHost_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetModuleFromHostUrl));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.EmptyResultSetCondition emptyResultSetCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition ReturnResultZero1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction UniqueNamedHostReturnsModule7_TestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition1;
            this.EmptyResultSetWithNullHostData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            this.UniqueNamedHostReturnsModule7Data = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            EmptyResultSetWithNullHost_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            emptyResultSetCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.EmptyResultSetCondition();
            ReturnResultZero1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            UniqueNamedHostReturnsModule7_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            scalarValueCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // EmptyResultSetWithNullHost_TestAction
            // 
            EmptyResultSetWithNullHost_TestAction.Conditions.Add(emptyResultSetCondition1);
            EmptyResultSetWithNullHost_TestAction.Conditions.Add(ReturnResultZero1);
            resources.ApplyResources(EmptyResultSetWithNullHost_TestAction, "EmptyResultSetWithNullHost_TestAction");
            // 
            // emptyResultSetCondition1
            // 
            emptyResultSetCondition1.Enabled = true;
            emptyResultSetCondition1.Name = "emptyResultSetCondition1";
            emptyResultSetCondition1.ResultSet = 1;
            // 
            // EmptyResultSetWithNullHostData
            // 
            this.EmptyResultSetWithNullHostData.PosttestAction = null;
            this.EmptyResultSetWithNullHostData.PretestAction = null;
            this.EmptyResultSetWithNullHostData.TestAction = EmptyResultSetWithNullHost_TestAction;
            // 
            // ReturnResultZero1
            // 
            ReturnResultZero1.ColumnNumber = 1;
            ReturnResultZero1.Enabled = true;
            ReturnResultZero1.ExpectedValue = "0";
            ReturnResultZero1.Name = "ReturnResultZero1";
            ReturnResultZero1.NullExpected = false;
            ReturnResultZero1.ResultSet = 2;
            ReturnResultZero1.RowNumber = 1;
            // 
            // UniqueNamedHostReturnsModule7Data
            // 
            this.UniqueNamedHostReturnsModule7Data.PosttestAction = null;
            this.UniqueNamedHostReturnsModule7Data.PretestAction = null;
            this.UniqueNamedHostReturnsModule7Data.TestAction = UniqueNamedHostReturnsModule7_TestAction;
            // 
            // UniqueNamedHostReturnsModule7_TestAction
            // 
            UniqueNamedHostReturnsModule7_TestAction.Conditions.Add(rowCountCondition1);
            UniqueNamedHostReturnsModule7_TestAction.Conditions.Add(scalarValueCondition1);
            resources.ApplyResources(UniqueNamedHostReturnsModule7_TestAction, "UniqueNamedHostReturnsModule7_TestAction");
            // 
            // rowCountCondition1
            // 
            rowCountCondition1.Enabled = true;
            rowCountCondition1.Name = "rowCountCondition1";
            rowCountCondition1.ResultSet = 1;
            rowCountCondition1.RowCount = 1;
            // 
            // scalarValueCondition1
            // 
            scalarValueCondition1.ColumnNumber = 1;
            scalarValueCondition1.Enabled = true;
            scalarValueCondition1.ExpectedValue = "0";
            scalarValueCondition1.Name = "scalarValueCondition1";
            scalarValueCondition1.NullExpected = false;
            scalarValueCondition1.ResultSet = 2;
            scalarValueCondition1.RowNumber = 1;
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


        [TestMethod()]
        public void EmptyResultSetWithNullHost()
        {
            SqlDatabaseTestActions testActions = this.EmptyResultSetWithNullHostData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            // Execute the test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
            SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            // Execute the post-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
            SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
        }
        [TestMethod()]
        public void UniqueNamedHostReturnsModule7()
        {
            SqlDatabaseTestActions testActions = this.UniqueNamedHostReturnsModule7Data;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            // Execute the test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
            SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            // Execute the post-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
            SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
        }

        private SqlDatabaseTestActions EmptyResultSetWithNullHostData;
        private SqlDatabaseTestActions UniqueNamedHostReturnsModule7Data;
    }
}
