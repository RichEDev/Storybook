using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using SpendManagementLibrary;
using System.Diagnostics;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    /// <summary>
    /// cPendingEmails 
    /// </summary>
    public class cPendingEmails
    {
        /// <summary>
        /// Customer Account Id
        /// </summary>
        private int nAccountId;
        /// <summary>
        /// Current Employee record
        /// </summary>
        private Employee curEmployee;


        /// <summary>
        /// Reference to the cache
        /// </summary>
        private readonly Utilities.DistributedCaching.Cache _cache = new Utilities.DistributedCaching.Cache();

        private readonly IDataFactory<IProductModule, Modules> _productModuleFactory =
            FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>();

        /// <summary>
        /// The cache couchbase cache key for this item.
        /// </summary>
        public const string CacheKey = "Task";

        #region properties
        /// <summary>
        /// Gets the current customer account ID
        /// </summary>
        public int AccountID
        {
            get { return nAccountId; }
        }
        /// <summary>
        /// Gets the current employee record
        /// </summary>
        public Employee Employee
        {
            get { return curEmployee; }
        }
        #endregion

        /// <summary>
        /// cPendingEmails constructor
        /// </summary>
        /// <param name="accountid">Custom Account Id</param>
        /// <param name="employeeid">Employee Id</param>
        public cPendingEmails(int accountid, int? employeeid)
        {
            nAccountId = accountid;

            cEmployees clsemployees = new cEmployees(AccountID);
            if (employeeid.HasValue)
            {
                curEmployee = clsemployees.GetEmployeeById(employeeid.Value);
            }
            else
            {
                curEmployee = null;
            }
        }

        /// <summary>
        /// GetPendingEmails: Get collection of pending emails
        /// </summary>
        /// <returns>Collection of cPendingEmail entities</returns>
        public Dictionary<int, cPendingEmail> GetPendingEmails()
        {
            Dictionary<int, cPendingEmail> emails = new Dictionary<int, cPendingEmail>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            string sql = "SELECT * FROM pending_email";

            System.Data.SqlClient.SqlDataReader reader;
            using (reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int emailId = reader.GetInt32(reader.GetOrdinal("emailId"));
                    PendingMailType emailType = (PendingMailType)reader.GetInt32(reader.GetOrdinal("emailType"));
                    DateTime datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
                    string subject = reader.GetString(reader.GetOrdinal("subject"));
                    string body = reader.GetString(reader.GetOrdinal("body"));
                    int recipientId = reader.GetInt32(reader.GetOrdinal("recipientId"));
                    sendType recipienttype = (sendType)reader.GetInt32(reader.GetOrdinal("recipientType"));

                    cPendingEmail pendingemail = new cPendingEmail(emailId, emailType, datestamp, subject, body, recipientId, recipienttype);
                    emails.Add(emailId, pendingemail);
                }
                reader.Close();
            }
            return emails;
        }

        /// <summary>
        /// Create a pending task email
        /// </summary>
        /// <param name="mailtype">Email Type</param>
        /// <param name="task">Task entity to create email for</param>
        /// <returns>ID of the created pending email</returns>
        public int CreatePendingTaskEmail(PendingMailType mailtype, cTask task)
        {
            int retVal = 0;

            switch (mailtype)
            {
                case PendingMailType.TaskOverdue:
                    retVal = CreateOverdueTaskEmail(task);
                    break;
                case PendingMailType.TaskOwnerAllocate:
                    retVal = CreateOwnerAllocateEmail(task, false);
                    break;
                case PendingMailType.TaskOwnerDeallocate:
                    retVal = CreateOwnerDeallocateEmail(task);
                    break;
                case PendingMailType.TaskStatusChange:
                    retVal = CreateStatusChangeEmail(task);
                    break;
                case PendingMailType.TaskLate:
                    retVal = CreateLateTaskEmail(task);
                    break;
                case PendingMailType.TaskEmployeeActivate:

                    break;
                case PendingMailType.TaskEmployeeArchive:

                    break;
                case PendingMailType.TaskAllocateNew:
                    retVal = CreateOwnerAllocateEmail(task, true);

                    break;
                default:
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Create a status change email
        /// </summary>
        /// <param name="task">Task entity</param>
        /// <returns>ID of created email</returns>
        private int CreateStatusChangeEmail(cTask task)
        {
            int pendingEmailId = 0;
            StringBuilder sMessage = new StringBuilder();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string brandName = "Framework";
            if (currentUser != null)
            {
                var module = this._productModuleFactory[currentUser.CurrentActiveModule];
                brandName = (module != null) ? module.BrandName : "Framework";
            }
            else
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] != null)
                {
                    brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"];
                }
            }

            if (task != null && task.TaskOwner != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                cAccountProperties accProperties = new cAccountProperties();

                db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@recipientType", (int)task.TaskOwner.OwnerType);
                db.sqlexecute.Parameters.AddWithValue("@recipientId", task.TaskOwner.OwnerId);
                db.sqlexecute.Parameters.AddWithValue("@subject", "Status change for task : [" + task.Subject + "]");
                db.sqlexecute.Parameters.AddWithValue("@emailType", (int)PendingMailType.TaskStatusChange);

                StringBuilder body = new StringBuilder();

                body.Append("A task in " + brandName + " has had it's status changed by " + Employee.Forename + " " + Employee.Surname + ".\n\n");
                body.Append("You are currently included in the task ownership and are being notified that the status is now set as '" + task.StatusId.ToString() + "'\n\n");

                if (accProperties.ApplicationURL != null)
                {
                    body.Append("Click the link below to view the task\n\n\n");
                    string slash = "/";
                    if (accProperties.ApplicationURL.EndsWith("/"))
                    {
                        slash = "";
                    }
                    body.Append(accProperties.ApplicationURL + slash + "tasks/ViewTask.aspx?tid=" + task.TaskId.ToString() + "&rid=" + task.RegardingId.ToString() + "&rtid=" + ((int)task.RegardingArea).ToString());
                }

                body.Append("\n\n\n");
                body.Append("*** THIS IS AN AUTOMATED EMAIL - DO NOT REPLY ***");
                db.sqlexecute.Parameters.AddWithValue("@body", body.ToString());
                db.ExecuteProc("savePendingEmail");
                pendingEmailId = (int)db.sqlexecute.Parameters["@Identity"].Value;
            }
            else
            {
                sMessage.Append("Spend_Management.cPendingEmails.CreateStatusChangeEmail(cTask task): An error occurred whilst trying to create an automated email ready to send by the scheduler of a \"Task Status Change\". The following information may help with diagnosing the issue - \n");

                sMessage.Append("[AccountID] = [" + AccountID.ToString() + "];\n");

                if (task != null)
                {
                    sMessage.Append("[task.TaskId] = [" + task.TaskId + "];\n");
                    sMessage.Append("[task.TaskCreator] = [" + task.TaskCreator + "];\n");
                    sMessage.Append("[task.Subject] = [" + task.Subject + "];\n");
                    sMessage.Append("[task.Description] = [" + task.Description + "];\n");
                }

                if (task.TaskOwner != null)
                {
                    sMessage.Append("[task.TaskOwner.OwnerId] = [" + task.TaskOwner.OwnerId + "];\n");
                    sMessage.Append("[task.TaskOwner.OwnerType] = [" + task.TaskOwner.OwnerType.ToString() + "];\n");
                }
                else
                {
                    sMessage.Append("[task.TaskOwner] = [null];\n");
                }

                if (Employee != null)
                {
                    sMessage.Append("[Employee.employeeid] = [" + Employee.EmployeeID + "];\n");
                    sMessage.Append("[Employee.firstname] = [" + Employee.Forename + "];\n");
                    sMessage.Append("[Employee.surname] = [" + Employee.Surname + "];\n");
                }

                cEventlog.LogEntry(sMessage.ToString());
            }

            return pendingEmailId;
        }

        /// <summary>
        /// CreateOwnerAllocateEmail
        /// </summary>
        /// <param name="task">Task entity</param>
        /// <returns>ID of created email</returns>
        private int CreateOwnerAllocateEmail(cTask task, bool isAddNew)
        {
            int pendingEmailId = 0;
            StringBuilder sMessage = new StringBuilder();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string brandName = "Framework";
            if (currentUser != null)
            {
                var module = this._productModuleFactory[currentUser.CurrentActiveModule];
                brandName = (module != null) ? module.BrandName : "Framework";
            }
            else
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] != null)
                {
                    brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"];
                }
            }

            if (task != null && task.TaskOwner != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                cAccountProperties accProperties = new cAccountProperties();

                db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@recipientType", (int)task.TaskOwner.OwnerType);
                db.sqlexecute.Parameters.AddWithValue("@recipientId", task.TaskOwner.OwnerId);

                if (isAddNew)
                {
                    db.sqlexecute.Parameters.AddWithValue("@subject", "New Task allocation for task : [" + task.Subject + "]");
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@subject", "Task Ownership change for task : [" + task.Subject + "]");
                }
                db.sqlexecute.Parameters.AddWithValue("@emailType", (int)PendingMailType.TaskOwnerAllocate);

                StringBuilder body = new StringBuilder();

                if (isAddNew)
                {
                    body.Append("A task in " + brandName + " has been assigned to you by " + Employee.Forename + " " + Employee.Surname + ".\n\n");
                }
                else
                {
                    body.Append("A task in " + brandName + " has had it's ownership changed by " + Employee.Forename + " " + Employee.Surname + ".\n\n");
                }

                if (task.TaskOwner.OwnerType == sendType.team)
                {
                    body.Append("You are currently included in the team assigned for the task.\n\n");
                }
                else
                {
                    body.Append("You are currently assigned as the task owner.\n\n");
                }

                if (accProperties.ApplicationURL != null)
                {
                    body.Append("Click the link below to view the task\n\n\n");
                    string slash = "/";
                    if (accProperties.ApplicationURL.EndsWith("/"))
                    {
                        slash = "";
                    }
                    body.Append(accProperties.ApplicationURL + slash + "tasks/ViewTask.aspx?tid=" + task.TaskId.ToString() + "&rid=" + task.RegardingId.ToString() + "&rtid=" + ((int)task.RegardingArea).ToString());
                }

                body.Append("\n\n\n");
                body.Append("*** THIS IS AN AUTOMATED EMAIL - DO NOT REPLY ***");

                db.sqlexecute.Parameters.AddWithValue("@body", body.ToString());
                db.ExecuteProc("savePendingEmail");
                pendingEmailId = (int)db.sqlexecute.Parameters["@Identity"].Value;
            }
            else
            {
                sMessage.Append("Spend_Management.cPendingEmails.CreateOwnerAllocateEmail(cTask task, bool isAddNew): An error occurred whilst trying to create an automated email ready to send by the scheduler of a \"Task Owner Allocate\". The following information may help with diagnosing the issue - \n");

                sMessage.Append("[AccountID] = [" + AccountID.ToString() + "];\n");

                if (task != null)
                {
                    sMessage.Append("[task.TaskId] = [" + task.TaskId + "];\n");
                    sMessage.Append("[task.TaskCreator] = [" + task.TaskCreator + "];\n");
                    sMessage.Append("[task.Subject] = [" + task.Subject + "];\n");
                    sMessage.Append("[task.Description] = [" + task.Description + "];\n");
                }

                if (task.TaskOwner != null)
                {
                    sMessage.Append("[task.TaskOwner.OwnerId] = [" + task.TaskOwner.OwnerId + "];\n");
                    sMessage.Append("[task.TaskOwner.OwnerType] = [" + task.TaskOwner.OwnerType.ToString() + "];\n");
                }
                else
                {
                    sMessage.Append("[task.TaskOwner] = [null];\n");
                }

                if (Employee != null)
                {
                    sMessage.Append("[Employee.employeeid] = [" + Employee.EmployeeID + "];\n");
                    sMessage.Append("[Employee.firstname] = [" + Employee.Forename + "];\n");
                    sMessage.Append("[Employee.surname] = [" + Employee.Surname + "];\n");
                }

                cEventlog.LogEntry(sMessage.ToString());
            }

            return pendingEmailId;
        }

        /// <summary>
        /// CreateOwnerDeallocateEmail
        /// </summary>
        /// <param name="task">Task entity</param>
        /// <returns>ID of created email</returns>
        private int CreateOwnerDeallocateEmail(cTask task)
        {
            int pendingEmailId = 0;
            StringBuilder sMessage = new StringBuilder();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string brandName = "Framework";
            if (currentUser != null)
            {
                var module = this._productModuleFactory[currentUser.CurrentActiveModule];
                brandName = (module != null) ? module.BrandName : "Framework";
            }
            else
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] != null)
                {
                    brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"];
                }
            }

            if (task != null && task.TaskOwner != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                cAccountProperties accProperties = new cAccountProperties();

                db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@recipientType", (int)task.TaskOwner.OwnerType);
                db.sqlexecute.Parameters.AddWithValue("@recipientId", task.TaskOwner.OwnerId);
                db.sqlexecute.Parameters.AddWithValue("@subject", "Task Ownership changed for task : [" + task.Subject + "]");
                db.sqlexecute.Parameters.AddWithValue("@emailType", (int)PendingMailType.TaskOwnerDeallocate);

                StringBuilder body = new StringBuilder();
                body.Append("A task in " + brandName + " has had it's ownership changed by " + Employee.Forename + " " + Employee.Surname + ".\n\n");
                if (task.TaskOwner.OwnerType == sendType.team)
                {
                    body.Append("You were previously included in the team assigned for the task.\n\n");
                }
                else
                {
                    body.Append("You were previously assigned as the task owner.\n\n");
                }
                body.Append("You will be notified in a separate notification if you are included in the new task ownership.\n\n");

                if (accProperties.ApplicationURL != null)
                {
                    body.Append("Click the link below to view the task\n\n\n");
                    string slash = "/";
                    if (accProperties.ApplicationURL.EndsWith("/"))
                    {
                        slash = "";
                    }
                    body.Append(accProperties.ApplicationURL + slash + "tasks/ViewTask.aspx?tid=" + task.TaskId.ToString() + "&rid=" + task.RegardingId.ToString() + "&rtid=" + ((int)task.RegardingArea).ToString());
                }

                body.Append("\n\n\n");
                body.Append("*** THIS IS AN AUTOMATED EMAIL - DO NOT REPLY ***");

                db.sqlexecute.Parameters.AddWithValue("@body", body.ToString());
                db.ExecuteProc("savePendingEmail");
                pendingEmailId = (int)db.sqlexecute.Parameters["@Identity"].Value;
            }
            else
            {
                sMessage.Append("Spend_Management.cPendingEmails.CreateOwnerDeallocateEmail(cTask task): An error occurred whilst trying to create an automated email ready to send by the scheduler of a \"Task Owner Deallocate\". The following information may help with diagnosing the issue - \n");

                sMessage.Append("[AccountID] = [" + AccountID.ToString() + "];\n");

                if (task != null)
                {
                    sMessage.Append("[task.TaskId] = [" + task.TaskId + "];\n");
                    sMessage.Append("[task.TaskCreator] = [" + task.TaskCreator + "];\n");
                    sMessage.Append("[task.Subject] = [" + task.Subject + "];\n");
                    sMessage.Append("[task.Description] = [" + task.Description + "];\n");
                }

                if (task.TaskOwner != null)
                {
                    sMessage.Append("[task.TaskOwner.OwnerId] = [" + task.TaskOwner.OwnerId + "];\n");
                    sMessage.Append("[task.TaskOwner.OwnerType] = [" + task.TaskOwner.OwnerType.ToString() + "];\n");
                }
                else
                {
                    sMessage.Append("[task.TaskOwner] = [null];\n");
                }

                if (Employee != null)
                {
                    sMessage.Append("[Employee.employeeid] = [" + Employee.EmployeeID + "];\n");
                    sMessage.Append("[Employee.firstname] = [" + Employee.Forename + "];\n");
                    sMessage.Append("[Employee.surname] = [" + Employee.Surname + "];\n");
                }

                cEventlog.LogEntry(sMessage.ToString());
            }

            return pendingEmailId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private int CreateOverdueTaskEmail(cTask task)
        {
            int pendingEmailId = 0;
            StringBuilder sMessage = new StringBuilder();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string brandName = "Framework";
            if (currentUser != null)
            {
                var module = this._productModuleFactory[currentUser.CurrentActiveModule];
                brandName = (module != null) ? module.BrandName : "Framework";
            }
            else
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] != null)
                {
                    brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"];
                }
            }

            if (task != null && task.TaskOwner != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                cAccountProperties accProperties = new cAccountProperties();

                db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@recipientType", (int)task.TaskOwner.OwnerType);
                db.sqlexecute.Parameters.AddWithValue("@recipientId", task.TaskOwner.OwnerId);
                db.sqlexecute.Parameters.AddWithValue("@subject", "Task Overdue : [" + task.Subject + "]");
                db.sqlexecute.Parameters.AddWithValue("@emailType", (int)PendingMailType.TaskOverdue);

                StringBuilder body = new StringBuilder();
                TimeSpan datediff_days = DateTime.Now - task.DueDate.Value; // task.DueDate.Value - task.StartDate.Value;
                if (datediff_days.Days > 0)
                {
                    body.Append("A scheduled task has not been started in " + brandName + ", but is now " + datediff_days.Days + " days overdue.\n\n");
                }
                else
                {
                    body.Append("A scheduled task has not been started in " + brandName + ", but is now overdue.\n\n");
                }

                if (task.TaskOwner.OwnerType == sendType.team)
                {
                    if (task.TaskEscalated)
                    {
                        cTeams clsTeams = new cTeams(AccountID);
                        cTeam team = clsTeams.GetTeamById(task.TaskOwner.OwnerId);

                        if (team != null && team.teamLeaderId.HasValue)
                        {
                            db.sqlexecute.Parameters["@recipientType"].Value = (int)sendType.employee;
                            db.sqlexecute.Parameters["@recipientId"].Value = team.teamLeaderId.Value;
                            body.Append("As the team leader, you are being alerted as the task has been previously escalated, to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                        }
                        else
                        {
                            body.Append("As a member of the team you are being alerted again, as the task has been previously escalated, to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                        }
                    }
                    else
                    {
                        body.Append("As a member of the team, you are being alerted to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                    }
                }
                else
                {
                    body.Append("This reminder has been issued as you are nominated as the task owner.\n\n");
                }

                if (accProperties.ApplicationURL != null)
                {
                    body.Append("Click the link below to view the task\n\n\n");
                    string slash = "/";
                    if (accProperties.ApplicationURL.EndsWith("/"))
                    {
                        slash = "";
                    }
                    body.Append(accProperties.ApplicationURL + slash + "tasks/ViewTask.aspx?tid=" + task.TaskId.ToString() + "&rid=" + task.RegardingId.ToString() + "&rtid=" + ((int)task.RegardingArea).ToString());
                }

                body.Append("\n\n\n");
                body.Append("*** THIS IS AN AUTOMATED EMAIL - DO NOT REPLY ***");

                db.sqlexecute.Parameters.AddWithValue("@body", body.ToString());
                db.ExecuteProc("savePendingEmail");
                pendingEmailId = (int)db.sqlexecute.Parameters["@Identity"].Value;

                db.sqlexecute.Parameters.Clear();
                db.sqlexecute.Parameters.AddWithValue("@escalated", true);
                db.sqlexecute.Parameters.AddWithValue("@escalationDate", DateTime.Now);
                db.sqlexecute.Parameters.AddWithValue("@taskId", task.TaskId);

                string sql = "update tasks set escalated = @escalated, escalationDate = @escalationDate where taskId = @taskId";
                db.ExecuteSQL(sql);

                _cache.Delete(AccountID, CacheKey, task.TaskId.ToString());
            }
            else
            {
                sMessage.Append("Spend_Management.cPendingEmails.CreateOverdueTaskEmail(cTask task): An error occurred whilst trying to create an automated email ready to send by the scheduler of a \"Task Overdue\". The following information may help with diagnosing the issue - \n");

                sMessage.Append("[AccountID] = [" + AccountID.ToString() + "];\n");

                if (task != null)
                {
                    sMessage.Append("[task.TaskId] = [" + task.TaskId + "];\n");
                    sMessage.Append("[task.TaskCreator] = [" + task.TaskCreator + "];\n");
                    sMessage.Append("[task.Subject] = [" + task.Subject + "];\n");
                    sMessage.Append("[task.Description] = [" + task.Description + "];\n");
                }

                if (task.TaskOwner != null)
                {
                    sMessage.Append("[task.TaskOwner.OwnerId] = [" + task.TaskOwner.OwnerId + "];\n");
                    sMessage.Append("[task.TaskOwner.OwnerType] = [" + task.TaskOwner.OwnerType.ToString() + "];\n");
                }
                else
                {
                    sMessage.Append("[task.TaskOwner] = [null];\n");
                }

                if (Employee != null)
                {
                    sMessage.Append("[Employee.employeeid] = [" + Employee.EmployeeID + "];\n");
                    sMessage.Append("[Employee.firstname] = [" + Employee.Forename + "];\n");
                    sMessage.Append("[Employee.surname] = [" + Employee.Surname + "];\n");
                }

                cEventlog.LogEntry(sMessage.ToString());
            }

            return pendingEmailId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private int CreateLateTaskEmail(cTask task)
        {
            int pendingEmailId = 0;
            StringBuilder sMessage = new StringBuilder();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string brandName = "Framework";
            if (currentUser != null)
            {
                var module = this._productModuleFactory[currentUser.CurrentActiveModule];
                brandName = (module != null) ? module.BrandName : "Framework";
            }
            else
            {
                if (System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] != null)
                {
                    brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"];
                }
            }

            if (task != null && task.TaskOwner != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                cAccountProperties accProperties = new cAccountProperties();

                db.sqlexecute.Parameters.Add("@Identity", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@Identity"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@recipientType", (int)task.TaskOwner.OwnerType);
                db.sqlexecute.Parameters.AddWithValue("@recipientId", task.TaskOwner.OwnerId);
                db.sqlexecute.Parameters.AddWithValue("@subject", "Task is now Overdue and Late : [" + task.Subject + "]");
                db.sqlexecute.Parameters.AddWithValue("@emailType", (int)PendingMailType.TaskOverdue);

                StringBuilder body = new StringBuilder();
                TimeSpan datediff_days = DateTime.Now - task.DueDate.Value; // -task.StartDate.Value;
                if (task.StatusId == TaskStatus.NotStarted)
                {
                    body.Append("A scheduled task has not been started in " + brandName);
                }
                else
                {
                    body.Append("A scheduled task in " + brandName + " is in progress");
                }
                body.Append(", but is now " + datediff_days.Days.ToString() + " days overdue.\n\n");
                body.Append("The task's due date was set at " + task.DueDate.Value.ToShortDateString() + " and this has now been passed.\n\n");

                if (task.TaskOwner.OwnerType == sendType.team)
                {
                    if (task.TaskEscalated)
                    {
                        cTeams clsTeams = new cTeams(AccountID);
                        cTeam team = clsTeams.GetTeamById(task.TaskOwner.OwnerId);

                        if (team != null && team.teamLeaderId.HasValue)
                        {
                            db.sqlexecute.Parameters["@recipientType"].Value = (int)sendType.employee;
                            db.sqlexecute.Parameters["@recipientId"].Value = team.teamLeaderId.Value;
                            body.Append("As the team leader, you are being alerted as the task has been previously escalated, to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                        }
                        else
                        {
                            body.Append("As a member of the team you are being alerted again, as the task has been previously escalated, to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                        }
                    }
                    else
                    {
                        body.Append("As a member of the team, you are being alerted to the need for this task to be addressed by the team to prevent missing the task due date.\n\n");
                    }
                }
                else
                {
                    body.Append("This reminder has been issued as you are nominated as the task owner.\n\n");
                }

                if (accProperties.ApplicationURL != null)
                {
                    body.Append("Click the link below to view the task\n\n\n");
                    string slash = "/";
                    if (accProperties.ApplicationURL.EndsWith("/"))
                    {
                        slash = "";
                    }
                    body.Append(accProperties.ApplicationURL + slash + "tasks/ViewTask.aspx?tid=" + task.TaskId.ToString() + "&rid=" + task.RegardingId.ToString() + "&rtid=" + ((int)task.RegardingArea).ToString());
                }

                body.Append("\n\n\n");
                body.Append("*** THIS IS AN AUTOMATED EMAIL - DO NOT REPLY ***");

                db.sqlexecute.Parameters.AddWithValue("@body", body.ToString());
                db.ExecuteProc("savePendingEmail");

                pendingEmailId = (int)db.sqlexecute.Parameters["@Identity"].Value;

                task.TaskEscalated = true;
                task.TaskEscalatedDate = DateTime.Now;

                db.sqlexecute.Parameters.AddWithValue("@escalated", true);
                db.sqlexecute.Parameters.AddWithValue("@escalationDate", task.TaskEscalatedDate.Value);
                db.sqlexecute.Parameters.AddWithValue("@taskId", task.TaskId);

                string sql = "update tasks set escalated = @escalated, escalationDate = @escalationDate where taskId = @taskId";
                db.ExecuteSQL(sql);

                _cache.Delete(AccountID, CacheKey, task.TaskId.ToString());
            }
            else
            {
                sMessage.Append("Spend_Management.cPendingEmails.CreateLateTaskEmail(cTask task): An error occurred whilst trying to create an automated email ready to send by the scheduler of a \"Task Overdue\". The following information may help with diagnosing the issue - \n");

                sMessage.Append("[AccountID] = [" + AccountID.ToString() + "];\n");

                if (task != null)
                {
                    sMessage.Append("[task.TaskId] = [" + task.TaskId + "];\n");
                    sMessage.Append("[task.TaskCreator] = [" + task.TaskCreator + "];\n");
                    sMessage.Append("[task.Subject] = [" + task.Subject + "];\n");
                    sMessage.Append("[task.Description] = [" + task.Description + "];\n");
                }

                if (task.TaskOwner != null)
                {
                    sMessage.Append("[task.TaskOwner.OwnerId] = [" + task.TaskOwner.OwnerId + "];\n");
                    sMessage.Append("[task.TaskOwner.OwnerType] = [" + task.TaskOwner.OwnerType.ToString() + "];\n");
                }
                else
                {
                    sMessage.Append("[task.TaskOwner] = [null];\n");
                }

                if (Employee != null)
                {
                    sMessage.Append("[Employee.employeeid] = [" + Employee.EmployeeID + "];\n");
                    sMessage.Append("[Employee.firstname] = [" + Employee.Forename + "];\n");
                    sMessage.Append("[Employee.surname] = [" + Employee.Surname + "];\n");
                }

                cEventlog.LogEntry(sMessage.ToString());
            }

            return pendingEmailId;
        }
    }
}
