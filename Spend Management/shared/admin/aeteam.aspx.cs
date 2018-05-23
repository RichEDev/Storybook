using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Web.Services;
using System.Text;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
    /// Summary description for aeteam.
    /// </summary>
    public partial class aeteam : Page
    {
        //int action;
        //protected System.Web.UI.WebControls.ImageButton cmdhelp;
        private int nTeamId;

        /// <summary>
        /// TeamID for javascript
        /// </summary>
        public int TeamID
        {
            get { return nTeamId; }
            set { nTeamId = value; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            Title = "Add / Edit Team";
            Master.title = Title;
            Master.PageSubTitle = "Team Details";
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1045;
                    break;
                default:
                    Master.helpid = 1035;
                    break;
            }

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, true, true);
                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);
                cTeam reqTeam = null;
                cEmployees clsEmployees = new cEmployees(currentUser.AccountID);
                Employee tempEmp = null;

                int.TryParse(Request.QueryString["teamid"], out nTeamId);

                if (nTeamId > 0)
                {
                    currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Teams, true, true);
                    reqTeam = clsTeams.GetTeamById(nTeamId);
                }
                else
                {
                    currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Teams, true, true);
                }

                // create the leader dropdown from team members
                if (reqTeam != null) 
                {
                    txtTeamName.Text = reqTeam.teamname;
                    txtDescription.Text = reqTeam.description;
                    foreach(int val in reqTeam.teammembers)
                    {
                        tempEmp = clsEmployees.GetEmployeeById(val);
                        ddlTeamLeader.Items.Add(new ListItem(tempEmp.Surname + ", " + tempEmp.Title + " " + tempEmp.Forename, tempEmp.EmployeeID.ToString()));
                    }
                    if (reqTeam.teamLeaderId.HasValue)
                    {
                        ddlTeamLeader.SelectedValue = reqTeam.teamLeaderId.Value.ToString();
                    }
                }

                string sSQL = "SELECT teamemps.teamempid, teamemps.teamid, teamemps.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM teamemps";
                cGridNew gridTeamMembers = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeamMembers", sSQL);

                gridTeamMembers.KeyField = "teamempid";
                gridTeamMembers.EmptyText = "There are no Users in this Team yet.";
                gridTeamMembers.getColumnByName("teamempid").hidden = true;
                gridTeamMembers.getColumnByName("teamid").hidden = true;
                gridTeamMembers.getColumnByName("employeeid").hidden = true;

                gridTeamMembers.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Teams, true);
                gridTeamMembers.enableupdating = false;
                gridTeamMembers.deletelink = "javascript:DeleteTeamEmployee({teamempid}, {employeeid});";

                var clsFields = new cFields(currentUser.AccountID);
                var teamId = clsFields.GetFieldByID(new Guid("6B1EF8D5-12EE-4E4A-9815-142AA739F508"));
                gridTeamMembers.addFilter(teamId, ConditionType.Equals, new object[] { nTeamId }, null, ConditionJoiner.None);

                string[] membersGridData = gridTeamMembers.generateGrid();
                litTeamMembersGrid.Text = membersGridData[1];

                string sSQLmdl = "SELECT employees.employeeid, employees.username, employees.title, employees.firstname, employees.surname, employees.archived FROM employees";
                cGridNew gridEmployees = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeamEmployees", sSQLmdl);

                gridEmployees.KeyField = "employeeid";
                gridEmployees.EmptyText = "There are no users with approval privileges.";
                gridEmployees.getColumnByName("employeeid").hidden = true;
                gridEmployees.getColumnByName("archived").hidden = true;

                gridEmployees.enabledeleting = false;
                gridEmployees.enableupdating = false;
                gridEmployees.EnableSelect = true;
                gridEmployees.pagesize = 10;

                cField username = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                gridEmployees.addFilter(username, ConditionType.NotLike, new object[] { "admin%" }, null, ConditionJoiner.And);
                var archived = clsFields.GetFieldByID(new Guid("3A6A93F0-9B30-4CC2-AFC4-33EC108FA77A"));
                gridEmployees.addFilter(archived, ConditionType.DoesNotEqual, new object[] { 1 }, null, ConditionJoiner.And);

                string[] employeeGridData = gridEmployees.generateGrid();
                litEmployeesGrid.Text = employeeGridData[1];

                // set the sel.grid javascript variables
                List<string> jsBlockObjects = new List<string>();
                jsBlockObjects.Add(membersGridData[0]);
                jsBlockObjects.Add(employeeGridData[0]);
                
                Page.ClientScript.RegisterStartupScript(this.GetType(), "TeamGridVars", cGridNew.generateJS_init("TeamGridVars", jsBlockObjects, currentUser.CurrentActiveModule), true);
            }
        }
    }
}
