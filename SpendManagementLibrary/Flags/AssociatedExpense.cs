namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An expense that is associated with a flag where it has caused the flag to occur.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AssociatedExpense
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssociatedExpense"/> class.
        /// </summary>
        /// <param name="claimname">
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
        /// <param name="expenseID">
        /// The expense id.
        /// </param>
        public AssociatedExpense(
            string claimname,
            string date,
            string referenceNumber,
            string total,
            string expenseItem,
            int expenseID)
        {
            this.ClaimName = claimname;
            this.Date = date;
            this.ReferenceNumber = referenceNumber;
            this.Total = total;
            this.ExpenseItem = expenseItem;
            this.ExpenseID = expenseID;
        }

        /// <summary>
        /// Gets or sets the claim name the expense belongs to.
        /// </summary>
        [DataMember]
        public string ClaimName { get; protected set; }

        /// <summary>
        /// Gets or sets the date of the expense item.
        /// </summary>
        [DataMember]
        public string Date { get; protected set; }

        /// <summary>
        /// Gets or sets the reference number of the expense item.
        /// </summary>
        [DataMember]
        public string ReferenceNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the total of the expense item.
        /// </summary>
        [DataMember]
        public string Total { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the expense item (subat).
        /// </summary>
        [DataMember]
        public string ExpenseItem { get; protected set; }

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        [DataMember]
        public int ExpenseID { get; protected set; }
    }
}
