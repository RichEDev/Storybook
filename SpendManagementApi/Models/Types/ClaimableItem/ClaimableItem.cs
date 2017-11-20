namespace SpendManagementApi.Models.Types.ClaimableItem
{
    /// <summary>
    /// The claimable item.
    /// </summary>
    public class ClaimableItem : BaseExternalType
    {
        /// <summary>
        /// The unique Id of the sub category.
        /// </summary>
        public int SubCatId { get; set; }

        /// <summary>
        /// Gets or sets the expense item name.
        /// </summary>
        public string ExpenseItemName { get; set; }

        /// <summary>
        /// Gets or sets the expense category name.
        /// </summary>
        public string ExpenseCategoryName { get; set; }

        /// <summary>
        /// Gets or sets the description of the expense.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the maximum limit without receipt.
        /// </summary>
        public decimal MaximumLimitWithoutReceipt { get; set; }

        /// <summary>
        /// Gets or sets the maximum limit.
        /// </summary>
        public decimal MaximumLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is appear or not when user adds a new expense.
        /// </summary>
        public bool IsAppear { get; set; }

        /// <summary>
        /// The symbol of the currency
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}