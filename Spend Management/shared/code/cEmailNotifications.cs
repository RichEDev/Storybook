using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Web.Caching;
using System.Configuration;
using System.Text;
using SpendManagementLibrary.Helpers;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    public class cEmailNotifications
    {
        public int nAccountID;
        private SortedList<int, cEmailNotification> lstCachedEmailNotifications;
        private Cache Cache = HttpRuntime.Cache;

        public cEmailNotifications()
        {
        }

        /// <summary>
        /// Default constructor for cEmailNotifications
        /// </summary>
        /// <param name="accountID"></param>
        public cEmailNotifications(int accountID)
        {
            nAccountID = accountID;
            InitializeData();
        }

        /// <summary>
        /// Initializes the cache
        /// </summary>
        public void InitializeData()
        {
            lstCachedEmailNotifications = (SortedList<int, cEmailNotification>)Cache.Get("EmailNotifications");
            lstCachedEmailNotifications = lstCachedEmailNotifications ?? CacheList();

        }

        private SortedList<int, cEmailNotification> CacheList()
        {
            SortedList<int, cEmailNotification> lstEmailNotifications = new SortedList<int, cEmailNotification>();
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            const string strSQL = "SELECT emailNotificationID, name, description, emailTemplateID, enabled, customerType, emailNotificationType FROM dbo.emailNotifications";

            smData.sqlexecute.CommandText = strSQL;
            SqlCacheDependency dep = new SqlCacheDependency(smData.sqlexecute);
            using (System.Data.SqlClient.SqlDataReader reader = smData.GetReader(strSQL))
            {
                smData.sqlexecute.Parameters.Clear();

                while(reader.Read())
                {
                    int emailNotificationID = reader.GetInt32(reader.GetOrdinal("emailNotificationID"));
                    string name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name"));
                    string description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description"));
                    int emailTemplateID = reader.GetInt32(reader.GetOrdinal("emailTemplateID"));
                    bool enabled = reader.GetBoolean(reader.GetOrdinal("enabled"));

                    CustomerType customerType = (CustomerType) reader.GetInt32(reader.GetOrdinal("customertype"));
                    EmailNotificationType emailNotificationType = (EmailNotificationType) reader.GetInt32(reader.GetOrdinal("emailNotificationType"));

                    cEmailNotification tmpEmailNotification = new cEmailNotification(emailNotificationID, name, description, emailTemplateID, enabled, customerType, emailNotificationType);

                    lstEmailNotifications.Add(tmpEmailNotification.EmailNotificationID, tmpEmailNotification);
                }

                reader.Close();
            }

            Cache.Insert("EmailNotifications", lstEmailNotifications, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);            

            return lstEmailNotifications;
        }

        /// <summary>
        /// Saves the email notifications for either a team or an employee 
        /// </summary>
        /// <param name="lstEmailNotificationIDs"></param>
        /// <param name="employeeID"></param>
        /// <param name="teamID"></param>
        public void SaveNotificationLink(List<int> lstEmailNotificationIDs, int? employeeID, int? teamID)
        {
            if (employeeID.HasValue == true || teamID.HasValue == true)
            {
                DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
                
                string field;
                int value;
                EmployeeEmailNotifications employeeEmailNotifications = null;

                if (teamID.HasValue)
                {
                    field = "teamID";
                    value = teamID.Value;
                }
                else
                {
                    field = "employeeID";
                    value = employeeID.Value;
                    Employee employee = Employee.Get(employeeID.Value, nAccountID, new DatabaseConnection(cAccounts.getConnectionString(nAccountID)));
                    employeeEmailNotifications = employee.GetEmailNotificationList();
                }

                string sql = "DELETE FROM emailNotificationLink WHERE " + field + "=@value";
                smData.sqlexecute.Parameters.AddWithValue("@value", value);
                smData.ExecuteSQL(sql);
                smData.sqlexecute.Parameters.Clear();
                
                foreach (int id in lstEmailNotificationIDs)
                {
                    sql = "INSERT INTO emailNotificationLink (emailNotificationID, " + field + ") VALUES (@emailNotificationID, @value)";
                    smData.sqlexecute.Parameters.AddWithValue("@emailNotificationID", id);
                    smData.sqlexecute.Parameters.AddWithValue("@value", value);
                    smData.ExecuteSQL(sql);
                    smData.sqlexecute.Parameters.Clear();
                    
                    if (employeeID.HasValue && employeeEmailNotifications != null)
                    {
                        employeeEmailNotifications.Remove(id);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a cEmailNotification by emailNotificationID, returns null if not found
        /// </summary>
        /// <param name="emailNotificationID"></param>
        /// <returns></returns>
        public cEmailNotification GetEmailNotificationByID(int emailNotificationID)
        {
            cEmailNotification emailNotification;
            lstCachedEmailNotifications.TryGetValue(emailNotificationID, out emailNotification);
            return emailNotification;
        }

        /// <summary>
        /// Gets a cEmailNotification by EmailNotificationType, returns null if not found
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public cEmailNotification GetEmailNotificationByNotificationType(EmailNotificationType type)
        {
            return lstCachedEmailNotifications.Values.FirstOrDefault(notification => notification.EmailNotificationType == type);
        }

        /// <summary>
        /// Returns a list of subscribers to a notification
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public List<object[]> GetNotificationSubscriptions(EmailNotificationType notificationType)
        {
            string sCacheKey = String.Format("subscriptions_{0}_{1}", AccountID, (int)notificationType);
            Caching eCache = new Caching();
            List<object[]> lstSubscriptions = new List<object[]>();
            if (eCache.Cache.Contains(sCacheKey))
            {
                return (List<object[]>) eCache.Cache[sCacheKey];
            }
            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cEmailNotification reqEmailNotification = GetEmailNotificationByNotificationType(notificationType);

            if (reqEmailNotification != null)
            {
                const string strSql = "SELECT emailNotificationID, teamID, employeeID FROM dbo.emailNotificationLink";
                string strWhere = String.Format(" WHERE emailNotificationID=@emailNotificationID and {0} = {0}", AccountID);
                smData.sqlexecute.Parameters.AddWithValue("@emailNotificationID", reqEmailNotification.EmailNotificationID);
                using(SqlDataReader reader = smData.GetReader(strSql + strWhere))
                {
                    while(reader.Read())
                    {
                        if(!reader.IsDBNull(reader.GetOrdinal("teamID")))
                        {
                            int? teamID = reader.GetInt32(reader.GetOrdinal("teamID"));
                            lstSubscriptions.Add(new object[] {sendType.team, teamID});
                        }

                        if(!reader.IsDBNull(reader.GetOrdinal("employeeID")))
                        {
                            int? employeeID = reader.GetInt32(reader.GetOrdinal("employeeID"));
                            lstSubscriptions.Add(new object[] {sendType.employee, employeeID});
                        }
                    }

                    reader.Close();
                }
                smData.sqlexecute.Parameters.Clear();
                eCache.Add(sCacheKey, lstSubscriptions, new List<string>() { String.Format("{0} where {1} = {1} and emailnotificationid = {2}", strSql, AccountID, (int)notificationType) }, Caching.CacheTimeSpans.Medium, Caching.CacheDatabaseType.Customer, AccountID);
            }
            return lstSubscriptions;
        }

        /// <summary>
        /// Sends all email notifications for a specific type
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="bodyUpdate"></param>
        public void SendNotifications(EmailNotificationType notificationType, string bodyUpdate)
        {
            #region Put current summary action log data into a temporary table so it can be processed for the recipients to get notifications of any summary of employee archiving/activation

            //A temporary table is created with the currect action log data so it can be reported on for the email template summary
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            string strSQL = "IF EXISTS(SELECT * FROM sys.tables WHERE NAME='autoActionLogTemp') BEGIN DROP TABLE dbo.autoActionLogTemp; END ";
            strSQL += "SELECT * INTO dbo.autoActionLogTemp FROM autoactionlog WHERE processed = 0;";
            expdata.ExecuteSQL(strSQL);

            #endregion

            cEmailTemplates clsEmailTemplates = new cEmailTemplates(nAccountID);

            cEmailNotification reqEmailNotification = GetEmailNotificationByNotificationType(notificationType);
            List<object[]> lstRequired = GetNotificationSubscriptions(notificationType);
            List<sSendDetails> lstSendDetails = new List<sSendDetails>();
             
            foreach (object[] obj in lstRequired)
            {
                sSendDetails tmpSendDetails;

                switch ((sendType)obj[0])
                {
                    case sendType.team:
                        //cTeam reqTeam = (cTeam) obj[1];
                        tmpSendDetails = new sSendDetails {idoftype = (int)obj[1], recType = recipientType.to, senderType = sendType.team};
                        lstSendDetails.Add(tmpSendDetails);
                        break;
                    case sendType.employee:
                        cEmployees emps = new cEmployees(AccountID);
                        Employee reqEmployee = emps.GetEmployeeById((int)obj[1]);
                        tmpSendDetails = new sSendDetails {idoftype = reqEmployee.EmployeeID, recType = recipientType.to, senderType = sendType.employee, sender = reqEmployee.EmailAddress};
                        lstSendDetails.Add(tmpSendDetails);
                        break;
                }
            }

            if (lstSendDetails.Count > 0)
            {
                clsEmailTemplates.sendEmail(reqEmailNotification.EmailTemplateID, null, 0, 0, lstSendDetails, bodyUpdate);
            }

            #region Drop all data from the temporary auto action log and update all processed data as processed

            strSQL = "UPDATE autoActionLog SET processed = 1 where actionID in(SELECT actionID FROM autoActionLogTemp);";
            strSQL += "DROP TABLE dbo.autoActionLogTemp;";
            expdata.ExecuteSQL(strSQL);

            #endregion
        }

        /// <summary>
        /// Returns a list of cEmailNotifications filtered by CustomerType
        /// </summary>
        /// <param name="customerType"></param>
        /// <returns></returns>
        public SortedList<int, cEmailNotification> EmailNotificationsByCustomerType(CustomerType customerType)
        {
            SortedList<int, cEmailNotification> lstEmailNotifications = new SortedList<int, cEmailNotification>();

            foreach (cEmailNotification notification in lstCachedEmailNotifications.Values)
            {
                if (notification.CustomerType == customerType)
                {
                    lstEmailNotifications.Add(notification.EmailNotificationID, notification);
                }
            }

            return lstEmailNotifications;
        }

        /// <summary>
        /// Gets or sets the list of EmailNotifications - this is unfiltered
        /// </summary>
        public SortedList<int, cEmailNotification> EmailNotifications
        {
            get { return lstCachedEmailNotifications; }
            set { lstCachedEmailNotifications = value; }
        }

        /// <summary>
        /// Gets or sets the accountID
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
            set { nAccountID = value; }
        }
    }
}
