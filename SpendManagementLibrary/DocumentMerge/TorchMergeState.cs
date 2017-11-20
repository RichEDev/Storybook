using System;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using System.Data;

namespace SpendManagementLibrary.DocumentMerge
{
    /// <summary>
    /// The MergeState structure
    /// </summary>
    public class TorchMergeState
    {
        private readonly ICurrentUserBase _currentUser;

        /// <summary>
        /// The merge project id
        /// </summary>
        public int MergeProjectId { get; set; }

        /// <summary>
        /// The progress count
        /// </summary>
        public int ProgressCount { get; set; }

        /// <summary>
        /// The request number
        /// </summary>
        public Guid RequestNumber { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// The total number to process
        /// </summary>
        public int TotalToProcess { get; set; }

        /// <summary>
        /// The merge completion date time
        /// </summary>
        public DateTime? MergeCompletionDate { get; set; }

        /// <summary>
        /// The type of the output document
        /// </summary>
        public TorchExportDocumentType OutputDocType { get; set; }

        /// <summary>
        /// The output document
        /// </summary>
        public string DocumentPath { get; set; }

        /// <summary>
        /// The output document
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// A GUID to uniquely identify the produced document.
        /// </summary>
        public Guid DocumentIdentifier { get; set; }

        /// <summary>
        /// The error message (if any) returned from the merge.
        /// </summary>
        public string ErrorMessage { get; set; }

        private TorchMergeState(ICurrentUserBase currentUser)
        {
            var debugLog = new DebugLogger(currentUser)
                .Log("TorchMergeState.Constructor", String.Empty, currentUser.EmployeeID);

            debugLog.Log("TorchMergeState.Constructor", "Setting currentUser", currentUser.EmployeeID);
            this._currentUser = currentUser;
        }

        /// <summary>
        /// Read the specified Torch Merge State from the database
        /// </summary>
        /// <param name="mergeProjectId">The merge project id to get</param>
        /// <param name="requestNumber">The request number to get</param>
        /// <param name="currentUser">current user</param>
        /// <param name="connection">database connection</param>
        /// <returns>The specified Torch Merge State or a new one if one was not found</returns>
        public static TorchMergeState Get(int mergeProjectId, Guid requestNumber, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var debugLog = new DebugLogger(currentUser)
                .Log("TorchMergeState.Get", String.Empty, mergeProjectId, requestNumber, currentUser.EmployeeID, connection);

            try
            {
                debugLog.Log("TorchMergeState.Get", "Using database connection");
                using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
                {
                    debugLog.Log("TorchMergeState.Get", "Parameters.Clear");
                    databaseConnection.sqlexecute.Parameters.Clear();

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@mergeProjectId", mergeProjectId);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@mergeProjectId", mergeProjectId);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@requestNumber", requestNumber);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@requestNumber", requestNumber);

                    debugLog.Log("TorchMergeState.Get", "databaseConnection.GetReader", "getTorchMergeState", CommandType.StoredProcedure);
                    using (IDataReader reader = databaseConnection.GetReader("getTorchMergeState", CommandType.StoredProcedure))
                    {
                        debugLog.Log("TorchMergeState.Get", "reader.Read");
                        if (reader.Read())
                        {
                            debugLog.Log("TorchMergeState.Get", "Returning TorchMergeState from database", currentUser.AccountID);
                            return new TorchMergeState(currentUser)
                            {
                                MergeProjectId = (int)reader["MergeProjectId"],
                                RequestNumber = (Guid)reader["RequestNumber"],
                                ProgressCount = (int)reader["ProgressCount"],
                                TotalToProcess = (int)reader["TotalToProcess"],
                                DocumentPath = reader["DocumentPath"] is DBNull ? null : (string)reader["DocumentPath"],
                                DocumentUrl = reader["DocumentUrl"] is DBNull ? null : (string)reader["DocumentUrl"],
                                DocumentIdentifier = (Guid)reader["DocumentIdentifier"],
                                MergeCompletionDate = reader["MergeCompletionDate"] is DBNull ? null : (DateTime?)reader["MergeCompletionDate"],
                                ErrorMessage = (string)reader["ErrorMessage"],
                                Status = Convert.ToByte((int)reader["Status"]),
                                OutputDocType = (TorchExportDocumentType)reader["OutputDocType"]
                            };
                        }

                    }
                }

                debugLog.Log("TorchMergeState.Get", "Returning new TorchMergeState", currentUser.AccountID);
                return new TorchMergeState(currentUser)
                {
                    MergeProjectId = mergeProjectId,
                    RequestNumber = requestNumber,
                    ErrorMessage = String.Empty
                };

            }
            catch (Exception ex)
            {
                debugLog.Log("TorchMergeState.Get", ex);
                throw;
            }
        }

