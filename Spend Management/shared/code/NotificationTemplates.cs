namespace Spend_Management
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net.Mail;
    using Convert = System.Convert;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;
    using BusinessLogic.Reasons;

    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary;

    using Spend_Management.shared.code.Helpers;
    using Spend_Management.shared.code.DVLA;

    using shared.code;

    /// <summary>
    /// Class for the creation of email templates
    /// </summary>

    public class NotificationTemplates : INotificationTemplates
    {
        private readonly string passwordKey;

        private readonly int attempts;

        private ICurrentUser currentUser;

        readonly int nAccountid;
        SortedList<int, NotificationTemplate> list;

        private string mobilePrefix;

        private string hostName;

        private string brandName;

        private string path;

        private string serverName;

        private string identityName;

        private JoinVias joinVias;

        private readonly cEmployees _clsemps;

        /// <summary>
        /// Alternate Email Address to which email need to send
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// Boolean value which determine if the source of employee activation is ESR
        /// </summary>
        private bool fromEsr;

        /// <summary>
        /// string builder object
        /// </summary>
        private StringBuilder _inputStringBuilder;

        /// <summary>
        /// Claim Summary for [Pending Claim Details Of An Approver] tag
        /// </summary>
        private ApproverClaimsSummary _claimsSummary;

        private readonly Lazy<IDataFactory<IGeneralOptions, int>> _generalOptionsFactory = new Lazy<IDataFactory<IGeneralOptions, int>>(() => FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>());

        private readonly Lazy<IDataFactory<IProductModule, Modules>> _productModuleFactory = new Lazy<IDataFactory<IProductModule, Modules>>(() => FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>());

        /// <summary>
        /// The company name name.
        /// </summary>
        private string _companyName;

        /// <summary>
        /// The employee id.
        /// </summary>
        private int _employeeId;

        /// <summary>
        /// Create a new instance of <see cref="cEmailTemplates"/>
        /// </summary>
        public List<Guid?> PermittedMobileNotificationTemplateIds { get; } = new List<Guid?>() { new Guid("F929969F-B2F3-4B98-9252-7AE6B17A418B") };

        /// <summary>
        /// Create a new instance of <see cref="NotificationTemplates"/>
        /// </summary>
        /// <param name="accountid">The current account ID</param>
        public NotificationTemplates (ICurrentUser user)
        {
            this.nAccountid = user.AccountID;
            this.currentUser = user;
            this.joinVias = new JoinVias(this.currentUser);
            this._clsemps = new cEmployees(user.AccountID);
            this.InitialiseData();
            this._claimsSummary = new ApproverClaimsSummary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTemplates"/> class.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        public NotificationTemplates(int accountId)
        {
            this.nAccountid = accountId;
            this._clsemps = new cEmployees(accountId);

            this._claimsSummary = new ApproverClaimsSummary();
            this._inputStringBuilder = new StringBuilder();
            this.mobilePrefix = ConfigurationManager.AppSettings["MobilePrefix"];
            this.path = cMisc.Path == "/" ? string.Empty : cMisc.Path;

            this._employeeId = 0;

            var accounts = new cAccounts().GetAccountByID(accountId);

            this._companyName = accounts.companyid;

            this.hostName = HostManager.GetHostName(accounts.HostnameIds, Modules.Expenses, accounts.companyid);
            try
            {
                this.serverName = Environment.MachineName;
            }
            catch (Exception)
            {
                this.serverName = string.Empty;
            }

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();

            if (windowsIdentity != null)
            {
                this.identityName = windowsIdentity.Name;
            }

            this.brandName = this._productModuleFactory.Value[Modules.Expenses].BrandName;

            this.list = this.CacheList();
        }

        /// <summary>
        /// Create a new instance of <see cref="NotificationTemplates"/>
        /// </summary>
        /// <param name="user">Logged in user</param>
        /// <param name="alternateEmailId">Email id to which the email need to be send(This is for email address if it is not from employee table)</param>
        public NotificationTemplates(ICurrentUser user, string alternateEmailId)
        {
            this.nAccountid = user.AccountID;
            this.currentUser = user;
            this.joinVias = new JoinVias(this.currentUser);
            this._clsemps = new cEmployees(user.AccountID);
            this.InitialiseData();
            this._emailAddress = alternateEmailId;
        }

        /// <summary>
        /// Create a new NotificationTemplates for Employee locked / password request / Employee activated through ESR where current user cannot be obtained.
        /// </summary>
        /// <param name="accountId">The current account ID</param>
        /// <param name="employeeId">The current Employee ID</param>
        /// <param name="passwordKey">The password key for password request</param>
        /// <param name="attempts">The number of attempts when employee locked.</param>
        /// <param name="currentModule">The current module for this request</param>
        /// <param name="fromEsr">Boolean value determine if employee activation is made through ESR</param>
        public NotificationTemplates(int accountId, int employeeId, string passwordKey, int attempts, Modules currentModule,bool fromEsr = false)
        {
            this.currentUser = cMisc.GetCurrentUser($"{accountId}, {employeeId}");
            this.nAccountid = accountId;
            this.passwordKey = passwordKey;
            this.attempts = attempts;
            this.joinVias = new JoinVias(this.currentUser);
            this._clsemps = new cEmployees(accountId);
            this.InitialiseData(currentModule);
            this.fromEsr = fromEsr;
        }

        #region Properties

        private int accountid
        {
            get { return nAccountid; }
        }

        /// <summary>
        /// Alternate Email Address to which email need to send
        /// </summary>
        private string EmailAddress
        {
            get { return this._emailAddress; }
        }


        #endregion

        /// <summary>
        /// Cache the email templates
        /// </summary>
        /// <param name="currentModule"></param>
        public void InitialiseData(Modules currentModule = Modules.None)
        {
            this.joinVias = new JoinVias(this.currentUser);
            this._claimsSummary = new ApproverClaimsSummary();
            this._inputStringBuilder = new StringBuilder();
            this.mobilePrefix = ConfigurationManager.AppSettings["MobilePrefix"];
            this.path = cMisc.Path == "/" ? string.Empty : cMisc.Path;
            this.hostName = HostManager.GetHostName(this.currentUser.Account.HostnameIds, this.currentUser.CurrentActiveModule, this.currentUser.Account.companyid);
            try
            {
                this.serverName = Environment.MachineName;
            }
            catch (Exception)
            {
                this.serverName = string.Empty;
            }

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                this.identityName = windowsIdentity.Name;
            }

            var module = this._productModuleFactory.Value[currentModule == Modules.None ? this.currentUser.CurrentActiveModule : currentModule];
            this.brandName = module.BrandName;
            this.list = this.CacheList();
        }

        /// <summary>
        /// The determine default sender.
        /// </summary>
        /// <param name="accountProperties">
        /// An instance of <see cref="cAccountProperties"/>
        /// </param>
        /// <param name="employeeEmailAddress">
        /// The employee email address.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> of the senders e-mail address.
        /// </returns>
        public string DetermineDefaultSender(cAccountProperties accountProperties, string employeeEmailAddress)
        {
            var senderAddress = string.Empty;
            const string AdminEmailAddress = "admin@sel-expenses.com";

            if (accountProperties.SourceAddress == 1)
            {
                senderAddress = accountProperties.EmailAdministrator.Trim() == string.Empty
                              ? AdminEmailAddress
                              : accountProperties.EmailAdministrator;
            }
            else
            {
                if (employeeEmailAddress != string.Empty)
                {
                    senderAddress = employeeEmailAddress;
                }
                else
                {
                    // If no email address set then send from admin
                    senderAddress = accountProperties.EmailAdministrator.Trim() == string.Empty
                                  ? AdminEmailAddress
                                  : accountProperties.EmailAdministrator;
                }
            }

            return senderAddress;
        }

        private SortedList<int, NotificationTemplate> CacheList()
        {
            var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid));
            SortedList<int, NotificationTemplate> lstEmailTemplates = new SortedList<int, NotificationTemplate>();
            int emailtemplateid;
            bool systemTemplate;
            MailPriority priority;
            Guid baseTableID;
            string subject, templatename, bodyhtml, note, mobileNotificationMessage;
            int createdby;
            DateTime createdon;
            DateTime? modifiedon;
            int? modifiedby;
            bool sendnote;
            bool sendemail;
            bool canSendMobileNotification;
            bool sendCopyToDelegates;
            bool archived = false;

            SortedList<int, List<sSendDetails>> lstRecipients = this.getRecipients();
            SortedList<int, List<sEmailFieldDetails>> lstSubjectDetails = this.GetSubjectFields("Subject");
            SortedList<int, List<sEmailFieldDetails>> lstBodyDetails = this.GetSubjectFields("Body");
            SortedList<int, List<sEmailFieldDetails>> lstNoteDetails = this.GetSubjectFields("Notes");

            List<sSendDetails> recipients;
            List<sEmailFieldDetails> subjectDetails;
            List<sEmailFieldDetails> bodyDetails;
            List<sEmailFieldDetails> noteDetails;
            sBuildDetails emailSubjectDetails;
            sBuildDetails emailBodyDetails;
            sBuildDetails emailNoteDetails;

            const string sqlMain = "dbo.getEmailTemplate";

            using (IDataReader reader = expdata.GetReader(sqlMain, CommandType.StoredProcedure))
            {
                var templateIdOrd = reader.GetOrdinal("templateid");
                while (reader.Read())
                {
                    emailtemplateid = reader.GetInt32(reader.GetOrdinal("emailtemplateid"));

                    if (reader.IsDBNull(reader.GetOrdinal("templatename")))
                    {
                        templatename = string.Empty;
                    }
                    else
                    {
                        templatename = reader.GetString(reader.GetOrdinal("templatename"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("subject")))
                    {
                        subject = string.Empty;
                    }
                    else
                    {
                        subject = this.NormaliseNodeValue(reader.GetString(reader.GetOrdinal("subject")));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("bodyhtml")))
                    {
                        bodyhtml = string.Empty;
                    }
                    else
                    {
                        bodyhtml = this.NormaliseNodeValue(reader.GetString(reader.GetOrdinal("bodyhtml")));
                    }

                    priority = (MailPriority)reader.GetByte(reader.GetOrdinal("priority"));
                    baseTableID = reader.GetGuid(reader.GetOrdinal("basetableid"));
                    systemTemplate = reader.GetBoolean(reader.GetOrdinal("systemtemplate"));

                    if (reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("sendNote")))
                    {
                        sendnote = false;
                    }
                    else
                    {
                        sendnote = reader.GetBoolean(reader.GetOrdinal("sendNote"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("sendEmail")))
                    {
                        sendemail = false;
                    }
                    else
                    {
                        sendemail = reader.GetBoolean(reader.GetOrdinal("sendEmail"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("sendCopyToDelegates")))
                    {
                        sendCopyToDelegates = false;
                    }
                    else
                    {
                        sendCopyToDelegates = reader.GetBoolean(reader.GetOrdinal("sendCopyToDelegates"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("note")))
                    {
                        note = null;
                    }
                    else
                    {
                        note = this.NormaliseNodeValue(reader.GetString(reader.GetOrdinal("note")));
                    }

                    byte? emaildirection = null;

                    if (reader.IsDBNull(reader.GetOrdinal("emaildirection")))
                    {
                        emaildirection = null;
                    }
                    else
                    {
                        emaildirection = reader.GetByte(reader.GetOrdinal("emaildirection"));
                    }

                    Guid? templateId = null;

                    if (!reader.IsDBNull(templateIdOrd))
                    {
                        templateId = reader.GetGuid(templateIdOrd);
                    }

                    lstRecipients.TryGetValue(emailtemplateid, out recipients);

                    lstSubjectDetails.TryGetValue(emailtemplateid, out subjectDetails);
                    emailSubjectDetails = new sBuildDetails();
                    emailSubjectDetails.details = subject;
                    emailSubjectDetails.fieldDetails = subjectDetails;

                    lstBodyDetails.TryGetValue(emailtemplateid, out bodyDetails);
                    emailBodyDetails = new sBuildDetails();
                    emailBodyDetails.details = bodyhtml;
                    emailBodyDetails.fieldDetails = bodyDetails;

                    lstNoteDetails.TryGetValue(emailtemplateid, out noteDetails);
                    emailNoteDetails = new sBuildDetails();
                    emailNoteDetails.details = note;
                    emailNoteDetails.fieldDetails = noteDetails;

                    if (reader.IsDBNull(reader.GetOrdinal("CanSendMobileNotification")))
                    {
                        canSendMobileNotification = false;
                    }
                    else
                    {
                        canSendMobileNotification = reader.GetBoolean(reader.GetOrdinal("CanSendMobileNotification"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("MobileNotificationMessage")))
                    {
                        mobileNotificationMessage = null;
                    }
                    else
                    {
                        mobileNotificationMessage = reader.GetString(reader.GetOrdinal("MobileNotificationMessage"));
                    }
                   
                    lstEmailTemplates.Add(emailtemplateid, new NotificationTemplate(emailtemplateid, templatename, recipients, emailSubjectDetails, emailBodyDetails, systemTemplate, priority, baseTableID, createdon, createdby, modifiedon, modifiedby, sendemail, sendCopyToDelegates, emaildirection, sendnote, emailNoteDetails, templateId, canSendMobileNotification, mobileNotificationMessage));
      
                 }

                reader.Close();
            }

            return lstEmailTemplates;
        }

        private string NormaliseNodeValue(string text)
        {
            var regex = new Regex(@"<span.*?\[.*?\].*?<", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Value.Contains("["))
                {
                    var node = GetNode(match);
                    text = text.Replace(node, node.ToLower());
                }
            }

            return text;
        }

        public NotificationTemplate GetNotificationTemplateById(int id)
        {
            NotificationTemplate temp = null;
            list.TryGetValue(id, out temp);
            return temp;
        }

        public List<ListItem> CreateDropDown(Guid tableid)
        {
            SortedList<int, NotificationTemplate> sorted = list;
            List<ListItem> items = new List<ListItem>();

            foreach (NotificationTemplate temp in sorted.Values)
            {
                if (tableid == temp.BaseTableId)
                {
                    items.Add(new ListItem(temp.TemplateName, temp.EmailTemplateId.ToString()));
                }
            }

            return items;
        }

        public SortedList<int, List<sSendDetails>> getRecipients()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            SortedList<int, List<sSendDetails>> lstSendDetails = new SortedList<int, List<sSendDetails>>();
            List<sSendDetails> details;
            sSendDetails sendDetails;
            int emailtemplateid;

            strsql = "SELECT * FROM emailTemplateRecipients";
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    sendDetails = new sSendDetails();

                    emailtemplateid = reader.GetInt32(reader.GetOrdinal("emailtemplateid"));
                    sendDetails.senderType = (sendType)reader.GetByte(reader.GetOrdinal("sendertype"));

                    if (reader.IsDBNull(reader.GetOrdinal("sender")))
                    {
                        sendDetails.sender = string.Empty;
                    }
                    else
                    {
                        sendDetails.sender = reader.GetString(reader.GetOrdinal("sender"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("idofsender")))
                    {
                        sendDetails.idoftype = 0;
                    }
                    else
                    {
                        sendDetails.idoftype = reader.GetInt32(reader.GetOrdinal("idofsender"));
                    }

                    sendDetails.recType = (recipientType)reader.GetByte(reader.GetOrdinal("recipienttype"));

                    lstSendDetails.TryGetValue(emailtemplateid, out details);
                    if (details == null)
                    {
                        details = new List<sSendDetails>();
                        lstSendDetails.Add(emailtemplateid, details);
                    }

                    details.Add(sendDetails);
                }
                reader.Close();
            }

            return lstSendDetails;
        }

        private SortedList<int, List<sEmailFieldDetails>> GetSubjectFields(string table)
        {
            SortedList<int, List<sEmailFieldDetails>> lstFieldDetails;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                lstFieldDetails = new SortedList<int, List<sEmailFieldDetails>>();
                cFields clsfields = new cFields(this.accountid);

                const string selectSql = "SELECT EmailTemplateId, FieldId, EmailFieldType, joinViaId FROM ";
                string tableSql = string.Empty;

                switch (table.ToLower())
                {
                    case "body":
                        tableSql = "emailTemplateBodyFields";
                        break;
                    case "subject":
                        tableSql ="emailTemplateSubjectFields";
                        break;
                    case "notes":
                        tableSql = "emailTemplateNoteFields";
                        break;                  
                }

                using (var reader = expdata.GetReader(selectSql + tableSql))
                {
                    var emailTemplateIdOrd = reader.GetOrdinal("emailtemplateid");
                    var fieldIdOrd = reader.GetOrdinal("fieldid");
                    var emailFieldTypeOrd = reader.GetOrdinal("emailfieldtype");
                    var joinViaIdOrd = reader.GetOrdinal("joinViaId");
                    while (reader.Read())
                    {
                        var emailFieldDetails = new sEmailFieldDetails();

                        var emailtemplateid = reader.GetInt32(emailTemplateIdOrd);
                        var fieldid = reader.GetGuid(fieldIdOrd);
                        emailFieldDetails.field = clsfields.GetFieldByID(fieldid);
                        emailFieldDetails.fieldType = (emailFieldType)reader.GetByte(emailFieldTypeOrd);
                        emailFieldDetails.JoinViaId = reader.IsDBNull(joinViaIdOrd) ? 0 : reader.GetInt32(joinViaIdOrd);
                        List<sEmailFieldDetails> fieldDetails;
                        lstFieldDetails.TryGetValue(emailtemplateid, out fieldDetails);

                        if (fieldDetails == null)
                        {
                            fieldDetails = new List<sEmailFieldDetails>();
                            lstFieldDetails.Add(emailtemplateid, fieldDetails);
                        }

                        fieldDetails.Add(emailFieldDetails);
                    }
                    reader.Close();
                }
            }

            return lstFieldDetails;
        }



        /// <summary>
        /// Save a notification template to the database.
        /// </summary>
        /// <param name="notificationTemplate">notification template object</param>
        /// <param name="update">boolean that checks if it is an insert or update of the notification template to the database</param>
        /// <returns>The save notification template Id</returns>
        public int SaveNotificationTemplate(NotificationTemplate notificationTemplate, bool update)
        {
            IDBConnection connection = null;
            if (this.currentUser == null)
            {
                this.currentUser = cMisc.GetCurrentUser();
            }

            using (IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                if (!update && notificationTemplate.EmailTemplateId == 0)
                {
                    if (this.list.Any(e => e.Value.TemplateName.Trim().ToLower() == notificationTemplate.TemplateName.Trim().ToLower()))
                    {
                        expdata.sqlexecute.Parameters.Clear();
                        return -1;
                    }
                }

                expdata.sqlexecute.Parameters.AddWithValue("@emailtemplateid", notificationTemplate.EmailTemplateId);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.currentUser.EmployeeID);
                if (this.currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                if (notificationTemplate.TemplateName == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@templatename", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@templatename", notificationTemplate.TemplateName);
                }
                if (notificationTemplate.Subject.details == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@subject", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@subject", notificationTemplate.Subject.details);
                }

                if (notificationTemplate.Body.details == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@bodyhtml", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@bodyhtml", WebUtility.HtmlDecode(notificationTemplate.Body.details));
                }

                if (notificationTemplate.Note.details == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@note", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@note", WebUtility.HtmlDecode(notificationTemplate.Note.details));
                }

                expdata.sqlexecute.Parameters.AddWithValue("@priority", notificationTemplate.Priority);
                expdata.sqlexecute.Parameters.AddWithValue("@basetableid", notificationTemplate.BaseTableId);
                expdata.sqlexecute.Parameters.AddWithValue("@systemtemplate", notificationTemplate.SystemTemplate);

                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                if (notificationTemplate.ModifiedBy == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", notificationTemplate.CreatedBy);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", notificationTemplate.ModifiedBy);
                }

                if (notificationTemplate.SendNote == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendnote", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendnote", notificationTemplate.SendNote);
                }

                if (notificationTemplate.SendEmail == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendemail", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@sendemail", notificationTemplate.SendEmail);
                }

                if (notificationTemplate.CanSendMobileNotification == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@canSendMobileNotification", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@canSendMobileNotification", notificationTemplate.CanSendMobileNotification);
                }

                if (notificationTemplate.MobileNotificationMessage == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@mobileNotificationMessage", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@mobileNotificationMessage", notificationTemplate.MobileNotificationMessage);
                }

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveEmailTemplate");
                int notificationTemplateId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();

                this.saveRecipients(notificationTemplate.RecipientTypes, notificationTemplateId);
                this.saveEmailFields(notificationTemplate.Subject.fieldDetails, emailTemplateSection.subject, notificationTemplateId);
                this.saveEmailFields(notificationTemplate.Body.fieldDetails, emailTemplateSection.body, notificationTemplateId);
                this.saveEmailFields(notificationTemplate.Note.fieldDetails, emailTemplateSection.note, notificationTemplateId);

                return notificationTemplateId;
            }
        }

        private void saveRecipients(List<sSendDetails> recipients, int emailtemplateid)
        {
            if (this.currentUser == null)
            {
                this.currentUser = cMisc.GetCurrentUser();
            }
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                foreach (sSendDetails sdet in recipients)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@emailtemplateid", emailtemplateid);
                    expdata.sqlexecute.Parameters.AddWithValue("@sendertype", Convert.ToByte(sdet.senderType));
                    expdata.sqlexecute.Parameters.AddWithValue("@sender", sdet.sender);

                    if (sdet.idoftype == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@idofsender", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@idofsender", sdet.idoftype);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@recipienttype", Convert.ToByte(sdet.recType));

                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (this.currentUser.isDelegate == true)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }

                    expdata.ExecuteProc("saveEmailTemplateRecipients");
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
        }

        private void saveEmailFields(List<sEmailFieldDetails> fieldDetails, emailTemplateSection section, int emailtemplateid)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                foreach (sEmailFieldDetails efd in fieldDetails.Where(x => x.field != null))
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@emailtemplateid", emailtemplateid);
                    expdata.sqlexecute.Parameters.AddWithValue("@fieldid", efd.field.FieldID);
                    expdata.sqlexecute.Parameters.AddWithValue("@emailfieldtype", Convert.ToByte(efd.fieldType));

                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.currentUser.EmployeeID);
                    expdata.sqlexecute.Parameters.AddWithValue("@joinviaid", efd.JoinViaId);
                    if (this.currentUser.isDelegate == true)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }

                    switch (section)
                    {
                        case emailTemplateSection.subject:
                            expdata.ExecuteProc("saveEmailTemplateSubjectFields");
                            break;

                        case emailTemplateSection.body:
                            expdata.ExecuteProc("saveEmailTemplateBodyFields");
                            break;

                        case emailTemplateSection.note:
                            expdata.ExecuteProc("saveEmailTemplateNoteFields");
                            break;
                    }

                    expdata.sqlexecute.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// Deletes a notification template by its Id.
        /// </summary>
        /// <param name="notificationTemplateId">
        /// The notification template id.
        /// </param>
        public void DeleteNotificationTemplate(int notificationTemplateId)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@emailtemplateid", notificationTemplateId);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.currentUser.EmployeeID);
                if (this.currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                expdata.ExecuteProc("deleteEmailTemplate");
                expdata.sqlexecute.Parameters.Clear();
            }
        }



        public void getRecipientEmailFields(ref List<sSendDetails> lstRecipients, string value, recipientType recType, Guid basetableId)
        {
            sSendDetails sendDet;
            if (value.EndsWith("; "))
            {
                value = value.Substring(0, value.Length - 2);
            }

            string[] emails = value.Split(';');
            string substr;
            string sender;
            bool elementExists = true;
            Guid greenLighFieldId;

            foreach (string val in emails)
            {
                sendDet = new sSendDetails();
                if (val.Contains("{Team:"))
                {
                    substr = val.Replace("{Team:", string.Empty);
                    sender = substr.Substring(0, substr.Length - 1);
                    cTeams clsteams = new cTeams(this.accountid);

                    sendDet.sender = val.Trim();
                    sendDet.idoftype = clsteams.getTeamidByName(sender.Trim());

                    if (sendDet.idoftype == 0)
                    {
                        elementExists = false;
                    }

                    sendDet.recType = recType;
                    sendDet.senderType = sendType.team;
                }
                else if (val.Contains("{Budget Holder:"))
                {
                    substr = val.Replace("{Budget Holder:", string.Empty);
                    sender = substr.Substring(0, substr.Length - 1);
                    cBudgetholders clsbudget = new cBudgetholders(this.accountid);

                    sendDet.sender = val.Trim();
                    sendDet.idoftype = clsbudget.getBudgetHolderidByName(sender.Trim());

                    if (sendDet.idoftype == 0)
                    {
                        elementExists = false;
                    }

                    sendDet.recType = recType;
                    sendDet.senderType = sendType.budgetHolder;
                }
                else if (val.Contains("{GreenLight:"))
                {
                    substr = val.Replace("{GreenLight:", string.Empty);
                    sender = substr.Substring(0, substr.Length - 1);

                    var clsCustomEntities = new cCustomEntities(this.currentUser);
                    var emailTemplatesService = new svcNotificationTemplates();
                    var attributes = emailTemplatesService.GetGreenLightAttributes(basetableId.ToString());


                    foreach (var item in attributes)
                    {
                        var attributeDetails = clsCustomEntities.getAttributeByFieldId(item.Id);

                        if (!string.IsNullOrWhiteSpace(attributeDetails.displayname) && !string.IsNullOrWhiteSpace(sender) && attributeDetails.displayname == sender.Trim())
                        {
                            sendDet.idoftype = attributeDetails.attributeid;
                            sendDet.sender = val.Trim();
                            if (sendDet.idoftype == 0)
                            {
                                elementExists = false;
                            }

                            sendDet.recType = recType;
                            sendDet.senderType = sendType.Greenlightattributes;
                        }
                    }
                }
                else if (val.Contains("[") && val.Contains("]"))
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.employee;
                    sendDet.idoftype = this._clsemps.getEmployeeidByUsername(this.accountid, val.Substring(val.IndexOf('[') + 1).TrimEnd(']'));
                }
                else if (val.Contains("@"))
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.employee;
                }
                else if (val.Contains("{Approver}"))
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.approver;
                }
                else if (val.Contains("{Line Manager}"))
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.lineManager;
                }
                else if (val.Contains("{Item Owner}"))
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.itemOwner;
                }
                else if (Guid.TryParse(val, out greenLighFieldId) && new cFields(this.currentUser.AccountID).GetFieldByID(greenLighFieldId) != null)
                {
                    sendDet.sender = val.Trim();
                    sendDet.recType = recType;
                    sendDet.senderType = sendType.Field;
                }

                if (elementExists)
                {
                    if (sendDet.sender != null)
                    {
                        if (sendDet.sender.Length > 0 && !lstRecipients.Contains(sendDet))
                        {
                            lstRecipients.Add(sendDet);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Get the field details from the HTML text entered.
        /// </summary>
        /// <param name="text">The HTML from the email templates page</param>
        /// <param name="baseTableId">The GUID of the Base Table</param>
        /// <param name="user">The current user</param>
        /// <returns>A List of sEmailFieldDetails</returns>
        public List<sEmailFieldDetails> getFieldsFromText(string text, Guid baseTableId, ICurrentUser user)
        {
            cFields clsfields = new cFields(this.accountid);
            var result = new List<sEmailFieldDetails>();
            var regex = new Regex(@"<span.*?\[.*?\].*?<", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(text);
            var joinVias = new JoinVias(user);
            foreach (Match match in matches)
            {
                if (match.Value.Contains("["))
                {
                    sEmailFieldDetails emailFieldDetail;
                    var node = GetNode(match);
                    var description = GetDescription(match, out emailFieldDetail);

                    emailFieldDetail.JoinViaId = this.GetFieldJoinViaId(node, description, joinVias);
                    emailFieldDetail.field = clsfields.GetFieldByID(joinVias.LastField);
                    result.Add(emailFieldDetail);
                }
            }

            return result;

        }

        private static string GetDescription(Match match, out sEmailFieldDetails emailFieldDetail)
        {
            emailFieldDetail = new sEmailFieldDetails();
            var description = match.Value.Substring(match.Value.IndexOf("["))
                .Split(']')
                .FirstOrDefault()
                .Replace("[", string.Empty);
            emailFieldDetail.fieldType = emailFieldType.field;
            if (description.Contains("To:"))
            {
                emailFieldDetail.fieldType = emailFieldType.receiver;
            }
            if (description.Contains("From:"))
            {
                emailFieldDetail.fieldType = emailFieldType.sender;
            }

            return description;
        }

        private static string GetNode(Match match)
        {
            var nodeLocation = match.Value.IndexOf("node=");
            var node = match.Value.Substring(nodeLocation + 6).Split('"').FirstOrDefault();
            return node;
        }

        private int GetFieldJoinViaId(string joinViaIDs, string joinViaDescription, JoinVias joinVias)
        {
            var joinViaList = joinVias.JoinViaPartsFromCompositeGuid(joinViaIDs, null);
            if (joinViaList.Count > 0)
            {
                var joinVia = new JoinVia(0, joinViaDescription, Guid.Empty, joinViaList);
                var id = joinVias.SaveJoinVia(joinVia);
                return id;
            }

            return 0;
        }

        /// <summary>
        /// Process email template and send it
        /// </summary>
        /// <param name="emailtemplateid">
        /// Emailtemplate id.
        /// </param>
        /// <param name="entityfield">
        /// Entity field.
        /// </param>
        /// <param name="filterval">
        /// Filter value.
        /// </param>
        /// <param name="senderid">
        /// Sender id.
        /// </param>
        /// <param name="bodyUpdate">
        /// Body update.
        /// </param>
        public void SendMessage(int emailtemplateid, cField entityfield, int filterval, int senderid, string bodyUpdate)
        {
            NotificationTemplate emailTemp = this.GetNotificationTemplateById(emailtemplateid);

            if (emailTemp.SendEmail == true)
            {
                if (emailTemp.SystemTemplate && emailTemp.RecipientTypes == null)
                {
                    // Check if the email template is a DoC one that needs to send the email to an approver
                    if (emailTemp.TemplateId != null && EnumHelpers<DutyOfCareApproverEmailTemplates>.ContainsDescription(emailTemp.TemplateId.ToString().ToUpper()))
                    {
                        var dutyOfCare = new DutyOfCare();
                        int[] recipientIds = dutyOfCare.GetRecipientsForApproverEmails(this.accountid, senderid);

                        // loop through each recipient and send the message
                        foreach (var recipientId in recipientIds.Distinct())
                        {
                            if (recipientId > 0)
                            {
                                this.ProcessEmailWithToken(
                                    emailTemp,
                                    entityfield,
                                    filterval,
                                    senderid,
                                    bodyUpdate,
                                    recipientId);
                            }
                        }
                    }

                    // if it's a duty of care vehicle documents email that will be send to the claimant there is no need to define the approver
                    if (emailTemp.TemplateId != null && EnumHelpers<DutyOfCareClaimantEmailTemplates>.ContainsDescription(emailTemp.TemplateId.ToString().ToUpper()))
                    {
                        var dutyOfCare = new DutyOfCare();
                        int vehicleOwnerId = dutyOfCare.GetOwnerIdForDutyOfCareDocument(filterval, this.accountid, true);
                        this.ProcessEmailWithToken(emailTemp, entityfield, filterval, senderid, bodyUpdate,recieverId: vehicleOwnerId);
                    }

                    // if it's a duty of care driving licence email that will be send to the claimant there is no need to define the approver
                    if (emailTemp.TemplateId != null && EnumHelpers<DutyOfCareDrivingLicenceClaimantEmailTemplates>.ContainsDescription(emailTemp.TemplateId.ToString().ToUpper()))
                    {
                        var dutyOfCare = new DutyOfCare();
                        int vehicleOwnerId = dutyOfCare.GetOwnerIdForDutyOfCareDocument(filterval, this.accountid, false);
                        this.ProcessEmailWithToken(emailTemp, entityfield, filterval, senderid, bodyUpdate, recieverId: vehicleOwnerId);
                    }
                }
                else
                {
                    this.ProcessEmailWithToken(emailTemp, entityfield, filterval, senderid, bodyUpdate);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="emailtemplateid">
        /// </param>
        /// <param name="entityfield">
        /// </param>
        /// <param name="filterval">
        /// </param>
        /// <param name="senderid">
        /// </param>
        /// <param name="lstRecipientTypes">
        /// </param>
        /// <param name="bodyUpdate">
        /// Body Update.
        /// </param>
        public void SendMessage(int emailtemplateid, cField entityfield, int filterval, int senderid, List<sSendDetails> lstRecipientTypes, string bodyUpdate)
        {
            NotificationTemplate emailTemp = this.GetNotificationTemplateById(emailtemplateid);
            if (emailTemp.SendEmail == true)
            {
                processEmail(emailTemp, lstRecipientTypes, entityfield, filterval, senderid, bodyUpdate);
            }
        }

        /// <summary>
        /// Send an email or note to employee/user
        /// </summary>
        /// <param name="templateId">
        /// The email template id.
        /// </param>
        /// <param name="senderid">
        /// Sender id.
        /// </param>
        /// <param name="recipientsId">
        /// Recipients id.
        /// </param>
        /// <param name="filterfieldval">
        /// Filter value.
        /// </param>
        /// <param name="forceSend">
        /// Value indicating if send message forcibly.
        /// </param>
        /// <param name="defaultSender">
        /// The default sender.
        /// </param>
        /// <param name="filterFieldGuid">
        /// The filter Field Guid.
        /// </param>
        /// <param name="EmployeeDetailsForBody">
        /// The Employee Details used to replace tags in body.
        /// </param>
        public void SendMessage(Guid? templateId, int senderid, int[] recipientsId, object filterfieldval = null, bool forceSend = false, string defaultSender = null, Guid? filterFieldGuid = null, DataTable EmployeeDetailsForBody = null )
        {
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                NotificationTemplate reqmsg = this.Get(templateId);

                if (reqmsg != null)
                {
                    if (!forceSend && reqmsg.SendEmail == false && reqmsg.SendNote == false)
                    {
                        return;
                    }

                    //loop through each recipient and prepare the message 
                    foreach (var recipientId in recipientsId.Distinct())
                    {
                        if (recipientId <= 0) continue;

                        var cTable = new cTables(this.accountid);

                        var field = (!cTable.IsTableExistsById(reqmsg.BaseTableId) ? null : cTable.GetTableByID(reqmsg.BaseTableId).GetPrimaryKey());

                        if (filterFieldGuid != null)
                        {
                            var cField = new cFields(this.accountid);
                            field = cField.GetFieldByID(filterFieldGuid.Value);
                        }

                        var emailTemplate = new NotificationTemplate(reqmsg.EmailTemplateId, reqmsg.TemplateName,
                                reqmsg.RecipientTypes, reqmsg.Subject, reqmsg.Body, reqmsg.SystemTemplate,
                                reqmsg.Priority, reqmsg.BaseTableId, reqmsg.CreatedOn,
                                reqmsg.CreatedBy, reqmsg.ModifiedOn, reqmsg.ModifiedBy, reqmsg.SendEmail, reqmsg.SendCopyToDelegates,
                                reqmsg.EmailDirection, reqmsg.SendNote, reqmsg.Note, reqmsg.TemplateId);

                        if (filterfieldval == null) filterfieldval = recipientId;

                        if (forceSend || reqmsg.SendEmail == true)
                        {

                            this.ProcessEmailWithToken(emailTemplate, field, filterfieldval, senderid, string.Empty, recipientId, defaultSender, employeeDetailsForBody: EmployeeDetailsForBody);
                        }

                        if (reqmsg.SendNote == true)
                        {
                            var noteWithToken = this.ProcessNoteWithToken(emailTemplate, field,
                               filterfieldval, senderid, recipientId);

                            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", recipientId);
                            expdata.sqlexecute.Parameters.AddWithValue("@note", noteWithToken);
                            expdata.ExecuteProc("SaveNotes");
                            expdata.sqlexecute.Parameters.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process the email by building the recipients list, subject and body and then sending the email.
        /// </summary>
        /// <param name="emailTemp"></param>
        /// <param name="lstRecipientTypes"></param>
        /// <param name="entityfield"></param>
        /// <param name="filterval"></param>
        /// <param name="senderid"></param>
        /// <param name="bodyUpdate"></param>
        public void processEmail(NotificationTemplate emailTemp, List<sSendDetails> lstRecipientTypes, cField entityfield, int filterval, int senderid, string bodyUpdate)
        {
            if (emailTemp != null)
            {
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.accountid);
                cAccountSubAccount reqSubAccount = clsSubAccounts.getFirstSubAccount();
                EmailSender sender = new EmailSender(reqSubAccount.SubAccountProperties.EmailServerAddress);

                var tolst = new EmailRecipients();
                var cclst = new EmailRecipients();
                var bcclst = new EmailRecipients();

                foreach (sSendDetails det in lstRecipientTypes)
                {
                    switch (det.recType)
                    {
                        case recipientType.to:
                            this.getEmailAddressesBySenderType(ref tolst, det, emailTemp.BaseTableId, filterval);
                            break;
                        case recipientType.cc:
                            this.getEmailAddressesBySenderType(ref cclst, det, emailTemp.BaseTableId, filterval);
                            break;
                        case recipientType.bcc:
                            this.getEmailAddressesBySenderType(ref bcclst, det, emailTemp.BaseTableId, filterval);
                            break;
                    }
                }

                foreach (EmailRecipient to in tolst)
                {
                    int empid = to.EmployeeId;

                    var msg = new MailMessage();
                    msg.To.Add(to.EmailAddress);

                    if (emailTemp.SendCopyToDelegates && this._clsemps.userIsOnHoliday(empid))
                    {
                        this._clsemps.GetProxiesEmailAddress(ref cclst, empid);
                    }

                    foreach (EmailRecipient cc in cclst)
                    {
                        msg.CC.Add(cc.EmailAddress);
                    }

                    foreach (EmailRecipient bcc in bcclst)
                    {
                        msg.Bcc.Add(bcc.EmailAddress);
                    }
                    
                    if (emailTemp.Subject.fieldDetails != null)
                    {
                        msg.Subject = this.buildSQL(emailTemp.BaseTableId, emailTemp.Subject.details, emailTemp.Subject.fieldDetails, entityfield, filterval, empid, senderid).Replace("<br />", string.Empty).Replace("<br/>", string.Empty).Replace("\n", string.Empty);
                    }
                    else
                    {
                        msg.Subject = emailTemp.Subject.details;
                    }

                    msg.Subject = this.RemoveAllEmptyTags(msg.Subject);
                    msg.Priority = emailTemp.Priority;

                    if (emailTemp.Body.fieldDetails != null)
                    {
                        msg.Body = this.buildSQL(emailTemp.BaseTableId, emailTemp.Body.details, emailTemp.Body.fieldDetails, entityfield, filterval, empid, senderid);
                    }
                    else
                    {
                        msg.Body = emailTemp.Body.details;
                    }

                    if (msg.Body.Contains("[Item Details]") && emailTemp.BaseTableId == new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8"))
                    {
                        msg.Body = this.GetItemDetails(msg.Body, filterval, clsSubAccounts);
                    }

                    if (msg.Body.Contains("[Duty Of Care Document Details]") && emailTemp.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                    {
                        var docDocumentProcedure = emailTemp.TemplateId == new Guid("74cc59f4-61b5-4e66-931a-8710bc86022d") ? "GetEmailDetailsForClaimantsWithExpiredDutyOfCare" : "GetEmailDetailsForClaimantsWithExpiredDutyOfCareForApprover";
                        msg.Body = this.GetDutyOfCareDetails(msg.Body, (Guid)emailTemp.TemplateId, senderid, docDocumentProcedure);
                    }

                    if (msg.Body.Contains("[Pending Claim Details Of An Approver]"))
                    {
                        var formattedSummary = this._claimsSummary.GetFormattedSummaryForEmailReminder(empid, this.accountid, this._inputStringBuilder);
                        msg.Body = msg.Body.Replace("[Pending Claim Details Of An Approver]", formattedSummary);
                        msg.Body = this.ReplaceStaticFieldsWithValue(msg.Body);
                    }

                    if (msg.Body.Contains("[URL]") && (emailTemp.TemplateId == new Guid("61C92AE2-2FAA-4E81-9B7A-71F25AE32936") || emailTemp.TemplateId == new Guid("A7BB1638-8368-4814-9DB7-3754BE55DBDB")) && emailTemp.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                    {
                        var consentSecurityCode = this.GetConsentSecurityCode(this._clsemps);
                        var licencePortalUrl = this.GetLicencePortalUrl();
                        msg.Body = this.GetDvlaConsentDetails(msg.Body, (Guid)consentSecurityCode, licencePortalUrl);
                    }

                    if (emailTemp.TemplateId == new Guid("81EBC966-2D61-4C1A-BB39-B3D9BA9F5544") && emailTemp.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                    {
                        msg.Body = this.ReplaceUrl(msg.Body);
                    }

                    if (emailTemp.TemplateId == new Guid("56EC0718-A570-4E06-9D41-791D68FFDA11"))
                    {
                        msg.Body = this.ReplaceStaticFieldsWithValue(msg.Body);
                    }

                    if (msg.Body.Contains("#ListData#"))
                    {
                        msg.Body = msg.Body.Replace("#ListData#", bodyUpdate);
                    }

                    msg.Body = this.RemoveAllEmptyTags(msg.Body);
                    msg.IsBodyHtml = true;
                    msg.From = new MailAddress("admin@sel-expenses.com");

                    sender.SendEmail(msg);
                }


            }

            //msg.Attachments.Add(attachment);
        }

        /// <summary>
        /// Get Security code for the employees to access the consent portal to submit the consent
        /// </summary>
        ///  <param name="employees">Employees object required for current user employee details</param>
        /// <returns>Security code for accessting the licence portal</returns>
        private Guid? GetConsentSecurityCode(cEmployees employees)
        {
            var employee = employees.GetEmployeeById(this.currentUser.EmployeeID);
            var consentSecurityCode = employee.SecurityCode;
            return consentSecurityCode;
        }

        /// <summary>
        /// Process the email by building the recipients list, subject and body and then sending the email.
        /// </summary>
        /// <param name="emailTemplate">
        /// Email template object
        /// </param>
        /// <param name="entityField">
        /// Primary key
        /// </param>
        /// <param name="filterValue">
        /// Filter value
        /// </param>
        /// <param name="senderId">
        /// Sender id
        /// </param>
        /// <param name="bodyUpdate">
        /// Text to update body
        /// </param>
        /// <param name="recieverId">
        /// Reciever id
        /// </param>
        /// <param name="defaultSender">
        /// The default Sender.
        /// </param>
        /// <param name="employeeDetailsForBody">
        /// Employee details used to replace tags in email body
        /// </param>
        private void ProcessEmailWithToken(NotificationTemplate emailTemplate, cField entityField, object filterValue, int senderId, string bodyUpdate, int recieverId = 0, string defaultSender = null, DataTable employeeDetailsForBody = null)
        {
            if (emailTemplate != null)
            {
                cAuditLog clsaudit = new cAuditLog(accountid, senderId); 
             
                var lstRecipientTypes = emailTemplate.RecipientTypes;
                var clsSubAccounts = new cAccountSubAccounts(accountid);
                cAccountSubAccount reqSubAccount = clsSubAccounts.getFirstSubAccount();
                var sender = new EmailSender(reqSubAccount.SubAccountProperties.EmailServerAddress);

                var tolst = new EmailRecipients();
                var cclst = new EmailRecipients();
                var bcclst = new EmailRecipients();
                var entityId = 0;

                if (lstRecipientTypes != null)
                {
                    if (filterValue != null)
                    {
                        var listValue = filterValue as List<int>;
                        if (listValue != null && listValue.Count >= 1)
                        {
                            entityId = listValue[0];
                        }
                        else
                        {
                            entityId = (int)filterValue;
                        }
                    }

                    foreach (sSendDetails det in lstRecipientTypes)
                    {
                        switch (det.recType)
                        {
                            case recipientType.to:
                                this.getEmailAddressesBySenderType(ref tolst, det, emailTemplate.BaseTableId, entityId, filterValue);
                                break;
                            case recipientType.cc:
                                this.getEmailAddressesBySenderType(ref cclst, det, emailTemplate.BaseTableId, entityId, filterValue);
                                break;
                            case recipientType.bcc:
                                this.getEmailAddressesBySenderType(ref bcclst, det, emailTemplate.BaseTableId, entityId, filterValue);
                                break;
                        }
                    }
                }
                
                ////For DVLA consent submission email, the recipient address is not the employee email address. This will be provided in UI while submitting the consent
                if (emailTemplate.TemplateId == new Guid("61C92AE2-2FAA-4E81-9B7A-71F25AE32936") || emailTemplate.TemplateId == new Guid("A7BB1638-8368-4814-9DB7-3754BE55DBDB") || emailTemplate.TemplateId == new Guid("4457F8DF-BDB1-4C5A-BFA1-1B1FA3C52DF6") || emailTemplate.TemplateId == new Guid("877D893C-3FCB-40A3-A3DD-B2382D79459C") || emailTemplate.TemplateId == new Guid("9354DBDB-3F77-40CD-843E-02E23F9F23F5"))
                {
                    if (emailTemplate.TemplateId != new Guid("877D893C-3FCB-40A3-A3DD-B2382D79459C"))
                    {
                        tolst.Add(this._clsemps.getEmployeeidByEmailAddress(this.accountid, this.EmailAddress),this.EmailAddress);
                    }
                    else
                    {
                        var properties = reqSubAccount.SubAccountProperties;
                        if (properties.MainAdministrator > 0)
                        {
                            var employee = this._clsemps.GetEmployeeById(properties.MainAdministrator);
                            tolst.Add(properties.MainAdministrator, employee.EmailAddress);
                        }
                    }
                }
                else
                {                   
                    var emp = this. _clsemps.GetEmployeeById(recieverId);

                    if (emp != null && !string.IsNullOrEmpty(emp.EmailAddress))
                    {
                        tolst.Add(recieverId, emp.EmailAddress);
                    }
                }

                if (senderId != 0)
                {
                    var subaccs = new cAccountSubAccounts(this.accountid);
                    var employee = this._clsemps.GetEmployeeById(senderId);
                    var properties = subaccs.getSubAccountById(employee.DefaultSubAccount).SubAccountProperties;
                    if (properties.SourceAddress == 1 && properties.EmailAdministrator != string.Empty)
                    {
                        // override sender with email administrator address if present
                        defaultSender = properties.EmailAdministrator;
                    }
                    else
                    {
                        defaultSender = employee.EmailAddress;
                    }
                }

                foreach (EmailRecipient to in tolst)
                {
                    int empid = to.EmployeeId;

                    var msg = new MailMessage();
                    msg.To.Add(to.EmailAddress);

                    if (emailTemplate.SendCopyToDelegates && this._clsemps.userIsOnHoliday(empid))
                    {
                        this._clsemps.GetProxiesEmailAddress(ref cclst, empid);
                    }
                    
                    foreach (EmailRecipient cc in cclst)
                    {
                        msg.CC.Add(cc.EmailAddress);
                    }

                    foreach (EmailRecipient bcc in bcclst)
                    {
                        msg.Bcc.Add(bcc.EmailAddress);
                    }

                    if (recieverId > 0)
                    {
                        empid = recieverId;
                    }

                    // Dvla response code 501 : Insufficient credits for DVLA Connect service - Email to main administrator
                    if (emailTemplate.TemplateId == new Guid("877D893C-3FCB-40A3-A3DD-B2382D79459C"))
                    {
                        var properties = reqSubAccount.SubAccountProperties;
                        var employee = this._clsemps.GetEmployeeById(properties.MainAdministrator);
                        empid = employee.EmployeeID;
                    }

                    if (emailTemplate.Subject.fieldDetails != null)
                    {
                        var subject = this.buildSQL(emailTemplate.BaseTableId, emailTemplate.Subject.details,
                            emailTemplate.Subject.fieldDetails, entityField, filterValue, empid, senderId).Replace("<br />", string.Empty).Replace("<br/>", string.Empty).Replace("\n", string.Empty);
                        msg.Subject = subject;
                    }
                    else
                    {
                        msg.Subject = emailTemplate.Subject.details;
                    }

                    // Replace hierarachy of fields for Subject with values
                    msg.Subject = this.ReplaceStaticFieldsWithValue(msg.Subject);
                    msg.Subject = this.RemoveAllEmptyTags(msg.Subject);
                    msg.Priority = emailTemplate.Priority;

                    if (emailTemplate.Body.fieldDetails != null)
                    {
                        msg.Body = this.buildSQL(emailTemplate.BaseTableId, emailTemplate.Body.details, emailTemplate.Body.fieldDetails,
                            entityField, filterValue, empid, senderId);
                    }
                    else
                    {
                        msg.Body = emailTemplate.Body.details;
                    }

                    msg.Body = this.ReplaceStaticFieldsWithValue(msg.Body);
                    if (msg.Body.Contains("[Item Details]") && emailTemplate.BaseTableId == new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8"))
                    {
                        msg.Body = this.GetItemDetails(msg.Body, filterValue, clsSubAccounts);
                    }

                    if (msg.Body.Contains("[Duty Of Care Document Details]") && emailTemplate.BaseTableId == new Guid("28D592D7-4596-49C4-96B8-45655D8DF797"))
                    {
                        var docDocumentProcedure = emailTemplate.TemplateId == new Guid("74cc59f4-61b5-4e66-931a-8710bc86022d") ? "GetEmailDetailsForClaimantsWithExpiredDutyOfCare" : "GetEmailDetailsForClaimantsWithExpiredDutyOfCareForApprover";
                        msg.Body = this.GetDutyOfCareDetails(msg.Body, (Guid)emailTemplate.TemplateId, senderId, docDocumentProcedure);
                    }
                    if (msg.Body.Contains("[Pending Claim Details Of An Approver]"))
                    {
                        var formattedSummary = this._claimsSummary.GetFormattedSummaryForEmailReminder(empid, this.accountid, this._inputStringBuilder);
                        msg.Body = msg.Body.Replace("[Pending Claim Details Of An Approver]", formattedSummary.ToString());
                        msg.Body = this.ReplaceStaticFieldsWithValue(msg.Body);
                    }

                    if (msg.Body.Contains("[URL]") && (emailTemplate.TemplateId == new Guid("61C92AE2-2FAA-4E81-9B7A-71F25AE32936") || emailTemplate.TemplateId == new Guid("A7BB1638-8368-4814-9DB7-3754BE55DBDB")) && emailTemplate.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                    {
                        var consentSecurityCode = this.GetConsentSecurityCode(this._clsemps);
                        var licencePortalUrl = this.GetLicencePortalUrl();
                        msg.Body = this.GetDvlaConsentDetails(msg.Body, (Guid)consentSecurityCode, licencePortalUrl);
                    }

                    if (emailTemplate.TemplateId == new Guid("2599CFCA-9A3A-4530-906A-3EC5C697F6C6") || emailTemplate.TemplateId == new Guid("EC677111-ABE6-45F3-A47E-3BF706186ACE"))
                    {
                        var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid));
                        var status = AutoPopulatedDrivingLicence.GetDrivingLicenceStatus(this.accountid, senderId == 0 ? recieverId : senderId, connection);
                        msg.Body = this.GetDrivingLicenceStatus(msg.Body, status);
                    }

                    if (emailTemplate.TemplateId == new Guid("877D893C-3FCB-40A3-A3DD-B2382D79459C") || emailTemplate.TemplateId == new Guid("9354DBDB-3F77-40CD-843E-02E23F9F23F5"))
                    {
                        msg.Body = this.ReplaceErrorCodeInformation(this._clsemps, msg.Body,recieverId);
                    }

                    if (emailTemplate.TemplateId == new Guid("81EBC966-2D61-4C1A-BB39-B3D9BA9F5544") && emailTemplate.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                    {
                        msg.Body = this.ReplaceUrl(msg.Body);
                    }

                    if (msg.Body.Contains("[Review Date]") && emailTemplate.TemplateId == new Guid("6CDA99DA-3F42-4084-BF7A-E016598F12E9"))
                    {
                        msg.Body = this.ReplaceReviewExpiryDetails(msg.Body, employeeDetailsForBody);
                    }

                    msg.Body = this.RemoveAllEmptyTags(msg.Body);
                    if (msg.Body.Contains("#ListData#"))
                    {
                        msg.Body = msg.Body.Replace("#ListData#", bodyUpdate);
                    }

                    msg.IsBodyHtml = true;

                    if (!string.IsNullOrWhiteSpace(defaultSender))
                    {
                        msg.From = new MailAddress(defaultSender);
                    }
                    else
                    {
                        msg.From = new MailAddress("admin@sel-expenses.com");
                    }


                    var attachments = new cAttachments(this.accountid);
                    List<cAttachment> attachmentsList = attachments.GetAttachments("emailTemplate_attachments", emailTemplate.EmailTemplateId);

                    foreach (var attachment in attachmentsList)
                    {
                        msg.Attachments.Add(new Attachment(new MemoryStream(attachment.AttachmentData), attachment.FileName));
                    }

                    sender.SendEmail(msg);

                    if (senderId != 0)
                    {
                        var audit = string.Format("E-mail Sent (Subject:{0}, Sender:{1}, Recipient:{2})", msg.Subject, msg.From, msg.To);
                        clsaudit.addRecord(SpendManagementElement.EmailServer, audit, emailTemplate.EmailTemplateId);
                    }
                }
            }
        }

        /// <summary>
        /// Replace the Logon url tag in template with appropriate one based on the source of employee activation
        /// </summary>
        /// <param name="bodyText">Email template body text</param>
        /// <returns>Retunrs Email body text </returns>
        private string ReplaceUrl(string bodyText)
        {
          bodyText = bodyText.Replace("[LogOnUrl]", this.fromEsr  ? "https://" + ConfigurationManager.AppSettings["ApplicationURL"] :this.hostName);
          return bodyText;
        }

        /// <summary>
        /// The replace review expiry details.
        /// </summary>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        /// <param name="employeeDetails">
        /// The employee details.
        /// </param>
        /// <returns>
        /// Returns review details
        /// </returns>
        private string ReplaceReviewExpiryDetails(string bodyText, DataTable employeeDetails)
        {
            bodyText = bodyText.Replace("[Review Date]", employeeDetails.Rows[0]["ReviewDate"].ToString());
            bodyText = bodyText.Replace("[Review Expiry Date]", employeeDetails.Rows[0]["ExpiryDate"].ToString());
            bodyText = bodyText.Replace("[Frequency]", employeeDetails.Rows[0]["FrequencyInMonths"].ToString());
            return bodyText;
        }

        /// <summary>
        /// This method returns URL for accessing the licence portal for consent submit
        /// </summary>
        /// <returns>Returns URL based on config setting</returns>
        private string GetLicencePortalUrl()
        {
            string licencePortalUrl;
            var useTestMode = GlobalVariables.LicenceCheckPortalAccessMode == null ? "Live": GlobalVariables.LicenceCheckPortalAccessMode;
            if (useTestMode == "Live")
            {
                licencePortalUrl = GlobalVariables.LicenceCheckConsentPortalLiveUrl == null? string.Empty: GlobalVariables.LicenceCheckConsentPortalLiveUrl;
            }
            else
            {
                licencePortalUrl = GlobalVariables.LicenceCheckConsentPortalDemoUrl == null? string.Empty: GlobalVariables.LicenceCheckConsentPortalDemoUrl;
            }
            return licencePortalUrl;
        }

        /// <summary>
        /// Replace any [x] fields with the static info.  e.g. passwordKey
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>The completed text</returns>
        private string ReplaceStaticFieldsWithValue(string subject)
        {
            var result = subject.Replace("[passwordkey]", this.passwordKey);
            result = result.Replace("[attempts]", this.attempts.ToString());
            result = result.Replace("[brandname]", this.brandName);

            if (this.currentUser == null)
            {
                result = result.Replace("[companyid]", this._companyName);
                result = result.Replace("[employeeid]", this._employeeId.ToString());
            }
            else
            {
                result = result.Replace("[companyid]", this.currentUser.Account.companyid);
                result = result.Replace("[employeeid]", this.currentUser.EmployeeID.ToString());
            }
        
            result = result.Replace("[hostname]", this.hostName);
            result = result.Replace("[host]", this.hostName);
            result = result.Replace("[path]", this.path);
      
            result = result.Replace("[MobilePrefix]", this.mobilePrefix);
            result = result.Replace("[servername]", this.serverName);
            result = result.Replace("[identityname]", this.identityName);
            return result;
        }

        private string RemoveAllEmptyTags(string text)
        {
            var reg = new Regex(@"<(To:|From:|k|n|t|x).{8}-.*?>");
            return reg.Replace(text, string.Empty);
        }

        public string ProcessNoteWithToken(NotificationTemplate emailTemplate, cField entityField, object filterValue, int senderId, int receiverId = 0)
        {
            string noteWithToken = string.Empty;

            if (emailTemplate != null)
            {
                var receiverEmailList = new List<string>();

                var receiver = this._clsemps.GetEmployeeById(receiverId);

                if (receiver != null) receiverEmailList.Add(receiver.EmailAddress);

                foreach (string toaddr in receiverEmailList)
                {
                    int empid = this._clsemps.getEmployeeidByEmailAddress(this.accountid, toaddr);

                    if (receiverId > 0)
                    {
                        empid = receiverId;
                    }

                    if (emailTemplate.Note.fieldDetails != null)
                    {
                        noteWithToken = buildSQL(emailTemplate.BaseTableId, emailTemplate.Note.details, emailTemplate.Note.fieldDetails,
                            entityField, filterValue, empid, senderId);
                    }
                    else
                    {
                        noteWithToken = emailTemplate.Note.details;
                    }

                    if (noteWithToken != null)
                    {
                        // Replace hierarachy of fields for Email Content with values
                        noteWithToken = this.ReplaceStaticFieldsWithValue(noteWithToken);
                        if (noteWithToken.Contains("[Item Details]") && emailTemplate.BaseTableId == new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8"))
                        {
                            noteWithToken = this.GetItemDetails(noteWithToken, filterValue, new cAccountSubAccounts(this.accountid));
                        }
                        if (noteWithToken.Contains("[Duty Of Care Document Details]") && emailTemplate.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                        {
                            var docDocumentProcedure = emailTemplate.TemplateId == new Guid("74cc59f4-61b5-4e66-931a-8710bc86022d") ? "GetEmailDetailsForClaimantsWithExpiredDutyOfCare" : "GetEmailDetailsForClaimantsWithExpiredDutyOfCareForApprover";
                            noteWithToken = this.GetDutyOfCareDetails(noteWithToken, (Guid)emailTemplate.TemplateId, senderId, docDocumentProcedure);
                        }
                        if (noteWithToken.Contains("[Pending Claim Details Of An Approver]"))
                        {
                            var formattedSummary = this._claimsSummary.GetFormattedSummaryForEmailReminder(empid, this.accountid, this._inputStringBuilder);
                            noteWithToken = noteWithToken.Replace("[Pending Claim Details Of An Approver]", formattedSummary.ToString());
                            noteWithToken = this.ReplaceStaticFieldsWithValue(noteWithToken);
                        }

                        if (noteWithToken.Contains("[URL]") && (emailTemplate.TemplateId == new Guid("61C92AE2-2FAA-4E81-9B7A-71F25AE32936") || emailTemplate.TemplateId == new Guid("A7BB1638-8368-4814-9DB7-3754BE55DBDB")) && emailTemplate.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                        {
                            var consentSecurityCode = this.GetConsentSecurityCode(this._clsemps);
                            var licencePortalUrl = this.GetLicencePortalUrl();
                            noteWithToken = this.GetDvlaConsentDetails(noteWithToken, (Guid)consentSecurityCode, licencePortalUrl);
                        }

             if (emailTemplate.TemplateId == new Guid("81EBC966-2D61-4C1A-BB39-B3D9BA9F5544") && emailTemplate.BaseTableId == new Guid("618DB425-F430-4660-9525-EBAB444ED754"))
                        {
                            noteWithToken = this.ReplaceUrl(noteWithToken);
                        }

                        noteWithToken = this.RemoveAllEmptyTags(noteWithToken);
                    }
                }
            }

            return noteWithToken;
        }

        /// <summary>
        /// Replace email fields with values
        /// </summary>
        /// <param name="fieldQuery"></param>
        /// <param name="content"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private string ReplaceHierarcyFields(string fieldQuery, string content, string fields)
        {
            var value = new StringBuilder();
            var customerConnectionString = cAccounts.getConnectionString(accountid);
            var isValueFetched = false;
            using (var connection = new DatabaseConnection(customerConnectionString))
            {
                var dataSetValues = connection.GetDataSet(fieldQuery);
                if (dataSetValues != null && dataSetValues.Tables.Count > 0)
                {
                    foreach (DataRow row in dataSetValues.Tables[0].Rows)
                    {
                        if (isValueFetched) value.Append("<br/>");
                        value.Append(row[0]);
                        isValueFetched = true;
                    }
                }
                content = content.Replace(fields, value.ToString());
            }
            return content;
    }
       
        /// <summary>
        /// Get email addresses by sender type.
        /// </summary>
        /// <param name="lst">
        /// List of mail address collection.
        /// </param>
        /// <param name="dets">
        /// Send details.
        /// </param>
        /// <param name="basetableid">
        /// Base table id.
        /// </param>
        /// <param name="entityid">
        /// Entity id.
        /// </param>
        /// <param name="filterValue">The filter value (usually the record id for custom entities)</param>
        public void getEmailAddressesBySenderType(ref EmailRecipients lst, sSendDetails dets, Guid basetableid, int entityid, object filterValue = null)
        {
            int workflowid = 0;
            cWorkflows clsWorkflows = (currentUser != null ? new cWorkflows(currentUser) : null);
            cTables clstables = new cTables(accountid);
            cTable table = clstables.GetTableByID(basetableid);

            switch (dets.senderType)
            {
                case sendType.employee:
                    if (dets.sender != string.Empty)
                    {
                        if (dets.sender.Contains("@"))
                        {
                            lst.Add(dets.idoftype, dets.sender);
                        }
                        else
                        {
                            var empEmailAddress = this._clsemps.GetEmployeeById(dets.idoftype).EmailAddress;
                            if (!String.IsNullOrEmpty(empEmailAddress))
                            {
                                lst.Add(dets.idoftype, empEmailAddress);
                            }
                        }
                    }
                    break;
                case sendType.team:
                    cTeams clsteams = new cTeams(this.accountid);

                    cTeam team = clsteams.GetTeamById(dets.idoftype);

                    if (team != null)
                    {
                        foreach (int i in team.teammembers)
                        {
                            Employee emp = this._clsemps.GetEmployeeById(i);
                            if (emp != null)
                            {
                                if (emp.EmailAddress != string.Empty)
                                {
                                    lst.Add(i, emp.EmailAddress);
                                }
                            }
                        }
                    }
                    break;
                case sendType.budgetHolder:
                    cBudgetholders clsbudget = new cBudgetholders(this.accountid);
                    cBudgetHolder budgetHolder = clsbudget.getBudgetHolderById(dets.idoftype);

                    if (budgetHolder != null)
                    {
                        Employee employee = this._clsemps.GetEmployeeById(budgetHolder.employeeid);

                        if (employee != null)
                        {
                            if (employee.EmailAddress != string.Empty)
                            {
                                lst.Add(budgetHolder.employeeid, employee.EmailAddress);
                            }
                        }
                    }
                    break;
                case sendType.Greenlightattributes:

                    var customEntities = new cCustomEntities();
                    var attributeValue = customEntities.GetAttributeValueByAttributeId(filterValue, dets.idoftype, this.accountid);

                    var employeeId = 0;
                    if (int.TryParse(attributeValue, out employeeId))
                    {
                        attributeValue = Employee.Get(employeeId, this.accountid).EmailAddress;
                    }

                    if (!string.IsNullOrWhiteSpace(attributeValue))
                    {
                        lst.Add(employeeId, attributeValue);
                    }
                    break;
                case sendType.approver:
                    if (clsWorkflows != null)
                    {
                        workflowid = clsWorkflows.GetWorkflowIDForEntity(table, entityid);
                        if (workflowid > 0)
                        {
                            cWorkflowEntityDetails wedapp = clsWorkflows.GetCurrentEntityStatus(entityid, workflowid);
                            if (wedapp != null)
                            {
                                Employee appemp = wedapp.EntityAssignedApprover;
                                if (appemp != null)
                                {
                                    if (appemp.EmailAddress != string.Empty)
                                    {
                                        lst.Add(appemp.EmployeeID, appemp.EmailAddress);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case sendType.itemOwner:
                    if (clsWorkflows != null)
                    {
                        workflowid = clsWorkflows.GetWorkflowIDForEntity(table, entityid);
                        if (workflowid > 0)
                        {
                            cWorkflowEntityDetails weditem = clsWorkflows.GetCurrentEntityStatus(entityid,
                                workflowid);
                            Employee itememp = weditem?.EntityOwner;
                            if (itememp != null)
                            {
                                if (itememp.EmailAddress != string.Empty)
                                {
                                    lst.Add(itememp.EmployeeID, itememp.EmailAddress);
                                }
                            }
                        }
                    }
                    break;
                case sendType.lineManager:
                    if (clsWorkflows != null)
                    {
                        workflowid = clsWorkflows.GetWorkflowIDForEntity(table, entityid);
                        if (workflowid > 0)
                        {
                            cWorkflowEntityDetails wfEntityStatus = clsWorkflows.GetCurrentEntityStatus(entityid,
                                workflowid);

                            if (wfEntityStatus?.EntityAssignedApprover != null)
                            {
                                if (wfEntityStatus.EntityAssignedApprover.EmailAddress != string.Empty)
                                {
                                    lst.Add(wfEntityStatus.EntityAssignedApprover.EmployeeID, wfEntityStatus.EntityAssignedApprover.EmailAddress);
                                }
                            }
                        }
                    }
                    break;
                case sendType.Field:

                    Guid toGuid;
                    if (Guid.TryParseExact(dets.sender, "D", out toGuid))
                    {
                        cTables tables = new cTables(this.accountid);
                        cFields fields = new cFields(this.accountid);
                        cTable basetable = tables.GetTableByID(basetableid);
                        if (basetable != null)
                        {
                            cQueryBuilder qb = new cQueryBuilder(this.accountid, cAccounts.getConnectionString(accountid),
                                ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, basetable,
                                tables, fields);
                            cField tmpField = fields.GetFieldByID(toGuid);
                            if (tmpField != null)
                            {
                                qb.addColumn(tmpField);
                                qb.addFilter(basetable.GetPrimaryKey(), ConditionType.Equals,
                                    new object[] { entityid }, null, ConditionJoiner.And, null);
                                using (SqlDataReader reader = qb.getReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.GetFieldType(0) == typeof(string))
                                        {
                                            lst.Add(this._clsemps.getEmployeeidByEmailAddress(this.accountid, reader.GetString(0)), reader.GetString(0));
                                        }
                                    }
                                    reader.Close();
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public string buildSQL(Guid basetableid, string emailtext, List<sEmailFieldDetails> lstFields, cField filterfield, object filterfieldval, int receiverid, int senderid)
        {
            bool hasFields = false;
            bool hasSenderFields = false;
            bool hasReceiverFields = false;
            string text = this.RemoveFieldSpanHtml(emailtext);
            string connstring = cAccounts.getConnectionString(accountid);
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            cTable basetable = clstables.GetTableByID(basetableid);
            cTable employeebasetable = clstables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cQueryBuilder clsSQLquerybuild = new cQueryBuilder(accountid, connstring, ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, basetable, clstables, clsfields);
            clsSQLquerybuild.Distinct = true;
            clsSQLquerybuild.topLimit = 1;
            cQueryBuilder clsSQLreceiverbuild = new cQueryBuilder(accountid, connstring, ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, employeebasetable, clstables, clsfields);
            clsSQLreceiverbuild.Distinct = true;
            clsSQLreceiverbuild.topLimit = 1;
            cQueryBuilder clsSQLsenderbuild = new cQueryBuilder(accountid, connstring, ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, employeebasetable, clstables, clsfields);
            clsSQLsenderbuild.Distinct = true;
            clsSQLsenderbuild.topLimit = 1;

            var bodyFields = new List<sEmailFieldDetails>();
            var receiverFields = new List<sEmailFieldDetails>();
            var senderFields = new List<sEmailFieldDetails>();

            foreach (sEmailFieldDetails fieldDet in lstFields.Where(x => x.field != null))
            {
                JoinVia joinVia = null;
                if (fieldDet.JoinViaId > 0)
                {
                    joinVia = joinVias.GetJoinViaByID(fieldDet.JoinViaId);
                }

                switch (fieldDet.fieldType)
                {
                    case emailFieldType.field:
                        if (!this.FilterDistinctFields(fieldDet, clsSQLquerybuild.lstColumns, joinVia))
                        {
                            clsSQLquerybuild.addColumn(fieldDet.field, joinVia);
                            bodyFields.Add(fieldDet);
                            hasFields = true;
                        }

                        break;
                    case emailFieldType.receiver:
                        if (!this.FilterDistinctFields(fieldDet, clsSQLreceiverbuild.lstColumns, joinVia))
                        {
                            clsSQLreceiverbuild.addColumn(fieldDet.field, joinVia);
                            receiverFields.Add(fieldDet);
                            hasReceiverFields = true;
                        }

                        break;
                    case emailFieldType.sender:
                        if (!this.FilterDistinctFields(fieldDet, clsSQLsenderbuild.lstColumns, joinVia))
                        {
                            clsSQLsenderbuild.addColumn(fieldDet.field, joinVia);
                            senderFields.Add(fieldDet);
                            hasSenderFields = true;
                        }
                        break;
                }
            }

            cField employeefilterfield = clsfields.GetFieldByID(new Guid("eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"));

            object[] val1 = null;

            if (filterfieldval != null)
            {
                var listValue = filterfieldval as List<int>;

                val1 = (listValue != null ? new object[listValue.Count] : new[] { filterfieldval });

                if (listValue != null && val1.GetType().IsArray)
                {
                    for (int i = 0; i < listValue.Count; i++) { val1[i] = listValue[i]; }
                }
            }


            object[] receiver;
            receiver = new object[] { receiverid };
            object[] sender;
            sender = new object[] { senderid };
            clsSQLquerybuild.addFilter(filterfield, ConditionType.Equals, val1, null, ConditionJoiner.None, null);
            clsSQLreceiverbuild.addFilter(employeefilterfield, ConditionType.Equals, receiver, null, ConditionJoiner.None, null); // null as no joinvia in email? !!!!!!!
            clsSQLsenderbuild.addFilter(employeefilterfield, ConditionType.Equals, sender, null, ConditionJoiner.None, null); // null as no joinvia in email? !!!!!!!

            if (hasSenderFields)
            {
                DataSet senderds = clsSQLsenderbuild.getDataset();
                List<sFieldVals> lstSenderVals = this.getFieldValues(senderds, senderFields, emailFieldType.sender);
                this.replaceFieldAliasWithValue(ref text, lstSenderVals, emailFieldType.sender);
            }

            if (hasReceiverFields)
            {
                DataSet receiverds = clsSQLreceiverbuild.getDataset();
                List<sFieldVals> lstRecieverVals = this.getFieldValues(receiverds, receiverFields, emailFieldType.receiver);
                this.replaceFieldAliasWithValue(ref text, lstRecieverVals, emailFieldType.receiver);
            }

            if (hasFields)
            {
                DataSet fieldds = clsSQLquerybuild.getDataset();
                List<sFieldVals> lstFieldVals = this.getFieldValues(fieldds, bodyFields, emailFieldType.field);
                this.replaceFieldAliasWithValue(ref text, lstFieldVals, emailFieldType.field);
            }
            return text;
        }

        private bool FilterDistinctFields(sEmailFieldDetails fieldDet, List<cQueryField> columns, JoinVia joinVia)
        {
            return columns.Any(queryField => queryField.field != null && queryField.field.FieldID == fieldDet.field.FieldID && queryField.JoinVia == joinVia);
        }

        private string RemoveFieldSpanHtml(string text)
        {
            var regex = new Regex(@"\<\/*span *?(?:(?!\bstyle\b).)*? node=.*?\>.*?<\/\s*span>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(text))
            {
                var node = new Regex(@"node="".+?""", RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(match.Value);
                var prefix = string.Empty;
                if (match.Value.Contains("[To:"))
                {
                    prefix = "To:";
                }

                if (match.Value.Contains("[From:"))
                {
                    prefix = "From:";
                }

                var replaceText = string.Format("<{0}{1}>", prefix, node.Value.Replace("node=", string.Empty).Replace(@"""", string.Empty).ToLower());

                text = text.Replace(match.Value, replaceText);
            }

            return text;
        }

        public List<sFieldVals> getFieldValues(DataSet ds, List<sEmailFieldDetails> lstFields, emailFieldType type)
        {
            List<sFieldVals> lstFieldVals = new List<sFieldVals>();
            sFieldVals fieldval;

            if (ds == null || ds.Tables.Count < 1)
            {
                return lstFieldVals;
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int x = 0;
                foreach (sEmailFieldDetails fieldDet in lstFields)
                {
                    fieldval = new sFieldVals();

                    if (fieldDet.fieldType == type && fieldDet.field != null)
                    {
                        var fieldNode = "n" + fieldDet.field.FieldID.ToString();


                        if (fieldDet.JoinViaId > 0)
                        {
                            var joinVia = this.joinVias.GetJoinViaByID(fieldDet.JoinViaId);
                            var joinViaPath = TreeViewNodes.FormatNodeIdFromJoinViaParts(joinVia, new StringBuilder());
                            fieldNode = joinViaPath.Append(fieldNode).ToString();
                        }

                        fieldval.fieldname = fieldNode;
                        if (fieldDet.field.ValueList)
                        {
                            if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                            {
                                foreach (KeyValuePair<object, string> fieldListItem in fieldDet.field.ListItems)
                                {
                                    if (fieldListItem.Key.ToString() == ds.Tables[0].Rows[i][x].ToString())
                                    {
                                        fieldval.fieldvalue = fieldListItem.Value;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                fieldval.fieldvalue = string.Empty;
                            }
                        }
                        else
                        {
                            fieldval.fieldvalue = string.Empty;
                            switch (fieldDet.field.FieldType)
                            {
                                case "S":
                                case "FS":
                                case "LT":
                                case "CO":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        fieldval.fieldvalue = (string)ds.Tables[0].Rows[i][x];
                                    }
                                    break;
                                case "D":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        DateTime date = (DateTime)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = date.ToShortDateString();
                                    }
                                    break;
                                case "DT":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        DateTime date = (DateTime)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = date.ToString();
                                    }
                                    break;
                                case "T":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        DateTime date = (DateTime)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = date.ToString("T");
                                    }
                                    break;
                                case "X":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        fieldval.fieldvalue = ds.Tables[0].Rows[i][x].ToString();
                                    }
                                    break;
                                case "M":
                                case "FD":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        decimal num = (decimal)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = num.ToString("########0.00");
                                    }
                                    break;
                                case "C":

                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        decimal num = (decimal)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = num.ToString("########0.00");
                                    }
                                    break;
                                case "FI":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        int num = (int)ds.Tables[0].Rows[i][x];
                                        fieldval.fieldvalue = num.ToString();
                                    }
                                    break;
                                case "N":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        fieldval.fieldvalue = ds.Tables[0].Rows[i][x].ToString();
                                    }
                                    break;
                                case "F":
                                    if (ds.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        fieldval.fieldvalue = ds.Tables[0].Rows[i][x].ToString();
                                    }
                                    break;
                                default:
                                    fieldval.fieldvalue = "Field Type not recognised";
                                    break;

                            }
                        }

                        if (fieldval.fieldvalue != null && lstFieldVals.All(fieldValue => fieldValue.fieldname != fieldval.fieldname))
                        {
                            lstFieldVals.Add(fieldval);
                        }

                        x++;
                    }
                }
            }
            return lstFieldVals;
        }

        /// <summary>
        /// Get replace field alias with value.
        /// </summary>
        /// <param name="text">
        /// Text value.
        /// </param>
        /// <param name="lstVals">
        /// List of field value.
        /// </param>
        /// <param name="type">
        /// Email field type.
        /// </param>
        public void replaceFieldAliasWithValue(ref string text, List<sFieldVals> lstVals, emailFieldType type)
        {
            var fieldValues = new StringBuilder();
            var oldFieldName = string.Empty;
            var oldField = string.Empty;
            text = text.Replace("<_", "<");

            foreach (sFieldVals fieldVal in lstVals.OrderBy(x => x.fieldname))
            {
                var fieldPrefix = string.Empty;
                if (type == emailFieldType.receiver) fieldPrefix = "To:";
                else if (type == emailFieldType.sender) fieldPrefix = "From:";

                if (String.IsNullOrEmpty(fieldValues.ToString()))
                {
                    oldField = "<" + fieldPrefix + fieldVal.fieldname.ToLower() + ">";
                    fieldValues.Append(fieldVal.fieldvalue);
                    oldFieldName = fieldVal.fieldname;
                }
                else if (fieldVal.fieldname == oldFieldName)
                {
                    fieldValues.Append("<br/>");
                    fieldValues.Append(fieldVal.fieldvalue);
                }

                if (fieldVal.fieldname != oldFieldName)
                {
                    text = text.Replace(oldField, fieldValues.ToString());
                    fieldValues.Clear();
                    oldField = "<" + fieldPrefix + fieldVal.fieldname + ">";
                    var fieldValue = (string.IsNullOrWhiteSpace(fieldVal.fieldvalue) ? " " : fieldVal.fieldvalue);
                    fieldValues.Append(fieldValue);
                    oldFieldName = fieldVal.fieldname;
                }
            }

            if (!string.IsNullOrWhiteSpace(fieldValues.ToString()))
            {
                text = text.Replace(oldField, fieldValues.ToString());
            }
        }

        /// <summary>
        /// Get an email template with a matching email template Id
        /// </summary>
        /// <param name="templateid"></param>
        /// <returns></returns>
        public NotificationTemplate Get(Guid? templateid)
        {
            return this.list.FirstOrDefault(x => x.Value.TemplateId == templateid).Value;
        }

        private string GetItemDetails(string bodyText, object filterValue, cAccountSubAccounts subaccs)
        {
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            var filterExpenseId = new List<int>();

            if (filterValue is int)
            {
                filterExpenseId.Add((int)filterValue);
            }
            else if (filterValue is List<int>)
            {
                filterExpenseId = (List<int>)filterValue;
            }
            else
            {
                filterExpenseId = new List<int>((int[])filterValue);
            }

            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            var output = new StringBuilder();
            cMisc clsmisc = new cMisc(accountid);
            cGlobalCountries clsglobalcountries = new cGlobalCountries();
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();

            cSubcats clssubcats = new cSubcats(accountid);
            cCountries clscountries = new cCountries(accountid, subAccountID);
            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
            cAllowances clsallowances = new cAllowances(accountid);

            var claims = new cClaims(this.accountid);
            var filterItem = claims.getExpenseItemById(filterExpenseId[0]);
            if (filterItem == null)
            {
                return bodyText;
            }

            var claimExpenseItems = claims.getExpenseItemsFromDB(filterItem.claimid);


            output.Append("Please find details of the item(s) in question below:<br/><br/>");
            output.Append("<table>");
            int i = 0;
            foreach (cExpenseItem reqitem in claimExpenseItems.Values.Where(item => filterExpenseId.Contains(item.expenseid)))
            {
                cSubcat reqsubcat = clssubcats.GetSubcatById(reqitem.subcatid);

                if (claimExpenseItems.Count > 1)
                {
                    output.Append("<tr/><td><span style='text-decoration:underline;font-weight:bold;'>Item " + (i + 1) + "</span></td><tr/>");
                }
                //general details
                output.Append("<tr><td>Date:</td><td>" + reqitem.date.ToShortDateString() + "</td></tr>");

                if (reqitem.fromid != 0)
                {
                    if (Address.Get(this.accountid, reqitem.fromid) != null)
                    {
                        output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("from").description + ": </td><td>" + Address.Get(this.accountid, reqitem.fromid).FriendlyName + "</td></tr>");
                    }
                }
                if (reqitem.companyid != 0)
                {
                    output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("organisation").description + ": </td><td>" + Organisation.Get(accountid, reqitem.companyid).Name + "</td></tr>");
                }
                if (reqitem.reasonid != 0)
                {
                    var reason =
                        FunkyInjector.Container.GetInstance<IDataFactoryArchivable<IReason, int, int>>()[reqitem.reasonid];

                    output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("reason").description + ": </td><td>" + reason.Name + "</td></tr>");
                }
                if (reqitem.countryid != 0)
                {
                    output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("country").description + ": </td><td>" + clsglobalcountries.getGlobalCountryById(clscountries.getCountryById(reqitem.countryid).GlobalCountryId).Country + "</td></tr>");
                }
                if (reqitem.currencyid != 0)
                {
                    output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("currency").description + ": </td><td>" + clsglobalcurrencies.getGlobalCurrencyById(clscurrencies.getCurrencyById(reqitem.currencyid).globalcurrencyid).label + "</td></tr>");
                }
                if (reqitem.reason != string.Empty)
                {
                    output.Append("<tr><td>" + clsmisc.GetGeneralFieldByCode("otherdetails").description + ": </td><td>" + reqitem.reason + "</td></tr>");
                }

                //sub category specific
                output.Append("<tr><td>Expense Item: </td><td>" + reqsubcat.subcat + "</td></tr>");
                if (reqsubcat.mileageapp == true)
                {
                    output.Append("<tr><td>No Miles:" + reqitem.miles + "</td></tr>");
                }

                if (reqsubcat.passengersapp == true)
                {
                    output.Append("<tr><td>No Passengers: </td><td>" + reqitem.nopassengers + "</td></tr>");
                }

                if (reqsubcat.bmilesapp == true)
                {
                    output.Append("<tr><td>No Miles (Business): </td><td>" + reqitem.bmiles + "</td></tr>");
                }

                if (reqsubcat.pmilesapp == true)
                {
                    output.Append("<tr><td>No Miles (Personal): </td><td>" + reqitem.pmiles + "</td></tr>");
                }

                if (reqsubcat.staffapp == true)
                {
                    output.Append("<tr><td>No of Staff: </td><td>" + reqitem.staff + "</td></tr>");
                }

                if (reqsubcat.othersapp == true)
                {
                    output.Append("<tr><td>No of Others: </td><td>" + reqitem.others + "</td></tr>");
                }

                if (reqsubcat.attendeesapp == true)
                {
                    output.Append("<tr><td>Attendees: </td><td>" + reqitem.attendees + "</td></tr>");
                }

                if (reqsubcat.nonightsapp == true)
                {
                    output.Append("<tr><td>No Nights: </td><td>" + reqitem.nonights + "</td></tr>");
                }

                if (reqsubcat.tipapp == true)
                {
                    output.Append("<tr><td>Tip: </td><td>" + reqitem.tip + "</td></tr>");


                }

                if (reqsubcat.receiptapp == true)
                {

                    output.Append("<tr><td>Do you have a receipt:</td><td>");

                    if (reqitem.normalreceipt == true)
                    {
                        output.Append(" Yes");
                    }
                    else
                    {
                        output.Append(" No");
                    }
                    output.Append("</td></tr>");

                }

                if (reqsubcat.vatapp == true && reqsubcat.receiptapp == true)
                {
                    output.Append("<tr><td>Does it include VAT Details:</td><td>");

                    if (reqitem.receipt == true)
                    {
                        output.Append(" Yes");
                    }
                    else
                    {
                        output.Append(" No");
                    }
                    output.Append("</td></tr>");
                }
                if (reqsubcat.eventinhomeapp == true)
                {
                    output.Append("<tr><td>Event in home city:</td><td>");

                    if (reqitem.home == true)
                    {
                        output.Append(" Yes");
                    }
                    else
                    {
                        output.Append(" No");
                    }
                    output.Append("</td></tr>");

                }


                if (reqsubcat.calculation == CalculationType.FixedAllowance)
                {
                    output.Append("<tr><td>Claim Allowance:</td><td>");

                    if (reqitem.total != 0)
                    {
                        output.Append(" Yes");
                    }
                    else
                    {
                        output.Append(" No");
                    }
                    output.Append("</td>");
                    output.Append("<tr><td>No Allowances: </td><td>" + reqitem.quantity + "</td></tr>");

                }

                if (reqsubcat.calculation == CalculationType.DailyAllowance) //daily allowance stuff
                {


                    output.Append("<tr><td>Allowance: </td><td>" + clsallowances.getAllowanceById(reqitem.allowanceid).allowance + "</td></tr>");



                    output.Append("<tr><td>Start Date: </td><td>" + reqitem.allowancestartdate.ToString("dd/MM/yyyy hh:mm") + "</td></tr>");



                    output.Append("<tr><td>End Date:</td><td>" + reqitem.allowanceenddate.ToString("dd/MM/yyyy hh:mm") + "<br/>");

                    output.Append("<tr><td>Deduct amount (in GBP):</td><td>" + reqitem.allowancededuct.ToString("£###,##,##0.00") + "</td></tr>");


                }
                if (reqsubcat.calculation != CalculationType.PencePerMile && reqsubcat.calculation != CalculationType.DailyAllowance && reqsubcat.calculation != CalculationType.FixedAllowance)
                {
                    output.Append("<tr><td>Total (");
                    if (reqsubcat.addasnet == true)
                    {
                        output.Append("NET");
                    }
                    else
                    {
                        output.Append("Gross");
                    }
                    output.Append("):</td><td>");

                    var generalOptions = this._generalOptionsFactory.Value[subAccountID].WithCurrency();

                    if (reqitem.currencyid == generalOptions.Currency.BaseCurrency)
                    {
                        output.Append(reqitem.total.ToString("######0.00"));
                    }
                    else
                    {
                        output.Append(reqitem.convertedtotal.ToString("######0.00"));
                    }
                    output.Append("</td></tr>");
                }

                i++;
            }
            output.Append("</table>");


            return bodyText.Replace("[Item Details]", output.ToString());

        }

        /// <summary>
        /// Get Duty of care details
        /// </summary>
        /// <param name="bodyText">Body text which needs to be formatted</param>
        /// <param name="templateId">Email template ID</param>
        /// <param name="senderId">Sender ID for the email item</param>
        /// <returns>Email Body text</returns>
        private string GetDutyOfCareDetails(string bodyText, Guid templateId, int senderId, string docDocumentProcedure)
        {
            var claimantDutyOfCareDetails = new DutyOfCareEmailDetails();
            var documents = new StringBuilder();
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeId", senderId);
                var employeeDetails = this._clsemps.GetEmployeeById(senderId);
                using (var claimantInfo = expdata.GetReader(docDocumentProcedure, CommandType.StoredProcedure))
                {
                    while (claimantInfo.Read())
                    {
                        documents.Append(claimantInfo.GetString(0)).Append("<br />");
                        claimantDutyOfCareDetails.NumberOfDays = claimantInfo.GetInt32(1);
                        claimantDutyOfCareDetails.ExpiryDate = claimantInfo.GetDateTime(2);
                    }

                    claimantInfo.Close();
                }

                claimantDutyOfCareDetails.DocumentName = documents.ToString();
                expdata.sqlexecute.Parameters.Clear();
                var subAccounts = new cAccountSubAccounts(employeeDetails.AccountID);
                var properties = subAccounts.getSubAccountById(employeeDetails.DefaultSubAccount).SubAccountProperties;
                string logOnMessagesApprover = "<br/>", logOnMessagesClaimant = "<br/>";

                if (!properties.EnableAutomaticDrivingLicenceLookup)
                {
                    logOnMessagesApprover = "<br/>An email has been sent to remind them to upload their latest document.<br/><br/>";
                    logOnMessagesClaimant = "<br/>Please logon to Expenses and upload your latest documents for approval to ensure you can continue claiming for mileage.<br/><br/>";
                }

                if (claimantDutyOfCareDetails != null)
                {
                    bodyText = bodyText.Replace("[Claimants Full Name]", employeeDetails.FullName);
                    bodyText = bodyText.Replace("[Duty Of Care Document Details]", claimantDutyOfCareDetails.DocumentName ?? "(Unknown Duty Of Care Document)");
                    bodyText = bodyText.Replace("[Expiry Date]", claimantDutyOfCareDetails.ExpiryDate.ToString("dd/MM/yyyy"));
                    bodyText = bodyText.Replace("[Number Of Days]", claimantDutyOfCareDetails.NumberOfDays.ToString(CultureInfo.InvariantCulture));
                    bodyText = bodyText.Replace("[Logon messages]", templateId == new Guid("74cc59f4-61b5-4e66-931a-8710bc86022d") ? logOnMessagesClaimant : logOnMessagesApprover);
                }
            }

            return bodyText;
        }

        ///  <summary>
        ///  Replace the account information error code and description in Dvla error notification eamil templates
        ///  </summary>
        ///  <param name="employees">Employees object required for current user employee details</param>
        ///  <param name="bodyText">Body text for the dvla email which needs to be updated</param>
        ///  <param name="employeeId">Id of the employee for which the error is logged</param>
        ///  <returns>Modified email template body text</returns>
        private string ReplaceErrorCodeInformation(cEmployees employees, string bodyText,int employeeId)
        {
            var employee = employees.GetEmployeeById(employeeId);
            bodyText = bodyText.Replace("[AccountId]", employee.AccountID.ToString());
            bodyText = bodyText.Replace("[CompanyId]", this.currentUser.Account.companyid);
            bodyText = bodyText.Replace("[Error Code]", employee.DvlaResponseCode);
           
            var dutyOfCareConstant = new DutyOfCareConstants();
            var responseCodes = dutyOfCareConstant.MapError(employee.DvlaResponseCode);
            bodyText = bodyText.Replace("[Error Description]", responseCodes.ResponseCodeFriendlyMessages);
            return bodyText;
        }

        /// <summary>
        /// Get Dvla Consent Details to send to claimant with the URL and the secrity code to access the portal
        /// </summary>
        /// <param name="bodyText">Body text which needs to be formatted</param>
        /// <param name="consentSecurityCode">Consent SecurityCode generated on submitting the request</param>   
        /// <param name="licencePortalUrl">url to access the DVLA consent portal to submit the consent</param>   
        /// <returns>Email Body text with the Security code and url to access the portal</returns>
        private string GetDvlaConsentDetails(string bodyText, Guid consentSecurityCode, string licencePortalUrl)
        {
            bodyText = bodyText.Replace("[Security Code]", consentSecurityCode.ToString());
            bodyText = bodyText.Replace("[URL]", licencePortalUrl+"/" + consentSecurityCode.ToString());
            return bodyText;
        }

        /// <summary>
        /// The get driving licence status.
        /// </summary>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// Final email message
        /// </returns>
        private string GetDrivingLicenceStatus(string bodyText, string status)
        {
            bodyText = bodyText.Replace("[Status]", status);
            return bodyText;
        }
    }
}
