using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cTeamObject
    {
        public static cTeam FromTemplate()
        {
            return new cTeam(cGlobalVariables.AccountID, 0, "Unit Test Team " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Unit Test Team Description", null, null, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }

        public static cTeam FromValidTemplate()
        {
            return new cTeam(cGlobalVariables.AccountID, 0, "Unit Test Team " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Unit Test Team Description", null, null, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }


        public static cTeam CreateObject()
        {
            int otherTeamMember = 518;

            cTeams clsTeams = new cTeams(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cTeam team = FromValidTemplate();

            int teamID = clsTeams.SaveTeam(team.teamid, team.teamname, team.description, cGlobalVariables.EmployeeID, cGlobalVariables.EmployeeID);
            clsTeams.addTeamMember(cGlobalVariables.EmployeeID, teamID);
            clsTeams.addTeamMember(otherTeamMember, teamID);

            team = clsTeams.GetTeamById(teamID);

            return team;
        }

        public static int CreateID()
        {
            cTeams clsTeams = new cTeams(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cTeam team = FromValidTemplate();
            int teamID = clsTeams.SaveTeam(team.teamid, team.teamname, team.description, cGlobalVariables.EmployeeID, cGlobalVariables.EmployeeID);
            clsTeams.addTeamMember(cGlobalVariables.EmployeeID, teamID);

            return teamID;
        }

        public static SortedList<int, int> GetTeamEmps(int teamid)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string SQL = "SELECT [teamempid], [employeeid] FROM [dbo].[teamemps] WHERE [teamid] = @teamID";
            data.sqlexecute.Parameters.AddWithValue("@teamID", teamid);

            SortedList<int, int> teamEmployees = new SortedList<int, int>();

            using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(SQL))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false && reader.IsDBNull(1) == false)
                    {
                        teamEmployees.Add(reader.GetInt32(0), reader.GetInt32(1));
                    }
                }

                reader.Close();
            }
            data.sqlexecute.Parameters.Clear();

            return teamEmployees;
        }
    }
}
