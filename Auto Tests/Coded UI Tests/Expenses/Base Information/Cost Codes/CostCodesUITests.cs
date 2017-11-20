using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams;
using Auto_Tests.UIMaps.CostCodesUIMapClasses;
using Auto_Tests.Product_Variables.PageUrlSurfices;
using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
using Auto_Tests.Product_Variables.ModalMessages;
using Auto_Tests.Tools;


namespace Auto_Tests.Coded_UI_Tests.Expenses.Base_Information.Cost_Codes
{
    /// <summary>
    /// Summary description for CostCodeTests
    /// </summary>
    [CodedUITest]
    public class CostCodesUITests
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
        /// cost codes methods UI Map
        /// </summary>
        private static CostCodesUIMap costCodesMethods;

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<BudgetHolders> budgetHolders;

        /// <summary>
        /// Cached list of Teams
        /// </summary>
        private static List<Teams> teams;

        /// <summary>
        /// Cached list of cost codes
        /// </summary>
        private static List<CostCodes> costCodes;

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            budgetHolders = BudgetHoldersRepository.PopulateBudgetHolders();
            teams = TeamsRepository.PopulateTeams();
            costCodes = CostCodesRepository.PopulateCostCodes();
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            sharedMethods.CloseBrowserWindow();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            sharedMethods = new SharedMethodsUIMap();
            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            costCodesMethods = new CostCodesUIMap();
            Guid genericName = Guid.NewGuid();

            foreach (var team in teams)
            {
                genericName = Guid.NewGuid();
                team.TeamName = string.Format(
                            "{0}{1}", new object[] { "Custom Team ", genericName });
                foreach (var member in team.TeamMembers)
                {
                    genericName = Guid.NewGuid();
                    if (member.Employee.employeeID == team.TeamLeader)
                    {
                        member.Employee.UserName = string.Format(
                            "{0}{1}", new object[] { "Custom Emp ", genericName });
                        team.TeamLeader = EmployeesRepository.CreateEmployee(member.Employee, ExecutingProduct);
                        Assert.IsTrue(team.TeamLeader > 0);
                    }
                    else
                    {
                        member.Employee.UserName = string.Format(
                            "{0}{1}", new object[] { "Custom Emp ", genericName });
                        int empId = EmployeesRepository.CreateEmployee(member.Employee, ExecutingProduct);
                        Assert.IsTrue(empId > 0);
                    }

                }

                team.TeamId = TeamsRepository.CreateTeam(team, ExecutingProduct);
                Assert.IsTrue(team.TeamId > 0);
            }

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

