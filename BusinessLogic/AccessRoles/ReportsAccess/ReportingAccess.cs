namespace BusinessLogic.AccessRoles.ReportsAccess
{
    /// <summary>
    /// An enum to define the reporting access
    /// </summary>
    public enum ReportingAccess
    {
        /// <summary>
        /// All employees whome this employee currently signs off
        /// </summary>
        EmployeesResponsibleFor = 1,

        /// <summary>
        /// The selected access roles
        /// </summary>
        SelectedRoles = 2,

        /// <summary>
        /// All data in the database
        /// </summary>
        AllData = 3
    }
}
