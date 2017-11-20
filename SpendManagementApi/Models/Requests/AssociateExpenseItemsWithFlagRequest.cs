namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;

    /// <summary>
    /// The associate expense items with flag request.
    /// </summary>
    public class AssociateExpenseItemsWithFlagRequest
    {
        /// <summary>
        /// Gets or sets the flag id.
        /// </summary>
        public int FlagId { get; set; }

        /// <summary>
        /// Gets or sets the expense item ids.
        /// </summary>
        public List<int> ExpenseItemIds { get; set; }
    }
}


