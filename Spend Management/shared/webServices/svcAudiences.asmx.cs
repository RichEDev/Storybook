using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;


namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcAudiences
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcAudiences : System.Web.Services.WebService
    {

        /// <summary>
        /// Generates the audiences grid
        /// </summary>
        /// <returns>Two element string array contain js object and html to render</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateAudiencesGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT audiences.audienceID, audiences.audienceName, audiences.description FROM dbo.audiences";
            cGridNew gridAudiences = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridAudiences", sSQL);

            gridAudiences.KeyField = "audienceID";
            gridAudiences.EmptyText = "No audiences to display.";
            gridAudiences.getColumnByName("audienceID").hidden = true;
            gridAudiences.deletelink = "javascript:SEL.Audience.DeleteAudience({audienceID});";
            gridAudiences.editlink = "aeAudience.aspx?audienceid={audienceID}";
            gridAudiences.enableupdating = true;
            gridAudiences.enabledeleting = true;

            return gridAudiences.generateGrid();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteAudience(int audienceID)
        {
            CurrentUser currenctUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currenctUser);

            return clsAudiences.DeleteAudience(audienceID);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveAudience(int audienceID, string audienceName, string audienceDescription)
        {
            CurrentUser currenctUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currenctUser);

            int returnID = clsAudiences.SaveAudience(audienceID, audienceName, audienceDescription);
            return returnID;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] SaveAudienceEmployees(int audienceID, object[] employeesList)
        {
            List<object> retVals = new List<object>();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            List<int> lstEmployees = new List<int>();

            foreach (int val in employeesList)
            {
                lstEmployees.Add(val);
            }

            if (lstEmployees.Count > 0)
            {
                clsAudiences.SaveAudienceEmployees(audienceID, lstEmployees);
            }
            retVals.Add(audienceID);
            retVals.AddRange(CreateEmployeesGrid(audienceID));

            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteAudienceEmployee(int audienceID, int audienceEmployeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            clsAudiences.DeleteAudienceEmployee(audienceEmployeeID);

            return audienceEmployeeID;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] SaveAudienceBudgetHolders(int audienceID, object[] budgetHoldersList)
        {
            List<object> retVals = new List<object>();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            List<int> lstBudgetHolders = new List<int>();

            foreach (int val in budgetHoldersList)
            {
                lstBudgetHolders.Add(val);
            }

            if (lstBudgetHolders.Count > 0)
            {
                clsAudiences.SaveAudienceBudgetHolders(audienceID, lstBudgetHolders);
            }
            retVals.Add(audienceID);
            retVals.AddRange(CreateBudgetHoldersGrid(audienceID));

            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteAudienceBudgetHolder(int audienceID, int audienceBudgetHolderID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            clsAudiences.DeleteAudienceBudgetHolder(audienceBudgetHolderID);


            return audienceBudgetHolderID;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] SaveAudienceTeams(int audienceID, object[] teamsList)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            List<int> lstTeams = new List<int>();

            foreach (int val in teamsList)
            {
                lstTeams.Add(val);
            }
            if (lstTeams.Count > 0)
            {
                clsAudiences.SaveAudienceTeams(audienceID, lstTeams);
            }

            List<object> retVals = new List<object>();
            retVals.Add(audienceID);
            retVals.AddRange(CreateTeamsGrid(audienceID));
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteAudienceTeam(int audienceID, int audienceTeamID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            clsAudiences.DeleteAudienceTeam(audienceTeamID);

            return audienceTeamID;
        }


        //
        // Audience User Control
        //

        [WebMethod(EnableSession = true)]
        public void DeleteAudienceUC(int entityID, string baseTable)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            //clsAudiences.DeleteAudienceUC();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateAudienceGridUC(int parentRecordId, Guid basetableid, int entityIdentifier, bool canEdit, bool canDelete)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            // Context key should be the entity id and then the base table and entity's key field name seperated by a comma
            cTables clsTables = new cTables(currentUser.AccountID);
            cTable baseTable = clsTables.GetTableByID(basetableid);

            cFields clsFields = new cFields(currentUser.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsFields.GetBy(baseTable.TableID, "id")));
            columns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("09FE8362-8AD1-449C-B3F8-382FFF0EF9DA")))); // audienceid
            columns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("22D141E3-DCA2-4E2A-9CD8-503F95725648")))); // audiencename
            columns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("0B6DDEBD-D7AF-423A-9B9C-5832EB20E2F8")))); // description

            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridAudienceUC", baseTable, columns);

            grid.KeyField = "id";
            grid.enabledeleting = canEdit && canDelete;
            grid.enableupdating = canEdit && canDelete;
            grid.EmptyText = "No audiences to display.";
            grid.deletelink = "javascript:SEL.Audience.DeleteAudienceRecord({id}," + parentRecordId.ToString() + ",'" + baseTable.TableID.ToString() + "'," + entityIdentifier.ToString() + ");";
            grid.editlink = "javascript:SEL.Audience.EditAudienceRecord({id}," + parentRecordId.ToString() + ",{audienceID},'" + baseTable.TableID.ToString() + "'," + entityIdentifier.ToString() + ");";
            grid.getColumnByName("id").hidden = true;
            grid.getColumnByName("audienceID").hidden = true;

            grid.addFilter(clsFields.GetBy(baseTable.TableID, "parentID"), ConditionType.Equals, new object[] { parentRecordId }, null, ConditionJoiner.None);

            return grid.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateAudienceSearchGridUC(Guid tableid, int parentid, int audienceID = -1)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            List<int> lstAssignedAudIds = new List<int>();
            if (audienceID == -1)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
                db.sqlexecute.Parameters.AddWithValue("@tableid", tableid);
                db.sqlexecute.Parameters.AddWithValue("@parentid", parentid);
                System.Data.SqlClient.SqlDataReader reader;
                using (reader = db.GetStoredProcReader("getAudienceIDs"))
                {
                    while (reader.Read())
                    {
                        lstAssignedAudIds.Add(reader.GetInt32(0));
                    }
                    reader.Close();
                }
            }
            string sSQL = "SELECT audiences.audienceID, audiences.audienceName, audiences.description FROM dbo.audiences";
            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "audienceSelectionGrid", sSQL);

            grid.KeyField = "audienceID";
            grid.enabledeleting = false;
            grid.enableupdating = false;
            grid.enablepaging = true;
            grid.EnableSelect = true;
            if (audienceID > 0)
                grid.EnableSelect = false;
            grid.EmptyText = "No audiences to display";
            grid.getColumnByName("audienceID").hidden = true;

            cFields clsFields = new cFields(currentUser.AccountID);
            var audienceId = clsFields.GetFieldByID(new Guid("09FE8362-8AD1-449C-B3F8-382FFF0EF9DA"));
            if (audienceID != -1)
            {
                grid.SelectedItems = new List<object>() { audienceID };
                grid.addFilter(audienceId, ConditionType.Equals, new object[] { audienceID }, null, ConditionJoiner.None);
            }
            else
                if (lstAssignedAudIds.Count > 0)
                    grid.addFilter(audienceId, ConditionType.DoesNotEqual, lstAssignedAudIds.ConvertAll(x => (object)x).ToArray(), null, ConditionJoiner.None);
            return grid.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] saveAudienceRecord(int entityId, int parentRecordId, Guid basetableid, string[] audienceids, bool canView, bool canEdit, bool canDelete, bool parentRecCanEdit, bool parentRecCanDelete)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAudiences clsAudiences = new cAudiences(currentUser);

            List<int> audienceIDs = (from x in audienceids
                                     select Convert.ToInt32(x)).ToList();

            int audienceRecordId = clsAudiences.SaveAudienceRecord(parentRecordId, basetableid, audienceIDs, canView, canEdit, canDelete);

            List<string> retVals = new List<string>();
            retVals.AddRange(CreateAudienceGridUC(parentRecordId, basetableid, entityId, parentRecCanEdit, parentRecCanDelete));
            retVals.Add(audienceRecordId == -1 && (!canView || !canEdit || !canDelete) ? "-1" : "0");
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteAudienceRecord(int recId, int parentRecordId, Guid basetableid, int entityId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return new cAudiences(currentUser).DeleteAudienceRecord(recId, parentRecordId, basetableid);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cAudienceRecordStatus getAudienceRecord(int recordId, Guid baseTableId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return new cAudiences(currentUser).GetAudienceRecord(recordId, baseTableId);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateEmployeesGrid(int audienceID = 0)
        {
            //if (audienceID > 0)
            //{
                CurrentUser currentUser = cMisc.GetCurrentUser();

                string sSQL = "SELECT audiences.audienceID, audienceEmployees.audienceEmployeeID, employees.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM dbo.audiences";
                cGridNew gridEmployees = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridEmployees", sSQL);

                gridEmployees.KeyField = "audienceEmployeeID";
                gridEmployees.EmptyText = "No employees to display.";

                gridEmployees.enabledeleting = true;
                gridEmployees.enableupdating = false;
                gridEmployees.deletelink = "javascript:SEL.Audience.DeleteAudienceEmployee({audienceID}, {audienceEmployeeID});";
                gridEmployees.getColumnByName("audienceID").hidden = true;
                gridEmployees.getColumnByName("audienceEmployeeID").hidden = true;
                gridEmployees.getColumnByName("employeeid").hidden = true;
                gridEmployees.pagesize = 10;

                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("4034904B-FD50-4BCF-B15F-1BBAC93709A9"));
                gridEmployees.addFilter(field, ConditionType.Equals, new object[] { audienceID }, null, ConditionJoiner.None);

                return gridEmployees.generateGrid();
            //}
            //else
            //{
            //    return new string[] { "", "<span>Employees can not be added until the main Audience record is saved.</span>" };
            //}
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateBudgetHoldersModalGrid(int audienceID, string filter)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT budgetholders.budgetholderid, budgetholders.budgetholder, budgetholders.description, employees.username FROM dbo.budgetholders";
            cGridNew gridBudgetHolders = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridBudgetHoldersSearch", sSQL);

            gridBudgetHolders.KeyField = "budgetholderid";
            gridBudgetHolders.EmptyText = "No budget holders to display.";

            gridBudgetHolders.enabledeleting = false;
            gridBudgetHolders.enableupdating = false;
            gridBudgetHolders.DisplayFilter = false;
            gridBudgetHolders.EnableSelect = true;
            gridBudgetHolders.getColumnByName("budgetholderid").hidden = true;
            gridBudgetHolders.pagesize = 10;

            if (!string.IsNullOrEmpty(filter))
            {
                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("28E88047-145A-45E5-AFEB-3F18A4683BA5"));
                gridBudgetHolders.addFilter(field, ConditionType.Like, new object[] { filter + "%" }, null, ConditionJoiner.None);
            }

            return gridBudgetHolders.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateBudgetHoldersGrid(int audienceID = 0)
        {
            //if (audienceID > 0)
            //{
                CurrentUser currentUser = cMisc.GetCurrentUser();

                string sSQL = "SELECT audienceBudgetHolders.audienceID, audienceBudgetHolders.audienceBudgetHolderID, budgetholders.budgetholderid, budgetholders.budgetholder, budgetholders.description, employees.username FROM dbo.audienceBudgetHolders";
                cGridNew gridBudgetHolders = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridBudgetHolders", sSQL);

                gridBudgetHolders.KeyField = "audienceBudgetHolderID";
                gridBudgetHolders.EmptyText = "No budget holders to display.";

                gridBudgetHolders.enabledeleting = true;
                gridBudgetHolders.enableupdating = false;
                gridBudgetHolders.deletelink = "javascript:SEL.Audience.DeleteAudienceBudgetHolder({audienceID}, {audienceBudgetHolderID});";
                gridBudgetHolders.getColumnByName("audienceID").hidden = true;
                gridBudgetHolders.getColumnByName("audienceBudgetHolderID").hidden = true;
                gridBudgetHolders.getColumnByName("budgetholderid").hidden = true;
                gridBudgetHolders.pagesize = 10;

                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("3582CA7D-3FB4-46AD-B541-B5DFB880F350"));
                gridBudgetHolders.addFilter(field, ConditionType.Equals, new object[] { audienceID }, null, ConditionJoiner.None);

                return gridBudgetHolders.generateGrid();
            //}
            //else
            //{
            //    return new string[] { "", "<span>Budget Holders can not be added until the main Audience record is saved.</span>" };
            //}
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateTeamsGrid(int audienceID = 0)
        {
            //if (audienceID > 0)
            //{
                CurrentUser currentUser = cMisc.GetCurrentUser();

                string sSQL = "SELECT audienceTeams.audienceID, audienceTeams.audienceTeamID, teams.teamid, teams.teamname, teams.description FROM dbo.audienceTeams";
                cGridNew gridTeams = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeams", sSQL);

                gridTeams.KeyField = "audienceTeamID";
                gridTeams.EmptyText = "No teams to display.";

                gridTeams.enabledeleting = true;
                gridTeams.enableupdating = false;
                gridTeams.deletelink = "javascript:SEL.Audience.DeleteAudienceTeam({audienceID}, {audienceTeamID});";
                gridTeams.getColumnByName("audienceID").hidden = true;
                gridTeams.getColumnByName("audienceTeamID").hidden = true;
                gridTeams.getColumnByName("teamid").hidden = true;
                gridTeams.pagesize = 10;

                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("A14E8743-ACC8-4FA8-AB1B-E127469E0ED6"));
                gridTeams.addFilter(field, ConditionType.Equals, new object[] { audienceID }, null, ConditionJoiner.None);

                return gridTeams.generateGrid();
            //}
            //else
            //{
            //    return new string[] { "", "<span>Teams can not be added until the main Audience record is saved.</span>" };
            //}
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateEmployeesModalGrid(int audienceID, string filter)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT employees.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM dbo.employees";
            cGridNew gridEmployees = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridEmployeesSearch", sSQL);

            gridEmployees.KeyField = "employeeid";
            gridEmployees.EmptyText = "No employees to display.";

            gridEmployees.enabledeleting = false;
            gridEmployees.enableupdating = false;
            gridEmployees.DisplayFilter = false;
            gridEmployees.EnableSelect = true;
            gridEmployees.getColumnByName("employeeid").hidden = true;
            gridEmployees.pagesize = 10;

            if (!string.IsNullOrEmpty(filter))
            {
                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                gridEmployees.addFilter(field, ConditionType.Like, new object[] { filter + "%" }, null, ConditionJoiner.None);
            }

            return gridEmployees.generateGrid();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreateTeamsModalGrid(int audienceID, string filter)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            string sSQL = "SELECT teams.teamid, teams.teamname, teams.description FROM dbo.teams";
            cGridNew gridTeams = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeamsSearch", sSQL);

            gridTeams.KeyField = "teamid";
            gridTeams.EmptyText = "No teams to display.";

            gridTeams.enabledeleting = false;
            gridTeams.enableupdating = false;
            gridTeams.DisplayFilter = false;
            gridTeams.EnableSelect = true;
            gridTeams.getColumnByName("teamid").hidden = true;
            gridTeams.pagesize = 10;

            if (!string.IsNullOrEmpty(filter))
            {
                cFields clsFields = new cFields(currentUser.AccountID);
                cField field = clsFields.GetFieldByID(new Guid("1422263F-882E-4B8D-BD10-B6A4E9267E61"));
                gridTeams.addFilter(field, ConditionType.Like, new object[] { filter + "%" }, null, ConditionJoiner.None);
            }

            return gridTeams.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int GetAudienceMemberCount(int audienceID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cAudiences audiences = new cAudiences(currentUser);
            return audiences.GetAudienceMemberCount(audienceID);
        }

    }
}
