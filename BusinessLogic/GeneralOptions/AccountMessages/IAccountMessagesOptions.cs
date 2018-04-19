namespace BusinessLogic.GeneralOptions.AccountMessages
{
    /// <summary>
    /// Defines a <see cref="IAccountMessagesOptions"/> and it's members
    /// </summary>
    public interface IAccountMessagesOptions
    {
        /// <summary>
        /// Gets or sets the account locked message.
        /// </summary>
        string AccountLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets the account currently locked message.
        /// </summary>
        string AccountCurrentlyLockedMessage { get; set; }
    }
}
