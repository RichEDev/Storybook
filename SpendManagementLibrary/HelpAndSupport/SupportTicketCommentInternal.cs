namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Represents an internal (customer database, not salesforce) support ticket comment
    /// </summary>
    public class SupportTicketCommentInternal : SupportTicketComment
    {
        /// <summary>
        /// The comment's identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// When the comment was created
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Retrieves a support ticket by its Identifier
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="supportTicketId">The identifier of the support ticket comment</param>
        /// <returns></returns>
        public static List<SupportTicketCommentInternal> GetBySupportTicketId(ICurrentUserBase user, int supportTicketId)
        {
            var response = new List<SupportTicketCommentInternal>();

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string SQL = "SELECT [SupportTicketCommentId], [Body], [CreatedOn], [CreatedBy] FROM [SupportTicketComments] WHERE [SupportTicketId] = @TicketId;";
                databaseConnection.AddWithValue("@TicketId", supportTicketId);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int idOrdinal = reader.GetOrdinal("SupportTicketCommentId"),
                        bodyOrdinal = reader.GetOrdinal("Body"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy");

                    while (reader.Read())
                    {
                        var employeeId = reader.GetInt32(createdByOrdinal);

                        response.Add(new SupportTicketCommentInternal
                        {
                            Id = reader.GetInt32(idOrdinal),
                            Body = reader.GetString(bodyOrdinal),
                            CreatedOn = reader.GetDateTime(createdOnOrdinal),
                            CreatedBy = employeeId,
                            ContactName = Employee.Get(employeeId, user.AccountID).FullName
                        });
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return response;
        }

        /// <summary>
        /// Creates and stores a new support ticket comment
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="supportTicketId">The identifier of the support ticket</param>
        /// <param name="commentBody">The comment text</param>
        /// <returns></returns>
        public static int Create(ICurrentUserBase user, int supportTicketId, string commentBody)
        {
            int returnValue;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.AddWithValue("@SubAccountID", user.CurrentSubAccountId);
                databaseConnection.AddWithValue("@SupportTicketId", supportTicketId);
                databaseConnection.AddWithValue("@Body", commentBody, 4000);
                databaseConnection.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveSupportTicketComment");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

    }
}