                budgetHolder.BudgetHolderId = BudgetHoldersRepository.CreateBudgetHolder(budgetHolder, ExecutingProduct);
                Assert.IsTrue(budgetHolder.BudgetHolderId > 0);
            }
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var costCode in costCodes)
            {
                {
                    if (costCode.CostCodeId > 0)
                    {
                        CostCodesRepository.DeleteCostcode(costCode.CostCodeId, budgetHolders[0].Employee.employeeID, ExecutingProduct);
                    }
                }
            }

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

            foreach (var team in teams)
            {
                if (team.TeamId > 0)
                {
                    TeamsRepository.DeleteTeam(team.TeamId, ExecutingProduct);
                }

                foreach (var member in team.TeamMembers)
                {
                    if (member.Employee.employeeID > 0)
                    {
                        EmployeesRepository.DeleteEmployee(member.Employee.employeeID, ExecutingProduct);
                    }
                }
            }
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyCreateNewCostCodeWithEmployeeOwner_UITest()
        {
            CostCodes costCodeToAdd = costCodes[0];
            costCodeToAdd.Owner = budgetHolders[0].Employee.ApproverFullName;

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the new costcode link
            costCodesMethods.ClickAddCostCodeLink();

            /// Enter details
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text = costCodeToAdd.CostCodeName;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text = costCodeToAdd.Description;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text = costCodeToAdd.Owner;

            // Press save
            costCodesMethods.PressSaveButton();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(CostCodesGrid, costCodeToAdd.CostCodeGridValues);

            // Click the edit costcode icon
            costCodeToAdd.CostCodeId = esrAssignmentsMethods.ReturnIdFromGrid(CostCodesGrid, costCodeToAdd.CostCodeName);
            esrAssignmentsMethods.ClickEditGridRow(CostCodesGrid, costCodeToAdd.CostCodeName);

            //verify costcode details
            Assert.AreEqual(costCodeToAdd.CostCodeName, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Description, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Owner, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text);
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyCreateNewCostCodeWithBudgetHolder_UITest()
        {
            CostCodes costCodeToAdd = costCodes[0];
            costCodeToAdd.Owner = budgetHolders[0].ApproverName;

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the new costcode link
            costCodesMethods.ClickAddCostCodeLink();

            /// Enter details
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text = costCodeToAdd.CostCodeName;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text = costCodeToAdd.Description;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text = costCodeToAdd.Owner;

            // Press save
            costCodesMethods.PressSaveButton();

            // validate grid
            costCodeToAdd.CostCodeId = esrAssignmentsMethods.ReturnIdFromGrid(CostCodesGrid, costCodeToAdd.CostCodeName);
            esrAssignmentsMethods.ValidateGrid(CostCodesGrid, costCodeToAdd.CostCodeGridValues);

            // Click the edit costcode icon
            esrAssignmentsMethods.ClickEditGridRow(CostCodesGrid, costCodeToAdd.CostCodeName);

            //verify costcode details
            Assert.AreEqual(costCodeToAdd.CostCodeName, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Description, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Owner, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text);
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyCreateNewCostCodeWithTeamOwner_UITest()
        {
            CostCodes costCodeToAdd = costCodes[0];
            costCodeToAdd.Owner = teams[0].ApproverName;

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the new costcode link
            costCodesMethods.ClickAddCostCodeLink();

            /// Enter details
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text = costCodeToAdd.CostCodeName;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text = costCodeToAdd.Description;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text = costCodeToAdd.Owner;

            // Press save
            costCodesMethods.PressSaveButton();

            // validate grid
            costCodeToAdd.CostCodeId = esrAssignmentsMethods.ReturnIdFromGrid(CostCodesGrid, costCodeToAdd.CostCodeName);
            esrAssignmentsMethods.ValidateGrid(CostCodesGrid, costCodeToAdd.CostCodeGridValues);

            // Click the edit costcode icon
            esrAssignmentsMethods.ClickEditGridRow(CostCodesGrid, costCodeToAdd.CostCodeName);

            //verify costcode details
            Assert.AreEqual(costCodeToAdd.CostCodeName, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Description, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Owner, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text);
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesUnsuccessfullyCreateNewCostCodeWithMandatoryFieldsBlank_UITest()
        {
            CostCodes costCodeToAdd = costCodes[0];
            costCodeToAdd.Owner = budgetHolders[0].Employee.ApproverFullName;

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the new costcode link
            costCodesMethods.ClickAddCostCodeLink();

            // Press save
            costCodesMethods.PressSaveButton();

            // validate modal message
            costCodesMethods.ValidateModalMessageExpectedValues.ModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), CostCodesModalMessages.EmptyFieldForCostCodeName});
            costCodesMethods.ValidateModalMessage();
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyEditCostCode_UITest()
        {
             this.ImportDataToTestingDatabase(this.TestContext.TestName);
             var costCodeToEdit = costCodes[0];
             var costCodeToAdd = costCodes[1];
             costCodeToAdd.Owner = teams[0].ApproverName;

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the edit costcode icon
            esrAssignmentsMethods.ClickEditGridRow(CostCodesGrid, costCodeToEdit.CostCodeName);

            // Edit costcode details
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text = costCodeToAdd.CostCodeName;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text = costCodeToAdd.Description;
            costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text = costCodeToAdd.Owner;

            // Save changes
            costCodesMethods.PressSaveButton();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(CostCodesGrid, costCodeToAdd.CostCodeGridValues);

            // Click the edit costcode icon
            esrAssignmentsMethods.ClickEditGridRow(CostCodesGrid, costCodeToAdd.CostCodeName);

            //verify costcode details
            Assert.AreEqual(costCodeToAdd.CostCodeName, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Description, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(costCodeToAdd.Owner, costCodesMethods.CostCodeControlsWindow.CostCodeControlsDocument.CostCodeOwnerTextBox.Text);
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyArchiveCostCode_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var costCodeToArchive = costCodes[0];

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the archive costcode icon
            esrAssignmentsMethods.ClickArchiveGridRow(CostCodesGrid, costCodeToArchive.CostCodeName);

            // validate grid
            esrAssignmentsMethods.ValidateArchiveGridRowStatus(CostCodesGrid, costCodeToArchive.CostCodeName);
        }

        [TestCategory("BuildServerTest"), TestMethod]
        public void CostCodesSuccessfullyUnarchiveCostCode_UITest()
        {
             this.ImportDataToTestingDatabase(this.TestContext.TestName);
             var costCodeToUnarchive = costCodes[0];

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the unarchive costcode icon
            esrAssignmentsMethods.ClickArchiveGridRow(CostCodesGrid, costCodeToUnarchive.CostCodeName);

            // validate grid
            esrAssignmentsMethods.ValidateArchiveGridRowStatus(CostCodesGrid, costCodeToUnarchive.CostCodeName, false);
        }

        [TestCategory("Cost Code Details"), TestCategory("Cost Codes"), TestCategory("Expenses"), TestMethod]
        public void CostCodesSuccessfullyDeleteCostCode_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var costCodeToDelete = costCodes[0];

            // Navigate to CostCode page
            sharedMethods.NavigateToPage(ProductType.expenses, CostCodesUrlSuffixes.CostCodesUrl);

            // Click the delete costcode icon
            esrAssignmentsMethods.ClickDeleteGridRow(CostCodesGrid, costCodeToDelete.CostCodeName);
            
            // confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(CostCodesGrid, costCodeToDelete.CostCodeGridValues, false);
        }

        private void ImportDataToTestingDatabase(string testName)
        {
            switch (testName) 
            {
                case "CostCodesSuccessfullyEditCostCode_UITest":
                case "CostCodesSuccessfullyDeleteCostCode_UITest":
                case "CostCodesSuccessfullyArchiveCostCode_UITest":
                    int costCodeId = CostCodesRepository.CreateCostCode(costCodes[0], budgetHolders[0].Employee.employeeID, ExecutingProduct);
                    Assert.IsTrue(costCodeId > 0);
                    break;
                case "CostCodesSuccessfullyUnarchiveCostCode_UITest":
                    costCodeId = CostCodesRepository.CreateCostCode(costCodes[0], budgetHolders[0].Employee.employeeID, ExecutingProduct);
                    Assert.IsTrue(costCodeId > 0);
                    CostCodesRepository.ChangeStatus(costCodeId, ExecutingProduct);
                    break;
                default:
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

        public Microsoft.VisualStudio.TestTools.UITesting.HtmlControls.HtmlTable CostCodesGrid 
        {
            get
            {
                return costCodesMethods.CostCodesGridWindow.CostCodesGridDocument.CostCodesGrid;
            }
        }
    }
}
