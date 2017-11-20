namespace ApiClientHelper.Responses
{
    using ApiClientHelper.ExchangeRateProviders.Enums;
    using ApiClientHelper.Models;

    /// <summary>
    /// Account details with enabled general option
    /// </summary>
    public class AccountForparticularGeneralOption
    {
        /// <summary>
        /// Gets or sets accountid
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets  company id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate provider for the account
        /// </summary>
        public ExchangeRateProvider Provider { get; set; }
    }
}