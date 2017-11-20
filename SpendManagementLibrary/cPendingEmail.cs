using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    #region PendingEmailType Enumerations
    /// <summary>
    /// Enumeration of Pending Email Types
    /// </summary>
    public enum PendingMailType
    {
        /// <summary>
        /// Notify that status of a task has changed
        /// </summary>
        TaskStatusChange = 0,
        /// <summary>
        /// Notify that a new task owner has been allocated
        /// </summary>
        TaskOwnerAllocate,
        /// <summary>
        /// Notify a user that they have been deallocated from a task
        /// </summary>
        TaskOwnerDeallocate,
        /// <summary>
        /// Notify a user or team that a task is overdue
        /// </summary>
        TaskOverdue,
        /// <summary>
        /// Notify a user or team that a task is late
        /// </summary>
        TaskLate,
        /// <summary>
        /// Notify that an employee needs activating
        /// </summary>
        TaskEmployeeActivate,
        /// <summary>
        /// Notify that an employee needs archiving
        /// </summary>
        TaskEmployeeArchive,

        /// <summary>
        /// Notify a user that a new task created has been allocated to them
        /// </summary>
        TaskAllocateNew

    }
    #endregion

    #region cPendingEmail
    /// <summary>
    /// cPendingEmails class
    /// </summary>
    public class cPendingEmail
    {
        #region properties
        /// <summary>
        /// Email Id
        /// </summary>
        private int emailid;
        /// <summary>
        /// Gets the pending email id
        /// </summary>
        public int EmailId
        {
            get { return emailid; }
        }
        /// <summary>
        /// Pending Email Type
        /// </summary>
        private PendingMailType emailtype;
        /// <summary>
        /// Gets the pending email type
        /// </summary>
        public PendingMailType EmailType
        {
            get { return emailtype; }
        }
        /// <summary>
        /// Date pending email created
        /// </summary>
        private DateTime datestamp;
        /// <summary>
        /// Gets the date the pending email was created
        /// </summary>
        public DateTime Datestamp
        {
            get { return datestamp; }
        }
        /// <summary>
        /// Email Subject
        /// </summary>
        private string subject;
        /// <summary>
        /// Gets the pending email subject
        /// </summary>
        public string Subject
        {
            get { return subject; }
        }
        /// <summary>
        /// Email body
        /// </summary>
        private string body;
        /// <summary>
        /// Gets the pending email body
        /// </summary>
        public string MailBody
        {
            get { return body; }
        }
        /// <summary>
        /// Pending email recipient Id
        /// </summary>
        private int recipientid;
        /// <summary>
        /// Getting pending email recipient Id
        /// </summary>
        public int RecipientId
        {
            get { return recipientid; }
        }
        /// <summary>
        /// Recipient type (individual or team)
        /// </summary>
        private sendType recipienttype;
        /// <summary>
        /// Gets the recipient type (individual or team)
        /// </summary>
        public sendType RecipientType
        {
            get { return recipienttype; }
        }
        #endregion

        /// <summary>
        /// cPendingEmail constructor
        /// </summary>
        /// <param name="emailId">Pending Email Id</param>
        /// <param name="emailType">Pending Email Type</param>
        /// <param name="emailDateStamp">Date email created</param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email Body</param>
        /// <param name="emailRecipientId">Recipient Id</param>
        /// <param name="emailRecipientType">Recipient Type (Individual or Team)</param>
        public cPendingEmail(int emailId, PendingMailType emailType, DateTime emailDateStamp, string emailSubject, string emailBody, int emailRecipientId, sendType emailRecipientType)
        {
            emailid = emailId;
            emailtype = emailType;
            datestamp = emailDateStamp;
            subject = emailSubject;
            body = emailBody;
            recipientid = emailRecipientId;
            recipienttype = emailRecipientType;
        }
    }
    #endregion
}
