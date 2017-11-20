
namespace SpendManagementLibrary.Logic_Classes.Tables
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.Logic_Classes.Fields;

    public class SubAccountTables :ITables
    {
        private readonly ITables _tables;

        private readonly IRelabler<cTable> _tableRelabler;

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

        /// <inheritdoc />
        public cTable GetTableByID(Guid tableID)
        {
            return this._tableRelabler.Convert(this._tables.GetTableByID(tableID));
        }

    }
}
