namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.BudgetHoldersUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    /// <summary>
    /// Summary description for BudgetHoldersUITest
    /// </summary>
    [CodedUITest]
    public class BudgetHoldersUITest
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static readonly SharedMethodsUIMap SharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// budget holders methods UI Map
        /// </summary>
        private static BudgetHoldersUIMap budgetHoldersMethods;

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<Employees> employees;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<BudgetHolders> budgetHolders;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        private static int adminId;

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            SharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            adminId = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
            //employees = EmployeesRepository.PopulateEmployee();
            budgetHolders = BudgetHoldersRepository.PopulateBudgetHolders();
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            SharedMethods.CloseBrowserWindow();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            budgetHoldersMethods = new BudgetHoldersUIMap();
            Guid genericName;
            foreach (var budgetHolder in budgetHolders)
            {
                genericName = Guid.NewGuid();
                budgetHolder.BudgetHolder = string.Format(
                    "{0}{1}", new object[] { "Custom BHold ", genericName });
                budgetHolder.EmployeeId = 0;
                budgetHolder.Employee.UserName = string.Format(
                    "{0}{1}", new object[] { "Custom Emp ", genericName });
                budgetHolder.EmployeeId = EmployeesRepository.CreateEmployee(budgetHolder.Employee, ExecutingProduct);
                Assert.IsTrue(budgetHolder.EmployeeId > 0);
            }
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var budgetHolder in budgetHolders)
            {
                if (budgetHolder.BudgetHolderId > 0)
                {
                    BudgetHoldersRepository.DeleteBudgetHolder(budgetHolder, ExecutingProduct);
                }
                if (budgetHolder.Employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(budgetHolder.Employee.employeeID, ExecutingProduct);
                }
            }
        }

        #region successfully create budget holder
        /// <summary>
        /// The budget holders successfully create budget holder_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersSuccessfullyCreateBudgetHolder_UITest()
        {
            var budgetHolderToAdd = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            budgetHoldersMethods.ClickNewBudgetHolderLink();

            // populate BudgetHolder details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.Employee.EmployeeFullName;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text = budgetHolderToAdd.Description;

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();

            // validate entry on grid
            esrAssignmentsMethods.ValidateGrid(this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolderGridValues);

            // click edit budget holder
            budgetHolderToAdd.BudgetHolderId = esrAssignmentsMethods.ReturnIdFromGrid(
                this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolder);
            esrAssignmentsMethods.ClickEditGridRow(this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolder);

            // validate all fields
            Assert.AreEqual(budgetHolderToAdd.BudgetHolder, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text);
            Assert.AreEqual(budgetHolderToAdd.Employee.EmployeeFullName, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text);
            Assert.AreEqual(budgetHolderToAdd.Description, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text);
        }
        #endregion

        #region successfully cancel add budget holder
        /// <summary>
        /// The budget holders successfully cancel add  budget holder_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersSuccessfullyCancelAddingBudgetHolder_UITest()
        {
            var budgetHolderToAdd = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            budgetHoldersMethods.ClickNewBudgetHolderLink();

            // populate BudgetHolder details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.BudgetHolder;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text = budgetHolderToAdd.Description;

            // press save button
            budgetHoldersMethods.PressCancelBudgetHolderButton();

            // validate entry on grid
            esrAssignmentsMethods.ValidateGrid(this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolderGridValues, false);
        }
        #endregion

        #region unsuccessfully add budget holder with mandatory fields blank
        /// <summary>
        /// The budget holders successfully cancel add  budget holder_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersUnsuccessfullyAddBudgetHolderWithMandatoryFieldsBlank_UITest()
        {
            var budgetHolderToAdd = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            budgetHoldersMethods.ClickNewBudgetHolderLink();

            // populate non mandatory details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text = budgetHolderToAdd.Description;

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();
            
            // validate modal messages
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText =
                string.Format(
                    "Message from {0}\r\n\r\n{1}{2}",
                    new object[]
                        {
                            EnumHelper.GetEnumDescription(ExecutingProduct), BudgetHolderModalMessages.EmptyFieldLabel,
                            BudgetHolderModalMessages.EmptyFieldForEmployeeResponsible
                        });
            employeesMethods.ValidateEmployeeModalMessage();
        }
        #endregion

        #region unsuccessfully add budget holder with invalid data
        /// <summary>
        /// The budget holders successfully cancel add  budget holder with invalid data_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersUnsuccessfullyAddBudgetHolderWithInvalidData_UITest()
        {
            var budgetHolderToAdd = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            budgetHoldersMethods.ClickNewBudgetHolderLink();

            // populate mandatory details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;

            // populate invalid details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.EmployeeId.ToString();

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText =
                           string.Format(
                               "Message from {0}\r\n\r\n{1}",
                               new object[]
                                   {
                                       EnumHelper.GetEnumDescription(ExecutingProduct), BudgetHolderModalMessages.InvalidFieldForEmployeeResponsible,
                                   });
            employeesMethods.ValidateEmployeeModalMessage();
        }
        #endregion

        #region unsuccessfully add duplicate budget holder
        /// <summary>
        /// The budget holders successfully cancel add  budget holder with invalid data_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersUnsuccessfullyAddDuplicateBudgetHolder_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var budgetHolderToAdd = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            budgetHoldersMethods.ClickNewBudgetHolderLink();

            // populate mandatory details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.Employee.EmployeeFullName;

            // populate duplicate details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();

            /*
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText =
                           string.Format(
                               "Message from {0}\r\n\r\n{1}{2}",
                               new object[]
                                   {
                                       EnumHelper.GetEnumDescription(ExecutingProduct), BudgetHolderModalMessages.DuplicateFieldForLabel,
                                       BudgetHolderModalMessages.EmptyFieldForEmployeeResponsible
                                   });
            employeesMethods.ValidateEmployeeModalMessage();
             */

            // press ok on browser window
            employeesMethods.PressOkOnBrowserValidation();

            // press cancel button
            budgetHoldersMethods.PressCancelBudgetHolderButton();
        }
        #endregion

        #region successfully edit budget holder
        /// <summary>
        /// The budget holders successfully edit budget holder_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersSuccessfullyEditBudgetHolder_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var budgetHolderToEdit = budgetHolders[0];
            var budgetHolderToAdd = budgetHolders[1];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            esrAssignmentsMethods.ClickEditGridRow(this.BudgetHolderGrid, budgetHolderToEdit.BudgetHolder);

            // populate BudgetHolder details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.Employee.EmployeeFullName;
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text = budgetHolderToAdd.Description;

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();

            // validate entry on grid
            esrAssignmentsMethods.ValidateGrid(this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolderGridValues);

            // click edit budget holder
            esrAssignmentsMethods.ClickEditGridRow(this.BudgetHolderGrid, budgetHolderToAdd.BudgetHolder);

            // validate all fields
            Assert.AreEqual(budgetHolderToAdd.BudgetHolder, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text);
            Assert.AreEqual(budgetHolderToAdd.Employee.EmployeeFullName, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text);
            Assert.AreEqual(budgetHolderToAdd.Description, budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.DescriptionTextBox.Text);
        }
        #endregion

        #region unsuccessfully edit duplicate budget holder
        /// <summary>
        /// The budget holders successfully cancel add  budget holder with invalid data_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersUnsuccessfullyEditDuplicateBudgetHolder_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var budgetHolderToEdit = budgetHolders[0];
            var budgetHolderToAdd = budgetHolders[1];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click add new budget holder link
            esrAssignmentsMethods.ClickEditGridRow(this.BudgetHolderGrid, budgetHolderToEdit.BudgetHolder);

            // populate mandatory details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.EmployeeResponsibleTextBox.Text = budgetHolderToAdd.Employee.EmployeeFullName;

            // populate duplicate details
            budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersControlsDocument.LabelTextBox.Text = budgetHolderToAdd.BudgetHolder;

            // press save button
            budgetHoldersMethods.PressSaveBudgetHolderButton();

            /*
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText =
                           string.Format(
                               "Message from {0}\r\n\r\n{1}{2}",
                               new object[]
                                   {
                                       EnumHelper.GetEnumDescription(ExecutingProduct), BudgetHolderModalMessages.DuplicateFieldForLabel,
                                       BudgetHolderModalMessages.EmptyFieldForEmployeeResponsible
                                   });
            employeesMethods.ValidateEmployeeModalMessage();
             */

            // press ok on browser window
            employeesMethods.PressOkOnBrowserValidation();

            // press cancel button
            budgetHoldersMethods.PressCancelBudgetHolderButton();
        }
        #endregion

        #region successfully delete budget holder
        /// <summary>
        /// The budget holders successfully delete budget holder_ ui tests.
        /// </summary>
        [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        public void BudgetHoldersSuccessfullyDeleteBudgetHolder_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var budgetHolderToDelete = budgetHolders[0];

            // Navigate to budget holders page
            SharedMethods.NavigateToPage(ExecutingProduct, BudgetHoldersUrlSuffixes.BudgetHolderAdministrationUrl);

            // Click delete budget holder link
            esrAssignmentsMethods.ClickDeleteGridRow(this.BudgetHolderGrid, budgetHolderToDelete.BudgetHolder);
            
            // confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            // validate grid row deleted
            esrAssignmentsMethods.ValidateGrid(this.BudgetHolderGrid, budgetHolderToDelete.BudgetHolderGridValues, false);
        }
        #endregion

        #region unsuccessfully delete employee when in use as budget holder
        /// <summary>
        /// The budget holders unsuccessfully delete budget holder when in use as budget holder_ ui tests.
        /// </summary>
        // [TestCategory("Budget Holder Details"), TestCategory("Budget Holders"), TestCategory("Expenses"), TestMethod]
        [TestMethod]
        public void BudgetHolderUnsuccessfullyDeleteEmployeeWhenInUseAsBudgetHolder_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var employeeToDelete = budgetHolders[0].Employee;

            // Navigate to search employee page
            SharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToDelete.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // Click delete employee link
            esrAssignmentsMethods.ClickArchiveGridRow(employeesMethods.EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid, employeeToDelete.UserName);
            esrAssignmentsMethods.ClickDeleteGridRow(employeesMethods.EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid, employeeToDelete.UserName);

            // confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText =
            string.Format(
                "Message from {0}\r\n\r\n{1}",
                new object[]
                        {
                            EnumHelper.GetEnumDescription(ExecutingProduct), EmployeeModalMessages.DeleteEmployeeWhenUsedAsBudgetHolder,
                        });
            employeesMethods.ValidateEmployeeModalMessage();

            // press close on modal message
            employeesMethods.PressCloseModalButton();

            // validate grid row deleted
            esrAssignmentsMethods.ValidateGrid(employeesMethods.EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid, employeeToDelete.EmployeeGridValues);

            // Navigate back to search employe page
            SharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);
        }
        #endregion

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        private void ImportDataToTestingDatabase(string testName)
        {
            int budgetHolderId = 0;
            switch (testName)
            {
                case "BudgetHoldersUnsuccessfullyAddDuplicateBudgetHolder_UITest":
                case "BudgetHoldersSuccessfullyEditBudgetHolder_UITest":
                case "BudgetHoldersSuccessfullyDeleteBudgetHolder_UITest":
                case "BudgetHolderUnsuccessfullyDeleteEmployeeWhenInUseAsBudgetHolder_UITest":
                    budgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(budgetHolders[0], ExecutingProduct);
                    Assert.IsTrue(budgetHolderId > 0);
                    break;
                case "BudgetHoldersUnsuccessfullyEditDuplicateBudgetHolder_UITest":
                    budgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(budgetHolders[0], ExecutingProduct);
                    Assert.IsTrue(budgetHolderId > 0);
                    budgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(budgetHolders[1], ExecutingProduct);
                    Assert.IsTrue(budgetHolderId > 0);
                    break;
                default:
                    budgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(budgetHolders[0], ExecutingProduct);
                    Assert.IsTrue(budgetHolderId > 0);

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

        /// <summary>
        /// Gets the budget holder grid.
        /// </summary>
        public HtmlTable BudgetHolderGrid
        {
            get
            {
                return budgetHoldersMethods.BudgetHoldersControlsWindow.BudgetHoldersGridDocument.BudgetHolderGrid;
            }
        }
    }
}
