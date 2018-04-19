namespace BusinessLogic.GeneralOptions.Emails
{
    /// <summary>
    /// Defines a <see cref="EmailOptions"/> and it's members
    /// </summary>
    public class EmailOptions : IEmailOptions
    {
        /// <summary>
        /// Gets or set the email server address
        /// </summary>
        public string EmailServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the email server from address
        /// </summary>
        public string EmailServerFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the error email address
        /// </summary>
        public string ErrorEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the error email from address
        /// </summary>
        public string ErrorEmailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the source address
        /// </summary>
        public byte SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the email administrator
        /// </summary>
        public string EmailAdministrator { get; set; }
    }
}
