using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using System.Data;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using Cache = Utilities.DistributedCaching.Cache;

namespace Spend_Management
{
    /// <summary>
	/// Summary description for teams.
	/// </summary>
	public class cTeams
	{
        /// <summary>
        /// SQL string
        /// </summary>
		string strsql;
        /// <summary>
        /// Current Account ID
        /// </summary>
		private int nAccountId;
        /// <summary>
        /// Current Sub-Account ID (nullable)
        /// </summary>
        private int? nSubAccountId;
        /// <summary>
        /// Cache reference
        /// </summary>
		Cache Cache = new Cache();

        /// <summary>
        /// Audit category definition
        /// </summary>
		string auditcat = "Teams";

        /// <summary>
        /// The cache area key for this class.
        /// </summary>
        public const string CacheArea = "Team";

        #region properties
        /// <summary>
        /// Gets the current customer account id
        /// </summary>
        public int AccountID
        {
            get { return nAccountId; }
        }
        /// <summary>
        /// Gets the SubAccount Id if used
        /// </summary>
        public int? SubAccountId
        {
            get { return nSubAccountId; }
        }

        #endregion

        /// <summary>
        /// cTeams constructor
        /// </summary>
        /// <param name="accountid">Current customer accountid to cache teams for</param>
        [Obsolete]
        public cTeams(int accountid)
        {
            nAccountId = accountid;
            nSubAccountId = null;
        }

        /// <summary>
        /// cTeams constructor
        /// </summary>
        /// <param name="accountid">Current customer accountid to cache teams for</param>
        /// <param name="subaccountid">Subaccount Id to filter by</param>
        public cTeams(int accountid, int? subaccountid)
        {
            nAccountId = accountid;
            nSubAccountId = subaccountid;
        }

        internal void RemoveCache(int teamId)
        {
            Cache.Delete(this.AccountID, CacheArea, teamId.ToString());
        }

        internal void AddCache(cTeam team)
        {
            Cache.Add(this.AccountID, CacheArea, team.teamid.ToString(), team);
        }

        internal cTeam GetCache(int teamid)
        {
            return Cache.Get(this.AccountID, CacheArea, teamid.ToString()) as cTeam;
        }

        /// <summary>
        /// Cache the teams from the database
        /// </summary>
        /// <returns></returns>
        internal cTeam GetTeamFromDatabase(int teamId, IDBConnection connection = null)
        {
            cTeam reqteam;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                var lstTeamemps = GetTeamMembers(teamId, connection);

                reqteam = null;

                strsql = "select teamid, teamname, description, teamleaderid, createdon, createdby, modifiedon, modifiedby from dbo.teams WHERE teamid = @teamid";
                expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamId);
                if (SubAccountId.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccountID", SubAccountId);
                    strsql += " AND subAccountId = @subAccountID";
                }
            
                expdata.sqlexecute.CommandText = strsql;
                using (IDataReader reader = expdata.GetReader(strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        var teamid = reader.GetInt32(reader.GetOrdinal("teamid"));
                        var teamname = reader.GetString(reader.GetOrdinal("teamname"));
                        var description = reader.IsDBNull(reader.GetOrdinal("description")) == false ? reader.GetString(reader.GetOrdinal("description")) : "";
                        int? teamleaderid = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("teamleaderid")))
                        {
                            teamleaderid = reader.GetInt32(reader.GetOrdinal("teamleaderid"));
                        }

                        var createdon = new DateTime(1900, 1, 1);
                        if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                        {
                            createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                        }

                        int? createdby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                        {
                            createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                        }

                        DateTime? modifiedon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        }

