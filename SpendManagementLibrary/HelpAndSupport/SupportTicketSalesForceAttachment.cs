namespace SpendManagementLibrary.HelpAndSupport
{
    using System.Linq;
    using System.Web;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.SalesForceApi;

    /// <summary>
    /// Represents a sales force support ticket attachment
    /// </summary>
    public class SupportTicketSalesForceAttachment : SupportTicketAttachment
    {
        /// <summary>
        /// The identifier of the attachment
        /// </summary>
        new public string Id { get; set; }

        /// <summary>
        /// Creates a new attachment
        /// </summary>
        /// <param name="ticketId">The identifier of the associated ticket</param>
        /// <param name="attachmentFilename">The filename</param>
        /// <param name="attachmentBytes">The file contents</param>
        /// <returns></returns>
        public static string Create(string ticketId, string attachmentFilename, byte[] attachmentBytes)
        {
            string attachmentId = string.Empty;

            var binding = Knowledge.GetSalesForceBinding();

            var attachment = new Attachment { Name = attachmentFilename, Body = attachmentBytes, ParentId = ticketId };

            SaveResult[] saveResults = binding.create(new sObject[] { attachment });

            foreach (var r in saveResults)
            {
                if (!r.success)
                {
                    foreach (Error e in r.errors)
                    {
                        // response.Errors.Add(e.message);
                    }
                }
                else
                {
                    attachmentId = r.id;
                }
            }

            return attachmentId;
        }

        /// <summary>
        /// Retrieves an attachment from sales force
        /// </summary>
        /// <param name="attachmentId">The attachment identifier</param>
        /// <param name="supportTicketId">The support ticket identifier</param>
        /// <param name="user">The user</param>
        /// <returns></returns>
        public static SupportTicketSalesForceAttachment Get(string attachmentId, string supportTicketId, ICurrentUserBase user)
        {
            SupportTicketSalesForceAttachment attachment = null;
            Attachment salesForceAttachment = null;

            var binding = Knowledge.GetSalesForceBinding();
            var query = string.Format("SELECT Name, Body, ContentType FROM Attachment WHERE Id = '{0}' AND ParentId = '{1}'", attachmentId, supportTicketId);
            QueryResult result = binding.query(query);

            if (result.records != null)
            {
                salesForceAttachment = (Attachment)result.records.First();
            }

            if (salesForceAttachment != null)
            {
                attachment = new SupportTicketSalesForceAttachment
                {
                    Id = salesForceAttachment.Id,
                    Filename = salesForceAttachment.Name,
                    FileBytes = salesForceAttachment.Body,
                    ContentType = salesForceAttachment.ContentType
                };
            }

            return attachment;
        }

        /// <summary>
        /// Download the attachment to the browser
        /// </summary>
        /// <param name="attachmentId">The identifier of the attachment</param>
        /// <param name="supportTicketId">The identifier of the associate support ticket</param>
        /// <param name="user">The user</param>
        /// <param name="context">The HTTP context</param>
        public static void Download(string attachmentId, string supportTicketId, ICurrentUserBase user, ref HttpContext context)
        {
            var attachment = Get(attachmentId, supportTicketId, user);

            context.Response.Clear();
            context.Response.ContentType = attachment.ContentType;
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + attachment.Filename + "\"");
            context.Response.ClearContent();
            context.Response.BinaryWrite(attachment.FileBytes);
            context.Response.End();
        }
    }
}
