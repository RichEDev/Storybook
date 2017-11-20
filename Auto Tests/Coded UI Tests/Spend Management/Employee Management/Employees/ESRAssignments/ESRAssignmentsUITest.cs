namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.ESRAssignments
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.Product_Variables.ModalMessages;

    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class ESRAssignmentsUITest
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// assignments methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<Employees> employees;

        /// <summary>
        /// Cached list of cars
        /// </summary>
        private static List<ESRAssignments> esrAssignments;

        #region Additional test attributes

        /// <summary>
        /// The class init.
        /// </summary>
        /// <param name="ctx">
        /// The ctx.
        /// </param>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            _sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            employees = EmployeesRepository.PopulateEmployee();
            esrAssignments = ESRAssignmentsRepository.PopulateESRAssigtnment();
        }

        /// <summary>
        /// The class clean up.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Guid genericName = Guid.NewGuid();
            employees[0].UserName = string.Format(
                "{0}{1}", new object[] { "Custom Emp ", genericName });
            int empId = EmployeesRepository.CreateEmployee(employees[0], ExecutingProduct);
            Assert.IsTrue(empId > 0);
            employees[0].employeeAssignments = new List<ESRAssignments>();
            foreach (var assignment in esrAssignments)
            {
                assignment.EmployeeId = employees[0].employeeID;
                employees[0].employeeAssignments.Add(assignment);
            }

            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
        }

        /// <summary>
        /// The my test cleanup.
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var employee in employees)
            {
                if (employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(employee.employeeID, ExecutingProduct);
                }
            }
        }

        #endregion

        #region successfully add assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully add assignment to employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsSuccessfullyAddAssignmentToEmployee_UITest()
        {
            var employeeToEdit = employees[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");
            }

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            var gridRow = assignmentToAdd.EsrAssignmentGridValues;

            // validate assignments grid
            esrAssignmentsMethods.ValidateGrid(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, gridRow);

            // click edit assignment
            esrAssignmentsMethods.ClickEditGridRow(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, assignmentToAdd.AssignmentNumber);

            // validate assignment details
            Assert.AreEqual(assignmentToAdd.AssignmentNumber, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text);
            Assert.AreEqual(assignmentToAdd.Active, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked);
            Assert.AreEqual(assignmentToAdd.PrimaryAssignment, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked);
            Assert.AreEqual(assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString(), esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text);
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                Assert.AreEqual(assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString(), esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text);
            }
        }
        #endregion

        #region successfully cancel add assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully cancel adding assignment to employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsSuccessfullyCancelAddAssignmentToEmployee_UITest()
        {
            var employeeToEdit = employees[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");
            }

            // press cancel button
            esrAssignmentsMethods.PressCancelESRAssignmentButton();

            var gridRow = assignmentToAdd.EsrAssignmentGridValues;

            // validate trust isnt on grid
            esrAssignmentsMethods.ValidateGrid(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, gridRow, false);
        }

        #endregion

        #region unsuccessfully add assignment to employee with mandatory fields blank
        /// <summary>
        /// The esr assignmrnts unsuccessfully add assignment to employee with mandatory fields blank_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsUnsuccessfullyAddAssignmentToEmployeeWithMandatoryFieldsBlank_UITest()
        {
            var employeeToEdit = employees[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}{2}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), EmployeeModalMessages.EmptyFieldForAssignmentNumber, EmployeeModalMessages.EmptyFieldForEarliestAssignmentStartDate });
            //employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);
        }
        #endregion

        #region unsuccessfully add assignment to employee with invalid data
        /// <summary>
        /// The esr assignmrnts unsuccessfully add assignment to employee with invalid data_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsUnsuccessfullyAddAssignmentToEmployeeWithInvalidData_UITest()
        {
            var employeeToEdit = employees[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");
            }
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), EmployeeModalMessages.InvalidFieldForEndDateEarlierThanStartDate });
            //employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);
        }
        #endregion

        #region unsuccessfully add duplicate assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully add duplicate assignment to employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsUnsuccessfullyAddDuplicateAssignmentToEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var employeeToEdit = employees[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), EmployeeModalMessages.DuplicateFieldForAssignmentNumber });
            //employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);
        }
        #endregion

        #region successfully edit assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully edit assignment to employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsSuccessfullyEditAssignmentToEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var employeeToEdit = employees[0];
            var assignmentToEdit = employeeToEdit.employeeAssignments[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[1];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickEditGridRow(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, assignmentToEdit.AssignmentNumber);

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");
            }

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            // validate assignments grid
            var gridRow = assignmentToAdd.EsrAssignmentGridValues;

            // validate assignments grid
            esrAssignmentsMethods.ValidateGrid(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, gridRow);

            // click edit assignment
            esrAssignmentsMethods.ClickEditGridRow(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, assignmentToAdd.AssignmentNumber);

            // validate assignment details
            Assert.AreEqual(assignmentToAdd.AssignmentNumber, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text);
            Assert.AreEqual(assignmentToAdd.Active, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked);
            Assert.AreEqual(assignmentToAdd.PrimaryAssignment, esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked);
            Assert.AreEqual(assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString(), esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text);
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                Assert.AreEqual(assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString(), esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text);
            }
        }
        #endregion

        #region unsuccessfully edit duplicate assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully add duplicate assignment to employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsUnsuccessfullyEditDuplicateAssignmentToEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var employeeToEdit = employees[0];
            var assignmentToEdit = employeeToEdit.employeeAssignments[0];
            var assignmentToAdd = employeeToEdit.employeeAssignments[1];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click new assignment
            esrAssignmentsMethods.ClickNewEsrAssignmentLink();

            // populate assignment details
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.AssignmentNumberTextBox.Text = assignmentToAdd.AssignmentNumber;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ActiveCheckBox.Checked = assignmentToAdd.Active;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.PrimaryAssignmentCheckBox.Checked = assignmentToAdd.PrimaryAssignment;
            esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.EarliestAssignmentStartDateTextBox.Text = assignmentToAdd.EarliestAssignmentStartDate.ToShortDateString().Replace("/", "0");
            if (assignmentToAdd.FinalAssignmentEndDate != null)
            {
                esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.FinalAssignmentEndDateTextBox.Text = assignmentToAdd.FinalAssignmentEndDate.Value.ToShortDateString().Replace("/", "0");
            }

            // press save button
            esrAssignmentsMethods.PressSaveESRAssignmentButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), EmployeeModalMessages.DuplicateFieldForAssignmentNumber });
            //employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);
        }
        #endregion

        #region successfully delete assignment to employee
        /// <summary>
        /// The esr assignmrnts successfully delete assignment from employee_ ui test.
        /// </summary>
        [TestCategory("ESRAssignments"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void ESRAssignmentsSuccessfullyDeleteAssignmentFromEmployee_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            var employeeToEdit = employees[0];
            var assignmentToDelete = employeeToEdit.employeeAssignments[0];

            // navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // enter work tab
            employeesMethods.ClickWorkTab();

            // click delete assignment
            esrAssignmentsMethods.ClickDeleteGridRow(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, assignmentToDelete.AssignmentNumber);

            // confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            var gridRow = assignmentToDelete.EsrAssignmentGridValues;

            // validate grid
            esrAssignmentsMethods.ValidateGrid(esrAssignmentsMethods.ESRAssignmentsControlsWindow.ESRAssignmentsControlsDocument.ESRAssignmentGrid, gridRow, false);

        }
        #endregion

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        internal void ImportDataToTestingDatabase(string testName)
        {
            int trustId;
            switch (testName)
            {
                case "ESRAssignmentsSuccessfullyEditAssignmentToEmployee_UITest":
                case "ESRAssignmentsUnsuccessfullyAddDuplicateAssignmentToEmployee_UITest":
                case "ESRAssignmentsSuccessfullyDeleteAssignmentFromEmployee_UITest":
                    trustId = ESRAssignmentsRepository.CreateEsrAssignment(esrAssignments[0], ExecutingProduct);
                    Assert.IsTrue(trustId > 0);
                    break;
                case "ESRAssignmentsUnsuccessfullyEditDuplicateAssignmentToEmployee_UITest":
                    trustId = ESRAssignmentsRepository.CreateEsrAssignment(esrAssignments[0], ExecutingProduct);
                    Assert.IsTrue(trustId > 0);
                    trustId = ESRAssignmentsRepository.CreateEsrAssignment(esrAssignments[1], ExecutingProduct);
                    Assert.IsTrue(trustId > 0);
                    break;
                default:
                    trustId = ESRAssignmentsRepository.CreateEsrAssignment(esrAssignments[0], ExecutingProduct);
                    Assert.IsTrue(trustId > 0);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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
        private TestContext testContextInstance;
    }
}
