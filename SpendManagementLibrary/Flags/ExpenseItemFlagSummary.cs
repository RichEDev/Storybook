namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The summary of flagged items for a given expense.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExpenseItemFlagSummary
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ExpenseItemFlagSummary"/> class.
        /// </summary>
        /// <param name="expenseID">
        /// The expense id.
        /// </param>
        public ExpenseItemFlagSummary(int expenseID)
        {
            this.ExpenseID = expenseID;
            this.FlagCollection = new List<FlagSummary>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ExpenseItemFlagSummary"/> class.
        /// </summary>
        /// <param name="expenseID">
        /// The expense id.
        /// </param>
        /// <param name="summary">
        /// A list of flagged items
        /// </param>
        public ExpenseItemFlagSummary(int expenseID, List<FlagSummary> summary)
        {
            this.ExpenseID = expenseID;
            this.FlagCollection = summary;
        }

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        [DataMember]
        public int ExpenseID { get; set; }

        /// <summary>
        /// Gets the name of the expense item (subcat).
        /// </summary>
        [DataMember]
        public string ExpenseSubcat
        {
            get
            {
                if (this.FlagCollection.Count == 0)
                {
                    return string.Empty;
                }

                FlagSummary summary = this.FlagCollection[0];
                if (summary != null && summary.FlaggedItem != null)
                {
                    return summary.FlaggedItem.ExpenseSubcat;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the expense date.
        /// </summary>
        [DataMember]
        public string ExpenseDate
        {
            get
            {
                if (this.FlagCollection.Count == 0)
                {
                    return string.Empty;
                }

                FlagSummary summary = this.FlagCollection[0];
                if (summary != null && summary.FlaggedItem != null)
                {
                    return summary.FlaggedItem.ExpenseDate.ToShortDateString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the expense total.
        /// </summary>
        [DataMember]
        public string ExpenseTotal
        {
            get
            {
                if (this.FlagCollection.Count == 0)
                {
                    return string.Empty;
                }

                FlagSummary summary = this.FlagCollection[0];
                if (summary != null && summary.FlaggedItem != null)
                {
                    return summary.FlaggedItem.ExpenseCurrencySymbol + summary.FlaggedItem.ExpenseTotal.ToString("###,###,##0.00");
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Adds a new flagged item to the collection
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <param name="flaggedItem">
        /// The flagged item.
        /// </param>
        public void Add(int flagID, FlaggedItem flaggedItem)
        {
            this.FlagCollection.Add(new FlagSummary(flagID, flaggedItem));
        }

        /// <summary>
        /// Gets or sets the flag collection.
        /// </summary>
        [DataMember]
        public List<FlagSummary> FlagCollection { get; set; }

        /// <summary>
        /// Tries to get the flag summary item.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <param name="summaryItem">
        /// The summary item.
        /// </param>
        /// <returns>
        /// Whether setting the summary item was successful.
        /// </returns>
        public bool TryGetValue(int flagID, out FlagSummary summaryItem)
        {
            summaryItem = null;
            foreach (FlagSummary summary in this.FlagCollection)
            {
                if (summary.FlagID == flagID)
                {
                    summaryItem = summary;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an entry with the given flagID already exists in the collection
        /// </summary>
        /// <param name="flagID"></param>
        /// <returns></returns>
        public bool ContainsKey(int flagID)
        {
            return this.FlagCollection.Any(summary => summary.FlagID == flagID);
        }
    }
}
