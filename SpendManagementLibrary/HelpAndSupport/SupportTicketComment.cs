namespace SpendManagementLibrary.HelpAndSupport
{
    using System;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Represents a basic support ticket comment
    /// </summary>
    public abstract class SupportTicketComment : ISupportTicketComment
    {
        /// <summary>
        /// The comment
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The name of the person who created the comment
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// When the comment was created
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
