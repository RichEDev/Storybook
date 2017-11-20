namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// The claim expense item.
    /// </summary>
    public class ClaimExpenseItem : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the expense item.
        /// </summary>
        public ExpenseItem ExpenseItem { get; set; }

        /// <summary>
        /// Gets or sets the subcat.
        /// </summary>
        public ExpenseSubCategory Subcat { get; set; }
    }
}