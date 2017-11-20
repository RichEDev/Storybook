namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// Represents the type of support ticket
    /// </summary>
    public enum SupportTicketType
    {
        /// <summary>
        /// Support ticket is stored in the customer's database and is handled by that company's administrators
        /// </summary>
        Internal = 1,

        /// <summary>
        /// Support ticket is stored in SalesForce and is handled by Software Europe's Helpdesk
        /// </summary>
        SalesForce = 2
    }
}
