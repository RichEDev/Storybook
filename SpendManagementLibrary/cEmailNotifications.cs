namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Configuration;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Interfaces;

    public class cEmailNotifications  
    {
        public int nAccountID;
        private static SortedList<int, cEmailNotification> _cachedEmailNotifications;
        private const string CacheKey = "subscriptions";

        public cEmailNotifications()
        {
        }

        /// <summary>
        /// Default constructor for cEmailNotifications
        /// </summary>
        /// <param name="accountId"></param>
        public cEmailNotifications(int accountId)
        {
            nAccountID = accountId;
            CacheList();
        }

        private static void CacheList()
        {
            if (_cachedEmailNotifications != null)
            {
                return;
            }

            _cachedEmailNotifications = new SortedList<int, cEmailNotification>();
            using (var connection = new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                const string Sql = "SELECT emailNotificationID, name, description, emailTemplateID, enabled, customerType, emailNotificationType FROM dbo.emailNotifications";

                connection.sqlexecute.CommandText = Sql;
                using (IDataReader reader = connection.GetReader(Sql))
                {
                    connection.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        int emailNotificationId = reader.GetInt32(reader.GetOrdinal("emailNotificationID"));
                        string name = reader.IsDBNull(reader.GetOrdinal("name"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("name"));
                        string description = reader.IsDBNull(reader.GetOrdinal("description"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("description"));
                        int emailTemplateId = reader.GetInt32(reader.GetOrdinal("emailTemplateID"));
                        bool enabled = reader.GetBoolean(reader.GetOrdinal("enabled"));

                        var customerType = (CustomerType)reader.GetInt32(reader.GetOrdinal("customertype"));
                        var emailNotificationType =
                            (EmailNotificationType)reader.GetInt32(reader.GetOrdinal("emailNotificationType"));

                        var tmpEmailNotification = new cEmailNotification(
                            emailNotificationId,
                            name,
                            description,
                            emailTemplateId,
                            enabled,
                            customerType,
                            emailNotificationType);

                        if (_cachedEmailNotifications != null)
                        {
                            _cachedEmailNotifications.Add(tmpEmailNotification.EmailNotificationID, tmpEmailNotification);
                        }
                    }

                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Saves the email notifications for either a team or an employee 
        /// </summary>
        /// <param name="emailNotificationIdList"></param>
        /// <param name="employeeId"></param>
        /// <param name="teamId"></param>
        public void SaveNotificationLink(List<int> emailNotificationIdList, int? employeeId, int? teamId)
        {
            if (employeeId.HasValue || teamId.HasValue)
            {
                using (var connection = new DatabaseConnection(cAccounts.getConnectionString(nAccountID)))
                {
                    string field;
                    int value;
                    EmployeeEmailNotifications employeeEmailNotifications = null;

                    if (teamId.HasValue)
                    {
                        field = "teamID";
                        value = teamId.Value;
                    }
                    else
                    {
                        field = "employeeID";
                        value = employeeId.Value;
                        Employee employee = Employee.Get(employeeId.Value, nAccountID, new DatabaseConnection(cAccounts.getConnectionString(nAccountID)));
                        employeeEmailNotifications = employee.GetEmailNotificationList();
                    }

                    string sql = "DELETE FROM dbo.emailNotificationLink WHERE " + field + "=@value";
                    connection.sqlexecute.Parameters.AddWithValue("@value", value);
                    connection.ExecuteSQL(sql);
                    connection.sqlexecute.Parameters.Clear();

                    if (emailNotificationIdList != null && emailNotificationIdList.Any())
                    {
                        foreach (int id in emailNotificationIdList)
                        {
                            sql = "INSERT INTO dbo.emailNotificationLink (emailNotificationID, " + field + ") VALUES (@emailNotificationID, @value)";
                            connection.sqlexecute.Parameters.AddWithValue("@emailNotificationID", id);
                            connection.sqlexecute.Parameters.AddWithValue("@value", value);
                            connection.ExecuteSQL(sql);
                            connection.sqlexecute.Parameters.Clear();
                        }
                    }

                    // drop all items for the employee from cache, they'll be re-cached next time the list is retrieved
                    if (employeeId.HasValue && employeeEmailNotifications != null)
                    {
                        employeeEmailNotifications.Clear();
                    }
                }                
            }

            var cache = new Utilities.DistributedCaching.Cache();
            var values = Enum.GetValues(typeof(EmailNotificationType)).Cast<EmailNotificationType>();

            foreach (EmailNotificationType emailNotificationType in values)
            {
                cache.Delete(AccountID, "subscriptions", emailNotificationType.ToString());
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
            _cachedEmailNotifications.TryGetValue(emailNotificationID, out emailNotification);
            return emailNotification;
        }

        /// <summary>
        /// Gets a cEmailNotification by EmailNotificationType, returns null if not found
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public cEmailNotification GetEmailNotificationByNotificationType(EmailNotificationType type)
        {
            return _cachedEmailNotifications.Values.FirstOrDefault(notification => notification.EmailNotificationType == type);
        }

        /// <summary>
        /// Returns a list of subscribers to a notification
        /// </summary>
        /// <param name="notificationType">The type of <see cref="EmailNotificationType"/> to return</param>
        /// <returns>A <see cref="List{T}"/> of <seealso>
        ///         <cref>object[]</cref>
        ///     </seealso>
        /// Where the first element of the Obejct array is <seealso cref="sendType"/>
        /// ansd the second is the ID (either employee or team.
        /// </returns>
        public List<object[]> GetNotificationSubscriptions(EmailNotificationType notificationType)
        {
            var cache = new Utilities.DistributedCaching.Cache();
            var lstSubscriptions = new List<object[]>();

            if (cache.Contains(AccountID, CacheKey, notificationType.ToString()))
            {
                return (List<object[]>)cache.Get(AccountID, CacheKey, notificationType.ToString());
            }

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(nAccountID)))
            {
                cEmailNotification reqEmailNotification = GetEmailNotificationByNotificationType(notificationType);

                if (reqEmailNotification != null)
                {
                    const string strSql = "SELECT emailNotificationID, teamID, dbo.emailNotificationLink.employeeID FROM dbo.emailNotificationLink left join employees on employees.employeeid	= dbo.emailNotificationLink.employeeID";
                    string strWhere = " WHERE emailNotificationID=@emailNotificationID and ISNULL(employees.archived, 1) = 0";
                    connection.sqlexecute.Parameters.AddWithValue("@emailNotificationID", reqEmailNotification.EmailNotificationID);

                    using (IDataReader reader = connection.GetReader(strSql + strWhere))
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("teamID")))
                            {
                                int? teamId = reader.GetInt32(reader.GetOrdinal("teamID"));
                                lstSubscriptions.Add(new object[] { sendType.team, teamId });
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("employeeID")))
                            {
                                int? employeeId = reader.GetInt32(reader.GetOrdinal("employeeID"));
                                lstSubscriptions.Add(new object[] { sendType.employee, employeeId });
                            }
                        }

                        reader.Close();
                    }

                    connection.sqlexecute.Parameters.Clear();
                    cache.Add(AccountID, CacheKey, notificationType.ToString(), lstSubscriptions);
                }                
            }

            return lstSubscriptions;
        }

        public static void InvalidateCache(int accountId, EmailNotificationType? onlyThisNotificationType = null)
        {
            var notificationTypes = new List<EmailNotificationType>();

            if (onlyThisNotificationType != null)
            {
                notificationTypes.Add(onlyThisNotificationType.Value);
            }
            else
            {
                notificationTypes = Enum.GetValues(typeof(EmailNotificationType)).Cast<EmailNotificationType>().ToList();
            }

            var cache = new Utilities.DistributedCaching.Cache();

            foreach (EmailNotificationType notificationType in notificationTypes)
            {
                cache.Delete(accountId, CacheKey, notificationType.ToString());
            }
        }

        /// <summary>
        /// Sends all email notifications for a specific type
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="bodyUpdate"></param>
        /// <param name="templates"></param>
        public void SendNotifications(EmailNotificationType notificationType, string bodyUpdate, IEmailTemplates templates)
        {
            // Put current summary action log data into a temporary table so it can be processed for the recipients to get notifications of any summary of employee archiving/activation
            // A temporary table is created with the currect action log data so it can be reported on for the email template summary
            const string SqlExists = "IF EXISTS(SELECT * FROM sys.tables WHERE NAME='autoActionLogTemp') BEGIN DROP TABLE dbo.autoActionLogTemp; END SELECT * INTO dbo.autoActionLogTemp FROM autoactionlog WHERE processed = 0;";
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(nAccountID)))
            {
                connection.ExecuteSQL(SqlExists);

                cEmailNotification reqEmailNotification = GetEmailNotificationByNotificationType(notificationType);
                List<object[]> lstRequired = GetNotificationSubscriptions(notificationType);
                var lstSendDetails = new List<sSendDetails>();

                foreach (object[] obj in lstRequired)
                {
                    sSendDetails tmpSendDetails;

                    switch ((sendType)obj[0])
                    {
                        case sendType.team:
                            tmpSendDetails = new sSendDetails
                                             {
                                                 idoftype = (int)obj[1],
                                                 recType = recipientType.to,
                                                 senderType = sendType.team
                                             };
                            lstSendDetails.Add(tmpSendDetails);
                            break;
                        case sendType.employee:
                            Employee reqEmployee = Employee.Get((int)obj[1], AccountID);
                            tmpSendDetails = new sSendDetails
                                             {
                                                 idoftype = reqEmployee.EmployeeID,
                                                 recType = recipientType.to,
                                                 senderType = sendType.employee,
                                                 sender = reqEmployee.EmailAddress
                                             };
                            lstSendDetails.Add(tmpSendDetails);
                            break;
                    }
                }

                if (lstSendDetails.Count > 0)
                {
                    templates.SendMessage(reqEmailNotification.EmailTemplateID, null, 0, 0, lstSendDetails, bodyUpdate);
                }

                // Drop all data from the temporary auto action log and update all processed data as processed
                const string UpdateSql = "UPDATE autoActionLog SET processed = 1 where actionID in(SELECT actionID FROM autoActionLogTemp); DROP TABLE dbo.autoActionLogTemp;";
                connection.ExecuteSQL(UpdateSql);
            }
        }

        /// <summary>
        /// Returns a list of cEmailNotifications filtered by CustomerType
        /// </summary>
        /// <param name="customerType"></param>
        /// <returns></returns>
        public SortedList<int, cEmailNotification> EmailNotificationsByCustomerType(CustomerType customerType)
        {
            var lstEmailNotifications = new SortedList<int, cEmailNotification>();

            foreach (cEmailNotification notification in _cachedEmailNotifications.Values.Where(notification => notification.CustomerType == customerType))
            {
                lstEmailNotifications.Add(notification.EmailNotificationID, notification);
            }

            return lstEmailNotifications;
        }

        /// <summary>
        /// Gets or sets the list of EmailNotifications - this is unfiltered
        /// </summary>
        public SortedList<int, cEmailNotification> EmailNotifications
        {
            get { return _cachedEmailNotifications; }
            set { _cachedEmailNotifications = value; }
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
