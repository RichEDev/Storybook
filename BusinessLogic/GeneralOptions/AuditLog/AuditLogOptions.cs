namespace BusinessLogic.GeneralOptions.AuditLog
{
    /// <summary>
    /// The audit log options.
    /// </summary>
    public class AuditLogOptions : IAuditLogOptions
    {
        /// <summary>
        /// Gets or sets the auditor email address.
        /// </summary>
        public string AuditorEmailAddress { get; set; }
    }
}
