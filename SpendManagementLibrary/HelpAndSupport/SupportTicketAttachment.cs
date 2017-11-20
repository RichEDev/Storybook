namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Represents a support ticket file attachment
    /// </summary>
    public class SupportTicketAttachment : ISupportTicketAttachment
    {
        /// <summary>
        /// The identifier of the attachment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The attachment's filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        public Int64 FileSize { get; set; }

        /// <summary>
        /// The contents of the file
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// When the attachment was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The Identifier of the user who created the attachment
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// The MIME content type of the file
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Saves a new support ticket attachment to the customer database
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="supportTicketId">The identifier of the support ticket</param>
        /// <param name="filename">The filename</param>
        /// <param name="fileBytes">The file contents</param>
        /// <returns></returns>
        public static int Create(ICurrentUserBase user, int supportTicketId, string filename, byte[] fileBytes)
        {
            int returnValue;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.AddWithValue("@SubAccountID", user.CurrentSubAccountId);
                databaseConnection.AddWithValue("@SupportTicketId", supportTicketId);
                databaseConnection.AddWithValue("@Filename", filename, 255);
                databaseConnection.AddWithValue("@FileData", fileBytes);
                databaseConnection.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveSupportTicketAttachment");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }

        /// <summary>
        /// Download the attachment to the browser
        /// </summary>
        /// <param name="attachmentId">The identifier of the attachment</param>
        /// <param name="supportTicketId">The identifier of the associate support ticket</param>
        /// <param name="user">The user</param>
        /// <param name="context">The HTTP context</param>
        public static void Download(int attachmentId, int supportTicketId, ICurrentUserBase user, ref HttpContext context)
        {
            string filename = null;
            byte[] fileData = null;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string SQL = "SELECT [Filename], [FileData] FROM [SupportTicketAttachments] WHERE [SupportTicketAttachmentId] = @AttachmentId AND [SupportTicketId] = @TicketId;";
                databaseConnection.AddWithValue("@AttachmentId", attachmentId);
                databaseConnection.AddWithValue("@TicketId", supportTicketId);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int filenameOrdinal = reader.GetOrdinal("Filename"), 
                        fileDataOrdinal = reader.GetOrdinal("FileData");

                    while (reader.Read())
                    {
                        filename = reader.GetString(filenameOrdinal);
                        fileData = (byte[])reader[fileDataOrdinal];

                        break;
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            if (filename != null && fileData != null)
            {
                context.Response.Clear();
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ClearContent();
                context.Response.BinaryWrite(fileData);
                context.Response.End();
            }
        }

        /// <summary>
        /// Retreives a list of all attachments associated with a support ticket
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="supportTicketId">The identifier of the support ticket</param>
        /// <returns>A list of attachments</returns>
        public static List<SupportTicketAttachment> GetBySupportTicketId(ICurrentUserBase user, int supportTicketId)
        {
            var response = new List<SupportTicketAttachment>();

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string SQL = "SELECT [SupportTicketAttachmentId], [Filename], LEN([Filedata]) AS Filesize, [CreatedOn], [CreatedBy] FROM [SupportTicketAttachments] WHERE [SupportTicketId] = @ticketId;";
                databaseConnection.AddWithValue("@TicketId", supportTicketId);

                using (IDataReader reader = databaseConnection.GetReader(SQL))
                {
                    int idOrdinal = reader.GetOrdinal("SupportTicketAttachmentId"),
                        filenameOrdinal = reader.GetOrdinal("Filename"),
                        filesizeOrdinal = reader.GetOrdinal("Filesize"),
                        createdOnOrdinal = reader.GetOrdinal("CreatedOn"),
                        createdByOrdinal = reader.GetOrdinal("CreatedBy");

                    while (reader.Read())
                    {
                        response.Add(new SupportTicketAttachment
                                        {
                                            Id = reader.GetInt32(idOrdinal),
                                            Filename = reader.GetString(filenameOrdinal),
                                            FileSize = reader.GetInt64(filesizeOrdinal),
                                            CreatedOn = reader.GetDateTime(createdOnOrdinal),
                                            CreatedBy = reader.GetInt32(createdByOrdinal)
                                        });
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return response;
        }
    }

}
