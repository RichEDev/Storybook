using System;
using System.Data;

namespace SpendManagementLibrary
{
    public class EsrAssignmentLocation
    {
        public int EsrAssignmentLocationId { get; private set; }
        public int EsrAssignId { get; set; }
        public long EsrLocationId { get; set; }
	    public DateTime StartDate { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public EsrAssignmentLocation Populate(IDataReader reader)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Cannot populate with a closed DataReader");
            }

            this.EsrAssignmentLocationId = (int)reader["ESRAssignmentLocationId"];
            this.EsrAssignId = (int)reader["EsrAssignId"];
            this.EsrLocationId = (long)reader["ESRLocationId"];
            this.StartDate = (DateTime)reader["StartDate"];
            this.DeletedDateTime = (reader["DeletedDateTime"] is DBNull ? null : (DateTime?)reader["DeletedDateTime"]);

            return this;
        }
    }
}
