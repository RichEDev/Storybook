using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace SpendManagementLibrary
{
    [Serializable]
    public class NotificationTemplate
    {
        public NotificationTemplate(int emailtemplateid, string templateName, List<sSendDetails> recipienttypes, sBuildDetails subject, sBuildDetails body, bool systemtemplate, MailPriority priority,
                              Guid basetableid, DateTime? createdon, int? createdby, DateTime? modifiedon, int? modifiedby, bool? sendemail = null, bool sendCopyToDelegates = false,
                              byte? emaildirection = null, bool? sendnote = null, sBuildDetails note = default(sBuildDetails), Guid? templateId = null, bool? canSendMobileNotification = null, string mobileNotificationMessage = "")
        {
            this.EmailTemplateId = emailtemplateid;
            this.TemplateName = templateName;
            this.RecipientTypes = recipienttypes;
            this.Subject = subject;
            this.Body = body;
            this.SystemTemplate = systemtemplate;
            this.Priority = priority;
            this.BaseTableId = basetableid;
            this.CreatedOn = createdon;
            this.CreatedBy = createdby;
            this.ModifiedOn = modifiedon;
            this.ModifiedBy = modifiedby;
            this.SendEmail = sendemail;
            this.SendCopyToDelegates = sendCopyToDelegates;
            this.EmailDirection = emaildirection;
            this.SendNote = sendnote;
            this.Note = note;
            this.TemplateId = templateId;
            this.CanSendMobileNotification = canSendMobileNotification;
            this.MobileNotificationMessage = mobileNotificationMessage;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NotificationTemplate"/> class.
        /// </summary>
        public NotificationTemplate()
        {
        }

        #region properties
        public int EmailTemplateId { get; }

        public string TemplateName { get; }

        public List<sSendDetails> RecipientTypes { get; }

        public sBuildDetails Subject { get; }

        public sBuildDetails Body { get; }

        public bool SystemTemplate { get; }

        public MailPriority Priority { get; }

        public Guid BaseTableId { get; }

        public DateTime? CreatedOn { get; }

        public int? CreatedBy { get; }

        public DateTime? ModifiedOn { get; }

        public int? ModifiedBy { get; }

        public bool? SendEmail { get; }

        /// <summary>
        /// Gets a value indicating whether can template can send mobile notification.
        /// </summary>
        public bool? CanSendMobileNotification { get; }

        /// <summary>
        /// Determines whether a copy of this email template should be sent to delegate or not 
        /// </summary>
        public bool SendCopyToDelegates { get; }

        public byte? EmailDirection { get; }

        public bool? SendNote { get; }

        public sBuildDetails Note { get; }

        /// <summary>
        /// Gets the mobile notification message.
        /// </summary>
        public String MobileNotificationMessage { get; }

        public Guid? TemplateId { get; }

        #endregion
    }

    public enum sendType
    {
        lineManager = 0,
        team,
        budgetHolder,
        employee,
        itemOwner,
        approver,
        Field,
        Greenlightattributes
    }

    public enum recipientType
    {
        to = 0,
        cc,
        bcc
    }

    public enum emailFieldType
    {
        field = 0,
        sender,
        receiver
    }

    public enum emailTemplateSection
    {
        subject = 0,
        body,
        note
    }

    public struct sSendDetails
    {
        public sendType senderType;
        public string sender;
        public int idoftype;
        public recipientType recType;
    }

    public struct sBuildDetails
    {
        public string details;
        public List<sEmailFieldDetails> fieldDetails;
    }

    public struct sEmailFieldDetails
    {
        public cField field;
        public emailFieldType fieldType;

        public int JoinViaId;
    }

    //Used to return values for client side
    public struct sEmailRecipientInfo
    {
        public byte senderType;
        public string sender;
        public int idofsender;
        public byte recType;
    }

    public struct sFieldVals
    {
        public string fieldname;
        public string fieldvalue;
    }

    public struct FieldDetails
    {
        public bool IsTable;
        public string Name;
        public Guid Id;
        public Object OtherDetail;
        public int Value;
        public emailFieldType FieldType;
    }
}
