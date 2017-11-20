namespace SpendManagementLibrary.Logic_Classes
{
    using System;

    /// <summary>
    /// A type factory to create an instance of <see cref="ExchangeRateModifier"/> that will generate default date range info when turning on
    /// The <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/> switch which turns on Date Ranges.
    /// </summary>
    internal static class AutoUpdateExchangeRateModifier
    {
        /// <summary>
        /// Create a new instance of <see cref="ExchangeRateModifier"/> based on the Exchange rate type
        /// </summary>
        /// <param name="currencyType">The type of exchange rates used before turning on <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/></param>
        /// <param name="accountId">The current <see cref="cAccount"/>ID</param>
        /// <returns>An instance of <see cref="ExchangeRateModifier"/>based on the currency type given</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the given <see cref="CurrencyType"/>is unknown</exception>
        public static ExchangeRateModifier New(CurrencyType currencyType, int accountId)
        {
            switch (currencyType)
            {
                case CurrencyType.Static:
                    return new StaticExchangeRateModifier(accountId);
                case CurrencyType.Monthly:
                    return new MonthlyExchangeRateModifier(accountId);
                case CurrencyType.Range:
                    return new DateRangeExchangeRateModifier(accountId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
            }
        }
    }
}