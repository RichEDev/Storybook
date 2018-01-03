using System;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cEmailNotification
    {
        private int _emailNotification;
        private string _name;
        private string _description;
        private int _emailTemplateId;
        private bool _enabled;
        private CustomerType _customerType;
        private EmailNotificationType _emailNotificationType;

        public cEmailNotification(int emailNotificationId, string name, string description, int emailTemplateId, bool enabled, CustomerType customerType, EmailNotificationType emailNotificationType)
        {
            _emailNotification = emailNotificationId;
            _name = name;
            _description = description;
            _emailTemplateId = emailTemplateId;
            _enabled = enabled;
            _customerType = customerType;
            _emailNotificationType = emailNotificationType;
        }

        /// <summary>
        /// Gets or sets the EmailNotificationID
        /// </summary>
        public int EmailNotificationID
        {
            get { return _emailNotification; }
            set { _emailNotification = value; }
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the EmailTemplateID
        /// </summary>
        public int EmailTemplateID
        {
            get { return _emailTemplateId; }
            set { _emailTemplateId = value; }
        }

        /// <summary>
        /// Gets or sets Enabled
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// Gets or sets the CustomerType
        /// </summary>
        public CustomerType CustomerType
        {
            get { return _customerType; }
            set { _customerType = value; }
        }

        /// <summary>
        /// Gets or sets the EmailNotificationType
        /// </summary>
        public EmailNotificationType EmailNotificationType
        {
            get { return _emailNotificationType; }
            set { _emailNotificationType = value; }
        }
    }

    /// <summary>
    /// Represents a type of Email Notification.
    /// </summary>
    public enum EmailNotificationType
    {
        /// <summary>A standard email notification.</summary>
        StandardNotification = 1,
        /// <summary>An ESR summary email notification.</summary>
        ESRSummaryNotification = 2,
        /// <summary>An ESR 'outbound file detected' email notification.</summary>
        ESROutboundFileDetected = 3,
        /// <summary>An ESR 'file start process' email notification.</summary>
        ESROutboundFileStartProcess = 4,
        /// <summary>An ESR 'line manager role update' email notification.</summary>
        ESRLineManagerRoleUpdate = 5,
        /// <summary>An ESR 'invalid postcode' email notification.</summary>
        ESROutboundInvalidPostcodes = 6,
        /// <summary>A support ticket notification.</summary>
        SupportTicketNotification = 7,
        /// <summary>Notification on clearing audit log.</summary>
        AuditLogCleared=8,

        /// <summary>Email notfication for when users addresses are updated and they have a excess mileage value set</summary>
        ExcessMileage = 9
    }
}
