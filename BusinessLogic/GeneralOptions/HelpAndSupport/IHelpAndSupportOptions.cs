namespace BusinessLogic.GeneralOptions.HelpAndSupport
{
    /// <summary>
    /// Defines a <see cref="IHelpAndSupportOptions"/> and it's members
    /// </summary>
    public interface IHelpAndSupportOptions
    {
        /// <summary>
        /// Gets or sets the customer help information
        /// </summary>
        string CustomerHelpInformation { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact name
        /// </summary>
        string CustomerHelpContactName { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact telephone
        /// </summary>
        string CustomerHelpContactTelephone { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact fax
        /// </summary>
        string CustomerHelpContactFax { get; set; }

        /// <summary>
        /// Gets or sets the customer help contract address
        /// </summary>
        string CustomerHelpContactAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer help contract email address
        /// </summary>
        string CustomerHelpContactEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the enable internal support tickets
        /// </summary>
        bool EnableInternalSupportTickets { get; set; }
    }
}
