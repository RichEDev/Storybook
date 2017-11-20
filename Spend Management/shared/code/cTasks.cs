using System;
using System.Data;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Data.SqlClient;
using SpendManagementLibrary.Helpers;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    #region cTaskHistory
    /// <summary>
    /// cTask History class
    /// </summary>
    public class cTaskHistory
    {
        /// <summary>
        /// List of Task History items
        /// </summary>
        readonly List<cTaskHistoryItem> historyitems;
        /// <summary>
        /// Current account id
        /// </summary>
        readonly int nAccountId;

        /// <summary>
        /// Current Employee Record
        /// </summary>
        readonly Employee curEmployee;

        /// <summary>
        /// cTaskHistory constructor
        /// </summary>
        /// <param name="accountid">Customer account id</param>
        /// <param name="employee">Employee record</param>
        public cTaskHistory(int accountid, Employee employee)
        {
            nAccountId = accountid;
            curEmployee = employee;
            historyitems = new List<cTaskHistoryItem>();
        }

        /// <summary>
        /// Add a task history item
        /// </summary>
        /// <param name="taskid">ID of the task</param>
        /// <param name="changedetails">Details of task change</param>
        /// <param name="preval">Previous value</param>
        /// <param name="postval">New Value</param>
        public void AddHistoryItem(int taskid, string changedetails, string preval, string postval)
        {
            var item = new cTaskHistoryItem(taskid, changedetails, preval, postval, curEmployee);
            historyitems.Add(item);
        }

        /// <summary>
        /// Store history records to the database
        /// </summary>
        public void CommitHistory()
        {
            using (var db = new DatabaseConnection(cAccounts.getConnectionString(nAccountId)))
            {
                const string sql = "insert into task_history (taskId, changeDetails, preVal, postVal, changedBy) values (@taskId, @changeDetails, @preVal, @postVal, @changedBy)";
                foreach (cTaskHistoryItem item in historyitems)
                {
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@taskId", item.TaskId);
                    db.sqlexecute.Parameters.AddWithValue("@changeDetails", item.ChangeDetails);
                    db.sqlexecute.Parameters.AddWithValue("@preVal", item.PreviousValue);
                    db.sqlexecute.Parameters.AddWithValue("@postVal", item.NewValue);
                    db.sqlexecute.Parameters.AddWithValue("@changedBy", item.ChangedByUser.EmployeeID);
                    db.ExecuteSQL(sql);
                }
            }

            historyitems.Clear();

            return;
        }
    }

    /// <summary>
    /// cTaskHistoryItem class
    /// </summary>
    [Serializable()]
    public class cTaskHistoryItem
    {
        /// <summary>
        /// Task Id
        /// </summary>
        private int nTaskId;
        /// <summary>
        /// History task detail
        /// </summary>
        private string sChangeDetails;
        /// <summary>
        /// Pre change value
        /// </summary>
        private string sPreVal;
        /// <summary>
        /// Post change value
        /// </summary>
        private string sPostVal;
        /// <summary>
        /// Change made by
        /// </summary>
        private Employee nChangedBy;

        /// <summary>
        /// cTaskHistoryItem constructor
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <param name="changedetails">Task Change Detail</param>
        /// <param name="preval">Pre change value</param>
        /// <param name="postval">Post change value</param>
        /// <param name="changedbyuser"></param>
        public cTaskHistoryItem(int taskid, string changedetails, string preval, string postval, Employee changedbyuser)
        {
            nTaskId = taskid;
            sChangeDetails = changedetails;
            sPreVal = preval;
            sPostVal = postval;
            nChangedBy = changedbyuser;
        }

        #region properties
        /// <summary>
        /// Get Task Id
        /// </summary>
        public int TaskId
        {
            get { return nTaskId; }
        }
        /// <summary>
        /// Get Change Details
        /// </summary>
        public string ChangeDetails
        {
            get { return sChangeDetails; }
        }
        /// <summary>
        /// Get Previous Value
        /// </summary>
        public string PreviousValue
        {
            get { return sPreVal; }
        }
        /// <summary>
        /// Get New Value
        /// </summary>
        public string NewValue
        {
            get { return sPostVal; }
        }
        /// <summary>
        /// Get who change will be made by
        /// </summary>
        public Employee ChangedByUser
        {
            get { return nChangedBy; }
        }
        #endregion
    }
    #endregion

    #region cTasks
    /// <summary>
    /// Tasks class definition
    /// </summary>
    public class cTasks
    {
        /// <summary>
        /// Current account id
        /// </summary>
        private readonly int nAccountID;

        /// <summary>
        /// Current user id
        /// </summary>
        //private int nEmployeeID;

        /// <summary>
        /// Current Sub-Account ID
        /// </summary>
        private readonly int? nSubAccountID;

        /// <summary>
        /// Reference to the cache
        /// </summary>
        private readonly Utilities.DistributedCaching.Cache _cache = new Utilities.DistributedCaching.Cache();

        /// <summary>
        /// The cache couchbase cache key for this item.
        /// </summary>
        public const string CacheKey = "Task";

        /// <summary>
        /// Cache marshalling variable
        /// </summary>
        //private static List<int> accountsCaching;

        /// <summary>
        /// cTasks constructor
        /// </summary>
        /// <param name="accountid">Current customer Account ID</param>
        /// <param name="subaccountid">Active sub-account id (null for all tasks)</param>
        /// <param name="usecaching">Optional parameter to disable caching of tasks</param>
        public cTasks(int accountid, int? subaccountid)
        {
            nAccountID = accountid;
            nSubAccountID = subaccountid;
        }

        #region properties
        /// <summary>
        /// Gets the current sub-account id
        /// </summary>
        public int? SubAccountID
        {
            get { return nSubAccountID; }
        }
        /// <summary>
        /// Gets the current Account ID
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Cache the tasks from the database for the current account and sub-account id
        /// </summary>
        /// <returns></returns>
        private cTask getTaskFromDB(int taskID)
        {
            cTask task = null;

            var clsBaseDefs = new cBaseDefinitions(AccountID, SubAccountID.Value,
                SpendManagementElement.TaskTypes);
            using (var db = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                const string sql =
                    "SELECT taskId, subAccountId, regardingId, regardingArea, taskCreatorId, taskCreationDate, taskTypeId, taskOwnerId, taskOwnerType, subject, description, startDate, dueDate, endDate, statusId, escalated, escalationDate, taskCmdType FROM dbo.tasks where tasks.taskID = @taskID";
                db.sqlexecute.Parameters.AddWithValue("@taskID", taskID);

                using (IDataReader reader = db.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        int creator = reader.GetInt32(reader.GetOrdinal("taskCreatorId"));
                        TaskCommand taskcmd = (TaskCommand) reader.GetInt16(reader.GetOrdinal("taskCmdType"));

                        DateTime created = reader.GetDateTime(reader.GetOrdinal("taskCreationDate"));
                        int regid = reader.GetInt32(reader.GetOrdinal("regardingId"));
                        int regarea = reader.GetInt16(reader.GetOrdinal("regardingArea"));
                        int ownerid = reader.GetInt32(reader.GetOrdinal("taskOwnerId"));
                        int ownertype = reader.GetInt16(reader.GetOrdinal("taskOwnerType"));
                        int nTaskType = 0;
                        cTaskType tasktype = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("taskTypeId")))
                        {
                            nTaskType = reader.GetInt32(reader.GetOrdinal("taskTypeId"));
                            tasktype = (cTaskType) clsBaseDefs.GetDefinitionByID(nTaskType);
                        }
                        else
                        {
                            nTaskType = 0;
                            tasktype = new cTaskType(0, "", null, null, null, null, false);
                        }

                        string subject = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("subject")))
                        {
                            subject = reader.GetString(reader.GetOrdinal("subject"));
                        }
                        string desc = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("description")))
                        {
                            desc = reader.GetString(reader.GetOrdinal("description"));
                        }
                        DateTime? start = null; // ? allows the variable to be null
                        DateTime? due = null;
                        DateTime? end = null;
                        DateTime? escalateddate = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("startDate")))
                        {
                            start = reader.GetDateTime(reader.GetOrdinal("startDate"));
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("dueDate")))
                        {
                            due = reader.GetDateTime(reader.GetOrdinal("dueDate"));
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("endDate")))
                        {
                            end = reader.GetDateTime(reader.GetOrdinal("endDate"));
                        }
                        bool escalated = reader.GetBoolean(reader.GetOrdinal("escalated"));
                        if (!reader.IsDBNull(reader.GetOrdinal("escalationDate")))
                        {
                            escalateddate = reader.GetDateTime(reader.GetOrdinal("escalationDate"));
                        }

                        TaskStatus status = (TaskStatus) reader.GetInt16(reader.GetOrdinal("statusId"));

                        cTeam taskTeam = null;
                        if ((sendType) ownertype == sendType.team)
                        {
                            cTeams teams = new cTeams(AccountID, SubAccountID.Value);
                            taskTeam = teams.GetTeamById(ownerid);
                        }
                        cTaskOwner ownership = new cTaskOwner(ownerid, (sendType) ownertype, taskTeam);
                        task = new cTask(taskID, SubAccountID, taskcmd, creator, created, tasktype, regid,
                            (AppliesTo) regarea, subject, desc, start, due, end, status, ownership, escalated, escalateddate);
                    }

                    reader.Close();
                }
            }

            _cache.Add(AccountID, CacheKey, taskID.ToString(), task);

            return task;
        }

        /// <summary>
        /// Get all tasks assigned to a particular user or team
        /// </summary>
        /// <param name="ownerid">ID to retrieve tasks for</param>
        /// <param name="ownertype">Audience Type of Team or Individual</param>
        /// <returns>Collection of cTask entities</returns>
        /// 
        [Obsolete("",true)]
        public Dictionary<int, cTask> GetTasksForOwnerId(int ownerid, sendType ownertype)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();

            return retList;
        }

        /// <summary>
        /// Get all tasks owned by a user whether directly to them or to a team they are a member of
        /// </summary>
        /// <param name="employeeid">Employee ID to retrieve tasks for</param>
        /// <returns>Collection of cTask entities</returns>
        public Dictionary<int, cTask> GetTasksForUserId(int employeeid)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();
            
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));

            data.sqlexecute.Parameters.AddWithValue("@taskOwnerType1", (byte)sendType.team);
            data.sqlexecute.Parameters.AddWithValue("@taskOwnerType2", (byte)sendType.employee);
            if (SubAccountID.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@subAccountID", SubAccountID.Value);
            }
            data.sqlexecute.Parameters.AddWithValue("@regardingArea", (int)AppliesTo.Employee);
            data.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);

            using (SqlDataReader reader = data.GetStoredProcReader("GetTasksForUserId"))
            {
                data.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    int taskid = reader.GetInt32(0);
                    retList.Add(taskid, GetTaskById(taskid));
                }
                reader.Close();
            }
            return retList;
        }

        /// <summary>
        /// Establish if a user has any currently active tasks
        /// </summary>
        /// <param name="employeeid">Employee ID of who to check for tasks</param>
        /// <returns>TRUE if user has tasks, otherwise FALSE</returns>
        public bool UserHasTasks(int employeeid)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));

            string sql = "select count(*) from tasks where ((taskOwnerType = " + (byte)sendType.team + " and taskOwnerId in (select teams.teamid from teams inner join teamemps on teamemps.[teamid] = teams.teamid where employeeid = @employeeid)) or (taskOwnerType = " + (byte)sendType.employee + " and taskOwnerID = @employeeid)) and (statusId = 0 or statusId = 1 or statusId = 2)";
            if (SubAccountID.HasValue)
            {
                sql += " and ((subAccountId is null AND regardingArea = @regardingArea) OR [subAccountId] = @locid)";
                data.sqlexecute.Parameters.AddWithValue("@locid", SubAccountID.Value);
                data.sqlexecute.Parameters.AddWithValue("@regardingArea", (int)AppliesTo.Employee);
            }
            data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            int count = data.getcount(sql);
            data.sqlexecute.Parameters.Clear();
            if (count == 0)
            {
                return false;
            }
            return true;          
        }

        /// <summary>
        /// Gets tasks of a particular status for the current user or all users
        /// </summary>
        /// <param name="status">Status to retrieve tasks</param>
        /// <param name="employeeid">Employee to obtain tasks for (-1 if for all users)</param>
        /// <returns>Collection of cTask entities</returns>
        public Dictionary<int, cTask> GetTasksByStatus(TaskStatus status, int employeeid)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();
            Dictionary<int, cTask> userTasks = (employeeid == -1 ? new Dictionary<int, cTask>() : GetTasksForUserId(employeeid));
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));

            string sql = "select taskID from tasks where statusid = @statusid";
            if (SubAccountID.HasValue)
            {
                sql += " and ((subAccountId is null AND regardingArea = @regardingArea) OR [subAccountId] = @locid)";
                data.sqlexecute.Parameters.AddWithValue("@locid", SubAccountID.Value);
                data.sqlexecute.Parameters.AddWithValue("@regardingArea", (int)AppliesTo.Employee);
            }
            data.sqlexecute.Parameters.AddWithValue("@statusid", (byte)status);
            using (SqlDataReader reader = data.GetReader(sql))
            {
                data.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    int taskid = reader.GetInt32(0);
                    cTask curTask = GetTaskById(taskid);

                    if (employeeid != -1)
                    {
                        // need to check if current user is part of the ownership
                        if (userTasks.ContainsKey(curTask.TaskId))
                        {
                            retList.Add(curTask.TaskId, curTask);
                        }
                    }
                    else
                    {
                        retList.Add(curTask.TaskId, curTask);
                    }
                }
                reader.Close();
            }
            return retList;
        }

        /// <summary>
        /// Get tasks for summary display for particular application area, record and status
        /// </summary>
        /// <param name="appArea">Application area to retrieve tasks for</param>
        /// <param name="parentId">Record Id associated to the task</param>
        /// <param name="status">Tasks status to obtain summary for</param>
        /// <returns>Dictionary collection of cTask entities</returns>
        public Dictionary<int, cTask> GetTaskSummary(AppliesTo appArea, int parentId, TaskStatus status)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));
            string sql = "select taskID from tasks where regardingArea = @regardingarea and regardingID = @regardingID and statusID = @statusID";
            if (SubAccountID.HasValue)
            {
                if (appArea == AppliesTo.Employee)
                {
                    sql += " and [subAccountId] is null";
                }
                else
                {
                    sql += " and [subAccountId] = @locid";
                    data.sqlexecute.Parameters.AddWithValue("@locid", SubAccountID.Value);
                }
            }
            data.sqlexecute.Parameters.AddWithValue("@regardingarea", (byte)appArea);
            data.sqlexecute.Parameters.AddWithValue("@regardingID", parentId);
            data.sqlexecute.Parameters.AddWithValue("@statusID", (byte)status);
            using (SqlDataReader reader = data.GetReader(sql))
            {
                data.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    int taskid = reader.GetInt32(0);
                    retList.Add(taskid, GetTaskById(taskid));
                }
                reader.Close();
            }
            return retList;
        }

        /// <summary>
        /// Convert the current task collection to a DataSet
        /// </summary>
        /// <param name="srcCollection">Dictionary collection of cTask</param>
        /// <returns>A DataSet</returns>
        /// 
        [Obsolete("",true)]
        public DataSet ConvertToDataSet(Dictionary<int, cTask> srcCollection)
        {
            DataSet retDS = new DataSet();
            DataTable dTable = new DataTable();

            dTable.Columns.Add(new DataColumn("taskId", typeof(int)));
            dTable.Columns.Add(new DataColumn("regardingId", typeof(int)));
            dTable.Columns.Add(new DataColumn("subject", typeof(string)));
            dTable.Columns.Add(new DataColumn("description", typeof(string)));
            dTable.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dTable.Columns.Add(new DataColumn("dueDate", typeof(DateTime)));
            dTable.Columns.Add(new DataColumn("endDate", typeof(DateTime)));
            dTable.Columns.Add(new DataColumn("createdDate", typeof(DateTime)));
            dTable.Columns.Add(new DataColumn("createdBy", typeof(int)));
            dTable.Columns.Add(new DataColumn("escalated", typeof(bool)));
            dTable.Columns.Add(new DataColumn("escalatedDate", typeof(DateTime)));
            dTable.Columns.Add(new DataColumn("taskOwnerId", typeof(int)));
            dTable.Columns.Add(new DataColumn("taskOwnerType", typeof(sendType)));
            dTable.Columns.Add(new DataColumn("taskTypeDescription", typeof(string)));
            dTable.Columns.Add(new DataColumn("tasksStatusId", typeof(int)));
            dTable.Columns.Add(new DataColumn("regardingArea", typeof(AppAreas)));

            foreach (KeyValuePair<int, cTask> i in srcCollection)
            {
                cTask curTask = (cTask)i.Value;

                object[] vals = { curTask.TaskId, curTask.RegardingId, curTask.Subject, curTask.Description, curTask.StartDate, curTask.DueDate, curTask.EndDate, curTask.TaskCreatedDate, curTask.TaskCreator, curTask.TaskEscalated, curTask.TaskEscalatedDate, curTask.TaskOwner.OwnerId, curTask.TaskOwner.OwnerType, curTask.TaskType.Description, curTask.StatusId, curTask.RegardingArea };
                dTable.Rows.Add(vals);
            }

            retDS.Tables.Add(dTable);

            return retDS;
        }

        /// <summary>
        /// Get all tasks created by a particular user
        /// </summary>
        /// <param name="employeeId">User ID of the </param>
        /// <returns>Collection of tasks</returns>
        public Dictionary<int, cTask> GetTaskByCreatorId(int employeeId)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();
            DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));

            //string sql = "select taskID from tasks where taskOwnerType = " + (byte)sendType.employee + " and taskOwnerID = @employeeid";
            const string sql = "select taskID from tasks where taskCreatorId = @employeeID";
            data.sqlexecute.Parameters.AddWithValue("@employeeID", employeeId);
            using (SqlDataReader reader = data.GetReader(sql))
            {
                data.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    int taskid = reader.GetInt32(0);
                    retList.Add(taskid, GetTaskById(taskid));
                }
                reader.Close();
            }
            return retList;
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="taskId">ID of task to delete</param>
        /// <param name="employeeid">ID of employee requesting deletion</param>
        public void DeleteTask(int taskId, int employeeid)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cTask delTask = GetTaskById(taskId);
            
            cAuditLog ALog = new cAuditLog(AccountID, employeeid);

            if (delTask.TaskOwner.OwnerType == sendType.team)
            {
                cTeams teams = new cTeams(AccountID);
                cTeam team = teams.GetTeamById(delTask.TaskOwner.OwnerId);
            }

            db.sqlexecute.Parameters.AddWithValue("@taskId", taskId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("deleteTask");
            ALog.deleteRecord(SpendManagementElement.Tasks, taskId, delTask.Subject);

            removeFromCache(taskId);
        }

        /// <summary>
        /// Remove an entry from cache (quicker than the dependencies)
        /// </summary>
        /// <param name="taskID"></param>
        public void removeFromCache(int taskID)
        {
            _cache.Delete(AccountID, CacheKey, taskID.ToString());
        }

        /// <summary>
        /// Add a new task
        /// </summary>
        /// <param name="newTask">Task item to be added</param>
        /// <param name="employeeid">ID of employee adding task</param>
        /// <returns></returns>
        public int AddTask(cTask newTask, int employeeid)
        {
            cEmployees clsemployees = new cEmployees(AccountID);
            Employee curEmployee = clsemployees.GetEmployeeById(employeeid);

            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            db.sqlexecute.Parameters.AddWithValue("@taskId", newTask.TaskId);
            db.sqlexecute.Parameters.AddWithValue("@regardingId", newTask.RegardingId);

            if (newTask.SubAccountID != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@subAccountId", newTask.SubAccountID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@subAccountId", DBNull.Value);
            }

            if (newTask.TaskCreator != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@userId", newTask.TaskCreator);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@userId", DBNull.Value);
            }

            db.sqlexecute.Parameters.AddWithValue("@taskCmdType", newTask.TaskCommandType);
            //db.sqlexecute.Parameters.AddWithValue("@taskCreationDate", newTask.TaskCreatedDate.ToShortDateString(), "D", false);
            if (newTask.TaskType != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@taskTypeId", newTask.TaskType.ID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@taskTypeId", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@regardingArea", (int)newTask.RegardingArea);
            db.sqlexecute.Parameters.AddWithValue("@subject", newTask.Subject);
            db.sqlexecute.Parameters.AddWithValue("@description", newTask.Description);
            db.sqlexecute.Parameters.AddWithValue("@taskOwnerId", newTask.TaskOwner.OwnerId);
            db.sqlexecute.Parameters.AddWithValue("@taskOwnerType", (int)newTask.TaskOwner.OwnerType);
            db.sqlexecute.Parameters.AddWithValue("@statusId", (int)newTask.StatusId);
            if (newTask.StartDate.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@startDate", newTask.StartDate.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@startDate", DBNull.Value);
            }
            if (newTask.DueDate.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@dueDate", newTask.DueDate);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@dueDate", DBNull.Value);
            }
            if (newTask.EndDate.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@endDate", newTask.EndDate);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@endDate", DBNull.Value);
            }
            if (newTask.TaskEscalatedDate.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@escalationDate", newTask.TaskEscalatedDate.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@escalationDate", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@escalated", newTask.TaskEscalated);
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.sqlexecute.Parameters.Add("@Identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@Identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveTask");

            int newTaskId = (int)db.sqlexecute.Parameters["@Identity"].Value;
            newTask.TaskId = newTaskId;
            
            cTaskHistory history = new cTaskHistory(AccountID, curEmployee);
            history.AddHistoryItem(newTaskId, "Task Created", "", newTask.Subject);
            history.CommitHistory();

            cPendingEmails clsPendingEmails = null;

            switch (newTask.TaskOwner.OwnerType)
            {
                case sendType.employee:
                case sendType.approver:
                case sendType.budgetHolder:
                case sendType.itemOwner:
                case sendType.lineManager:
                    if (newTask.TaskOwner.OwnerId != newTask.TaskCreator)
                    {
                        clsPendingEmails = new cPendingEmails(AccountID, employeeid);
                        clsPendingEmails.CreatePendingTaskEmail(PendingMailType.TaskAllocateNew, newTask);
                    }
                    break;
                case sendType.team:
                    clsPendingEmails = new cPendingEmails(AccountID, employeeid);
                    clsPendingEmails.CreatePendingTaskEmail(PendingMailType.TaskAllocateNew, newTask);
                    break;
            }

            
            return newTaskId;
        }

        /// <summary>
        /// Update Task in the database
        /// </summary>
        /// <param name="editTask">Task to be updated</param>
        /// <param name="employeeid">ID of employee updating the task</param>
        public void UpdateTask(cTask editTask, int employeeid)
        {
            cEmployees clsemployees = new cEmployees(AccountID);
            Employee curEmployee = clsemployees.GetEmployeeById(employeeid);
            
            bool firstchange = true;

            cTask task = getUnchangedDBTask(editTask.TaskId);
            if (task != null)
            {
                cTaskHistory history = new cTaskHistory(AccountID, curEmployee);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

                cAuditLog ALog = new cAuditLog(AccountID, employeeid);

                db.sqlexecute.Parameters.AddWithValue("@taskId", editTask.TaskId);
                db.sqlexecute.Parameters.AddWithValue("@regardingId", editTask.RegardingId);

                if (editTask.SubAccountID != null)
                {
                    db.sqlexecute.Parameters.AddWithValue("@subAccountId", editTask.SubAccountID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@subAccountId", DBNull.Value);
                }

                if (editTask.TaskCreator != null)
                {
                    db.sqlexecute.Parameters.AddWithValue("@userId", editTask.TaskCreator);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@userId", DBNull.Value);
                }

                db.sqlexecute.Parameters.AddWithValue("@taskCmdType", (int)editTask.TaskCommandType);

                string postVal;
                if (editTask.StartDate.HasValue)
                {
                    db.sqlexecute.Parameters.AddWithValue("@startDate", editTask.StartDate.Value);
                    postVal = editTask.StartDate.Value.ToShortDateString();
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@startDate", DBNull.Value);
                    postVal = "";
                }

                string preVal;
                if (task.StartDate != editTask.StartDate)
                {
                    if (task.StartDate.HasValue)
                    {
                        preVal = task.StartDate.Value.ToShortDateString();
                    }
                    else
                    {
                        preVal = "";
                    }

                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("56578805-3F20-48EC-B158-632ED5A4D0EE"), preVal, postVal);
                    history.AddHistoryItem(task.TaskId, "Task Start Date", preVal, postVal);
                    firstchange = false;
                }


                if (editTask.DueDate.HasValue)
                {
                    db.sqlexecute.Parameters.AddWithValue("@dueDate", editTask.DueDate.Value);
                    postVal = editTask.DueDate.Value.ToShortDateString();
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@dueDate", DBNull.Value);
                    postVal = "";
                }

                if (task.DueDate != editTask.DueDate)
                {
                    if (task.DueDate.HasValue)
                    {
                        preVal = task.DueDate.Value.ToShortDateString();
                    }
                    else
                    {
                        preVal = "";
                    }

                    history.AddHistoryItem(task.TaskId, "Task Due Date", preVal, postVal);
                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("09CC1B73-93DB-44FE-85E8-7874719C5F6B"), preVal, postVal);
                    firstchange = false;
                }

                if (editTask.EndDate.HasValue)
                {
                    db.sqlexecute.Parameters.AddWithValue("@endDate", editTask.EndDate.Value);
                    postVal = editTask.EndDate.Value.ToShortDateString();
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@endDate", DBNull.Value);
                    postVal = "";
                }

                if (task.EndDate != editTask.EndDate)
                {
                    if (task.EndDate.HasValue)
                    {
                        preVal = task.EndDate.Value.ToShortDateString();
                    }
                    else
                    {
                        preVal = "";
                    }

                    history.AddHistoryItem(task.TaskId, "Task End Date", preVal, postVal);
                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("6A52E48D-FCDA-4F37-BA32-C4D687B63FAD"), preVal, postVal);
                    firstchange = false;
                }

                db.sqlexecute.Parameters.AddWithValue("@subject", editTask.Subject);

                if (task.Subject != editTask.Subject)
                {
                    history.AddHistoryItem(task.TaskId, "Subject", task.Subject, editTask.Subject);
                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("56578805-3F20-48EC-B158-632ED5A4D0EE"), task.Subject, editTask.Subject);
                    firstchange = false;
                }

                db.sqlexecute.Parameters.AddWithValue("@description", editTask.Description);

                if (task.Description != editTask.Description)
                {
                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("A22803ED-95A8-45EE-AA18-12D622CAFDB3"), task.Description, editTask.Description);
                    history.AddHistoryItem(task.TaskId, "Task Description", task.Description, editTask.Description);
                    firstchange = false;
                }

                if (editTask.TaskType != null)
                {
                    if (editTask.TaskType.ID > 0)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@taskTypeId", editTask.TaskType.ID);
                        postVal = editTask.TaskType.Description;
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@taskTypeId", DBNull.Value);
                        postVal = "";
                    }
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@taskTypeId", DBNull.Value);
                    postVal = "";
                }

                if (task.TaskType != editTask.TaskType)
                {
                    if (task.TaskType != null)
                    {
                        preVal = task.TaskType.Description;
                    }
                    else
                    {
                        preVal = "";
                    }

                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("86743EEE-4D0A-4C95-AAA8-B1093F844600"), preVal, postVal);

                    firstchange = false;
                    history.AddHistoryItem(task.TaskId, "Task Type", preVal, postVal);
                }

                db.sqlexecute.Parameters.AddWithValue("@statusId", (int)editTask.StatusId);

                if (task.StatusId != editTask.StatusId)
                {
                    preVal = task.GetStatusDescription;
                    postVal = editTask.GetStatusDescription;

                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("D4F3DD3A-F614-4446-AC56-6FFA46B05344"), preVal, postVal);
                    firstchange = false;
                    history.AddHistoryItem(task.TaskId, "Task Status", preVal, postVal);
                }

                cTeams teams = new cTeams(AccountID, SubAccountID);

                switch (editTask.TaskOwner.OwnerType)
                {
                    case sendType.employee:
                    case sendType.approver:
                    case sendType.budgetHolder:
                    case sendType.itemOwner:
                    case sendType.lineManager:
                        Employee postUser = clsemployees.GetEmployeeById(editTask.TaskOwner.OwnerId);
                        postVal = postUser.Forename + " " + postUser.Surname;
                        break;
                    case sendType.team:
                        cTeam postTeam = teams.GetTeamById(editTask.TaskOwner.OwnerId);
                        postVal = postTeam.teamname;
                        break;
                    default:
                        break;
                }
                db.sqlexecute.Parameters.AddWithValue("@regardingArea", (int)editTask.RegardingArea);
                db.sqlexecute.Parameters.AddWithValue("@taskOwnerId", editTask.TaskOwner.OwnerId);
                db.sqlexecute.Parameters.AddWithValue("@taskOwnerType", (int)editTask.TaskOwner.OwnerType);

                if (task.TaskOwner.OwnerId != editTask.TaskOwner.OwnerId)
                {
                    switch (task.TaskOwner.OwnerType)
                    {
                        case sendType.employee:
                        case sendType.approver:
                        case sendType.budgetHolder:
                        case sendType.itemOwner:
                        case sendType.lineManager:
                            Employee preUser = clsemployees.GetEmployeeById(task.TaskOwner.OwnerId);
                            preVal = preUser.FullName;
                            break;
                        case sendType.team:
                            cTeam preTeam = teams.GetTeamById(task.TaskOwner.OwnerId);
                            preVal = "**" + preTeam.teamname;
                            break;
                        default:
                            preVal = "Unknown";
                            break;
                    }

                    firstchange = false;
                    ALog.editRecord(editTask.TaskId, editTask.Subject, SpendManagementElement.Tasks, new Guid("E5309530-9949-4EC9-864C-0ACC03D50CA2"), preVal, postVal);
                    history.AddHistoryItem(task.TaskId, "Task Ownership", preVal, postVal);
                    cPendingEmails pendingemail = new cPendingEmails(AccountID, curEmployee.EmployeeID);
                    pendingemail.CreatePendingTaskEmail(PendingMailType.TaskOwnerDeallocate, task);
                    pendingemail.CreatePendingTaskEmail(PendingMailType.TaskOwnerAllocate, editTask);
                }
                
                if (!firstchange)
                {
                    //db.sqlexecute.Parameters.AddWithValue("@userId", employeeid);
                    db.sqlexecute.Parameters.AddWithValue("@escalated", editTask.TaskEscalated);
                    if (editTask.TaskEscalatedDate != null)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@escalationDate", editTask.TaskEscalatedDate);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@escalationDate", DBNull.Value);
                    }
                    //db.sqlexecute.Parameters.AddWithValue("@taskStatus", (int)editTask.StatusId);

                    history.CommitHistory();
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@escalated", editTask.TaskEscalated);
                    if (editTask.TaskEscalatedDate != null)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@escalationDate", editTask.TaskEscalatedDate);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@escalationDate", DBNull.Value);
                    }
                }

                CurrentUser currentUser = cMisc.GetCurrentUser();

                if (currentUser != null)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate == true)
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                db.ExecuteProc("saveTask");
            }

            removeFromCache(editTask.TaskId);
        }

        /// <summary>
		/// getUnchangedDBTask
        /// </summary>
        /// <param name="taskId">ID of the task to obtain</param>
        /// <returns>cTask class entity</returns>
        private cTask getUnchangedDBTask(int taskId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(AccountID, SubAccountID.Value, SpendManagementElement.TaskTypes);
            cTask oldtask = null;
            const string strSQL = "SELECT taskId, subAccountId, regardingId, regardingArea, taskCreatorId, taskCmdType, taskCreationDate, taskOwnerId, taskOwnerType, taskTypeId, subject, description, startDate, dueDate, endDate, escalated, escalationDate, statusId FROM tasks WHERE taskId = @taskID";
            db.sqlexecute.Parameters.AddWithValue("@taskID", taskId);

            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                int taskIdOrd = reader.GetOrdinal("taskId");
                int subAccountIdOrd = reader.GetOrdinal("subAccountId");
                int regardingIdOrd = reader.GetOrdinal("regardingId");
                int regardingAreaOrd = reader.GetOrdinal("regardingArea");
                int taskCreatorIdOrd = reader.GetOrdinal("taskCreatorId");
                int taskCreationDateOrd = reader.GetOrdinal("taskCreationDate");
                int taskTypeIdOrd = reader.GetOrdinal("taskTypeId");
                int taskOwnerIdOrd = reader.GetOrdinal("taskOwnerId");
                int taskOwnerTypeOrd = reader.GetOrdinal("taskOwnerType");
                int subjectOrd = reader.GetOrdinal("subject");
                int descriptionOrd = reader.GetOrdinal("description");
                int startDateOrd = reader.GetOrdinal("startDate");
                int dueDateOrd = reader.GetOrdinal("dueDate");
                int endDateOrd = reader.GetOrdinal("endDate");
                int statusIdOrd = reader.GetOrdinal("statusId");
                int escalatedOrd = reader.GetOrdinal("escalated");
                int escalationDateOrd = reader.GetOrdinal("escalationDate");
                int taskCmdTypeOrd = reader.GetOrdinal("taskCmdType");

                while (reader.Read())
                {
                    int taskid = reader.GetInt32(taskIdOrd);
                    int creator = reader.GetInt32(taskCreatorIdOrd);
                    TaskCommand taskcmd = (TaskCommand)reader.GetInt16(taskCmdTypeOrd);
                    DateTime created = reader.GetDateTime(taskCreationDateOrd);
                    int regid = reader.GetInt32(regardingIdOrd);
                    int regarea = reader.GetInt16(regardingAreaOrd);
                    int ownerid = reader.GetInt32(taskOwnerIdOrd);
                    int ownertype = reader.GetInt16(taskOwnerTypeOrd);
                    cTaskType tasktype = reader.IsDBNull(taskTypeIdOrd)
                                             ? new cTaskType(0, "", null, null, null, null, false)
                                             : (cTaskType)clsBaseDefs.GetDefinitionByID(reader.GetInt32(taskTypeIdOrd));

                    string subject = reader.IsDBNull(subjectOrd) ? string.Empty : reader.GetString(subjectOrd);
                    string desc = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
                    DateTime? start = null;
                    DateTime? due = null;
                    DateTime? end = null;
                    DateTime? escalateddate = null;
                    if (!reader.IsDBNull(startDateOrd))
                    {
                        start = reader.GetDateTime(startDateOrd);
                    }
                    if (!reader.IsDBNull(dueDateOrd))
                    {
                        due = reader.GetDateTime(dueDateOrd);
                    }
                    if (!reader.IsDBNull(endDateOrd))
                    {
                        end = reader.GetDateTime(endDateOrd);
                    }
                    bool escalated = reader.GetBoolean(escalatedOrd);
                    if (!reader.IsDBNull(escalationDateOrd))
                    {
                        escalateddate = reader.GetDateTime(escalationDateOrd);
                    }

                    TaskStatus status = (TaskStatus)reader.GetInt16(statusIdOrd);

                    cTeam taskTeam = null;
                    if ((sendType)ownertype == sendType.team)
                    {
                        cTeams teams = new cTeams(AccountID, SubAccountID);
                        taskTeam = teams.GetTeamById(ownerid);
                    }
                    cTaskOwner ownership = new cTaskOwner(ownerid, (sendType)ownertype, taskTeam);
                    oldtask = new cTask(taskid, SubAccountID, taskcmd, creator, created, tasktype, regid, (AppliesTo)regarea, subject, desc, start, due, end, status, ownership, escalated, escalateddate);
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();

            return oldtask;
        }

        /// <summary>
        /// Get task record by Id
        /// </summary>
        /// <param name="taskId">ID of the task to retrieve</param>
        /// <returns>A cTask entity</returns>
        public cTask GetTaskById(int taskId)
        {
            return _cache.Get(AccountID, CacheKey, taskId.ToString()) as cTask ?? getTaskFromDB(taskId);
        }

        /// <summary>
        /// Gets current tasks by Command Type and Task Status
        /// </summary>
        /// <param name="cmdtype">Task Command Type</param>
        /// <param name="status">Task Status to match</param>
        /// <returns>Collection of tasks matching the criteria</returns>
        /// 
        [Obsolete("",true)]
        public Dictionary<int, cTask> getTasksByCmdType(TaskCommand cmdtype, TaskStatus status)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();

            return retList;
        }

        /// <summary>
        /// Gets current tasks by Command Type
        /// </summary>
        /// <param name="cmdtype">Task Command Type</param>
        /// <returns>Collection of tasks matching the criteria</returns>
        /// 
        [Obsolete("",true)]
        public Dictionary<int, cTask> getTasksByCmdType(TaskCommand cmdtype)
        {
            Dictionary<int, cTask> retList = new Dictionary<int, cTask>();


            return retList;
        }

        /// <summary>
        /// Update tasks as completed
        /// </summary>
        /// <param name="taskIds">List of Task Ids to set to complete</param>
        /// <param name="employeeid">Employee Id performing the update</param>
        public void setTasksToComplete(List<int> taskIds, int employeeid)
        {
            //cTasks tasks = new cTasks(AccountID, null);

            foreach (int taskId in taskIds)
            {
                cTask taskToComplete = GetTaskById(taskId);
                if(!taskToComplete.isTaskCreator(employeeid) && !taskToComplete.isTaskOwner(employeeid)) continue;

                taskToComplete.StatusId = TaskStatus.Completed;
                UpdateTask(taskToComplete, employeeid);
            }
        }

		/// <summary>
		/// taskExists: Check whether a particular task already exists
		/// </summary>
		/// <param name="regardingID">Record ID associated with the task</param>
		/// <param name="regardingArea">Area of the application the task is associated with</param>
		/// <param name="cmdType">Task Command Type</param>
		/// <param name="taskOwnerID">Team or Employee ID that 'owns' the task</param>
		/// <param name="taskOwnerType">Is the task owner a team or employee</param>
        /// <param name="ignoreClosedTasks">If true, complete or cancelled tasks will return as not exists</param>
		/// <returns>True if task already exists, otherwise returns False</returns>
        public bool taskExists(int regardingID, AppliesTo regardingArea, TaskCommand cmdType, int taskOwnerID, sendType taskOwnerType, bool ignoreClosedTasks)
		{
		    DBConnection data = new DBConnection(cAccounts.getConnectionString(AccountID));

		    string sql = "select count(taskId) from tasks where regardingId = @regardingId and regardingArea = @regardingArea and taskCmdType = @commandtype and taskOwnerId = @ownerid and taskOwnerType = @ownertype";
		    if(ignoreClosedTasks)
		    {
		        sql += " and statusId <> " + (byte) TaskStatus.Completed + " and statusId <> " + (byte) TaskStatus.Cancelled;
		    }
		    data.sqlexecute.Parameters.AddWithValue("@regardingId", regardingID);
		    data.sqlexecute.Parameters.AddWithValue("@regardingArea", (byte) regardingArea);
		    data.sqlexecute.Parameters.AddWithValue("@commandtype", (byte) cmdType);
		    data.sqlexecute.Parameters.AddWithValue("@ownerid", taskOwnerID);
		    data.sqlexecute.Parameters.AddWithValue("@ownertype", (byte) taskOwnerType);
		    int count = data.getcount(sql);
		    data.sqlexecute.Parameters.Clear();
		    if(count == 0)
		    {
		        return false;
		    }

		    return true;
		}

        /// <summary>
        /// getTasksByType
        /// </summary>
        /// <param name="regarding"></param>
        /// <param name="status"></param>
        /// <returns>Empty string - function incomplete</returns>
        public string getTasksByType(int regarding, TaskStatus status)
        {
			return "";
        }
    }
    #endregion

}
