namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using Currency = SpendManagementLibrary.Mobile.Currency;
    using CurrencyResult = SpendManagementLibrary.Mobile.CurrencyResult;

    /// <summary>
    /// The mobile controller dealing with the accounts currencies.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileCurrenciesV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets all of the currencies on the mobile users account.
        /// </summary>
        /// <returns>A list of all currencies for the mobile user.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/currencies")]
        public CurrencyResult Get()
        {
            CurrencyResult result = new CurrencyResult { FunctionName = "GetCurrencyList", ReturnCode = this.ServiceResultMessage.ReturnCode };

            switch (this.ServiceResultMessage.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        List<Currency> currencies = new List<Currency>();

                        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                        cCurrencies clscurrencies = new cCurrencies(this.PairingKeySerialKey.PairingKey.AccountID, null);

                        foreach (cCurrency r in clscurrencies.currencyList.Values)
                        {
                            if (!r.archived)
                            {
                                Currency currency = new Currency { currencyID = r.currencyid };
                                cGlobalCurrency globalcurrency = clsglobalcurrencies.getGlobalCurrencyById(r.globalcurrencyid);
                                if (globalcurrency != null)
                                {
                                    currency.label = globalcurrency.label;
                                    currency.symbol = globalcurrency.symbol;
                                }

                                currencies.Add(currency);
                            }
                        }

                        result.List = currencies;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetCurrencyList():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                        throw ex;
                    }

                    break;
            }

            return result;
        }
    }
}