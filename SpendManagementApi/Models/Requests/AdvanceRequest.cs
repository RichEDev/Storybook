namespace SpendManagementApi.Models.Requests
{
    using System;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The advance request.
    /// </summary>
    public class AdvanceRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the advance Id.
        /// </summary>
        public int AdvanceId { get; set; }

        /// <summary>
        /// Gets or sets the advance name.
        /// </summary>
        public  string AdvanceName { get; set; }

        /// <summary>
        /// Gets or sets the advance reason.
        /// </summary>
        public string AdvanceReason { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency id.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the required by date, if any.
        /// </summary>
        public DateTime? RequiredByDate { get; set; }
    }
}