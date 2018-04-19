namespace BusinessLogic.GeneralOptions.Emails
{
    /// <summary>
    /// Defines a <see cref="IEmailOptions"/> and it's members
    /// </summary>
    public interface IEmailOptions
    {
        /// <summary>
        /// Gets or set the email server address
        /// </summary>
        string EmailServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the email server from address
        /// </summary>
        string EmailServerFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the error email address
        /// </summary>
        string ErrorEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the error email from address
        /// </summary>
        string ErrorEmailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the source address
        /// </summary>
        byte SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the email administrator
        /// </summary>
        string EmailAdministrator { get; set; }
    }
}