        /// <summary>
        /// Inserts or updates the database record of this Torch Merge State
        /// </summary>
        /// <param name="connection">database connection</param>
        /// <returns>The saved Torch Merge State</returns>
        public TorchMergeState Save(IDBConnection connection = null)
        {
            var debugLog = new DebugLogger(this._currentUser)
                .Log("TorchMergeState.Save", String.Empty, connection);

            try
            {
                debugLog.Log("TorchMergeState.Save", "Using database connection", connection);
                using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._currentUser.AccountID)))
                {
                    debugLog.Log("TorchMergeState.Get", "Parameters.Clear");
                    databaseConnection.sqlexecute.Parameters.Clear();

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@MergeProjectId", this.MergeProjectId);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@MergeProjectId", MergeProjectId);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@RequestNumber", this.RequestNumber);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@RequestNumber", RequestNumber);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@ProgressCount", this.ProgressCount);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ProgressCount", ProgressCount);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@TotalToProcess", this.TotalToProcess);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@TotalToProcess", TotalToProcess);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@DocumentPath", this.DocumentPath);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DocumentPath", (object)DocumentPath ?? DBNull.Value);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@DocumentUrl", this.DocumentUrl);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DocumentUrl", (object)DocumentUrl ?? DBNull.Value);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@DocumentIdentifier", this.DocumentIdentifier);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DocumentIdentifier", DocumentIdentifier);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@MergeCompletionDate", this.MergeCompletionDate);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@MergeCompletionDate", (object)MergeCompletionDate ?? DBNull.Value);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@ErrorMessage", ErrorMessage);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ErrorMessage", ErrorMessage);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@Status", this.Status);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Status", Status);

                    debugLog.Log("TorchMergeState.Get", "Parameters.AddWithValue", "@OutputDocType", this.OutputDocType);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@OutputDocType", OutputDocType);

                    debugLog.Log("TorchMergeState.Get", "databaseConnection.ExecuteProc", "saveTorchMergeState");
                    databaseConnection.ExecuteProc("saveTorchMergeState");

                    debugLog.Log("TorchMergeState.Get", "Returning this", this);
                    return this;

                }
            }
            catch (Exception ex)
            {
                debugLog.Log("TorchMergeState.Get", ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes any Torch Merge States which are older than 36 hours
        /// </summary>
        /// <param name="currentUser">current user</param>
        /// <param name="connection">database connection</param>
        public static void Cleanup(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var debugLog = new DebugLogger(currentUser)
                .Log("TorchMergeState.Cleanup", String.Empty, currentUser.AccountID, connection);

            try
            {
                debugLog.Log("TorchMergeState.Cleanup", "Using database connection", connection);
                using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
                {
                    debugLog.Log("TorchMergeState.Cleanup", "Parameters.Clear");
                    databaseConnection.sqlexecute.Parameters.Clear();

                    debugLog.Log("TorchMergeState.Cleanup", "Parameters.AddWithValue", "MergeCompletionDateBefore", DateTime.Now.AddHours(-36));
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@MergeCompletionDateBefore", DateTime.Now.AddHours(-36));

                    debugLog.Log("TorchMergeState.Cleanup", "databaseConnection.ExecuteProc", "deleteTorchMergeStates");
                    databaseConnection.ExecuteProc("deleteTorchMergeStates");
                }

                debugLog.Log("TorchMergeState.Cleanup", "Returning");

            }
            catch (Exception ex)
            {
                debugLog.Log("TorchMergeState.Get", ex);
                throw;
            }
        }

    }
}
