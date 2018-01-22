namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Represents a "local" (not sales force) support ticket
    /// </summary>
    public class SupportTicketInternal : ISupportTicket
    {
        #region Properties

        /// <summary>
        /// The identifier
        /// </summary>
        public int Identifier { get; set; }

        /// <summary>
        /// A friendly number used to identify the support ticket (just a string representation of the Identifier for internal tickets)
        /// </summary>
        public string CaseNumber {
            get
            {
                return this.Identifier.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                this.Identifier = int.Parse(value);
            }
        }

        /// <summary>
        /// The subject line of the support request
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The full description of the problem
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of attached files
        /// </summary>
        public List<ISupportTicketAttachment> Attachments { get; set; }

        /// <summary>
        /// A list of associated comments
        /// </summary>
        public List<ISupportTicketComment> Comments { get; set; }

        /// <summary>
        /// The status of the ticket, as a friendly string
        /// </summary>
        public string Status
        {
            get
            {
                return InternalStatus.ToString().SplitCamel();
            }
            set
            {
                InternalStatus = (SupportTicketStatus) Enum.Parse(typeof(SupportTicketStatus), value);
            }
            
        }

        /// <summary>
        /// When the ticket was created
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// When the ticket was last modified
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The support request might be associated with a Greenlight (if the user chose from the "I have a problem with a greenlight" list)
        /// Used to determine who to send email notifications to
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// The user who last modified the ticket
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// The user who created the ticket
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// The user to whom this ticket belongs
        /// </summary>
        public int Owner { get; set; }

        /// <summary>
        /// The <see cref="SupportTicketStatus"/> status of this ticket
        /// </summary>
        public SupportTicketStatus InternalStatus { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SupportTicketInternal"/>
        /// </summary>
        public SupportTicketInternal()
        {
            Comments = new List<ISupportTicketComment>();
            Attachments = new List<ISupportTicketAttachment>();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Creates a new support ticket
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="subject">The subject line of the support request</param>
        /// <param name="description">The description of the problem</param>
        /// <param name="customEntityId">An associated greenlight</param>
        /// <returns></returns>
        public static int Create(ICurrentUserBase user, string subject, string description, int customEntityId = 0)
        {
            int returnedValue = Save(user, 0, subject, description, SupportTicketStatus.New, customEntityId);

            if (returnedValue > 0)
            {
                // send email notification
                SupportTicketInternal ticket = Get(user, returnedValue);
                string moduleBrandName = new cModules().GetModuleByEnum(user.CurrentActiveModule).BrandNamePlainText;
                string messageSubject = string.Format("{0} Support Ticket Created ({1})", moduleBrandName, ticket.Identifier);

                cAccountProperties accountProperties = new cAccountSubAccountsBase(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                var bodyBuilder = new StringBuilder();
                bodyBuilder.AppendFormat("{0} has created a support ticket\n", user.Employee.FullName);
                bodyBuilder.AppendFormat("{0}/shared/admin/adminHelpAndSupportTicket.aspx?SupportTicketId={1}\n", accountProperties.ApplicationURL, ticket.Identifier);
                bodyBuilder.AppendFormat("Because this ticket may contain security sensitive information, we ask that you please click the link above or log on to {0} to view the ticket.", moduleBrandName);
                string messageBody = bodyBuilder.ToString();

                SendNotification(user, ticket, messageSubject, messageBody);
            }

            return returnedValue;
        }

        /// <summary>
        /// Retreives a support ticket from the customer database
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="identifier">The support ticket identifier</param>
        /// <returns>The support ticket</returns>
        public static SupportTicketInternal Get(ICurrentUserBase user, int identifier)
        {
            SupportTicketInternal ticket = null;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string SQL = "SELECT [SupportTicketId], [Owner], [Subject], [Description], [Status], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [CustomEntityId] FROM [SupportTickets] WHERE [SupportTicketId] = @TicketId;";
                databaseConnection.AddWithValue("@TicketId", identifier);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int identifierOrdinal = reader.GetOrdinal("SupportTicketId"),
                        subjectOrdinal = reader.GetOrdinal("Subject"), 
                        descriptionOrdinal = reader.GetOrdinal("Description"),
                        statusOrdinal = reader.GetOrdinal("Status"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy"),
                        modifiedOnOrdinal = reader.GetOrdinal("ModifiedOn"),
                        modifiedByOrdinal = reader.GetOrdinal("ModifiedBy"),
                        ownerOrdinal = reader.GetOrdinal("Owner"),
                        customEntityOrdinal = reader.GetOrdinal("CustomEntityId");
                        
                    while (reader.Read())
                    {
                        ticket = new SupportTicketInternal
                                     {
                                         Identifier = reader.GetInt32(identifierOrdinal),
                                         Subject = reader.GetString(subjectOrdinal),
                                         Description = reader.GetString(descriptionOrdinal),
                                         Status = ((SupportTicketStatus)reader.GetByte(statusOrdinal)).ToString(),
                                         CreatedBy = reader.GetInt32(createdByOrdinal),
                                         CreatedOn = reader.GetDateTime(createdOnOrdinal),
                                         ModifiedBy = reader.IsDBNull(modifiedByOrdinal) ? 0 : reader.GetInt32(modifiedByOrdinal),
                                         ModifiedOn = reader.IsDBNull(modifiedOnOrdinal) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrdinal),
                                         Owner = reader.GetInt32(ownerOrdinal),
                                         CustomEntityId = reader.IsDBNull(customEntityOrdinal) ? 0 : reader.GetInt32(customEntityOrdinal)
                                     };

                        ticket.Attachments.AddRange(SupportTicketAttachment.GetBySupportTicketId(user, identifier));
                        ticket.Comments.AddRange(SupportTicketCommentInternal.GetBySupportTicketId(user, identifier));

                        break;
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return ticket;
        }

        /// <summary>
        /// Obtains the number of support tickets with a given status
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The number of support tickets</returns>
        public static int GetCountForAdministrator(ICurrentUserBase user)
        {
            int count = 0;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string Sql = "SELECT COUNT([SupportTicketId]) FROM [SupportTickets] WHERE [Status] = @Status1 OR [Status] = @Status2;";
                databaseConnection.AddWithValue("@Status1", (int)SupportTicketStatus.New);
                databaseConnection.AddWithValue("@Status2", (int)SupportTicketStatus.PendingAdministratorResponse);

                count = databaseConnection.ExecuteScalar<int>(Sql);

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return count;
        }

        /// <summary>
        /// Updates a support ticket in the customer database
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="ticket">The <see cref="SupportTicketInternal"/> to save</param>
        /// <returns></returns>
        public static int Update(ICurrentUserBase user, SupportTicketInternal ticket)
        {
            int returnedValue = Save(user, ticket.Identifier, ticket.Subject, ticket.Description, ticket.InternalStatus, ticket.CustomEntityId);

            if (returnedValue > 0)
            {
                string moduleBrandName = new cModules().GetModuleByEnum(user.CurrentActiveModule).BrandNamePlainText;
                string subject = string.Format("{0} Support Ticket Update ({1})", moduleBrandName, ticket.Identifier);
                string urlPrefix = (ticket.Owner == user.EmployeeID) ? "{0}/shared/admin/adminHelpAndSupportTicket.aspx?SupportTicketId={1}\n" : "{0}/shared/helpAndSupportTicket.aspx?TicketType=1&SupportTicketId={1}\n";

                cAccountProperties accountProperties = new cAccountSubAccountsBase(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                var bodyBuilder = new StringBuilder();
                bodyBuilder.AppendFormat("{0} has updated a support ticket\n", user.Employee.FullName);
                bodyBuilder.AppendFormat(urlPrefix, accountProperties.ApplicationURL, ticket.Identifier);
                bodyBuilder.AppendFormat("Because this ticket may contain security sensitive information, we ask that you please click the link above or log on to {0} to view the ticket.", moduleBrandName);
                string body = bodyBuilder.ToString();

                SendNotification(user, ticket, subject, body);
            }

            return returnedValue;
        }

        /// <summary>
        /// Used to send notification e-mails when a new ticket is created or an existing ticket is modified
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="ticket">The <see cref="SupportTicketInternal"/> ticket</param>
        /// <param name="subject">The notification message subject</param>
        /// <param name="body">The notification message body</param>
        public static void SendNotification(ICurrentUserBase user, SupportTicketInternal ticket, string subject, string body)
        {
            cAccountProperties accountProperties = new cAccountSubAccountsBase(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var emailSender = new EmailSender(accountProperties.EmailServerAddress);

            string fromAddress = "admin@sel-expenses.com";
            if (accountProperties.SourceAddress == 1 && accountProperties.EmailAdministrator != string.Empty)
            {
                fromAddress = accountProperties.EmailAdministrator;
            }
            else if (accountProperties.SourceAddress == 0 && user.Employee.EmailAddress != string.Empty)
            {
                fromAddress = user.Employee.EmailAddress;
            }

            // if the user is the ticket owner then notify other users, otherwise notify the owner
            if (ticket.Owner == user.EmployeeID)
            {
                bool notificationSent = false; 

                // the ticket might be associated with a custom entity, if so, send the notification to the "support contact"
                if (ticket.CustomEntityId > 0)
                {
                    int? customEntitySupportContactId = cCustomEntity.GetSupportContactIdByEntityId(ticket.CustomEntityId, user);
                    if (customEntitySupportContactId.HasValue)
                    {
                        string customEntitySupportContactEmailAddress = Employee.Get(customEntitySupportContactId.Value, user.AccountID).EmailAddress;

                        if (string.IsNullOrWhiteSpace(customEntitySupportContactEmailAddress) == false)
                        {
                            emailSender.SendEmail(fromAddress, customEntitySupportContactEmailAddress, subject, body);
                            cEventlog.LogEntry(string.Format("Support ticket e-mail sent to {0}, greenlight support contact.", customEntitySupportContactEmailAddress));
                            notificationSent = true;
                        }
                    }
                }

                if (notificationSent == false)
                {
                    // notify all employees who are subscribed, if there aren't any just notify the account administrator email address
                    List<object[]> notificationSubscriptions = new Notifications(user.AccountID).GetNotificationSubscriptions(EmailNotificationType.SupportTicketNotification);
                    if (notificationSubscriptions.Count > 0)
                    {
                        var notificationEmployeeIds = new List<int>(from notificationSubscription in notificationSubscriptions where (sendType)notificationSubscription[0] == sendType.employee select (int)notificationSubscription[1]);

                        foreach (KeyValuePair<int, string> emailAddress in Employee.GetEmailAddresses(user.AccountID, notificationEmployeeIds))
                        {
                            emailSender.SendEmail(fromAddress, emailAddress.Value, subject, body);
                            cEventlog.LogEntry(string.Format("Support ticket e-mail sent to {0}, support ticket subscription.", emailAddress.Value));
                        }
                    }
                    else
                    {
                        var toAddress = string.Empty;
                        var mainAdministrator = Employee.Get(accountProperties.MainAdministrator, user.AccountID);

                        if (mainAdministrator != null)
                        {
                            toAddress = mainAdministrator.EmailAddress;
                        }

                        if (string.IsNullOrWhiteSpace(toAddress) == false)
                        {
                            emailSender.SendEmail(fromAddress, toAddress, subject, body);
                            cEventlog.LogEntry(string.Format("Support ticket e-mail sent to {0}, account administrator.", toAddress));
                        }
                    }
                }
            }
            else
            {
                var ticketOwner = Employee.Get(ticket.Owner, user.AccountID);

                if (string.IsNullOrWhiteSpace(ticketOwner.EmailAddress) == false)
                {
                    emailSender.SendEmail(fromAddress, ticketOwner.EmailAddress, subject, body);
                    cEventlog.LogEntry(string.Format("Support ticket e-mail sent to {0}, ticket owner.", ticketOwner.EmailAddress));
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Saves support ticket data in the customer database
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="identifier">The support ticket identifier, 0 if new</param>
        /// <param name="subject">The support ticket subject line</param>
        /// <param name="description">A description of the problem</param>
        /// <param name="status">The support ticket status</param>
        /// <param name="customEntityId">An associated greenlight</param>
        /// <returns>A number indicating whether or not the save was successful</returns>
        private static int Save(ICurrentUserBase user, int identifier, string subject, string description, SupportTicketStatus status, int customEntityId)
        {
            int returnValue;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.AddWithValue("@SubAccountID", user.CurrentSubAccountId);
                databaseConnection.AddWithValue("@SupportTicketId", identifier);
                databaseConnection.AddWithValue("@Subject", subject, 255);
                databaseConnection.AddWithValue("@Description", description, 4000);
                databaseConnection.AddWithValue("@Status", status);
                databaseConnection.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);
                databaseConnection.AddWithValue("@CustomEntityId", DBNull.Value);

                if (customEntityId > 0)
                {
                    databaseConnection.sqlexecute.Parameters["@CustomEntityId"].Value = customEntityId;
                }

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveSupportTicket");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        #endregion Private Methods
    }
}
