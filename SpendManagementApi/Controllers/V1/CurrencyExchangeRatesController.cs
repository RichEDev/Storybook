using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using System;

    /// <summary>
    /// Manage Currency Exchange Rates
    /// </summary>
    [RoutePrefix("CurrencyExchangeRates")]
    [Version(1)]
    public class CurrencyExchangeRatesV1Controller : BaseApiController<CurrencyExchangeRates>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="CurrencyExchangeRates">CurrencyExchangeRates</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions]
        [Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets exchange rates for specified currency id
        /// </summary>
        /// <param name="id">The Id of the Currency</param>
        /// <returns>List of exchange rates</returns>
        [HttpGet]
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.View)]
        public CurrencyExchangeRatesResponse Get([FromUri] int id)
        {
            return this.Get<CurrencyExchangeRatesResponse>(id);
        }

        /// <summary>
        /// Gets the exchange rate for the specified criteria
        /// </summary>
        /// <param name="dateTimeOfRate">The date and time of the exchange rate</param>
        /// <param name="fromCurrencyId">The currency Id converting from</param>
        /// <param name="toCurrencyId">The currency Id converting to</param>
        /// <returns>The <see cref="CurrencyExchangeRateResponse"> </see> </returns>
        [HttpPost]
        [Route("GetExchangeRate")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CurrencyExchangeRateResponse GetExchangeRate([FromUri] DateTime dateTimeOfRate, int fromCurrencyId, int toCurrencyId)
        {
            var response = InitialiseResponse<CurrencyExchangeRateResponse>();
            response = ((CurrencyExchangeRatesRepository)Repository).GetCurrencyExchangeRate(dateTimeOfRate,toCurrencyId,fromCurrencyId, response);
            return response;
        }

        /// <summary>
        /// Updates the exchange rate tables for a currency id
        /// </summary>
        /// <param name="id">The Id of the Currency to update.</param>
        /// <param name="request">Currency exchange rates to update</param>
        /// <returns>Updated currency exchange rates</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Edit)]
        public CurrencyExchangeRatesResponse Put([FromUri] int id, [FromBody] CurrencyExchangeRates request)
        {
            request.CurrencyId = id;
            return this.Put<CurrencyExchangeRatesResponse>(request);
        }

        /// <summary>
        /// Deleted the records for specified currency id
        /// </summary>
        /// <param name="id">The Id of the currency for which rates are to be deleted.</param>
        ///<returns>A CurrencyExchangeResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Delete)]
        public CurrencyExchangeRatesResponse Delete(int id)
        {
            return this.Delete<CurrencyExchangeRatesResponse>(id);
        }

        /// <summary>
        /// Internal method used by the currency auto updating tool. Adds exchange rates directly to the specified account.
        /// </summary>
        /// <param name="accountId">The Account ID to add your exchange rates to.</param>
        /// <param name="exchangeRates">The exchange rates to add.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddToAccount")]
        [InternalSelenityMethod]
        public ExchangeRateResponse AddToAccount([FromUri] int accountId, [FromBody] CurrencyExchangeRatesList exchangeRates)
        {
            var response = new ExchangeRateResponse();
            var listOfExchangeRates = new List<CurrencyExchangeRates>();

            foreach (var exchangeRate in exchangeRates.List)
            {
                foreach (var daterange in exchangeRate.ExchangeRates.DateRangeExchangeRates)
                {
                    daterange.ExchangeRateTable = daterange.Cast(daterange.ApiExchangeRateTable);
                }
                
                var currencyType = new cAccountSubAccounts(accountId).getFirstSubAccount().SubAccountProperties.currencyType;

                switch (currencyType)
                {
                    case CurrencyType.Monthly:
                        var currencies = new cMonthlyCurrencies(accountId, null);

                        foreach (var rate in exchangeRate.ExchangeRates.MonthlyExchangeRates)
                        {
                            var currencyMonth = new cCurrencyMonth(
                                accountId,
                                exchangeRate.CurrencyId,
                                rate.CurrencyMonthId,
                                rate.Month,
                                rate.Year,
                                DateTime.UtcNow,
                                0, 
                                DateTime.UtcNow,
                                0,
                                rate.ExchangeRateTable);

                            currencies.saveCurrencyMonth(currencyMonth);
                        }
                        break;
                    case CurrencyType.Range:
                        if (exchangeRate.ExchangeRates.DateRangeExchangeRates != null)
                        {
                            var rangeCurrencies = new cRangeCurrencies(accountId, null);
                            foreach (var rate in exchangeRate.ExchangeRates.DateRangeExchangeRates)
                            {
                                rangeCurrencies.saveCurrencyRange(
                                    new cCurrencyRange(
                                        accountId,
                                        exchangeRate.CurrencyId,
                                        rate.CurrencyDateRangeId,
                                        rate.StartDate,
                                        rate.EndDate,
                                        DateTime.UtcNow,
                                        0,
                                        DateTime.UtcNow,
                                        0,
                                        rate.ExchangeRateTable));
                            }
                        }

                        break;
                    case CurrencyType.Static:
                        var staticCurrencies = new cStaticCurrencies(AccountID: accountId, subAccountID: null);

                        SortedList<int, double> staticExchangeRates = exchangeRate.ExchangeRates.StaticExchangeRates.ExchangeRateTable;
                        staticCurrencies.addExchangeRates(exchangeRate.CurrencyId, CurrencyType.Static, staticExchangeRates, DateTime.UtcNow, 0);

                        break;
                }

                listOfExchangeRates.Add(exchangeRate);
            }

            response.IsExchangeRateAdded = listOfExchangeRates.Count == exchangeRates.List.Count;
            response.Records = listOfExchangeRates;
            return response;
        }

        /// <summary>
        /// Merges a batch of exchange rates with the existing exchange rates and updates the currency type
        /// </summary>
        /// <param name="id">Currency Id</param>
        /// <param name="exchangeRates">Exchange rates and currency type to be updated</param>
        /// <returns>The complete list of exchange rates for the specified currency id and currency type</returns>
        [HttpPatch]
        [Route("{id:int}/UpdateExchangeRates")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Edit)]
        public CurrencyExchangeRatesResponse UpdateExchangeRates(
            [FromUri] int id, [FromBody] CurrencyExchangeRates exchangeRates)
        {
            exchangeRates.CurrencyId = id;
            var response = this.InitialiseResponse<CurrencyExchangeRatesResponse>();
            response.Item = (this.Repository as CurrencyExchangeRatesRepository).UpdatePartial(exchangeRates);
            return response;
        }

        /// <summary>
        /// Deletes the specified exchange rate id
        /// </summary>
        /// <param name="id">Currency Id</param>
        /// <param name="exchangeRateTypeId">If monthly currency type, specify monthly currency id. If range type, specify currency range id. If static, then 0</param>
        /// <param name="toCurrencyId">Destination currency id for conversion</param>
        ///<returns>A CurrencyExchangeResponse with the item set to null upon a successful delete.</returns>
        [HttpPatch]
        [Route("{id:int}/DeleteExchangeRate/{currencyExchangeRateId}/{toCurrencyId}")]
        [AuthAudit(SpendManagementElement.Currencies, AccessRoleType.Delete)]
        public CurrencyExchangeRatesResponse DeleteExchangeRate(int id, int exchangeRateTypeId, int toCurrencyId)
        {
            var response = this.InitialiseResponse<CurrencyExchangeRatesResponse>();
            response.Item = (this.Repository as CurrencyExchangeRatesRepository).DeleteExchangeRate(
                id, exchangeRateTypeId, toCurrencyId);
            return response;
        }
    }
}