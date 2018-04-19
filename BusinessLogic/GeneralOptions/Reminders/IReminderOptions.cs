namespace BusinessLogic.GeneralOptions.Reminders
{
    /// <summary>
    /// Defines a <see cref="IReminderOptions"/> and it's members
    /// </summary>
    public interface IReminderOptions
    {
        /// <summary>
        /// Gets or sets the enable claim approval reminders
        /// </summary>
        bool EnableClaimApprovalReminders { get; set; }

        /// <summary>
        /// Gets or sets the enable current claim reminders
        /// </summary>
        bool EnableCurrentClaimsReminders { get; set; }

        /// <summary>
        /// Gets or sets the claim approval reminder frequency
        /// </summary>
        int ClaimApprovalReminderFrequency { get; set; }

        /// <summary>
        /// Gets or sets the current claim reminder frequency
        /// </summary>
        int CurrentClaimsReminderFrequency { get; set; }
    }
}
