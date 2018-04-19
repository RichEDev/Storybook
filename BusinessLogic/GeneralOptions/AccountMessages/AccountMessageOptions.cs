namespace BusinessLogic.GeneralOptions.AccountMessages
{
    /// <summary>
    /// The account message options.
    /// </summary>
    public class AccountMessageOptions : IAccountMessagesOptions
    {
        /// <summary>
        /// Gets or sets the account locked message.
        /// </summary>
        public string AccountLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets the account currently locked message.
        /// </summary>
        public string AccountCurrentlyLockedMessage { get; set; }
    }
}
