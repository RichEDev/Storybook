namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The claim expense item overview to hold the basic details of an expense item
    /// </summary>
    public class ClaimExpenseOverview
    {
        /// <summary>
        /// Gets or sets ExpenseId of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is returned to the claimant.
        /// </summary>
        public bool Returned { get; set; }

        /// <summary>
        /// Gets or sets date of this item.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the sorted list of int,<see cref="JourneyStep"> JourneyStep</see> that apply to this item.
        /// </summary>
        public List<JourneyStep> JourneySteps { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public List<FlagSummary> Flags { get; set; }

        /// <summary>
        /// Gets or sets Base Currency (in case a conversion is needed).
        /// </summary>
        public int BaseCurrency { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the grand vat total.
        /// </summary>
        public decimal GrandTotalVat { get; set; }

        /// <summary>
        /// Gets or sets Global Total.
        /// </summary>
        public decimal GlobalTotal { get; set; }

        /// <summary>
        /// Gets or sets Global Grand Total.
        /// </summary>
        public decimal GrandTotalGlobal { get; set; }

        /// <summary>
        /// Gets a value indicating that current expense item has split items in it.
        /// </summary>
        public bool HasSplitItems { get; set; }

        /// <summary>
        /// Gets or sets the sub cat description.
        /// </summary>
        public string SubCatDescription { get; set; }
    }
}