namespace SpendManagementLibrary.HelpAndSupport
{
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.SalesForceApi;

    /// <summary>
    /// Represents a SalesForce support ticket comment 
    /// </summary>
    public class SupportTicketCommentSalesForce : SupportTicketComment
    {
        /// <summary>
        /// The identifier of the support ticket comment
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Creates a new support ticket comment
        /// </summary>
        /// <param name="ticketId">The identifier of the sales force ticket that the comment will be associated with</param>
        /// <param name="commentBody">The comment text</param>
        /// <returns></returns>
        public static string Create(string ticketId, string commentBody)
        {
            string commentId = string.Empty;

            var binding = Knowledge.GetSalesForceBinding();

            var comment = new CaseComment
            {
                CommentBody = commentBody,
                ParentId = ticketId,
                IsPublished = true,
                IsPublishedSpecified = true
            };

            SaveResult[] saveResults = binding.create(new sObject[] { comment });
            foreach (var r in saveResults)
            {
                if (r.success)
                {
                    commentId = r.id;
                }
            }

            return commentId;
        }

    }
}
