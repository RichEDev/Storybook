namespace BusinessLogic.AccessRoles.ReportsAccess
{
    /// <summary>
    /// Defines the <see cref="ReportingAccess"/> Level must be implemented.
    /// </summary>
    public interface IReportsAccess
    {
        /// <summary>
        /// Gets the <see cref="ReportingAccess"/> for an instance off <see cref="IReportsAccess"/> stating what level of access in the reports engine should be granted.
        /// </summary>
        ReportingAccess Level { get; }
    }
}
