namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Approval_Matrices
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using System.Windows.Forms;
    using System.Drawing;

    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams;

    using Auto_Tests.UIMaps.ApprovalMatricesUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.UIMaps.TeamsUIMapClasses;

    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;

    /// <summary>
    /// Summary description for ApprovalMatricesUITests
    /// </summary>
    [CodedUITest]
    public class ApprovalMatricesUITests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// assignments methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// The teams methods.
        /// </summary>
        private static ApprovalMatricesUIMap approvalMatricesMethods;

        /// <summary>
        /// Cached list of Teams
        /// </summary>
        private static List<ApprovalMatrices> approvalMatrices;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        private static int adminId;

        #region Additional class/test attributes
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            approvalMatrices = ApprovalMatricesRepository.PopulateApprovalMatrices();
            adminId = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
        }

        // Use TestCleanup to run code after each test has run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            sharedMethods.CloseBrowserWindow();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            approvalMatricesMethods = new ApprovalMatricesUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            foreach (var matrix in approvalMatrices)
            {
                if (matrix.DefaultApproverEmployeeId.HasValue)
                {
                    CreateTestUsers(ApproverType.Employee, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                if (matrix.DefaultApproverBudgetHolderId.HasValue)
                {
                    CreateTestUsers(ApproverType.BudgetHolder, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                if (matrix.DefaultApproverTeamId.HasValue)
                {
                    CreateTestUsers(ApproverType.Team, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                foreach (var matrixLevel in matrix.ApprovalMatrixLevels)
                {
                    if (matrixLevel.ApproverEmployeeId.HasValue)
                    {
                        CreateTestUsers(ApproverType.Employee, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }

                    if (matrixLevel.ApproverBudgetHolderId.HasValue)
                    {
                        CreateTestUsers(ApproverType.BudgetHolder, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }

                    if (matrixLevel.ApproverTeamId.HasValue)
                    {
                        CreateTestUsers(ApproverType.Team, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }
                }
            }
        }

        internal static void DeleteTestUsers(ApproverType approver, Employees employee, BudgetHolders bHolder, Teams team)
        {
            Random genericName = new Random();

            if (approver == ApproverType.Employee)
            {
                EmployeesRepository.DeleteEmployee(employee.employeeID, ExecutingProduct);
            }

            if (approver == ApproverType.BudgetHolder)
            {
                BudgetHoldersRepository.DeleteBudgetHolder(bHolder, ExecutingProduct);

                EmployeesRepository.DeleteEmployee(bHolder.Employee.employeeID, ExecutingProduct);
            }

            if (approver == ApproverType.Team)
            {
                TeamsRepository.DeleteTeam(team.TeamId, ExecutingProduct);
                foreach (var teamMember in team.TeamMembers)
                {
                    EmployeesRepository.DeleteEmployee(teamMember.Employee.employeeID, ExecutingProduct);
                }
            }
        }

        internal static void CreateTestUsers(ApproverType approver, Employees employee, BudgetHolders bHolder, Teams team)
        {
            //Random genericName = new Random();
            Guid genericName;
            if (approver == ApproverType.Employee)
            {
                genericName = Guid.NewGuid();
                employee.employeeID = 0;
                employee.UserName = string.Format(
                    "{0}{1}", new object[] { "Custom Emp ", genericName });
                employee.employeeID = EmployeesRepository.CreateEmployee(employee, ExecutingProduct);
                Assert.IsTrue(employee.employeeID > 0);
            }

            if (approver == ApproverType.BudgetHolder)
            {
                genericName = Guid.NewGuid();
                bHolder.Employee.employeeID = 0;
                bHolder.Employee.UserName = string.Format(
                    "{0}{1}", new object[] { "Custom Emp ", genericName });
                bHolder.Employee.employeeID = EmployeesRepository.CreateEmployee(bHolder.Employee, ExecutingProduct);
                Assert.IsTrue(bHolder.Employee.employeeID > 0);

                bHolder.BudgetHolderId = 0;
                bHolder.BudgetHolder = string.Format(
                    "{0}{1}", new object[] { "Custom BHold ", genericName });
                bHolder.BudgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(bHolder, ExecutingProduct);
                Assert.IsTrue(bHolder.BudgetHolderId > 0);
            }
            if (approver == ApproverType.Team)
            {
                foreach (var teamMember in team.TeamMembers)
                {
                    genericName = Guid.NewGuid();
                    teamMember.Employee.employeeID = 0;
                    teamMember.Employee.UserName = string.Format(
                        "{0}{1}", new object[] { "Custom Emp ", genericName });
                    teamMember.Employee.employeeID = EmployeesRepository.CreateEmployee(teamMember.Employee, ExecutingProduct);
                    Assert.IsTrue(teamMember.Employee.employeeID > 0);
                }
                genericName = Guid.NewGuid();
                team.TeamId = 0;
                team.TeamName = string.Format(
                        "{0}{1}", new object[] { "Custom Team ", genericName });
                team.TeamId = TeamsRepository.CreateTeam(team, ExecutingProduct);
                Assert.IsTrue(team.TeamId > 0);
            }
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var matrix in approvalMatrices)
            {
                if (matrix.ApprovalMatrixId > 0)
                {
                    ApprovalMatricesRepository.DeleteApprovalMatrix(matrix.ApprovalMatrixId, ExecutingProduct, adminId);
                }
            }

            foreach (var matrix in approvalMatrices)
            {
                if (matrix.DefaultApproverEmployeeId.HasValue)
                {
                    DeleteTestUsers(ApproverType.Employee, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                if (matrix.DefaultApproverBudgetHolderId.HasValue)
                {
                    DeleteTestUsers(ApproverType.BudgetHolder, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                if (matrix.DefaultApproverTeamId.HasValue)
                {
                    DeleteTestUsers(ApproverType.Team, matrix.DefaultEmployee, matrix.DefaultHolder, matrix.DefaultTeam);
                }

                foreach (var matrixLevel in matrix.ApprovalMatrixLevels)
                {
                    if (matrixLevel.ApproverEmployeeId.HasValue)
                    {
                        DeleteTestUsers(ApproverType.Employee, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }

                    if (matrixLevel.ApproverBudgetHolderId.HasValue)
                    {
                        DeleteTestUsers(ApproverType.BudgetHolder, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }

                    if (matrixLevel.ApproverTeamId.HasValue)
                    {
                        DeleteTestUsers(ApproverType.Team, matrixLevel.LevelEmployee, matrixLevel.LevelHolder, matrixLevel.LevelTeam);
                    }
                }
            }
        }
        #endregion
        
        /// <summary>
        /// ApprovalMatricesSuccesfullyAddNewApprovalMatrix_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesSuccesfullyAddNewApprovalMatrix_UITest()
        {
            var matrixToAdd = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            approvalMatricesMethods.ClickNewApprovalMatrixLink();

            // populate matrix details
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = matrixToAdd.DefaultEmployee.ApproverFullName;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = matrixToAdd.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = matrixToAdd.Description;

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // populate level details
            foreach (var matrixLevel in matrixToAdd.ApprovalMatrixLevels)
            {
                // click new level
                approvalMatricesMethods.ClickNewLevelsLink();

                if (matrixLevel.ApproverEmployeeId.HasValue)
                {
                    approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelEmployee.ApproverFullName;
                }
                else if (matrixLevel.ApproverBudgetHolderId.HasValue)
                {
                    approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelHolder.ApproverName;
                }
                else
                {
                    approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelTeam.ApproverName;
                }

                // populate details
                approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text = decimal.Round(matrixLevel.ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString();

                // press save
                approvalMatricesMethods.PressSaveLevelButton();
            }

            // press save
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate matrices grid
            esrAssignmentsMethods.ValidateGrid(MatricesGrid, matrixToAdd.MatricesGridValues);

            // click edit matrix
            matrixToAdd.ApprovalMatrixId = esrAssignmentsMethods.ReturnIdFromGrid(MatricesGrid, matrixToAdd.Name);
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToAdd.Name);
            
             Assert.AreEqual(matrixToAdd.DefaultEmployee.ApproverFullName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text);
             Assert.AreEqual(matrixToAdd.Name, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text);
             Assert.AreEqual(matrixToAdd.Description, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text);

            // press levels link
            approvalMatricesMethods.ClickLevelsLink();

            // validate levels grid
            foreach (var matrixLevel in matrixToAdd.ApprovalMatrixLevels)
            {
                esrAssignmentsMethods.ValidateGrid(MatrixLevelGrid, matrixLevel.MatrixLevelGridValues);

                // validate matrix level details
                esrAssignmentsMethods.ClickEditGridRow(MatrixLevelGrid, matrixLevel.GridThreshHoldAmount);

                // validate each field
                Assert.AreEqual(decimal.Round(matrixLevel.ThresholdAmount, 0, MidpointRounding.AwayFromZero).ToString(), approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text);
                if (matrixLevel.ApproverEmployeeId.HasValue)
                {
                    Assert.AreEqual(matrixLevel.LevelEmployee.ApproverFullName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }
                else if (matrixLevel.ApproverBudgetHolderId.HasValue)
                {
                    Assert.AreEqual(matrixLevel.LevelHolder.ApproverName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }
                else
                {
                    Assert.AreEqual(matrixLevel.LevelTeam.ApproverName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }

                // close matrix level
                approvalMatricesMethods.PressCancelSaveLevelButton();
            }
        }

        /// <summary>
        /// ApprovalMatricesSuccesfullyAddNewApprovalMatrix_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesSuccesfullyCancelAddNewApprovalMatrix_UITest()
        {
            var matrixToAdd = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            approvalMatricesMethods.ClickNewApprovalMatrixLink();

            // populate matrix details
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = matrixToAdd.DefaultEmployee.ApproverFullName;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = matrixToAdd.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = matrixToAdd.Description;

            // press cancel
            approvalMatricesMethods.PressCancelSaveApprovalMatrix();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(MatricesGrid, matrixToAdd.MatricesGridValues, false);
        }

        /// <summary>
        /// ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithMandatoryFieldsBlank_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithMandatoryFieldsBlank_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var matrixToEdit = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            approvalMatricesMethods.ClickNewApprovalMatrixLink();

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}{2}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.EmptyFieldForApprovalMatrixName, ApprovalMatricesModalMessages.EmptyFieldForDefaultApprover });
            approvalMatricesMethods.ValidateModalMessage();

            // press close on modal message
            approvalMatricesMethods.PressCloseOnModalMessage();

            // press save button
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessage();

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // press edit approval Matrix
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToEdit.Name);

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // click new level link
            approvalMatricesMethods.ClickNewLevelsLink();

            //press save level
            approvalMatricesMethods.PressSaveLevelButton();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}{2}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.EmptyFiedlForApprovalLimit, ApprovalMatricesModalMessages.EmptyFieldForApprover });
            approvalMatricesMethods.ValidateModalMessage();
        }

        /// <summary>
        /// ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithInvalidField_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithInvalidFields_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var matrixToEdit = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            approvalMatricesMethods.ClickNewApprovalMatrixLink();

            // populate invalid details where possible
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = DateTime.Now.ToShortDateString();
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = matrixToEdit.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = matrixToEdit.Description;

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.InvalidFieldForDefaultApprover });
            approvalMatricesMethods.ValidateModalMessage();

            // press close on modal message
            approvalMatricesMethods.PressCloseOnModalMessage();

            // press save button
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessage();

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // press edit approval Matrix
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToEdit.Name);

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // click new level link
            approvalMatricesMethods.ClickNewLevelsLink();

            // populate invalid details where possible
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text = Decimal.Round(matrixToEdit.ApprovalMatrixLevels[0].ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString();
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text =  DateTime.Now.ToShortDateString();;

            //press save level
            approvalMatricesMethods.PressSaveLevelButton();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.InvalidFieldForApprover });
            approvalMatricesMethods.ValidateModalMessage();
        }

        /// <summary>
        /// ApprovalMatricesUnsuccessfullyAddApprovalMatrixWithDuplicateFields_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesUnsuccessfullyAddApprovalMatrixWithDuplicateFields_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var duplicateMatrixToAdd = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            approvalMatricesMethods.ClickNewApprovalMatrixLink();

            // populate matrix details
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = duplicateMatrixToAdd.DefaultEmployee.ApproverFullName;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = duplicateMatrixToAdd.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = duplicateMatrixToAdd.Description;

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.DuplicateFieldForApprovalMatrixName });
            approvalMatricesMethods.ValidateModalMessage();

            // press close on modal message
            approvalMatricesMethods.PressCloseOnModalMessage();

            // press save button
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessage();
        }

        /// <summary>
        /// ApprovalMatricesSuccessfullyEditApprovalMatrix_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesSuccessfullyEditApprovalMatrix_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var matrixToAdd = approvalMatrices[1];
            var matrixToEdit = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click edit matrix to edit
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToEdit.Name);

            // populate matrix details
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = matrixToAdd.DefaultEmployee.ApproverFullName;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = matrixToAdd.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = matrixToAdd.Description;

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            for (int level = 0; level < matrixToEdit.ApprovalMatrixLevels.Count; level++)
            {
                if (matrixToEdit.ApprovalMatrixLevels.Count == 0)
                {
                    // add matrix levels
                    // populate level details
                    foreach (var matrixLevel in matrixToAdd.ApprovalMatrixLevels)
                    {
                        // click new level
                        approvalMatricesMethods.ClickNewLevelsLink();

                        if (matrixLevel.ApproverEmployeeId.HasValue)
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelEmployee.ApproverFullName;
                        }
                        else if (matrixLevel.ApproverBudgetHolderId.HasValue)
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelHolder.ApproverName;
                        }
                        else
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixLevel.LevelTeam.ApproverName;
                        }

                        // populate details
                        approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text = decimal.Round(matrixLevel.ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString();

                        // press save
                        approvalMatricesMethods.PressSaveLevelButton();
                    }
                }
                else
                {
                    if (level < matrixToAdd.ApprovalMatrixLevels.Count)
                    {
                        // edit matrix levels
                        // click edit level
                        esrAssignmentsMethods.ClickEditGridRow(MatrixLevelGrid, matrixToEdit.ApprovalMatrixLevels[level].GridThreshHoldAmount);

                        if (matrixToAdd.ApprovalMatrixLevels[level].ApproverEmployeeId.HasValue)
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixToAdd.ApprovalMatrixLevels[level].LevelEmployee.ApproverFullName;
                        }
                        else if (matrixToAdd.ApprovalMatrixLevels[level].ApproverBudgetHolderId.HasValue)
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixToAdd.ApprovalMatrixLevels[level].LevelHolder.ApproverName;
                        }
                        else
                        {
                            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = matrixToAdd.ApprovalMatrixLevels[level].LevelTeam.ApproverName;
                        }

                        // populate details
                        approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text = decimal.Round(matrixToAdd.ApprovalMatrixLevels[level].ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString();

                        // press save
                        approvalMatricesMethods.PressSaveLevelButton();
                    }
                    else
                    {
                        // delete matrix levels
                        esrAssignmentsMethods.ClickDeleteGridRow(MatrixLevelGrid, matrixToEdit.ApprovalMatrixLevels[level].GridThreshHoldAmount);

                        // confirm delete
                        approvalMatricesMethods.PressOKOnBrowserValidation();
                    }
                }
            }

            // press save
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate matrices grid
            esrAssignmentsMethods.ValidateGrid(MatricesGrid, matrixToAdd.MatricesGridValues);

            // click edit matrix
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToAdd.Name);

            Assert.AreEqual(matrixToAdd.DefaultEmployee.ApproverFullName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text);
            Assert.AreEqual(matrixToAdd.Name, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text);
            Assert.AreEqual(matrixToAdd.Description, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text);

            // press levels link
            approvalMatricesMethods.ClickLevelsLink();

            // validate levels grid
            foreach (var matrixLevel in matrixToAdd.ApprovalMatrixLevels)
            {
                esrAssignmentsMethods.ValidateGrid(MatrixLevelGrid, matrixLevel.MatrixLevelGridValues);

                // validate matrix level details
                esrAssignmentsMethods.ClickEditGridRow(MatrixLevelGrid, matrixLevel.GridThreshHoldAmount);

                // validate each field
                Assert.AreEqual(decimal.Round(matrixLevel.ThresholdAmount, 0, MidpointRounding.AwayFromZero).ToString(), approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text);
                if (matrixLevel.ApproverEmployeeId.HasValue)
                {
                    Assert.AreEqual(matrixLevel.LevelEmployee.ApproverFullName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }
                else if (matrixLevel.ApproverBudgetHolderId.HasValue)
                {
                    Assert.AreEqual(matrixLevel.LevelHolder.ApproverName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }
                else
                {
                    Assert.AreEqual(matrixLevel.LevelTeam.ApproverName, approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text);
                }

                // close matrix level
                approvalMatricesMethods.PressCancelSaveLevelButton();
            }
        }

        /// <summary>
        /// ApprovalMatricesUnsuccessfullyEditApprovalMatrixWithDuplicateFields_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesUnsuccessfullyEditApprovalMatrixWithDuplicateFields_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var approvalMatrixToEdit = approvalMatrices[0];
            var duplicateMatrixToAdd = approvalMatrices[1];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click approval matrix to edit
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, approvalMatrixToEdit.Name);

            // populate matrix details
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DefaultapproverTextBox.Text = duplicateMatrixToAdd.DefaultEmployee.ApproverFullName;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.ApprovalmatrixnameTextBox.Text = duplicateMatrixToAdd.Name;
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMaricesControlsDocument.DescriptionTextBox.Text = duplicateMatrixToAdd.Description;

            // press save button
            approvalMatricesMethods.PressSaveApprovalMatrix();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.DuplicateFieldForApprovalMatrixName });
            approvalMatricesMethods.ValidateModalMessage();

            // press close on modal message
            approvalMatricesMethods.PressCloseOnModalMessage();


        }

        /// <summary>
        /// ApprovalMatricesSuccessfullyDeleteApprovalMatrix_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesSuccessfullyDeleteApprovalMatrix_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var matrixToDelete = approvalMatrices[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click delete matrix
            esrAssignmentsMethods.ClickDeleteGridRow(MatricesGrid, matrixToDelete.Name);

            // confirm delete
            approvalMatricesMethods.PressOKOnBrowserValidation();

            // validate grid record deleted
            esrAssignmentsMethods.ValidateGrid(MatricesGrid, matrixToDelete.MatricesGridValues, false);
        }

        /// <summary>
        /// ApprovalMatricesUnsuccessfullyAddApprovalMatrixLevelWithDuplicateFields_UITest
        /// </summary>
        [TestCategory("Approval Matrix Details"), TestCategory("Approval Matrices"), TestCategory("Expenses"), TestMethod]
        public void ApprovalMatricesUnsuccessfullyAddApprovalMatrixLevelWithDuplicateFields_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            var matrixToEdit = approvalMatrices[0];
            var duplicateMatrixLevelToAdd = matrixToEdit.ApprovalMatrixLevels[0];

            // navigate to approval matrices
            sharedMethods.NavigateToPage(ExecutingProduct, ApprovalMatricesUrlSuffixes.ApprovalMatricesUrl);

            // click add new approval matrix
            esrAssignmentsMethods.ClickEditGridRow(MatricesGrid, matrixToEdit.Name);

            // click levels link
            approvalMatricesMethods.ClickLevelsLink();

            // click new level
            approvalMatricesMethods.ClickNewLevelsLink();

            // populate level details

            if (duplicateMatrixLevelToAdd.ApproverEmployeeId.HasValue)
            {
                approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = duplicateMatrixLevelToAdd.LevelEmployee.ApproverFullName;
            }
            else if (duplicateMatrixLevelToAdd.ApproverBudgetHolderId.HasValue)
            {
                approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = duplicateMatrixLevelToAdd.LevelHolder.ApproverName;
            }
            else
            {
                approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApproverTextBox.Text = duplicateMatrixLevelToAdd.LevelTeam.ApproverName;
            } 
            
            approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.ApprovallimitTextBox.Text = decimal.Round(duplicateMatrixLevelToAdd.ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString();


            // press save matrix level
            approvalMatricesMethods.PressSaveLevelButton();

            // validate modal message
            approvalMatricesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), ApprovalMatricesModalMessages.DuplicateFieldForApprovalLimit });
            approvalMatricesMethods.ValidateModalMessage();
        }

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        private void ImportDataToTestingDatabase(string testName)
        {
            int approvalMatrixId = 0;
            int levelId;
            switch (testName)
            {
                case "ApprovalMatricesSuccessfullyDeleteApprovalMatrix_UITest":
                case "ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithMandatoryFieldsBlank_UITest":
                case "ApprovalMatricesUnsuccessfullyAddApprovalMatrixAndMatrixLevelWithInvalidFields_UITest":
                case "ApprovalMatricesUnsuccessfullyAddApprovalMatrixWithDuplicateFields_UITest":
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[0], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    break;
                case "ApprovalMatricesSuccessfullyEditApprovalMatrix_UITest":
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[0], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    foreach (var level in approvalMatrices[0].ApprovalMatrixLevels)
                    {
                        level.ApprovalMatrixId = approvalMatrixId;
                        levelId = 0;
                        levelId = ApprovalMatricesRepository.SaveLevel(level, ExecutingProduct, adminId);
                        Assert.IsTrue(levelId > 0);
                    }
                    break;
                case "ApprovalMatricesUnsuccessfullyAddApprovalMatrixLevelWithDuplicateFields_UITest":
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[0], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    approvalMatrices[0].ApprovalMatrixLevels[0].ApprovalMatrixId = approvalMatrixId;
                    levelId = 0;
                    levelId = ApprovalMatricesRepository.SaveLevel(approvalMatrices[0].ApprovalMatrixLevels[0], ExecutingProduct, adminId);
                    Assert.IsTrue(levelId > 0);
                    break;
                case "ApprovalMatricesUnsuccessfullyEditApprovalMatrixWithDuplicateFields_UITest":
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[0], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    approvalMatrixId = 0;
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[1], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    break;
                default:
                    approvalMatrixId = ApprovalMatricesRepository.CreateApprovalMatrix(approvalMatrices[0], ExecutingProduct, adminId);
                    Assert.IsTrue(approvalMatrixId > 0);
                    break;
            }
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        public HtmlTable MatricesGrid
        {
            get
            {
                return approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatricesLinkGridDocument.ApprovalMatricesGrid;
            }
        }

        public HtmlTable MatrixLevelGrid
        {
            get
            {
                return approvalMatricesMethods.ApprovalMatricesControlsWindow.ApprovalMatrixLevelsControlsDocument.MatrixLevelsGrid;
            }
        }
    }
}
