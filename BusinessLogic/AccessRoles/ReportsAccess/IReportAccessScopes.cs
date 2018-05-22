namespace BusinessLogic.AccessRoles.ReportsAccess
{
    /// <summary>
    /// Defines a member of <see cref="IReportsAccess"/> ReportAccess must be implemented.
    /// </summary>
    public interface IReportAccessScopes
    {
        /// <summary>
        /// Gets the <see cref="IReportsAccess" /> for this <see cref="IAccessRole"/>.
        /// </summary>
        IReportsAccess ReportsAccess { get; }
    }
}
