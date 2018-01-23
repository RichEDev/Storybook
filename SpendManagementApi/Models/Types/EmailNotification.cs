namespace SpendManagementApi.Models.Types
{
    using Interfaces;
    using SpendManagementLibrary;
    
    
    /// <summary>
    /// Encapsultes the data necessaray fpr a user to subscribe to events, via email.
    /// </summary>
    public class EmailNotification : BaseExternalType, IApiFrontForDbObject<Notification, EmailNotification>
    {
        /// <summary>
        /// The unique Id of this EmailNotification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of this EmailNotification.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The description of this EmailNotification.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Id of the email template for this EmailNotification.
        /// </summary>
        public int EmailTemplateId { get; set; }

        /// <summary>
        /// Whether this EmailNotification is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The CustomerType of this EmailNotification.
        /// </summary>
        public CustomerType CustomerType { get; set; }

        /// <summary>
        /// The EmailNotificationType of this EmailNotification.
        /// </summary>
        public EmailNotificationType EmailNotificationType { get; set; }


        /// <summary>
        /// Converts from the DAL to the API type.
        /// </summary>
        /// <param name="dbType">the DAL type.</param>
        /// <param name="actionContext">The IActionContext</param>
        /// <returns>The API type.</returns>
        public EmailNotification From(Notification dbType, IActionContext actionContext)
        {
            Id = dbType.EmailNotificationID;
            Label = dbType.Name;
            Description = dbType.Description;
            EmailTemplateId = dbType.EmailTemplateID;
            Enabled = dbType.Enabled;
            CustomerType = dbType.CustomerType;
            EmailNotificationType = dbType.EmailNotificationType;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type, T.</returns>
        public Notification To(IActionContext actionContext)
        {
            return new Notification(Id, Label, Description, EmailTemplateId, Enabled, CustomerType, EmailNotificationType);
        }
    }
}