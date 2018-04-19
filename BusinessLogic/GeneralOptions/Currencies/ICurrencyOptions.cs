namespace BusinessLogic.GeneralOptions.Currencies
{
    using System;

    /// <summary>
    /// Defines a <see cref="ICurrencyOptions"/> and it's members
    /// </summary>
    public interface ICurrencyOptions
    {
        /// <summary>
        /// Gets or sets the base currency Id.
        /// </summary>
        int? BaseCurrency { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CurrencyType"/>
        /// </summary>
        CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable auto update of exchange rates.
        /// </summary>
        bool EnableAutoUpdateOfExchangeRates { get; set; }

        /// <summary>
        /// Gets or sets the enable auto update of exchange rates activated date.
        /// </summary>
        DateTime? EnableAutoUpdateOfExchangeRatesActivatedDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExchangeRateProvider"/>.
        /// </summary>
        ExchangeRateProvider ExchangeRateProvider { get; set; }
    }
}
