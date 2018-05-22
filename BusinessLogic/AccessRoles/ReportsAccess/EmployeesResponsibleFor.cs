namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System;

    /// <summary>
    /// Indicates a user has access to all data for users they are responsible for.
    /// </summary>
    [Serializable]
    public class EmployeesResponsibleFor : IReportsAccess
    {
        /// <inheritdoc />
        public ReportingAccess Level => ReportingAccess.EmployeesResponsibleFor;
    }
}
