namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    using Utilities;
    using Models.Requests;
    using Models.Responses;

    using CurrencyType = SpendManagementLibrary.CurrencyType;

    internal class CurrencyExchangeRatesRepository : BaseRepository<CurrencyExchangeRates>, ISupportsActionContext
    {
        private readonly cCurrencies _currencyData;

        private readonly cMonthlyCurrencies _monthlyCurrencies;

        private readonly cRangeCurrencies _rangeCurrencies;
        
        public CurrencyExchangeRatesRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, curr => curr.CurrencyId, null)
        {
            _currencyData = this.ActionContext.Currencies;
            _monthlyCurrencies = this.ActionContext.MonthlyCurrencies;
            _rangeCurrencies = this.ActionContext.RangeCurrencies;

       }

        private void PopulateStaticExchangeRates(CurrencyExchangeRates currencyExchangeRates)
        {
            int currencyId = currencyExchangeRates.CurrencyId;
            SortedList<int, SortedList<int, double>> dbRates = _currencyData.getExchangeRates(CurrencyType.Static, currencyId);
            ExchangeRates staticRates = new ExchangeRates
            {
                StaticExchangeRates = new BaseCurrency { ExchangeRateTable = dbRates.Values.FirstOrDefault() }
            };
            currencyExchangeRates.ExchangeRates = staticRates;
        }

        private void PopulateMonthlyExchangeRates(CurrencyExchangeRates currencyExchangeRates)
        {
            cMonthlyCurrency monthlyCurrency = _monthlyCurrencies.getCurrencyById(currencyExchangeRates.CurrencyId);

            List<CurrencyMonth> months = monthlyCurrency.exchangerates.Select(
                m =>
                new CurrencyMonth
                {
                    CurrencyMonthId = m.Key,
                    ExchangeRateTable = m.Value.exchangerates,
                    Month = m.Value.month,
                    Year = m.Value.year
                }).ToList();

            ExchangeRates monthlyRates = new ExchangeRates
            {
                MonthlyExchangeRates = months
            };
            currencyExchangeRates.ExchangeRates = monthlyRates;
        }

        private void PopulateRangeExchangeRates(CurrencyExchangeRates currencyExchangeRates)
        {
            cRangeCurrency rangeCurrency = _rangeCurrencies.getCurrencyById(currencyExchangeRates.CurrencyId);

            List<CurrencyDateRange> dateRanges = rangeCurrency.exchangerates.Select(
                m =>
                new CurrencyDateRange
                {
                    CurrencyDateRangeId = m.Key,
                    ExchangeRateTable = m.Value.exchangerates,
                    StartDate = m.Value.startdate,
                    EndDate = m.Value.enddate
                }).ToList();

            ExchangeRates rangeRates = new ExchangeRates
            {
                DateRangeExchangeRates = dateRanges
            };
            currencyExchangeRates.ExchangeRates = rangeRates;
        }

        private CurrencyType GetCurrentCurrencyType()
        {
            cMisc misc = new cMisc(User.AccountID);
            cGlobalProperties clsproperties = misc.GetGlobalProperties(User.AccountID);
            CurrencyType dbCurrencyType = (CurrencyType)clsproperties.currencytype;
            return dbCurrencyType;
        }

        /// <summary>
        /// Gets a list of exchange rates for a specified currency type and currency id
        /// </summary>
        /// <param name="currencyId">Currency Id</param>
        /// <returns></returns>
        public override CurrencyExchangeRates Get(int currencyId)
        {
            CurrencyExchangeRates currencyExchangeRates = new CurrencyExchangeRates();
            currencyExchangeRates.CurrencyId = currencyId;

            var currencyType = GetCurrentCurrencyType().Cast<Common.Enums.CurrencyType>();
            currencyExchangeRates.CurrencyType = currencyType;

            switch (currencyType)
            {
                case Common.Enums.CurrencyType.Static:
                    this.PopulateStaticExchangeRates(currencyExchangeRates);
                    break;

                case Common.Enums.CurrencyType.Monthly:
                    this.PopulateMonthlyExchangeRates(currencyExchangeRates);
                    break;

                case Common.Enums.CurrencyType.Range:
                    this.PopulateRangeExchangeRates(currencyExchangeRates);
                    break;
            }
            
            return currencyExchangeRates;
        }

        /// <summary>
        /// Gets the exchange rate for two currencies at a particular date and time.
        /// </summary>
        /// <param name="dateTimeOfRate">The date and time of the exchange rate</param>
        /// <param name="fromCurrencyId">The currency Id converting from</param>
        /// <param name="toCurrencyId">The currency Id converting to</param>
        /// <param name="response">The <see cref="CurrencyExchangeRateResponse">CurrencyExchangeRateResponse</see></param>
        /// <returns>The <see cref="CurrencyExchangeRateResponse"/></returns>
        public CurrencyExchangeRateResponse GetCurrencyExchangeRate(DateTime dateTimeOfRate, int toCurrencyId, int fromCurrencyId, CurrencyExchangeRateResponse response)
        {
            double exchangeRate = _currencyData.getExchangeRate(fromCurrencyId, toCurrencyId, dateTimeOfRate);

            var exRate = exchangeRate.ToString();
            string formattedExchangeRate  = exRate.Length > 13 ? exRate.Substring(0, 13) : exRate;
            response.Rate = Convert.ToDouble(formattedExchangeRate);

            return response;
        }

        private void SaveCurrencyRates(CurrencyExchangeRates currencyExchangeRates)
        {
            Common.Enums.CurrencyType currencyType = this.GetCurrentCurrencyType().Cast<Common.Enums.CurrencyType>();
            if (currencyExchangeRates.CurrencyType != currencyType)
            {
                _currencyData.ChangeCurrencyType(currencyExchangeRates.CurrencyType.Cast<Common.Enums.CurrencyType>(), User.EmployeeID);
            }

            switch (currencyType)
            {
                case Common.Enums.CurrencyType.Static:
                    SortedList<int, double> staticExchangeRates =
                    currencyExchangeRates.ExchangeRates.StaticExchangeRates.ExchangeRateTable;
                    _currencyData.addExchangeRates(currencyExchangeRates.CurrencyId, CurrencyType.Static, staticExchangeRates, DateTime.UtcNow, User.EmployeeID);
                    break;
                case Common.Enums.CurrencyType.Monthly:
                    if (currencyExchangeRates.ExchangeRates.MonthlyExchangeRates != null)
                    {
                        foreach (var exchangeRate in currencyExchangeRates.ExchangeRates.MonthlyExchangeRates)
                        {
                            _monthlyCurrencies.saveCurrencyMonth(
                                new cCurrencyMonth(
                                    User.AccountID,
                                    currencyExchangeRates.CurrencyId,
                                    exchangeRate.CurrencyMonthId,
                                    exchangeRate.Month,
                                    exchangeRate.Year,
                                    DateTime.UtcNow,
                                    User.EmployeeID,
                                    DateTime.UtcNow,
                                    User.EmployeeID,
                                    exchangeRate.ExchangeRateTable));
                        }    
                    }
                    break;
                case Common.Enums.CurrencyType.Range:
                    if (currencyExchangeRates.ExchangeRates.DateRangeExchangeRates != null)
                    {
                        foreach (var exchangeRate in currencyExchangeRates.ExchangeRates.DateRangeExchangeRates)
                        {
                            _rangeCurrencies.saveCurrencyRange(
                                new cCurrencyRange(
                                    User.AccountID,
                                    currencyExchangeRates.CurrencyId,
                                    exchangeRate.CurrencyDateRangeId,
                                    exchangeRate.StartDate,
                                    exchangeRate.EndDate,
                                    DateTime.UtcNow,
                                    User.EmployeeID,
                                    DateTime.UtcNow,
                                    User.EmployeeID,
                                    exchangeRate.ExchangeRateTable));
                        }
                    }
                    break;
            }
        }

        public override CurrencyExchangeRates Delete(int currencyId)
        {
            var currency = Get(currencyId);
            if (currency == null)
                throw new ApiException("Invalid currency id", "No data available for specified currency id");

            _currencyData.deleteExchangeRates(currencyId, this.GetCurrentCurrencyType());

            return this.Get(currencyId);
        }

        internal void DeleteMonthlyExchangeRate(CurrencyExchangeRates currency, int monthlyCurrencyId, int toCurrencyId)
        {
            CurrencyMonth month = currency.ExchangeRates.MonthlyExchangeRates.First(
                e => e.CurrencyMonthId == monthlyCurrencyId);
            if (month == null)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyExchangeRateDeleteMessage1, ApiResources.ApiErrorCurrencyExchangeRatesDeleteUnsuccessful);
            }

            if (month.ExchangeRateTable.ContainsKey(toCurrencyId))
            {
                month.ExchangeRateTable.Remove(toCurrencyId);
            }
        }

        internal void DeleteStaticExchangeRate(CurrencyExchangeRates currency, int toCurrencyId)
        {
            if (currency.ExchangeRates.StaticExchangeRates.ExchangeRateTable.ContainsKey(toCurrencyId))
            {
                currency.ExchangeRates.StaticExchangeRates.ExchangeRateTable.Remove(toCurrencyId);
            }
        }

        public CurrencyExchangeRates DeleteExchangeRate(int currencyId, int exchangeRateId, int toCurrencyId)
        {
            CurrencyType currencyType = this.GetCurrentCurrencyType();

            var currency = Get(currencyId);
            if (currency == null)
                throw new ApiException("Invalid currency id", "No data available for specified currency id");

            switch (currencyType)
            {
                case CurrencyType.Static:
                    this.DeleteStaticExchangeRate(currency, toCurrencyId);
                    break;
                case CurrencyType.Monthly:
                    this.DeleteMonthlyExchangeRate(currency, exchangeRateId, toCurrencyId);
                    break;
                case CurrencyType.Range:
                    this.DeleteRangeExchangeRate(currency, exchangeRateId, toCurrencyId);
                    break;
            }

            this.SaveCurrencyRates(currency);

            return this.Get(currencyId);
        }

        internal void DeleteRangeExchangeRate(CurrencyExchangeRates currency, int rangeCurrencyId, int toCurrencyId)
        {
            CurrencyDateRange range = currency.ExchangeRates.DateRangeExchangeRates.First(
                e => e.CurrencyDateRangeId == rangeCurrencyId);
            if (range == null)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyExchangeRateDeleteMessage1, ApiResources.ApiErrorCurrencyExchangeRatesDeleteUnsuccessful);
            }

            if (range.ExchangeRateTable.ContainsKey(toCurrencyId))
            {
                range.ExchangeRateTable.Remove(toCurrencyId);
            }
        }

        /// <summary>
        /// Adds a new currency exchange rate
        /// </summary>
        /// <param name="currencyExchangeRates">The currency exchange rate to be added</param>
        /// <returns>Returns the record added</returns>
        public override CurrencyExchangeRates Add(CurrencyExchangeRates currencyExchangeRates)
        {
            this.SaveCurrencyRates(currencyExchangeRates);
            return currencyExchangeRates;
        }

        /// <summary>
        /// Only updates the archive status of the record
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public override CurrencyExchangeRates Update(CurrencyExchangeRates currencyExchangeRates)
        {
            base.Update(currencyExchangeRates);
            SaveCurrencyRates(currencyExchangeRates);
            return currencyExchangeRates;
        }

        public CurrencyExchangeRates UpdatePartial(CurrencyExchangeRates currencyExchangeRates)
        {
            base.Update(currencyExchangeRates);

            CurrencyExchangeRates currency = this.Get(currencyExchangeRates.CurrencyId);

            currency.CurrencyType = currencyExchangeRates.CurrencyType;

            switch (currencyExchangeRates.CurrencyType)
            {
                case Common.Enums.CurrencyType.Static:
                    MergeExchangeRates(
                        currencyExchangeRates.ExchangeRates.StaticExchangeRates.ExchangeRateTable, currency.ExchangeRates.StaticExchangeRates.ExchangeRateTable);
                    break;
                case Common.Enums.CurrencyType.Monthly:
                    currencyExchangeRates.ExchangeRates.MonthlyExchangeRates =
                        currencyExchangeRates.ExchangeRates.MonthlyExchangeRates ?? new List<CurrencyMonth>();
                    currency.ExchangeRates.MonthlyExchangeRates = currency.ExchangeRates.MonthlyExchangeRates
                                                                  ?? new List<CurrencyMonth>();
                    currencyExchangeRates.ExchangeRates.MonthlyExchangeRates.ForEach(
                        month =>
                            {
                                if ((month.Month > 0 && month.Month <= 12) && (currency.ExchangeRates.MonthlyExchangeRates.FirstOrDefault(r => r.Month == month.Month && r.Year == month.Year) == null))
                                {
                                    currency.ExchangeRates.MonthlyExchangeRates.Add(month);
                                }
                                else
                                {
                                    this.MergeExchangeRates(
                                        month.ExchangeRateTable,
                                        currency.ExchangeRates.MonthlyExchangeRates.First(r => r.Month == month.Month && r.Year == month.Year).ExchangeRateTable);    
                                }
                            });
                    break;
                case Common.Enums.CurrencyType.Range:
                    currencyExchangeRates.ExchangeRates.DateRangeExchangeRates =
                        currencyExchangeRates.ExchangeRates.DateRangeExchangeRates ?? new List<CurrencyDateRange>();
                    currency.ExchangeRates.DateRangeExchangeRates = currency.ExchangeRates.DateRangeExchangeRates
                                                                  ?? new List<CurrencyDateRange>();
                    currencyExchangeRates.ExchangeRates.DateRangeExchangeRates.ForEach(
                        range =>
                            {
                                if (range.CurrencyDateRangeId == 0)
                                {
                                    currency.ExchangeRates.DateRangeExchangeRates.Add(range);
                                }
                                else
                                {
                                    this.MergeExchangeRates(
                                        range.ExchangeRateTable,
                                        currency.ExchangeRates.DateRangeExchangeRates.First(r => r.CurrencyDateRangeId == range.CurrencyDateRangeId).ExchangeRateTable);    
                                }
                            });
                    break;
            }
            this.SaveCurrencyRates(currency);
            return this.Get(currency.CurrencyId);
        }

        private void MergeExchangeRates(SortedList<int, double> exchangeRates, SortedList<int, double> existingCurrency)
        {
            foreach (var key in exchangeRates.Keys)
            {
                if (existingCurrency.ContainsKey(key))
                {
                    existingCurrency[key] = exchangeRates[key];
                }
                else
                {
                    existingCurrency.Add(key, exchangeRates[key]);
                }
            }
        }
        
        public override IList<CurrencyExchangeRates> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}