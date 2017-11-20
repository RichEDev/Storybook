namespace SpendManagementLibrary.Logic_Classes
{
    using System;

    /// <summary>
    /// Populate the Exchange Rate Date Ranges from the prevailing <see cref="CurrencyType"/> when turning on <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/>
    /// </summary>
    internal abstract class ExchangeRateModifier
    {
        /// <summary>
        /// A protected instance on the current <see cref="cAccount"/> ID
        /// </summary>
        protected readonly int AccountId;

        /// <summary>
        /// Create a new instance of <see cref="ExchangeRateModifier"/>
        /// </summary>
        /// <param name="accountId">The ID of the current <see cref="cAccount"/></param>
        protected ExchangeRateModifier(int accountId)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        /// Populate the Exchange Rate Date Range info based on the type given.
        /// </summary>
        /// <param name="activatedDate">The date that the <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/> was previously activated (if any)</param>
        public abstract void PopulateRanges(DateTime? activatedDate);
        
    }
}