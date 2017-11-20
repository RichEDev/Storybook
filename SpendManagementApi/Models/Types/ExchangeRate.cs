namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CurrencyType = SpendManagementApi.Common.Enums.CurrencyType;

    /// <summary>
    /// Currency exchange rates are collections of conversion values that affect <see cref="Currency">Currencies</see>.
    /// </summary>
    public class CurrencyExchangeRates : BaseExternalType, IEquatable<CurrencyExchangeRates>
    {
        /// <summary>
        /// The unique Id of the exchange rate.
        /// </summary>
        [Required]
        public int CurrencyId { get; set; }

        /// <summary>
        /// The Type of the currency this exchange rate applies to.
        /// </summary>
        [Required]
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// The list of actual exchange rates for their types.
        /// </summary>
        public ExchangeRates ExchangeRates { get; set; }

        public bool Equals(CurrencyExchangeRates other)
        {
            if (other == null)
            {
                return false;
            }
            return this.CurrencyId.Equals(other.CurrencyId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CurrencyExchangeRates);
        }
    }

    /// <summary>
    /// Represents a set of exchange rates for a given currency.
    /// </summary>
    public class ExchangeRates : BaseExternalType, IEquatable<ExchangeRates>
    {
        /// <summary>
        /// The Currency Month - Only required when currency type = Monthly
        /// </summary>
        public List<CurrencyMonth> MonthlyExchangeRates { get; set; }

        /// <summary>
        /// The Currency Date Range - Only required when currency type = Range
        /// </summary>
        public List<CurrencyDateRange> DateRangeExchangeRates { get; set; }

        /// <summary>
        /// The Static Currency Rates - Only required when currency type = Static
        /// </summary>
        public BaseCurrency StaticExchangeRates { get; set; }

        public bool Equals(ExchangeRates other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExchangeRates);
        }
    }

    /// <summary>
    /// Represents a monthly style currency for exchange rates.
    /// </summary>
    public class CurrencyMonth : BaseCurrency
    {
        /// <summary>
        /// The unique Id.
        /// </summary>
        public int CurrencyMonthId { get; set; }

        /// <summary>
        /// The Month.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// The Year.
        /// </summary>
        public int Year { get; set; }
    }

    /// <summary>
    /// Represents a date range specific currency for exchage rates.
    /// </summary>
    public class CurrencyDateRange : BaseCurrency
    {
        /// <summary>
        /// The unique Id.
        /// </summary>
        public int CurrencyDateRangeId { get; set; }

        /// <summary>
        /// The start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}