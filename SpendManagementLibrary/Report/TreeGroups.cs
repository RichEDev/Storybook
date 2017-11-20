namespace SpendManagementLibrary.Report
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Manages the colection of <see cref="TreeGroup"/> items from the metabase.
    /// </summary>
    public class TreeGroups : ITreeGroups
    {
        /// <summary>
        /// A private list of <see cref="TreeGroup"/>
        /// </summary>
        private static readonly List<TreeGroup> Groups;

        static TreeGroups()
        {
            Groups = CacheList();
        }

        private static List<TreeGroup> CacheList()
        {
            var result = new List<TreeGroup>();
            using (
                var expdata = new DatabaseConnection(
                                  ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                const string Strsql = "SELECT Id, Name FROM TreeGroup";
                using (var reader = expdata.GetReader(Strsql))
                {
                    if (reader != null)
                    {
                        var idOrd = reader.GetOrdinal("Id");
                        var nameOrd = reader.GetOrdinal("Name");
                        while (reader.Read())
                        {
                            var id = reader.GetGuid(idOrd);
                            var name = reader.GetString(nameOrd);
                            result.Add(new TreeGroup(id, name));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get a matching <see cref="TreeGroup"/> from the internal list if possible.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>to search for.</param>
        /// <returns>An instance of <see cref="TreeGroup"/> or null if not found.</returns>
        public TreeGroup Get(Guid id)
        {
            return Groups.FirstOrDefault(group => group.Id == id);
        }
    }
}
