
namespace SpendManagementLibrary.Logic_Classes.Tables
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.Logic_Classes.Fields;

    public class SubAccountTables :ITables
    {
        private readonly ITables _tables;

        private readonly IRelabler<cTable> _tableRelabler;

        /// <summary>
        /// Instantiate a new instance of the <see cref="SubAccountTables"/> class.
        /// </summary>
        /// <param name="tables">An instance of <see cref="ITables"/></param>
        /// <param name="tableRelabler">An instance of <see cref="IRelabler"/></param>
        public SubAccountTables(ITables tables, IRelabler<cTable> tableRelabler )
        {
            this._tables = tables;
            this._tableRelabler = tableRelabler;
        }

        /// <inheritdoc />
        public List<cTable> GetItemsForReportDropDown(Modules activeModule)
        {
            return this._tableRelabler.Convert(this._tables.GetItemsForReportDropDown(activeModule));
        }

        /// <summary>
        /// Get a list of reportable <see cref="cTable"/> objects.
        /// </summary>
        /// <param name="activeModule">The current active <see cref="Modules"/></param>
        /// <returns>A <see cref="List{T}"/> of <seealso cref="cTables"/>that have the report on attribute set to true.</returns>
        public List<cTable> GetReportableTables(Modules activeModule)
        {
            return this._tableRelabler.Convert(this._tables.GetReportableTables(activeModule));
        }

        /// <inheritdoc />
        public cTable GetTableByID(Guid tableID)
        {
            return this._tableRelabler.Convert(this._tables.GetTableByID(tableID));
        }

    }
}
