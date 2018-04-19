namespace BusinessLogic.GeneralOptions.AuditLog
{
    /// <summary>
    /// Defines a <see cref="IAuditLogOptions"/> and it's members
    /// </summary>
    public interface IAuditLogOptions
    {
        /// <summary>
        /// Gets or sets the auditor email address.
        /// </summary>
        string AuditorEmailAddress { get; set; }
    }
}
