namespace BusinessLogic.GeneralOptions.Reminders
{
    /// <summary>
    /// Defines a <see cref="ReminderOptions"/> and it's members
    /// </summary>
    public class ReminderOptions : IReminderOptions
    {
        /// <summary>
        /// Gets or sets the enable claim approval reminders
        /// </summary>
        public bool EnableClaimApprovalReminders { get; set; }

        /// <summary>
        /// Gets or sets the enable current claim reminders
        /// </summary>
        public bool EnableCurrentClaimsReminders { get; set; }

        /// <summary>
        /// Gets or sets the claim approval reminder frequency
        /// </summary>
        public int ClaimApprovalReminderFrequency { get; set; }

        /// <summary>
        /// Gets or sets the current claim reminder frequency
        /// </summary>
        public int CurrentClaimsReminderFrequency { get; set; }
    }
}
