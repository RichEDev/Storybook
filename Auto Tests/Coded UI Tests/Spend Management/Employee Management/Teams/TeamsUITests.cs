namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Imports_Exports.NHSTrusts;
    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.UIMaps.TeamsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    /// <summary>
    /// Summary description for TeamsUITests
    /// </summary>
    [CodedUITest]
    public class TeamsUITests
    {

        UIMaps.TeamsUIMapClasses.TeamsUIMap cTeamsMethods = new UIMaps.TeamsUIMapClasses.TeamsUIMap();

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
        private static TeamsUIMap teamsMethods;

        /// <summary>
        /// Cached list of Teams
        /// </summary>
        private static List<Teams> teams;

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
            sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            teams = TeamsRepository.PopulateTeams();
        }

        /// <summary>
        /// The class clean up.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            sharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Guid genericName;
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
            }

            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            teamsMethods = new TeamsUIMap();
        }

        /// <summary>
        /// The my test cleanup.
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
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
        #endregion

        /// <summary>
        /// The teams unsuccessfully add team with mandatory fields blank_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsSuccessfullyAddTeamWithTeamMembers_UITest()
        {
            var teamToAdd = teams[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click New Team link
            teamsMethods.ClickAddTeamLink();

            // populate all fields
            teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text = teamToAdd.TeamName;
            teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text = teamToAdd.Description;

            // search and add team members
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                teamsMethods.ClickAddTeamMemberLink();
                teamsMethods.TeamControlsWindow.TeamControlsDocument.UsernameFilterTextBox.Text = teamMember.Employee.UserName;
                teamsMethods.PressFilterByUsernameButton();
                esrAssignmentsMethods.ClickCheckGridRow(TeamEmployeesGrid, teamMember.Employee.UserName, teamMember.Employee.employeeID);
                teamsMethods.PressSaveTeamMemberButton();
            }

            string teamLeader = null;

            // set team leader
            if (teamToAdd.TeamLeader > 0)
            {
                foreach(var member in teamToAdd.TeamMembers.Where(member =>  member.Employee.employeeID == teamToAdd.TeamLeader))
                {
                    teamLeader =
                            string.Format(
                                "{0}, {1} {2}", new object[] { member.Employee.Surname, member.Employee.Title, member.Employee.FirstName });
                }

                //foreach (var member in teamToAdd.TeamMembers)
                //{
                //    if (member.Employee.employeeID == teamToAdd.TeamLeader)
                //    {
                //        teamLeader =
                //            string.Format(
                //                "{0}, {1} {2}", new object[] { member.Employee.surname, member.Employee.title, member.Employee.firstName });
                //    }
                //}
                if (teamLeader != null)
                {
                    teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamleaderComboBox.SelectedItem = teamLeader;
                }
            }

            // Click Save
            teamsMethods.PressSaveTeamButton();

            // validate teams grid
            teamToAdd.TeamId = esrAssignmentsMethods.ReturnIdFromGrid(TeamsGrid, teamToAdd.TeamName);
            esrAssignmentsMethods.ValidateGrid(TeamsGrid, teamToAdd.TeamGridValues);

            // click team members icon
            esrAssignmentsMethods.ClickMembersGridRow(TeamsGrid, teamToAdd.TeamName);

            // validate team members grid
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMember.TeamMembersGridValues);
            }

            // click edit team
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamToAdd.TeamName);

            // verify team details
            Assert.AreEqual(teamToAdd.TeamName, teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text);
            Assert.AreEqual(teamToAdd.Description, teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text);
            if (teamLeader != null)
            {
                Assert.AreEqual(teamLeader, teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamleaderComboBox.SelectedItem);
            }

            // validate team members grid
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMember.TeamMembersGridValues);
            }
        }

        /// <summary>
        /// The teams unsuccessfully add team with mandatory fields blank_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsUnsuccessfullyAddTeamWithMandatoryFieldsBlank_UITest()
        {
            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click New Team link
            teamsMethods.ClickAddTeamLink();

            // Click Save
            teamsMethods.PressSaveTeamButton();

            // Validate New Team with mandatory fields missing cannot be added
            teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.EmptyFieldForTeamName });
            teamsMethods.ValidateTeamsModalMessage();
        }

        /// <summary>
        /// The teams unsuccessfully add team without team member_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsUnsuccessfullyAddTeamWithoutTeamMember_UITest()
        {
            var teamToAdd = teams[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click New Team link
            teamsMethods.ClickAddTeamLink();

            // populate team name
            teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text = teamToAdd.TeamName;

            // Click Save
            teamsMethods.PressSaveTeamButton();

            // Validate New Team with mandatory fields missing cannot be added
            teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.EmptyGridForTeamMembers });
            teamsMethods.ValidateTeamsModalMessage();
        }

        /// <summary>
        /// The teams unsuccessfully add team with duplicate fields_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsUnsuccessfullyAddDuplicateTeam_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamToAdd = teams[1];
            var teamMember = teamToAdd.TeamMembers[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click New Team link
            teamsMethods.ClickAddTeamLink();

            // populate all fields
            teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text = teamToAdd.TeamName;
            teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text = teamToAdd.Description;

            // search and add team members
            teamsMethods.ClickAddTeamMemberLink();
            teamsMethods.TeamControlsWindow.TeamControlsDocument.UsernameFilterTextBox.Text = teamToAdd.TeamMembers[0].Employee.UserName;
            teamsMethods.PressFilterByUsernameButton();
            esrAssignmentsMethods.ClickCheckGridRow(TeamEmployeesGrid, teamMember.Employee.UserName, teamMember.Employee.employeeID);
            teamsMethods.PressSaveTeamMemberButton();

            // Validate duplicate team cannot be added
            teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Web Service Message\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.DuplicateFieldForTeamName });
            teamsMethods.ValidateTeamsModalMessage();
        }

        /// <summary>
        /// The teams unsuccessfully edit team with duplicate fields_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsUnsuccessfullyEditDuplicateTeam_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamToEdit = teams[0];
            var duplicateTeamToAdd = teams[1];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click edit Team link
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamToEdit.TeamName);

            // populate all fields
            teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text = duplicateTeamToAdd.TeamName;
            teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text = duplicateTeamToAdd.Description;

            // press save team
            teamsMethods.PressSaveTeamButton();

            // Validate duplicate team cannot be added
            teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Web Service Message\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.DuplicateFieldForTeamName });
            teamsMethods.ValidateTeamsModalMessage();
        }

        /// <summary>
        /// The teams successfully delete team _ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsSuccessfullyDeleteTeam_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamToDelete = teams[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click delete link
            esrAssignmentsMethods.ClickDeleteGridRow(TeamsGrid, teamToDelete.TeamName);

            // confirm delete team
            employeesMethods.PressOkOnBrowserValidation();

            // validate teams grid
            esrAssignmentsMethods.ValidateGrid(TeamsGrid,teamToDelete.TeamGridValues, false);
        }

        /// <summary>
        /// The teams successfully delete team member _ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsSuccessfullyDeleteTeamMember_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamFromWhichToDelete = teams[0];
            var teamMemberToDelete = teamFromWhichToDelete.TeamMembers[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click delete link
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamFromWhichToDelete.TeamName);

            // delete team member
            esrAssignmentsMethods.ClickDeleteGridRow(TeamMembersGrid, teamMemberToDelete.Employee.UserName);

            // save team
            teamsMethods.PressSaveTeamButton();

            // return to verify team member deleted
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamFromWhichToDelete.TeamName);

            // validate teams grid
            esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMemberToDelete.TeamMembersGridValues, false);
        }

        /// <summary>
        /// The teams successfully delete team _ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsUnsuccessfullyDeleteTheOnlyTeamMemberInTeam_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamFromWhichToDelete = teams[1];
            var teamMemberToDelete = teamFromWhichToDelete.TeamMembers[0];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click edit link
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamFromWhichToDelete.TeamName);

            // clcik delete only team member
            esrAssignmentsMethods.ClickDeleteGridRow(TeamMembersGrid, teamMemberToDelete.Employee.UserName);

            // Validate only team member cannot be deleted
            // teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.DeleteOnlyMember });
            teamsMethods.ValidateTeamsModalMessageExpectedValues.TeamsModalMessageInnerText = string.Format("Delete Team Member\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), TeamsModalMessages.DeleteOnlyMember });

            teamsMethods.ValidateTeamsModalMessage();

            // close modal message
            teamsMethods.PressCloseOnModalMessage();

            // save team
            teamsMethods.PressSaveTeamButton();

            // return to edit team
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamFromWhichToDelete.TeamName);

            // validate only team member still exists
            esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMemberToDelete.TeamMembersGridValues);
        }

        /// <summary>
        /// The teams unsuccessfully add team with mandatory fields blank_ ui test.
        /// </summary>
        [TestCategory("Team Details"), TestCategory("Teams"), TestCategory("Spend Management"), TestMethod]
        public void TeamsSuccessfullyEditTeamWithTeamMembers_UITest()
        {
            this.ImportDataToTestingDatabase(this.TestContext.TestName);
            var teamToEdit = teams[0];
            var teamToAdd = teams[1];

            // Navigate to Team summary page
            sharedMethods.NavigateToPage(ExecutingProduct, TeamUrlSuffixes.TeamUrl);

            // Click New Team link
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamToEdit.TeamName);

            // populate all fields
            teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text = teamToAdd.TeamName;
            teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text = teamToAdd.Description;

            // search and add new team members
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                teamsMethods.ClickAddTeamMemberLink();
                teamsMethods.TeamControlsWindow.TeamControlsDocument.UsernameFilterTextBox.Text = teamMember.Employee.UserName;
                teamsMethods.PressFilterByUsernameButton();
                esrAssignmentsMethods.ClickCheckGridRow(TeamEmployeesGrid, teamMember.Employee.UserName, teamMember.Employee.employeeID);
                teamsMethods.PressSaveTeamMemberButton();
            }

            // delete old teammembers
            foreach (var teamMember in teamToEdit.TeamMembers)
            {
                esrAssignmentsMethods.ClickDeleteGridRow(TeamMembersGrid, teamMember.Employee.UserName);
            }
            string teamLeader = null;

            // set team leader
            if (teamToAdd.TeamLeader > 0)
            {
                foreach (var member in teamToAdd.TeamMembers.Where(member => member.Employee.employeeID == teamToAdd.TeamLeader))
                {
                    teamLeader =
                            string.Format(
                                "{0}, {1} {2}", new object[] { member.Employee.Surname, member.Employee.Title, member.Employee.FirstName });
                }

                //foreach (var member in teamToAdd.TeamMembers)
                //{
                //    if (member.Employee.employeeID == teamToAdd.TeamLeader)
                //    {
                //        teamLeader =
                //            string.Format(
                //                "{0}, {1} {2}", new object[] { member.Employee.surname, member.Employee.title, member.Employee.firstName });
                //    }
                //}
                if (teamLeader != null)
                {
                    teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamleaderComboBox.SelectedItem = teamLeader;
                }
            }

            // Click Save
            teamsMethods.PressSaveTeamButton();

            // validate teams grid
            teamToAdd.TeamId = esrAssignmentsMethods.ReturnIdFromGrid(TeamsGrid, teamToAdd.TeamName);
            esrAssignmentsMethods.ValidateGrid(TeamsGrid, teamToAdd.TeamGridValues);

            // click team members icon
            esrAssignmentsMethods.ClickMembersGridRow(TeamsGrid, teamToAdd.TeamName);

            // validate team members grid
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMember.TeamMembersGridValues);
            }

            // click edit team
            esrAssignmentsMethods.ClickEditGridRow(TeamsGrid, teamToAdd.TeamName);

            // vaerify team deatails
            Assert.AreEqual(teamToAdd.TeamName, teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamNameTextBox.Text);
            Assert.AreEqual(teamToAdd.Description, teamsMethods.TeamControlsWindow.TeamControlsDocument.DescriptionTextBox.Text);

            if (teamLeader != null)
            {
                Assert.AreEqual(teamLeader, teamsMethods.TeamControlsWindow.TeamControlsDocument.TeamleaderComboBox.SelectedItem);
            }

            // validate team members grid
            foreach (var teamMember in teamToAdd.TeamMembers)
            {
                esrAssignmentsMethods.ValidateGrid(TeamMembersGrid, teamMember.TeamMembersGridValues);
            }
        }

        private void ImportDataToTestingDatabase(string testName)
        {
            int teamId;
            switch (testName)
            {
                case "TeamsSuccessfullyDeleteTeam_UITest":
                case "TeamsSuccessfullyDeleteTeamMember_UITest":
                case "EmployeesUnsuccessfullyAddDuplicateEmployee_UITest":
                case "TeamsSuccessfullyEditTeamWithTeamMembers_UITest":
                    teamId = TeamsRepository.CreateTeam(teams[0], ExecutingProduct);
                    Assert.IsTrue(teamId > 0);
                    break;
                case "TeamsUnsuccessfullyDeleteTheOnlyTeamMemberInTeam_UITest":
                case "TeamsUnsuccessfullyAddDuplicateTeam_UITest":
                    teamId = TeamsRepository.CreateTeam(teams[1], ExecutingProduct);
                    Assert.IsTrue(teamId > 0);
                    break;
                case "TeamsUnsuccessfullyEditDuplicateTeam_UITest":
                    teamId = TeamsRepository.CreateTeam(teams[0], ExecutingProduct);
                    Assert.IsTrue(teamId > 0);
                    teamId = TeamsRepository.CreateTeam(teams[1], ExecutingProduct);
                    Assert.IsTrue(teamId > 0);
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

        /// <summary>
        /// Gets the team employees grid.
        /// </summary>
        public HtmlTable TeamEmployeesGrid
        {
            get
            {
                return teamsMethods.TeamMembersGridWindow.TeamMembersGridDocument.TeamEmployeesGrid;
            }
        }

        /// <summary>
        /// Gets the team members grid.
        /// </summary>
        public HtmlTable TeamMembersGrid
        {
            get
            {
                return teamsMethods.TeamMembersGridWindow.TeamMembersGridDocument.TeamMembersGrid;
            }
        }

        public HtmlTable TeamsGrid
        {
            get
            {
                return teamsMethods.UITeamsWindowsInternetWindow.UITeamsDocument2.TeamsGrid;
            }
        }
    }
}