                        int? modifiedby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        }

                        reqteam = new cTeam(AccountID, teamid, teamname, description, lstTeamemps, teamleaderid, createdon, createdby, modifiedon, modifiedby);
                        AddCache(reqteam);
                    }
                    reader.Close();
                }
            }

            return reqteam;
        }

        /// <summary>
        /// Get collection of current team member ids
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal List<int> GetTeamMembers(int teamId, IDBConnection connection = null)
		{
            var lstTeams = new List<int>();
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                strsql = "select employeeid from teamemps WHERE teamid = @teamid";
                expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamId);
                using (var teamreader = expdata.GetReader(strsql))
                {
                    while (teamreader.Read())
                    {
                        var employeeid = teamreader.GetInt32(0);
                       
                        lstTeams.Add(employeeid);
                    }

                    teamreader.Close();
                }

                expdata.sqlexecute.Parameters.Clear();
            }
            return lstTeams;
		}

        /// <summary>
        /// Gets all the teams from the DB.
        /// </summary>
        /// <returns>List of Teams</returns>
        public List<cTeam> GetAllTeams(IDBConnection connection = null, int? currentSubAccountId = null)
        {
            var list = new List<cTeam>();

            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                strsql = "select teamid, teamname, description, teamleaderid, createdon, createdby, modifiedon, modifiedby from dbo.teams";

                if (currentSubAccountId != null)
                {
                    strsql += " where subAccountId = @subAccountId";
                    expdata.AddWithValue("@subAccountId", currentSubAccountId);
                }


                expdata.sqlexecute.CommandText = strsql;
                using (var reader = expdata.GetReader(strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        var teamid = reader.GetInt32(reader.GetOrdinal("teamid"));
                        var teamname = reader.GetString(reader.GetOrdinal("teamname"));
                        var description = reader.IsDBNull(reader.GetOrdinal("description")) == false ? reader.GetString(reader.GetOrdinal("description")) : "";
                        int? teamleaderid = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("teamleaderid")))
                        {
                            teamleaderid = reader.GetInt32(reader.GetOrdinal("teamleaderid"));
                        }

                        var createdon = new DateTime(1900, 1, 1);
                        if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                        {
                            createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                        }

                        int? createdby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                        {
                            createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                        }

                        DateTime? modifiedon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        }

                        int? modifiedby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        }

                        var lstTeamemps = GetTeamMembers(teamid, connection);
                        var reqteam = new cTeam(AccountID, teamid, teamname, description, lstTeamemps, teamleaderid, createdon, createdby, modifiedon, modifiedby);
                        list.Add(reqteam);
                    }
                    reader.Close();
                }
            }

            return list;
        }
	
        /// <summary>
        /// Save a new or existing team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="teamname">Team Name</param>
        /// <param name="description">Team Description</param>
        /// <param name="leaderemployeeid"></param>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public int SaveTeam(int teamID, string teamname, string description, int leaderemployeeid, int EmployeeID)
        {
            var data = new DatabaseConnection(cAccounts.getConnectionString(AccountID));

            data.sqlexecute.Parameters.AddWithValue("@employeeid", EmployeeID);
            data.sqlexecute.Parameters.AddWithValue("@teamid", teamID);
            data.sqlexecute.Parameters.AddWithValue("@teamname", teamname);
            data.sqlexecute.Parameters.AddWithValue("@description", description);
            if (leaderemployeeid > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@teamleaderid", leaderemployeeid);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@teamleaderid", DBNull.Value);
            }

            var currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@subAccountID", currentUser.CurrentSubAccountId);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                data.sqlexecute.Parameters.AddWithValue("@subAccountID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveTeam");
            var team = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();
            RemoveCache(teamID);
            return team;
        }

        

        /// <summary>
        /// Delete a team from the database
        /// </summary>
        /// <param name="teamid">Team Id to delete</param>
        /// <returns>0 (success), 1 (claims assigned to team), 2 (signoffs exist for team), 3 (tasks assigned for team)</returns>
		public int DeleteTeam(int teamid)
		{
            int retVal;
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@teamid", teamid);
                var currentUser = cMisc.GetCurrentUser();
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
            {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@return", SqlDbType.Int);
                data.sqlexecute.Parameters["@return"].Direction = ParameterDirection.ReturnValue;

                data.ExecuteProc("deleteTeam");
                retVal = (int) data.sqlexecute.Parameters["@return"].Value;
                data.sqlexecute.Parameters.Clear();
            }

            RemoveCache(teamid);
			return retVal;
		}

        /// <summary>
        /// Handles bulk addition and removal of the employees for a given team.
        /// </summary>
        /// <param name="teamId">The Id of the team.</param>
        /// <param name="toAdd">Ids of the employees to add.</param>
        /// <param name="toRemove">Ids of the employees to remove.</param>
        /// <param name="accountId">The Account Id</param>
        /// <param name="employeeId">The EmployeeId that is doing the call.</param>
        public void ChangeTeamEmployees(int teamId, List<int> toAdd, List<int> toRemove, int accountId, int employeeId)
        {
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                data.sqlexecute.Parameters.AddWithValue("@teamId", teamId);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeId);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

                data.AddWithValue("@toRemove", toRemove);
                data.AddWithValue("@toAdd", toAdd);
                
                data.ExecuteProc("ChangeTeamEmployees");
                data.sqlexecute.Parameters.Clear();
            }

            RemoveCache(teamId);
        }


        /// <summary>
        /// GetTeamById
        /// </summary>
        /// <param name="teamid">Team Id to retrieve</param>
        /// <param name="connection">The iDbConnectoin to use for unit testing.</param>
        /// <returns>Team entity</returns>
        public cTeam GetTeamById(int teamid, IDBConnection connection = null)
        {
            var result = this.GetCache(teamid) ?? this.GetTeamFromDatabase(teamid, connection);
            return result;
		}

        /// <summary>
        /// getTeamidByName
        /// </summary>
        /// <param name="name">Team name to obtain ID for</param>
        /// <param name="connection">The iDbConnectoin to use for unit testing.</param>
        /// <returns>Team Id or 0 if not found</returns>
        public int getTeamidByName(string name, IDBConnection connection = null)
        {
            int teamid;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
        {
                teamid = 0;
                expdata.sqlexecute.Parameters.AddWithValue("@name", name);

                strsql = "select teamid from teams where teamname = @name";
                using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                            teamid = reader.GetInt32(reader.GetOrdinal("teamid"));
                    }
                    }
                    reader.Close();
                }

                expdata.sqlexecute.Parameters.Clear();
                }

            return teamid;
        }

        public cTeam getTeamByName(string team)
        {
            var teamId = this.getTeamidByName(team);
            return this.GetTeamById(teamId);
        }

        /// <summary>
        /// addTeamMember
        /// </summary>
        /// <param name="employeeid">Employee Id to add</param>
        /// <param name="teamid">Team Id to add them to</param>
        /// <returns></returns>
		public int AddTeamMember(int employeeid, int teamid)
		{
            var currentUser = cMisc.GetCurrentUser();
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@teamEmpId", employeeid);
                data.sqlexecute.Parameters.AddWithValue("@teamId", teamid);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                data.ExecuteProc("addTeamEmp");
                data.sqlexecute.Parameters.Clear();
            }

            RemoveCache(teamid);

            return 0;
		}

        /// <summary>
        /// Delete a single team employee
        /// </summary>
        /// <param name="teamEmpID"></param>
        public void DeleteTeamEmp(int teamEmpID)
        {
            var currentUser = cMisc.GetCurrentUser();
            var teamId = 0;
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                data.sqlexecute.Parameters.AddWithValue("@teamEmpId", teamEmpID);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
            {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
                data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            
                data.ExecuteProc("deleteTeamEmp");
                teamId = (int)data.sqlexecute.Parameters["@identity"].Value;
            
                data.sqlexecute.Parameters.Clear();
            }

            RemoveCache(teamId);
        }
	
        /// <summary>
        /// CreateDropDown: Gets an array of list items for populating a drop down list
        /// </summary>
        /// <param name="teamid">Team Id to obtain list items for</param>
        /// <returns></returns>
		public ListItem[] CreateDropDown(int teamid, IDBConnection connection = null)
		{
            var tempItems = new List<ListItem>();

            using (var expData = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                var sql = "select teamname, teamid from teams";
                if (this.SubAccountId.HasValue)
                {
                    expData.sqlexecute.Parameters.AddWithValue("@subAccountId", this.SubAccountId.Value);
                    sql += " WHERE subaccountid = @subaccountid";
                }
                sql += " ORDER BY teamname";

                using (var reader = expData.GetReader(sql))
                {
                    var teamNameOrd = reader.GetOrdinal("teamname");
                    var teamIdOrd = reader.GetOrdinal("teamid");
                    while (reader.Read())
                    {
                        var newTeamId = reader.GetInt32(teamIdOrd);
                        tempItems.Add(new ListItem(reader.GetString(teamNameOrd), newTeamId.ToString()));
                        if (teamid == newTeamId)
                        {
                            tempItems[tempItems.Count - 1].Selected = true;
                        }       
                    }
                }
            }

			return tempItems.ToArray();
		}

        /// <summary>
        /// getTeamMemberGrid
        /// </summary>
        /// <param name="teamid">Team Id to retrieve member table for</param>
        /// <returns>An HTML Team member table</returns>
        public string getTeamMemberGrid(int teamid)
        {
            int i;
            var rowclass = "row1";
            var clsemployees = new cEmployees(AccountID);
            var team = GetTeamById(teamid);
            
            var output = new System.Text.StringBuilder();
            output.Append("<table>");
            output.Append("<tr><th>Team Members</th></tr>");
            if (team != null)
            {
                for (i = 0; i < team.teammembers.Count; i++)
                {
                    var employeeid = team.teammembers[i];
                    var reqemp = clsemployees.GetEmployeeById(employeeid);
                    output.Append("<tr>");
                    output.Append("<td class=\"" + rowclass + "\">");
                    output.Append(reqemp.Surname + ", " + reqemp.Title + " " + reqemp.Forename);
                    output.Append("</td>");
                
                    rowclass = rowclass == "row1" ? "row2" : "row1";
                }
            }

            output.Append("</table>");

            return output.ToString();
        }

        /// <summary>
        /// Gets drop down list item array containing employees and teams (team indicated by * prefix)
        /// </summary>
        /// <param name="addNoneSelection">Include [None] element as first option</param>
        /// <returns></returns>
        public ListItem[] GetCombinedEmployeeListItems(bool addNoneSelection)
		{
			var newItem = new List<ListItem>();
			newItem.AddRange(GetTeamsListControlItems(addNoneSelection));
			var employees = new cEmployees(AccountID);
			newItem.AddRange(employees.CreateDropDown(0, false));

			return newItem.ToArray();
		}

        /// <summary>
        /// Gets a list of Teams for populating into a drop down list
        /// </summary>
        /// <param name="addNoneSelection">Include [None] element as first option</param>
        /// <returns></returns>
        public ListItem[] GetTeamsListControlItems(bool addNoneSelection)
        {
            var newItem = new List<ListItem>();

            const string teamBgColor = "D0CBCA";

            foreach (var listItem in this.CreateDropDown(-1))
            {
                newItem.Add(new ListItem(listItem.Text + " (Team)", "TEAM_" + listItem.Value));
                newItem[newItem.Count - 1].Attributes.Add("style", "background-color: #" + teamBgColor);
            }

            if (addNoneSelection)
            {
                newItem.Insert(0, new ListItem("[None]", "0"));
            }

            return newItem.ToArray();
        }
	}
}
