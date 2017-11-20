namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;

    /// <summary>
    /// The teams repository.
    /// </summary>
    public class TeamsRepository
    {
        /// <summary>
        /// The populate teams.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal static List<Teams> PopulateTeams(int? teamId = null, string sqlToExecute = "")
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT teamid, teamname, description, teamleaderid, subAccountId FROM teams";

            if (sqlToExecute == string.Empty)
            {
                sqlToExecute = strSQL;
            }
            else
            {
                db.sqlexecute.Parameters.Add("@teamid", teamId);
            }
            List<Teams> teams = new List<Teams>();
            using (SqlDataReader reader = db.GetReader(sqlToExecute))
            {
                int teamIdOrdinal = reader.GetOrdinal("teamid");
                int teamNameOrdinal = reader.GetOrdinal("teamname");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int teamLeaderOrdinal = reader.GetOrdinal("teamleaderid");

                while (reader.Read())
                {
                    Teams team = new Teams();
                    team.TeamId = reader.GetInt32(teamIdOrdinal);
                    team.TeamName = reader.GetString(teamNameOrdinal);
                    team.Description = reader.GetString(descriptionOrdinal);
                    team.TeamLeader = reader.IsDBNull(teamLeaderOrdinal) ? null : (int?)reader.GetInt32(teamLeaderOrdinal);
                    team.TeamMembers = PopulateTeamMembers(team.TeamId);
                    teams.Add(team);
                }
            }

            return teams;
        }

        internal static List<TeamMembers> PopulateTeamMembers(int teamId)
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT teamid, teamempid, employeeid FROM teamemps where teamid = '" + teamId + "'";
            List<TeamMembers> teamMembers = new List<TeamMembers>();
            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                int teamIdOrdinal = reader.GetOrdinal("teamid");
                int teamEmpIdOrdinal = reader.GetOrdinal("teamempid");
                int employeeIdOrdinal = reader.GetOrdinal("employeeid");


                while (reader.Read())
                {   
                    TeamMembers teamMember = new TeamMembers();
                    int employeeID = reader.GetInt32(employeeIdOrdinal);
                    List<Employees> employees = EmployeesRepository.PopulateEmployee(sqlToExecute: Employees.SqlItems, employeeId: employeeID);

                    // TeamMembers teamMember = (TeamMembers)mem[0];

                    teamMember.Employee = employees.First();

                    // teamMember.employeeID = reader.GetInt32(employeeId);
                    teamMember.TeamId = reader.GetInt32(teamIdOrdinal);
                    teamMembers.Add(teamMember);
                }
            }

            return teamMembers;
        }

        /// <summary>
        /// The create team.
        /// </summary>
        /// <param name="teamToCreate">
        /// The team to create.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CreateTeam(Teams teamToCreate, ProductType executingProduct)
        {
            teamToCreate.TeamId = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            // change line bellow from teamToCreate.TeamLeader
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", teamToCreate.TeamMembers[0].Employee.employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamToCreate.TeamId);
            expdata.sqlexecute.Parameters.AddWithValue("@teamname", teamToCreate.TeamName);
            expdata.sqlexecute.Parameters.AddWithValue("@description", teamToCreate.Description);
            if (teamToCreate.TeamLeader > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@teamleaderid", teamToCreate.TeamLeader);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@teamleaderid", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountID", AutoTools.GetSubAccountId(executingProduct));

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveTeam");
            teamToCreate.TeamId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            foreach (var member in teamToCreate.TeamMembers)
            {
                TeamsRepository.AddTeamMember(member.Employee.employeeID, teamToCreate.TeamId, executingProduct);
            }
            return teamToCreate.TeamId;
        }

        /// <summary>
        /// The delete team member.
        /// </summary>
        /// <param name="memberId">
        /// The memberId.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        public static void DeleteTeamMember(int memberId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@teamEmpId", memberId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.ExecuteProc("deleteTeamEmp");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The delete team.
        /// </summary>
        /// <param name="teamId">
        /// The teamId.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int DeleteTeam(int teamId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@return", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@return"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("deleteTeam");
            int retVal = (int)expdata.sqlexecute.Parameters["@return"].Value;
            expdata.sqlexecute.Parameters.Clear();
            return retVal;
        }

        /// <summary>
        /// The add team member.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="teamId">
        /// The team id.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int AddTeamMember(int employeeId, int teamId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamId);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
            string strsql = "if not exists (select teamempid from teamemps where teamid = @teamid and employeeid = @employeeid) insert into teamemps (teamid, employeeid) values (@teamid,@employeeid)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return 0;
        }
    }
}
