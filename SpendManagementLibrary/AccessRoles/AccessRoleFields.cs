namespace SpendManagementLibrary.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The access role fields class.
    /// </summary>
    public class AccessRoleFields
    {
        /// <summary>
        /// Connection string for the customer database
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRoleFields"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string of the customer DB.
        /// </param>
        public AccessRoleFields(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// Get reportable fields for access roles.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     with the list of fields and AccessRoleId
        /// </returns>
        public Dictionary<int, List<Guid>> GetReportableFieldsForAccessRoles()
        {
            var reportableFieldsData = new Dictionary<int, List<Guid>>();
            using (var databaseConnection = new DatabaseConnection(this._connectionString))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                using (var reader = databaseConnection.GetReader("GetReportableFieldIdsForAccessRole", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var accessRoleId = reader.GetInt32(reader.GetOrdinal("accessroleId"));
                        var fieldId = reader.GetGuid(reader.GetOrdinal("fieldId"));

                        if (reportableFieldsData.ContainsKey(accessRoleId) == false)
                        {
                            reportableFieldsData.Add(accessRoleId, new List<Guid>());
                        }

                        reportableFieldsData[accessRoleId].Add(fieldId);
                    }

                    reader.Close();
                }
            }

            return reportableFieldsData;
        }
    }
}
