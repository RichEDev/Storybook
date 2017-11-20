namespace SpendManagementLibrary.Interfaces
{
    using System;

    /// <summary>
    /// A common interace for support ticket comments
    /// </summary>
    public interface ISupportTicketComment
    {
        /// <summary>
        /// The comment
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// The name of the person who created the comment
        /// </summary>
        string ContactName { get; set; }

        /// <summary>
        /// When the comment was created
        /// </summary>
        DateTime CreatedOn { get; set; }
    }
}
