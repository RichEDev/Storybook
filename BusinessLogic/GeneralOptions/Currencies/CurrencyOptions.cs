namespace BusinessLogic.GeneralOptions.Currencies
{
    using System;

    /// <summary>
    /// The currency options.
    /// </summary>
    public class CurrencyOptions : ICurrencyOptions
    {
        /// <summary>
        /// Gets or sets the base currency Id.
        /// </summary>
        public int? BaseCurrency { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CurrencyType"/>
        /// </summary>
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable auto update of exchange rates.
        /// </summary>
        public bool EnableAutoUpdateOfExchangeRates { get; set; }

        /// <summary>
        /// Gets or sets the enable auto update of exchange rates activated date.
        /// </summary>
        public DateTime? EnableAutoUpdateOfExchangeRatesActivatedDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExchangeRateProvider"/>.
        /// </summary>
        public ExchangeRateProvider ExchangeRateProvider { get; set; }
    }
}
