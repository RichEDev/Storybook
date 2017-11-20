namespace SpendManagementLibrary.Report
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Manage the allowed tables list for reports.
    /// This is the list of tables that are allowed for each base table <see cref="Guid"/>
    /// </summary>
    public class AllowedTables : IAllowedTables
    {
        private static readonly List<AllowedTable> allowedTables;

        /// <summary>
        /// Creates an instance of <see cref="AllowedTable"/>
        /// </summary>
        static AllowedTables()
        {
            allowedTables = CacheList();
        }

        /// <summary>
        /// Get the list of items from the metabase.
        /// </summary>
        /// <returns></returns>
        private static List<AllowedTable> CacheList()
        {
            var result = new List<AllowedTable>();
            using (var expdata = new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                var strsql = "select basetableid, tableid from reports_allowedtables_base order by basetableid";
                using (var reader = expdata.GetReader(strsql))
                {
                    if (reader != null)
                    {
                        var baseTableOrd = reader.GetOrdinal("basetableid");
                        var tableOrd = reader.GetOrdinal("tableid");
                        var currentTable = Guid.Empty;
                        var currentAllowedTables = new List<Guid>();
                        while (reader.Read())
                        {
                            var table = reader.GetGuid(baseTableOrd);
                            var allowedTable = reader.GetGuid(tableOrd);
                            if (table != currentTable)
                            {
                                if (currentTable != Guid.Empty)
                                {
                                    result.Add(new AllowedTable(currentTable, currentAllowedTables));
                                }

                                currentTable = table;
                                currentAllowedTables = new List<Guid>();
                            }

                            currentAllowedTables.Add(allowedTable);
                        }

                        if (currentTable != Guid.Empty)
                        {
                            result.Add(new AllowedTable(currentTable, currentAllowedTables));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get a matching <see cref="AllowedTable"/> from the internal list if possible.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>to search for.</param>
        /// <returns>An instance of <see cref="AllowedTable"/> or null if not found.</returns>
        public AllowedTable Get(Guid id)
        {
            return allowedTables.FirstOrDefault(t => t.TableId == id);
        }
    }
}