using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary
{
    public class EsrAssignmentLocations
    {
        private readonly int _accountid;

        public EsrAssignmentLocations(int accountId)
        {
            this._accountid = accountId;
        }

        public EsrAssignmentLocation GetEsrAssignmentLocationById(int esrAssignmentLocationId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@ESRAssignmentLocationId", esrAssignmentLocationId);

                using (IDataReader reader = connection.GetReader("GetEsrAssignmentLocationByESRAssignmentLocationId", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new EsrAssignmentLocation().Populate(reader);
                    }
                }
            }

            return null;
        }

        public EsrAssignmentLocation GetEsrAssignmentLocationById(int esrAssignId, DateTime asOfDate)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@esrAssignID", esrAssignId);
                connection.sqlexecute.Parameters.AddWithValue("@asOfDate", asOfDate);

                using (IDataReader reader = connection.GetReader("GetEsrAssignmentLocationById", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new EsrAssignmentLocation().Populate(reader);
                    }
                }
            }

            return null;
        }
    }
}
