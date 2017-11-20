namespace SpendManagementLibrary.Report
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A table Guid and it's list of allowed tables for reporting.
    /// </summary>
    public class AllowedTable
    {
        /// <summary>
        /// Gets or sets the tableId for this allowed table.
        /// </summary>
        public Guid TableId { get;}

        /// <summary>
        /// An internal list of the Table IDs that are allowed for the TableId
        /// </summary>
        private readonly List<Guid> _allowedTables;

        /// <summary>
        /// Creates an instance of <see cref="AllowedTable"/>
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="allowedTables"></param>
        public AllowedTable(Guid tableId, List<Guid> allowedTables)
        {
            this.TableId = tableId;
            this._allowedTables = allowedTables;
        }

        /// <summary>
        /// Does this <see cref="AllowedTable"/> contain the Table ID
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns>True if the list contains the given ID.</returns>
        public bool Contains(Guid tableId)
        {
            return this._allowedTables.Contains(tableId);
        }
    }
}