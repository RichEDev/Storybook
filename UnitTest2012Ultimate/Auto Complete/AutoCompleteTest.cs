namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary.Employees;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// This is a test class for AutoCompleteTest and is intended
    /// to contain all AutoCompleteTest Unit Tests
    /// </summary>
    [TestClass]
    public class AutoCompleteTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }

        #endregion

        #region GetAutoCompleteMatches

        #region maxRows tests

        /// <summary>
        /// Test that the correct number of rows are returned when maxRows is set
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_MaxRowsSet()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 1, matchTable, displayField, matchFields, MatchText, true, null);
            Assert.AreEqual(1, actual.Count);

            actual = AutoComplete.GetAutoCompleteMatches(currentUser, 9, matchTable, displayField, matchFields, MatchText, true, null);
            Assert.AreEqual(9, actual.Count);
        }

        /// <summary>
        /// Test that rows are still returned when maxRows is not set
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_MaxRowsNotSet()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 0, matchTable, displayField, matchFields, MatchText, true, null);
            Assert.IsTrue(actual.Count > 0);
        }

        #endregion maxRows tests

        #region matchTable tests

        /// <summary>
        /// Test that no results are returned when using an invalid Guid for matchTable
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_IncorrectMatchTableGuid()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            const string MatchTable = "69-69-69";
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, MatchTable, displayField, matchFields, MatchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test that no results are returned when using a matchTable that does not exist
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_MatchTableDoesNotExist()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            const string MatchTable = "00000000-0000-0000-0000-000000000069";
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, MatchTable, displayField, matchFields, MatchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region displayField tests

        /// <summary>
        /// Check that a non-int primary key will not return rows
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_BaseTableWithNonIntPrimaryKey()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable reportsTable = new cTables().GetTableByName("reports");

            string matchTable = reportsTable.TableID.ToString();
            string displayField = reportsTable.KeyFieldID.ToString();
            string matchFields = reportsTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Check that a non-string display field will not be returned
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_NonStringDisplayField()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");
            cField employeeArchived = new cFields().GetBy(employeesTable.TableID, "archived");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeeArchived.FieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, matchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test that no results are returned when using an incorrect display field guid
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_IncorrectDisplayFieldGuid()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            const string DisplayField = "69-69-69-69";
            string matchFields = employeesTable.KeyFieldID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, DisplayField, matchFields, matchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test that no results are returned when using a display field that does not exist
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_FieldGuidDoesNotExist()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            const string DisplayField = "00000000-0000-0000-000000000069";
            string matchFields = employeesTable.KeyFieldID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, DisplayField, matchFields, matchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region matchFields tests

        /// <summary>
        /// Test that no results are returned when there are no match fields specified
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_NoMatchFields()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = string.Empty;
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 1, matchTable, displayField, matchFields, MatchText, true, null);
            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test that results are correctly returned when multiple match fields are specified
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_MultipleMatchFields()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cEmployees employees = new cEmployees(Moqs.CurrentUser().AccountID);
            Employee reqEmployee = employees.GetEmployeeById(Moqs.CurrentUser().EmployeeID);

            cTable employeesTable = new cTables().GetTableByName("employees");
            Guid employeeFirstNameID = new cFields().GetBy(employeesTable.TableID, "firstname").FieldID;
            Guid employeeSurnameID = new cFields().GetBy(employeesTable.TableID, "surname").FieldID;

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID + "," + employeeFirstNameID + "," + employeeSurnameID;

            string matchText = reqEmployee.Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 6, matchTable, displayField, matchFields, matchText, true, null);
            Assert.IsTrue(actual.Count > 0);

            matchText = reqEmployee.Forename;

            actual = AutoComplete.GetAutoCompleteMatches(currentUser, 6, matchTable, displayField, matchFields, matchText, true, null);
            Assert.IsTrue(actual.Count > 0);

            matchText = reqEmployee.Surname;

            actual = AutoComplete.GetAutoCompleteMatches(currentUser, 6, matchTable, displayField, matchFields, matchText, true, null);
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        /// Test that results are returned if some of the match fields specified are invalid
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_MultipleMatchFieldsWithInvalidFieldID()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cTable employeesTable = new cTables().GetTableByName("employees");
            Guid employeeSurnameID = new cFields().GetBy(employeesTable.TableID, "surname").FieldID;

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID + ",69-69-69," + employeeSurnameID + ",9696-9696-9696";

            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 6, matchTable, displayField, matchFields, MatchText, true, null);
            Assert.IsTrue(actual.Count > 0);
        }

        #endregion

        #region matchText tests

        /// <summary>
        /// Test that results are returned when no matchText is set
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_NoMatchTextSet()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            string matchText = string.Empty;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, 6, matchTable, displayField, matchFields, matchText, true, null);
            Assert.IsTrue(actual.Count > 0);
        }

        #endregion

        #region useWildcards tests

        /// <summary>
        /// Test that the correct number of rows are returned when wildcards are not used
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_WithoutWildcards()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            sAutoCompleteResult expected = new sAutoCompleteResult
            {
                label = matchText,
                value = GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture)
            };

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, matchText, false, null);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        /// <summary>
        /// Test that no results are returned when wildcards are not used and results do not exist
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_WithoutWildcardsWhereNoResultsExist()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.PrimaryKeyID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, matchText, false, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Test that wildcard results are correctly returned when wildcards are used
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_WithWildcards()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            string employeeUserName = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;
            string matchText = employeeUserName;

            matchText = matchText.Length > 2 ? matchText.Substring(0, 3) : matchText.PadRight(3, '%');

            sAutoCompleteResult expected = new sAutoCompleteResult
            {
                label = employeeUserName,
                value = GlobalTestVariables.EmployeeId.ToString(CultureInfo.InvariantCulture)
            };

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, matchText, true, null);

            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Contains(expected));
        }

        /// <summary>
        /// Test that no results are returned when wildcards are used and results do not exist
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("AutoComplete"), TestMethod]
        public void AutoComplete_GetAutoCompleteMatches_WithWildcardsWhereNoResultsExist()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            const int MaxRows = 6;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.PrimaryKeyID.ToString();
            string matchText = new cEmployees(currentUser.AccountID).GetEmployeeById(currentUser.EmployeeID).Username;

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, matchText, true, null);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region filters tests

        /// <summary>
        /// Test an empty filters list does not break things
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_NotNullButEmptyFiltersList()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            const int MaxRows = 100;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            Dictionary<string, JSFieldFilter> javaScriptFieldFilters = new Dictionary<string, JSFieldFilter>();

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, javaScriptFieldFilters);

            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Count <= 100);
        }

        /// <summary>
        /// Test a filters list with an incorrect filter does not break things
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_IncorrectFiltersList()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            const int MaxRows = 100;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            Dictionary<string, JSFieldFilter> javaScriptFieldFilters = new Dictionary<string, JSFieldFilter> { { "jghkjh", new JSFieldFilter { FieldID = Guid.Empty, ConditionType = ConditionType.Today, ValueOne = string.Empty, ValueTwo = null, Order = 1, JoinViaID = 0 } } };

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, javaScriptFieldFilters);

            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Count <= 100);
        }

        /// <summary>
        /// Test a filters list with an incorrect filter does not break things
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        [ExpectedException(typeof(NullReferenceException))]
        public void AutoComplete_GetAutoCompleteMatches_FiltersListWithNullJSFieldFilter()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            const int MaxRows = 100;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            Dictionary<string, JSFieldFilter> javaScriptFieldFilters = new Dictionary<string, JSFieldFilter> { { "jghkjh", null } };

            AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, javaScriptFieldFilters);
        }

        /// <summary>
        /// Test a filters list with an incorrect filter does not break things
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("AutoComplete")]
        public void AutoComplete_GetAutoCompleteMatches_SingleFilterList()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            const int MaxRows = 100;

            cTable employeesTable = new cTables().GetTableByName("employees");

            string matchTable = employeesTable.TableID.ToString();
            string displayField = employeesTable.KeyFieldID.ToString();
            string matchFields = employeesTable.KeyFieldID.ToString();
            const string MatchText = "%%%";

            List<sAutoCompleteResult> actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, null);

            Assert.IsTrue(actual.Count > 0);
            Assert.IsTrue(actual.Count <= 100);

            int initialRows = actual.Count;
            cField surname = new cFields().GetBy(employeesTable.TableID, "surname");
            Dictionary<string, JSFieldFilter> javaScriptFieldFilters = new Dictionary<string, JSFieldFilter> { { "0", new JSFieldFilter { FieldID = surname.FieldID, ConditionType = ConditionType.Like, ValueOne = "admin", ValueTwo = null, Order = 0, JoinViaID = 0 } } };

            actual = AutoComplete.GetAutoCompleteMatches(currentUser, MaxRows, matchTable, displayField, matchFields, MatchText, true, javaScriptFieldFilters);

            Assert.AreNotEqual(actual.Count, initialRows);
            Assert.IsTrue(actual.Count < initialRows);

            foreach (sAutoCompleteResult r in actual)
            {
                Assert.IsTrue(r.label.ToLower().Contains("admin"));
            }
        }

        #endregion

        #endregion GetAutoCompleteMatches
    }
}
