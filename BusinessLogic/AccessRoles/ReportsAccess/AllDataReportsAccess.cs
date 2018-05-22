namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System;

    /// <summary>
    /// Indicates a user has access to all data within the reports engine.
    /// </summary>
    [Serializable]
    public class AllDataReportsAccess : IReportsAccess
    {
        /// <inheritdoc />
        public ReportingAccess Level => ReportingAccess.AllData;
    }
}
