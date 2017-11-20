using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcTeams
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class svcTeams : System.Web.Services.WebService
    {
        /// <summary>
        /// Delete a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteTeam(int teamID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int retVal = 0;
            if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Teams, true))
            {
                cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);
                retVal = clsTeams.DeleteTeam(teamID);
            }
            return retVal;
        }

        /// <summary>
        /// Delete a member of a team
        /// </summary>
        /// <param name="teamEmpID"></param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void DeleteTeamEmployee(int teamEmpID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Teams, true))
            {
                cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);
                clsTeams.DeleteTeamEmp(teamEmpID);
            }
            return;
        }


        /// <summary>
        /// create the team members grid used on the adminTeams popup
        /// </summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateTeamMembersGrid(int teamID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT teams.teamid, employees.username, employees.title, employees.firstname, employees.surname FROM dbo.teams";
            cGridNew gridTeamMembers = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeamMembers", sSQL);

            gridTeamMembers.KeyField = "teamid";
            gridTeamMembers.EmptyText = "There are no Users in this Team.";
            gridTeamMembers.getColumnByName("teamid").hidden = true;

            gridTeamMembers.enableupdating = false;
            gridTeamMembers.enabledeleting = false;
            gridTeamMembers.EnableSelect = false;

            cFields clsFields = new cFields(currentUser.AccountID);
            cField teamId = clsFields.GetFieldByID(new Guid("B60A43D6-1B0A-4F11-95CE-3A9D0C10F6CE"));

            gridTeamMembers.addFilter(teamId, ConditionType.Equals, new object[] { teamID }, null, ConditionJoiner.None);

            return gridTeamMembers.generateGrid();
        }

        /// <summary>
        /// Save the employees into a team and return a list of strings to populate teamleader select list
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<string> SaveTeamEmps(int teamID, int[] employeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);
            cTeam team = clsTeams.GetTeamById(teamID);
            List<int> tempTeamEmps = team.teammembers;
            List<string> lstOut = new List<string>();

            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Teams, true) == true || currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Teams, true) == true)
            {
                
                cEmployees clsEmployees = new cEmployees(currentUser.AccountID);

                if (employeeID != null)
                {
                    for (int i = 0; i < employeeID.Length; i++)
                    {
                        if (tempTeamEmps.Contains(employeeID[i]) == false)
                        {
                            Employee tempEmployee = clsEmployees.GetEmployeeById(employeeID[i]);
                            clsTeams.AddTeamMember(employeeID[i], teamID);

                            string eSurnm = tempEmployee.Surname ?? "";
                            string eTitle = tempEmployee.Title ?? "";
                            string eFirst = tempEmployee.Forename ?? "";
                            lstOut.Add(eSurnm + ", " + eTitle + " " + eFirst + ";" + tempEmployee.EmployeeID.ToString());
                        }
                    }
                }
            }

            return lstOut;
        }

        /// <summary>
        /// save a team details (not including teamemps)
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="teamName"></param>
        /// <param name="teamDescription"></param>
        /// <param name="teamLeaderID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveTeam(int teamID, string teamName, string teamDescription, int teamLeaderID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            AccessRoleType requiredPermission = (teamID == 0 ? AccessRoleType.Add : AccessRoleType.Edit);
            int team = 0;

            if (currentUser.CheckAccessRole(requiredPermission, SpendManagementElement.Teams, true))
            {
                cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);

                team = clsTeams.SaveTeam(teamID, teamName, teamDescription, teamLeaderID, currentUser.EmployeeID);
            }
            return team;
        }

        /// <summary>
        /// create the teamemps grid used on the main part of the aeTeams page
        /// </summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateTeamEmpsGrid(int teamID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT teamemps.teamempid, teamemps.teamid, teamemps.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM teamemps";
            cGridNew gridTeamMembers = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeamMembers", sSQL);

            gridTeamMembers.KeyField = "teamempid";
            gridTeamMembers.EmptyText = "There are no Users in this Team yet.";
            gridTeamMembers.getColumnByName("teamempid").hidden = true;
            gridTeamMembers.getColumnByName("teamid").hidden = true;
            gridTeamMembers.getColumnByName("employeeid").hidden = true;

            gridTeamMembers.enabledeleting = true;
            gridTeamMembers.enableupdating = false;
            gridTeamMembers.deletelink = "javascript:DeleteTeamEmployee({teamempid}, {employeeid});";

            cFields clsFields = new cFields(currentUser.AccountID);
            cField field = clsFields.GetFieldByID(new Guid("6B1EF8D5-12EE-4E4A-9815-142AA739F508"));
            gridTeamMembers.addFilter(field, ConditionType.Equals, new object[] { teamID }, null, ConditionJoiner.None);

            return gridTeamMembers.generateGrid();
        }

        /// <summary>
        /// Returns the number of employees, budget holders etc. currently in the team
        /// </summary>
        /// <param name="teamID">Team ID to obtain a count for</param>
        /// <returns>Number of current members of the team</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int GetTeamMemberCount(int teamID)
        {
            if (teamID == 0)
                return 0;

            CurrentUser currentUser = cMisc.GetCurrentUser();

            cTeams clsTeams = new cTeams(currentUser.AccountID, currentUser.CurrentSubAccountId);
            cTeam team = clsTeams.GetTeamById(teamID);
            
            return team.teammembers.Count;
        }
    }
}
