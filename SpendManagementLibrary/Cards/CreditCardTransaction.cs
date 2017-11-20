namespace SpendManagementLibrary.Cards
{
    using System;

    /// <summary>
    /// The credit card transaction.
    /// </summary>
    public class CreditCardTransaction
    {
        /// <summary>
        /// Gets the tranaction id.
        /// </summary>
        public int TranactionId { get; }

        /// <summary>
        /// Gets or sets the transaction date time.
        /// </summary>
        public DateTime? TransactionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets the orginal amount.
        /// </summary>
        public decimal? OrginalAmount { get; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate.
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the allocated amount.
        /// </summary>
        public decimal? AllocatedAmount { get; set; }

        /// <summary>
        /// Gets or sets the unallocated amount.
        /// </summary>
        public decimal? UnallocatedAmount { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol for the transaction.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the currency Id for the transaction.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardTransaction"/> class.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="transactionDateTime">
        /// The transaction date time.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="cardNumber">
        /// The card number.
        /// </param>
        /// <param name="transactionAmount">
        /// The transaction amount.
        /// </param>
        /// <param name="originalAmount">
        /// The original amount.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="exchangeRate">
        /// The exchange rate.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="allocatedAmount">
        /// The allocated amount.
        /// </param>
        /// <param name="unallocatedAmount">
        /// The unallocated amount.
        /// </param>
        /// <param name="currencySymbol">
        /// The currency Symbol.
        /// </param>
        /// <param name="currencyId">
        /// The currency Id.
        /// </param>
        public CreditCardTransaction(
            int transactionId,
            DateTime? transactionDateTime,
            string description,
            string cardNumber,
            decimal transactionAmount,
            decimal? originalAmount,
            string label,
            decimal? exchangeRate,
            string country,
            decimal? allocatedAmount,
            decimal? unallocatedAmount,
            string currencySymbol,
            int currencyId)
        {
            this.TranactionId = transactionId;
            this.TransactionDateTime = transactionDateTime;
            this.Description = description;
            this.CardNumber = cardNumber;
            this.TransactionAmount = transactionAmount;
            this.OrginalAmount = originalAmount;
            this.Label = label;
            this.ExchangeRate = exchangeRate;
            this.Country = country;
            this.AllocatedAmount = allocatedAmount;
            this.UnallocatedAmount = unallocatedAmount;
            this.CurrencySymbol = currencySymbol;
            this.CurrencyId = currencyId;
        }
    }
}
