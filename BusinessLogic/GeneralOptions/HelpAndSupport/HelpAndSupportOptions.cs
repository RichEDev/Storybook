namespace BusinessLogic.GeneralOptions.HelpAndSupport
{
    /// <summary>
    /// Defines a <see cref="HelpAndSupportOptions"/> and it's members
    /// </summary>
    public class HelpAndSupportOptions : IHelpAndSupportOptions
    {
        /// <summary>
        /// Gets or sets the customer help information
        /// </summary>
        public string CustomerHelpInformation { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact name
        /// </summary>
        public string CustomerHelpContactName { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact telephone
        /// </summary>
        public string CustomerHelpContactTelephone { get; set; }

        /// <summary>
        /// Gets or sets the customer help contact fax
        /// </summary>
        public string CustomerHelpContactFax { get; set; }

        /// <summary>
        /// Gets or sets the customer help contract address
        /// </summary>
        public string CustomerHelpContactAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer help contract email address
        /// </summary>
        public string CustomerHelpContactEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the enable internal support tickets
        /// </summary>
        public bool EnableInternalSupportTickets { get; set; }
    }
}
