namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;

    /// <summary>
    /// Handles the flag summary details for an expense item
    /// </summary>
    public class ExpenseItemFlagSummary : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Flags.ExpenseItemFlagSummary, ExpenseItemFlagSummary>
    {
        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        public int ExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the Flag Collection
        /// </summary>
        public List<FlagSummary> FlagCollection { get; set; }

        /// <summary>
        /// Gets the expense total.
        /// </summary>
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
        /// Gets the expense date.
        /// </summary>      
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
        /// Gets the name of the expense item (subcat).
        /// </summary>
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
        /// The constructor for <see cref="ExpenseItemFlagSummary"/>
        /// </summary>
        public ExpenseItemFlagSummary()
        {

        }

        /// <summary>
        /// The constructor for <see cref="ExpenseItemFlagSummary"/>
        /// </summary>
        /// <param name="expenseId">The expenseId</param>
        /// <param name="summary">The flag summary</param>
        public ExpenseItemFlagSummary(int expenseId, List<FlagSummary> summary)
        {
            this.ExpenseId = expenseId;
            this.FlagCollection = summary;
        }

        /// <summary>
        /// Converts a SpendManagement.ExpenseItemFlagSummary to a ExpenseItemFlagSummary type 
        /// </summary>
        /// <param name="dbType">The ExpenseItemFlagSummary spendmanagement type</param>
        /// <param name="actionContext">The actioncontext</param>
        /// <returns></returns>
        public ExpenseItemFlagSummary From(SpendManagementLibrary.Flags.ExpenseItemFlagSummary dbType, IActionContext actionContext)
        {

            if (dbType == null)
            {
                return null;
            }

            this.ExpenseId = dbType.ExpenseID;
            var flagSummmaries = dbType.FlagCollection.Select(flagSummary => new FlagSummary().From(flagSummary, actionContext)).ToList();
            this.FlagCollection = flagSummmaries;

            return this;
        }

        public SpendManagementLibrary.Flags.ExpenseItemFlagSummary To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}