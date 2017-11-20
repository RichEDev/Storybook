namespace Spend_Management.shared.code.EasyTree
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Linq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Report;

    /// <summary>
    /// Manage a <see cref="List{T}"/> of <seealso cref="ReportCommonColumn"/>
    /// </summary>
    public class ReportCommonColumns
    {
        /// <summary>
        /// a private list of <see cref="ReportCommonColumn"/>.
        /// </summary>
        private static readonly ConcurrentBag<ReportCommonColumn> columns;

        /// <summary>
        /// Initialise the private list of <see cref="ReportCommonColumn"/> and store in a static list.
        /// </summary>
        static ReportCommonColumns()
        {
            columns = new ConcurrentBag<ReportCommonColumn>(InitialiseList());
        }

        /// <summary>
        /// Return a <see cref="List{T}"/> of <seealso cref="ReportCommonColumn"/> to populate the internal list.
        /// </summary>
        /// <returns>A list of <see cref="ReportCommonColumn"/> from the metabase.</returns>
        private static IEnumerable<ReportCommonColumn> InitialiseList()
        {
            var result = new List<ReportCommonColumn>();
            using (
                var expdata = new DatabaseConnection(
                                  ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                const string Strsql = "select tableid, fieldid, joinPath from reports_common_columns";
                using (var reader = expdata.GetReader(Strsql))
                {
                    if (reader != null)
                    {
                        var tableOrd = reader.GetOrdinal("tableid");
                        var fieldOrd = reader.GetOrdinal("fieldid");
                        var joinOrd = reader.GetOrdinal("joinpath");
                        while (reader.Read())
                        {
                            var table = reader.GetGuid(tableOrd);
                            var field = reader.GetGuid(fieldOrd);
                            if (!reader.IsDBNull(joinOrd))
                            {
                                var join = reader.GetString(joinOrd);
                                var commonColumn = new ReportCommonColumn(table, field, join);
                                result.Add(commonColumn);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of <see cref="ReportCommonColumn"/> for the given Table ID which relates to a <seealso cref="cTable"/>
        /// </summary>
        /// <param name="tableId">the <see cref="cTable"/> ID to get Common fields for.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of <see cref="ReportCommonColumn"/> for the given Table ID </returns>
        public ReadOnlyCollection<ReportCommonColumn> GetFavourites(Guid tableId)
        {
            return columns.Where(common => common.TableId == tableId).ToList().AsReadOnly();
        }
    }
}