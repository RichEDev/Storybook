using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Web.Caching;
using System.Data;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    [Serializable()]
    public class cAudiences
    {
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
        ICurrentUser oCurrentUser = null;
        private int nAccountID;

        /// <summary>
        /// Constructor for Audiences class
        /// </summary>
        /// <param name="accountID">Account id</param>
        public cAudiences(ICurrentUser currentUser)
        {
            oCurrentUser = currentUser;
        }

        /// <summary>
        /// Returns an Audience for the specified audienceID
        /// </summary>
        /// <param name="audienceID"></param>
        /// <returns></returns>
        public cAudience GetAudienceByID(int audienceID)
        {
            cAudience reqAudience = GetAudienceFromDB(audienceID);

            return reqAudience;
        }

        /// <summary>
        /// Returns an audience from the database and adds it to the system cache
        /// </summary>
        /// <param name="audienceID"></param>
        /// <returns></returns>
        private cAudience GetAudienceFromDB(int audienceID)
        {
            cAudience tmpAudience = (cAudience)Cache["audience_" + oCurrentUser.AccountID + "_" + audienceID];

            if (tmpAudience == null)
            {
                int nAudienceID = audienceID;
                string sAudienceName;
                string sDescription;
                int createdBy;
                DateTime createdOn;
                int? modifiedBy;
                DateTime? modifiedOn;

                DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
                System.Data.SqlClient.SqlDataReader reader;

                const string sSQL = "SELECT audienceID, audienceName, description, createdBy, createdOn, modifiedBy, modifiedOn FROM dbo.audiences where audienceID=@audienceID";
                
                data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);

                using (reader = data.GetReader(sSQL))
                {
                    while (reader.Read())
                    {
                        audienceID = reader.GetInt32(reader.GetOrdinal("audienceID"));
                        sAudienceName = reader.GetString(reader.GetOrdinal("audienceName"));

                        if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                        {
                            sDescription = reader.GetString(reader.GetOrdinal("description"));
                        }
                        else
                        {
                            sDescription = "";
                        }

                        tmpAudience = new cAudience(nAudienceID, sAudienceName, sDescription);
                    }

                    if (tmpAudience != null && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                    {
                        SortedList<string, object> lstParamaters = new SortedList<string, object>();
                        lstParamaters.Add("@audienceID", audienceID);
                        SqlCacheDependency dep = data.CreateSQLCacheDependency(sSQL + " and " + nAccountID.ToString() + " = " + nAccountID.ToString(), lstParamaters);

                        Cache.Insert("audience_" + nAccountID + "_" + audienceID, tmpAudience, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
                    }

                    reader.Close();
                }

                data.sqlexecute.Parameters.Clear();
            }

            return tmpAudience;
        }

        /// <summary>
        /// Deletes an audience
        /// </summary>
        /// <param name="audienceID"></param>
        public int DeleteAudience(int audienceID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.sqlexecute.Parameters.Add("@usedvalue", SqlDbType.Int);
            data.sqlexecute.Parameters["@usedvalue"].Direction = ParameterDirection.ReturnValue;

            data.ExecuteProc("deleteAudience");
            int usedval = (int)data.sqlexecute.Parameters["@usedvalue"].Value;
            data.sqlexecute.Parameters.Clear();
            return usedval;
        }

        /// <summary>
        /// Returns the number of employees, budget holders and teams (with members) in the audience ID specified
        /// </summary>
        /// <param name="audienceID">Audience ID to obtain a count for</param>
        /// <returns>Total number of employees, budget holders and teams (with members) in the audience</returns>
        public int GetAudienceMemberCount(int audienceID)
        {
            DBConnection smdata = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            string sql = "select sum(empCount) from (select count(budgetHolderID) as empCount from audienceBudgetHolders where audienceID = @audienceID union all select count(employeeID) as empCount from audienceEmployees where audienceID = @audienceID union all select count(employeeID) as empCount from teamemps where teamid in (select teamid from audienceTeams where audienceID = @audienceID)) as unionEmps";
            smdata.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
            return smdata.getcount(sql);
        }

        /// <summary>
        /// Save an audience to the database, 0 for new audience, existing audienceID to update the audience 
        /// </summary>
        /// <returns></returns>
        public int SaveAudience(int audienceID, string audienceName, string audienceDescription)
        {
            DBConnection smdata = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            if (audienceID > 0)
            {
                int count = GetAudienceMemberCount(audienceID);
                if (count == 0)
                    return -2;

                smdata.sqlexecute.Parameters.Clear();
            }
            smdata.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
            smdata.sqlexecute.Parameters.AddWithValue("@audienceName", audienceName);
            if (audienceDescription != null)
            {
                smdata.sqlexecute.Parameters.AddWithValue("@description", audienceDescription);
            }
            else
            {
                smdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            smdata.sqlexecute.Parameters.AddWithValue("@employeeID", oCurrentUser.EmployeeID);

            smdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                smdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                smdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            smdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            smdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            smdata.ExecuteProc("saveAudience");

            audienceID = (int)smdata.sqlexecute.Parameters["@identity"].Value;

            smdata.sqlexecute.Parameters.Clear();

            return audienceID;
        }

        public List<int> SaveAudienceEmployees(int audienceID, List<int> lstEmployees)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            List<int> lstReturns = new List<int>();
            int audienceEmployeeID;

            foreach (int val in lstEmployees)
            {
                data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
                data.sqlexecute.Parameters.AddWithValue("@employeeID", val);

                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                if (oCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                data.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                data.ExecuteProc("saveAudienceEmployee");

                audienceEmployeeID = (int)data.sqlexecute.Parameters["@identity"].Value;
                lstReturns.Add(audienceEmployeeID);

                data.sqlexecute.Parameters.Clear();
            }

            return lstReturns;
        }

        public void DeleteAudienceEmployee(int audienceEmployeeID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            data.sqlexecute.Parameters.AddWithValue("@audienceEmployeeID", audienceEmployeeID);
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteAudienceEmployee");
            data.sqlexecute.Parameters.Clear();
            return;
        }

        public List<int> SaveAudienceBudgetHolders(int audienceID, List<int> lstBudgetHolders)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            List<int> lstReturns = new List<int>();
            int audienceBudgetHolderID;

            foreach (int val in lstBudgetHolders)
            {
                data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
                data.sqlexecute.Parameters.AddWithValue("@budgetHolderID", val);

                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                if (oCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                data.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                data.ExecuteProc("saveAudienceBudgetHolder");

                audienceBudgetHolderID = (int)data.sqlexecute.Parameters["@identity"].Value;
                lstReturns.Add(audienceBudgetHolderID);

                data.sqlexecute.Parameters.Clear();
            }

            return lstReturns;
        }

        public void DeleteAudienceBudgetHolder(int audienceBudgetHolderID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            data.sqlexecute.Parameters.AddWithValue("@audienceBudgetHolderID", audienceBudgetHolderID);
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteAudienceBudgetHolder");
            data.sqlexecute.Parameters.Clear();
            return;
        }

        public List<int> SaveAudienceTeams(int audienceID, List<int> lstTeams)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            List<int> lstReturns = new List<int>();
            int audienceTeamID;

            foreach (int val in lstTeams)
            {
                data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
                data.sqlexecute.Parameters.AddWithValue("@teamID", val);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                if (oCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                data.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                data.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                data.ExecuteProc("saveAudienceTeam");

                audienceTeamID = (int)data.sqlexecute.Parameters["@identity"].Value;
                lstReturns.Add(audienceTeamID);

                data.sqlexecute.Parameters.Clear();
            }

            return lstReturns;
        }

        public void DeleteAudienceTeam(int audienceTeamID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            data.sqlexecute.Parameters.AddWithValue("@audienceTeamID", audienceTeamID);
            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteAudienceTeam");
            data.sqlexecute.Parameters.Clear();
            return;
        }



        /// <summary>
        /// Returns a list of employees attached to an audience
        /// </summary>
        /// <param name="audienceID"></param>
        /// <returns></returns>
        public List<Employee> GetEmployeesByAudienceID(int audienceID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            string sSQL = "SELECT audienceEmployeeID, audienceID, employeeID FROM dbo.audienceEmployees WHERE audienceID=@audienceID";
            data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);

            int audienceEmployeeID;
            int employeeID;

            cEmployees clsEmployees = new cEmployees(oCurrentUser.AccountID);
            Employee tempEmployee;
            List<Employee> lstEmployees = new List<Employee>();

            using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(sSQL))
            {
                while (reader.Read())
                {
                    audienceEmployeeID = reader.GetInt32(reader.GetOrdinal("audienceEmployeeID"));
                    employeeID = reader.GetInt32(reader.GetOrdinal("employeeID"));

                    tempEmployee = clsEmployees.GetEmployeeById(employeeID);
                    lstEmployees.Add(tempEmployee);
                }

                data.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lstEmployees;
        }

        /// <summary>
        /// Returns a list of budget holders attached to an audience
        /// </summary>
        /// <param name="audienceID"></param>
        /// <returns></returns>
        public List<cBudgetHolder> GetBudgetHoldersByAudienceID(int audienceID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            string sSQL = "SELECT audienceBudgetHolderID, audienceID, budgetHolderID FROM dbo.audienceBudgetHolders WHERE audienceID=@audienceID";
            data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);

            int audienceBudgetHolderID;
            int budgetHolderID;

            cBudgetholders clsBudgetHolders = new cBudgetholders(oCurrentUser.AccountID);
            cBudgetHolder tempBudgetHolder;
            List<cBudgetHolder> lstBudgetHolders = new List<cBudgetHolder>();

            using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(sSQL))
            {
                while (reader.Read())
                {
                    audienceBudgetHolderID = reader.GetInt32(reader.GetOrdinal("audienceBudgetHolderID"));
                    budgetHolderID = reader.GetInt32(reader.GetOrdinal("budgetHolderID"));

                    tempBudgetHolder = clsBudgetHolders.getBudgetHolderById(budgetHolderID);
                    lstBudgetHolders.Add(tempBudgetHolder);
                }

                data.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lstBudgetHolders;
        }

        /// <summary>
        /// Returns a list of teams attached to an audience
        /// </summary>
        /// <param name="audienceID"></param>
        /// <returns></returns>
        public List<cTeam> GetTeamsByAudienceID(int audienceID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            string sSQL = "SELECT audienceTeamID, audienceID, teamID FROM dbo.audienceTeams WHERE audienceID=@audienceID";
            data.sqlexecute.Parameters.AddWithValue("@audienceID", audienceID);
            int audienceTeamID;
            int teamID;

            cTeams clsTeams = new cTeams(oCurrentUser.AccountID);
            cTeam tempTeam;
            List<cTeam> lstTeams = new List<cTeam>();

            using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(sSQL))
            {
                while (reader.Read())
                {
                    audienceTeamID = reader.GetInt32(reader.GetOrdinal("audienceTeamID"));
                    teamID = reader.GetInt32(reader.GetOrdinal("teamID"));

                    tempTeam = clsTeams.GetTeamById(teamID);
                    lstTeams.Add(tempTeam);
                }

                data.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lstTeams;
        }

        /// <summary>
        /// Save a list of audiences for an entity, or edit a single audience record
        /// </summary>
        /// <param name="parentRecordId">The entity record id to assign audiences to</param>
        /// <param name="baseTableID">The table guid for the entity table</param>
        /// <param name="audienceIDs">A list of audience IDs</param>
        /// <param name="canView"></param>
        /// <param name="canEdit"></param>
        /// <param name="canDelete"></param>
        /// <returns>Returns -1 if the audience could not be edited because it would leave the entity record with no full access audience, otherwise the last saved/edited record id or 0 if nothing was processed</returns>
        public int SaveAudienceRecord(int parentRecordId, Guid baseTableID, List<int> audienceIDs, bool canView, bool canEdit, bool canDelete)
        {
            cTables clsTables = new cTables(oCurrentUser.AccountID);
            cTable baseTable = clsTables.GetTableByID(baseTableID);

            DBConnection db = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            int audienceRecordId = 0;

            foreach (int audienceID in audienceIDs)
            {
                if (audienceID > 0)
                {
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@audienceId", audienceID);
                    db.sqlexecute.Parameters.AddWithValue("@tablename", baseTable.TableName);
                    db.sqlexecute.Parameters.AddWithValue("@recordId", parentRecordId);
                    db.sqlexecute.Parameters.AddWithValue("@canView", canView);
                    db.sqlexecute.Parameters.AddWithValue("@canEdit", canEdit);
                    db.sqlexecute.Parameters.AddWithValue("@canDelete", canDelete);
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                    if (oCurrentUser.isDelegate == true)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                    db.sqlexecute.Parameters.Add("@returnID", System.Data.SqlDbType.Int);
                    db.sqlexecute.Parameters["@returnID"].Direction = System.Data.ParameterDirection.ReturnValue;

                    db.ExecuteProc("saveAudienceRecord");

                    audienceRecordId = (int)db.sqlexecute.Parameters["@returnID"].Value;

                    if (audienceRecordId == -1)
                    {
                        break;
                    }
                }
            }

            return audienceRecordId;
        }

        /// <summary>
        /// Delete and entity audience record
        /// </summary>
        /// <param name="recID">The audience record id</param>
        /// <param name="parentRecordID">The entity record id</param>
        /// <param name="baseTableID">The table id of the entity's table</param>
        /// <returns></returns>
        public int DeleteAudienceRecord(int recID, int parentRecordID, Guid baseTableID)
        {
            cTables clsTables = new cTables(oCurrentUser.AccountID);
            cTable baseTable = clsTables.GetTableByID(baseTableID);
            DBConnection db = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            List<string> returnData = new List<string>();

            db.sqlexecute.Parameters.Clear();
            db.sqlexecute.Parameters.AddWithValue("@audienceRecordId", recID);
            db.sqlexecute.Parameters.AddWithValue("@tablename", baseTable.TableName);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            if (oCurrentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.sqlexecute.Parameters.Add("@deleteOK", System.Data.SqlDbType.Bit);
            db.sqlexecute.Parameters["@deleteOK"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.ExecuteProc("deleteAudienceRecord");

            int nDeleteOK = (int)db.sqlexecute.Parameters["@deleteOK"].Value;
            if (nDeleteOK < 1)
                return nDeleteOK;
            else
                return recID;
        }

        /// <summary>
        /// Get an audience record from a certain base table
        /// </summary>
        /// <param name="recordID">The record id</param>
        /// <param name="baseTableID">The table id</param>
        /// <returns>The audience record</returns>
        public cAudienceRecordStatus GetAudienceRecord(int recordID, Guid baseTableID)
        {
            cAudienceRecordStatus recStatus = null;

            cTables clsTables = new cTables(oCurrentUser.AccountID);
            cTable baseTable = clsTables.GetTableByID(baseTableID);
            DBConnection db = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));

            string sql = "select audienceID, canView, canEdit, canDelete from [" + baseTable.TableName + "] where id = @recordID";
            db.sqlexecute.Parameters.AddWithValue("@recordID", recordID);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    recStatus = new cAudienceRecordStatus(recordID, reader.GetInt32(0), 0, reader.GetBoolean(1), reader.GetBoolean(2), reader.GetBoolean(3));
                }
                reader.Close();
            }

            return recStatus;
        }
    }

    [Serializable()]
    public class cAudienceRecordStatus
    {
        private int? nAudienceID;
        private int nStatus;
        private int nRecordID;
        private bool bCanView;
        private bool bCanEdit;
        private bool bCanDelete;

        public cAudienceRecordStatus()
        {

        }

        public cAudienceRecordStatus(int recordid, int? audienceid, int status, bool canview, bool canedit, bool candelete)
        {
            nAudienceID = audienceid;
            nRecordID = recordid;
            nStatus = status;
            bCanDelete = candelete;
            bCanEdit = canedit;
            bCanView = canview;
        }

        #region properties
        public int? AudienceID
        {
            get { return nAudienceID; }
            set { nAudienceID = value; }
        }
        public int Status
        {
            get { return nStatus; }
            set { nStatus = value; }
        }
        public int RecordID
        {
            get { return nRecordID; }
            set { nRecordID = value; }
        }
        public bool CanView
        {
            get { return bCanView; }
            set { bCanView = value; }
        }
        public bool CanEdit
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }
        public bool CanDelete
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }
        #endregion
    }
}
