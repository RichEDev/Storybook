namespace DutyOfCareServices.ApiCalls
{
    using System;
    using System.Diagnostics;

    using APICallsHelper;

    using ApiClientHelper;
    using ApiClientHelper.ExchangeRateProviders;
    using ApiClientHelper.Models;

    /// <summary>
    /// The class that populates exchange rates for all accounts
    /// </summary>
    public class PopulateExchangeRates
    {
        /// <summary>
        /// Api URL to  the Get Accounts With Auto Update of Exchange Rates Enabled.
        /// </summary>
        private const string ApiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled = "Account/GetAccountsWithExchangeRatesUpdateEnabled";

        /// <summary>
        /// Api URL to  the Get the list of active currecnies for an account.
        /// </summary>
        private const string ApiEndPointToGetAListOfActiveCurrencies = "Currencies/GetActiveCurrenciesForAccount";

        /// <summary>
        /// The api end point to post exchange rates.
        /// </summary>
        private const string ApiEndPointToPostExchangeRates = "CurrencyExchangeRates/AddToAccount";

        /// <summary>
        /// Common log message
        /// </summary>
        private const string LogMessage = "Get Exchange Rate :";

        /// <summary>
        /// The API client.
        /// </summary>
        private Client _apiClient;

        /// <summary>
        /// Updates daily exchange rates for all accounts that have auto update exchange rates feature enabled
        /// </summary>
        /// <param name="apiUsername">
        /// The api username.
        /// </param>
        /// <param name="apiPassword">
        /// The api password.
        /// </param>
        /// <param name="apiUrlPath">
        /// The api url path.
        /// </param>
        /// <param name="defaultCompanyId">
        /// The default company id.
        /// </param>
        /// <param name="date">
        /// The date for which you want the exchange rate updated
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public void UpdateDailyExchangeRates(
            string apiUrlPath,
            DateTime date,
            EventLogger logger)
        {
            this._apiClient = new Client(apiUrlPath);

            // Get all accounts with feature auto update exchange rate feature enabled
            Console.WriteLine(@"Populating accounts with auto update of exchange rates enabled");
            var accountList = this._apiClient.GetAccountsWithAutoUpdateExchangeRateEnabled(ApiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled);

            if (accountList != null && (accountList.Result.Data == null || accountList.Result.Data.AccountList.Count == 0))
            {
                Console.WriteLine("No accounts with auto update of exchange rates enabled");
                logger.MakeEventLogEntry(LogMessage + "Failed to load accounts", ApiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled, "No accounts with dvla autopopulate general option enabled");
                return;
            }

            if (accountList == null)
            {
                Console.WriteLine("Error in fetching accounts");
                logger.MakeEventLogEntry(LogMessage + "Failed to load accounts", ApiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled, "Error in fetching accounts");
                return;
            }

            var exchangeRateProvider = new ExchangeRatesProviderFactory();

            // Loop through each account to update exchange rates
            foreach (var account in accountList.Result.Data.AccountList)
            {
                // Switch accounts
                this._apiClient.SetCompany(account.CompanyId); 

                // Get all active currencies
                Console.WriteLine(@"Getting all active currencies for the account " + account.AccountId);
                var activeCurrencies = this._apiClient.GetActiveCurrencies(ApiEndPointToGetAListOfActiveCurrencies + "/" + account.AccountId);
                if (activeCurrencies == null || activeCurrencies.List.Count == 0)
                {
                    Console.WriteLine("No exchange rates to populate for account as there are no active currencies");
                    logger.MakeEventLogEntry("Failed to load active currencies ", ApiEndPointToGetAListOfActiveCurrencies, " for account: " + account.AccountId);
                    continue;
                }

                // Get Exchange Rates for each active currency
                Console.WriteLine(@"Populating exchange rates from 3rd party provider for each active currency");

                CurrencyExchangeRatesList exchangeRates = new CurrencyExchangeRatesList();
                foreach (var baseCurrency in activeCurrencies.List)
                {
                    var rate = exchangeRateProvider.GetExchangeRates(baseCurrency, date, account.Provider, activeCurrencies);

                    if (rate == null)
                    {
                        continue;
                    }

                    exchangeRates.List.Add(rate);
                }

                // Update exchange rates for the account
                Console.WriteLine(@"Saving exchange rates for the account " + account.AccountId);
                var result = this._apiClient.PostExchangeRates(ApiEndPointToPostExchangeRates, exchangeRates, account.AccountId);
                if (result)
                {
                    Console.WriteLine(@"Saving exchange rates completed successfully for the account " + account.AccountId);
                }
                else
                {
                    Console.WriteLine("Failed to save exchange rates details for account " + account.AccountId);
                }
            }
        }

    }
}
