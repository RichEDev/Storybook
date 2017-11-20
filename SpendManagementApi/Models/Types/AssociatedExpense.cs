namespace SpendManagementApi.Models.Types
{
    using System;

    /// <summary>
    /// An expense that is associated with a flag where it has caused the flag to occur.
    /// </summary>
    public class AssociatedExpense
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssociatedExpense"/> class.
        /// </summary>
        /// <param name="claimName">
        /// The claim name the item belongs to.
        /// </param>
        /// <param name="date">
        /// The date of the expense item.
        /// </param>
        /// <param name="referenceNumber">
        /// The reference number of the expense item.
        /// </param>
        /// <param name="total">
        /// The total of the expense item.
        /// </param>
        /// <param name="expenseItem">
        /// The expense item name (subcat).
        /// </param>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        public AssociatedExpense(
            string claimName,
            DateTime date,
            string referenceNumber,
            string total,
            string expenseItem,
            int expenseId)
        {
            this.ClaimName = claimName;
            this.Date = date;
            this.ReferenceNumber = referenceNumber;
            this.Total = total;
            this.ExpenseItem = expenseItem;
            this.ExpenseId = expenseId;
        }

        /// <summary>
        /// Gets or sets the claim name the expense belongs to.
        /// </summary>
        public string ClaimName { get; protected set; }

        /// <summary>
        /// Gets or sets the date of the expense item.
        /// </summary>
        public DateTime Date { get; protected set; }

        /// <summary>
        /// Gets or sets the reference number of the expense item.
        /// </summary>
        public string ReferenceNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the total of the expense item.
        /// </summary>
        public string Total { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the expense item (subat).
        /// </summary>
        public string ExpenseItem { get; protected set; }

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        public int ExpenseId { get; protected set; }
    }
}