namespace SpendManagementApi.Models.Types.MyDetails
{
    using SpendManagementApi.Common.Enum;

    /// <summary>
    /// The notify admin of change.
    /// </summary>
    public class NotifyAdminOfChange
    {
        /// <summary>
        /// Gets or sets the outcome of a notify admin of change request
        /// </summary>
        public NotifyAdminOfChangesOutcome ActionOutcome { get; set; }
    }
}