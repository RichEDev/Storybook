namespace Spend_Management.shared.code.Helpers
{
    using System;
    using System.Data;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Custom entity locking / unlocking class.
    /// </summary>
    public class CustomEntityRecordLocking
    {
        /// <summary>
        /// The ID of the custom entitiy that is locked.
        /// </summary>
        public int CustomEntityId { get; private set; }
        /// <summary>
        /// The ID of the custom entitiy record that is locked
        /// </summary>
        public int RecordId { get; private set; }
        /// <summary>
        /// The ID of the employee that has locked the record.
        /// </summary>
        public int EmployeeId { get; private set; }
        /// <summary>
        /// The Date / Time of the last lock update.
        /// </summary>
        public DateTime LockDateTime { get; private set; }

        /// <summary>
        /// Create new custom Entity locking class.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="recordId"></param>
        /// <param name="employeeId"></param>
        /// <param name="lockDateTime"></param>
        public CustomEntityRecordLocking(int customEntityId, int recordId, int employeeId, DateTime lockDateTime)
        {
            this.CustomEntityId = customEntityId;
            this.RecordId = recordId;
            this.EmployeeId = employeeId;
            this.LockDateTime = lockDateTime;
        }

        /// <summary>
        /// Unlock customEntityId / recordId combination.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="recordId"></param>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int UnlockElement(int customEntityId, int recordId, ICurrentUser user, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", recordId);
                expdata.sqlexecute.Parameters.AddWithValue("@customEntityId", customEntityId);
                expdata.ExecuteProc("dbo.DeleteCustomEntityLocking");
            }

            return 0;
        }

        /// <summary>
        /// Lock customEntityId / recordId combination.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="recordId"></param>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool LockElement(int customEntityId, int recordId, ICurrentUser user,
            IDBConnection connection = null)
        {
            bool result;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", recordId);
                expdata.sqlexecute.Parameters.AddWithValue("@customEntityId", customEntityId);
                expdata.sqlexecute.Parameters.AddWithValue("@employeeId", user.EmployeeID);
                expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.LockCustomEntity");
                var aaa = expdata.sqlexecute.Parameters["@returnvalue"].Value;
                
                result = aaa.ToString() == "1";
            }

            return result;
        }

        /// <summary>
        /// Check for locking record for the customEntityId and entity.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="recordId"></param>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static CustomEntityRecordLocking IsRecordLocked(int customEntityId, int recordId, ICurrentUser user,
            IDBConnection connection = null)
        {
            var result = new CustomEntityRecordLocking(customEntityId, recordId, 0, new DateTime());
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", recordId);
                expdata.sqlexecute.Parameters.AddWithValue("@customEntityId", customEntityId);
                using (var reader = expdata.GetReader("dbo.GetCustomEntityLocking", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var lockedBy = reader.GetInt32(reader.GetOrdinal("LockedBy"));
                        var lockedDateTime = reader.GetDateTime(reader.GetOrdinal("LockedDateTime"));
                        result.EmployeeId = lockedBy;
                        result.LockDateTime = lockedDateTime;

                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Update locking record, with current heartbeat.
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="recordId"></param>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        public static void UpdateLocking(int customEntityId, int recordId, ICurrentUser user,
            IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", recordId);
                expdata.sqlexecute.Parameters.AddWithValue("@customEntityId", customEntityId);
                expdata.sqlexecute.Parameters.AddWithValue("@employeeId", user.EmployeeID);
                expdata.ExecuteProc("dbo.UpdateCustomEntityLocking");
            }
        }
    }
}