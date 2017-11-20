namespace ApiClientHelper.Models
{
    using System;

    /// <summary>
    /// Represents a date range specific currency for exchage rates.
    /// </summary>
    public class CurrencyDateRange : BaseCurrency
    {
        /// <summary>
        /// Gets or sets the unique Id.
        /// </summary>
        public int CurrencyDateRangeId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}