namespace SpendManagementApi.Common.Enum
{
    /// <summary>
    /// An enum for recording the outcome of notifying the admin of a change
    /// </summary>
    public enum NotifyAdminOfChangesOutcome
    {
        /// <summary>
        /// The email sent sucessfully.
        /// </summary>
        EmailSentSuccessfully = 0,

        /// <summary>
        /// An occured during sending the email.
        /// </summary>
        ErrorDuringSending = 1,

        /// <summary>
        /// The account has no administrator.
        /// </summary>
        AccountHasNoAdministrator = 2,

        /// <summary>
        /// The account administrator has no email address.
        /// </summary>
        AccountAdministratorHasNoEmailAddress = 3,

        /// <summary>
        /// The account does not permit change of detail notfications to be sent to the Administrator.
        /// </summary>
        AccountDoesNotPermitChangeOfDetailsNotifications = 4
     }
}