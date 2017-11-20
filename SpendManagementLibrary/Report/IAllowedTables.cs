namespace SpendManagementLibrary.Report
{
    using System;

    /// <summary>
    /// Manage the allowed tables list for reports.
    /// This is the list of tables that are allowed for each base table <see cref="Guid"/>
    /// </summary>
    public interface IAllowedTables : IGet<AllowedTable, Guid>
    {

    }
}