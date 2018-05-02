namespace SpendManagementApi.Models.Responses
{
    using BusinessLogic.GeneralOptions.Currencies;

    /// <summary>
    /// Account details with particular general option enabled.
    /// </summary>
    public class GeneralOptionEnabledAccount
    {
        /// <summary>
        /// Customer account id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The company id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate provider.
        /// </summary>
        private ExchangeRateProvider ExchangeRateProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionEnabledAccount"/> class. 
        /// </summary>
        /// <param name="accountId">
        /// Customer account id
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        public GeneralOptionEnabledAccount(int accountId,string companyId)
        {
            this.AccountId = accountId;
            this.CompanyId = companyId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionEnabledAccount"/> class. 
        /// </summary>
        /// <param name="accountId">The acount id</param>
        /// <param name="companyId">The company id</param>
        /// <param name="provider">The exchange rate provider</param>
        public GeneralOptionEnabledAccount(int accountId, string companyId, ExchangeRateProvider provider)
        {
            this.AccountId = accountId;
            this.CompanyId = companyId;
            this.ExchangeRateProvider = provider;
        }

        /// <summary>
        /// Empty constructor for RestSharp pattern matching
        /// </summary>
        public GeneralOptionEnabledAccount()
        {
            
        }
    }
}