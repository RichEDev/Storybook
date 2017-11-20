namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The credit card transaction.
    /// </summary>
    public class CreditCardTransaction : IBaseClassToAPIType<SpendManagementLibrary.Cards.CreditCardTransaction, CreditCardTransaction>
    {
        /// <summary>
        /// Gets or sets the tranaction id.
        /// </summary>
        public int TransactionId { get; set; }

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
        /// Gets or sets the orginal amount.
        /// </summary>
        public decimal? OrginalAmount { get; set; }

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
        /// Gets or sets the currency symbol of the transaction.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the currency Id of the transaction.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the primary currency symbol of the employee.
        /// </summary>
        public string PrimaryCurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the orginal remaining amount on the transaction (this is in the orginating currency)
        /// </summary>
        public decimal RemainingAmountToBeReconciled { get; set; }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="dbType">
        /// The db Type.
        /// </param>
        /// <param name="actionContext">
        /// The actionContext which contains DAL classes.
        /// </param>
        /// <returns>
        /// A API Type
        /// </returns>
        public CreditCardTransaction ToApiType(SpendManagementLibrary.Cards.CreditCardTransaction dbType, IActionContext actionContext)
        {
            if (dbType == null) 
            {
                return null;
            }

            this.TransactionId = dbType.TranactionId;
            this.TransactionDateTime = dbType.TransactionDateTime;
            this.Description = dbType.Description;
            this.CardNumber = dbType.CardNumber;
            this.TransactionAmount = dbType.TransactionAmount;
            this.OrginalAmount = dbType.OrginalAmount;
            this.Label = dbType.Label;
            this.ExchangeRate = dbType.ExchangeRate;
            this.Country = dbType.Country;
            this.AllocatedAmount = dbType.AllocatedAmount;
            this.UnallocatedAmount = dbType.UnallocatedAmount;
            this.CurrencySymbol = dbType.CurrencySymbol;
            this.CurrencyId = dbType.CurrencyId;

            return this;

        }
    }
}