namespace SpendManagementLibrary.Logic_Classes.Tables
{
    using System;
    using System.Collections.Generic;

    public interface ITables
    {
        /// <summary>
        /// Get a <see cref="List{T}"/> of <seealso cref="cTable"/>that should be displayed as options for reports
        /// </summary>
        /// <param name="activeModule">The current <see cref="Modules"/></param>
        /// <returns>A <see cref="List{T}"/>of <seealso cref="cTable"/> that should be displayed for reporting.</returns>
        List<cTable> GetItemsForReportDropDown(Modules activeModule);

        /// <summary>
        /// Get an instance of <see cref="cTable"/> that matches the given Table ID
        /// </summary>
        /// <param name="tableID">The <see cref="Guid"/>ID to retrieve</param>
        /// <returns>An instance of <see cref="cTable"/>or null if not found.</returns>
        cTable GetTableByID(Guid tableID);
    }
}