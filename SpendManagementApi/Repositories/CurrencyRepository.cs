namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    using Currency = Models.Types.Currency;
    using CurrencyType = Common.Enums.CurrencyType;
    using Utilities;

    internal class CurrencyRepository : ArchivingBaseRepository<Currency>, ISupportsActionContext
    {
        private readonly cCurrencies _currencyData;
        private cCurrency _cCurrency;
        private readonly cGlobalCurrencies _cGlobalCurrencies;
        private readonly cMisc _cMisc;

        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory;

        public CurrencyRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, curr => curr.CurrencyId, null)
        {
            ActionContext.SubAccountId = User.CurrentSubAccountId;
            _currencyData = this.ActionContext.Currencies;
            _cGlobalCurrencies = this.ActionContext.GlobalCurrencies;
            _cMisc = new cMisc(User.AccountID);
            this._generalOptionsFactory = WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>();
        }

        //Returns the list of countries associated to a GlobalCountry entry as well as the unassociated global countries
        public override IList<Currency> GetAll()
        {
            //List of associated currencies
            SortedList cCurrencies = _currencyData.currencyList;
            List<Currency> currencies =
            cCurrencies.Values.OfType<cCurrency>().Select(c => c.Cast<Currency>(_cGlobalCurrencies)).ToList();

            List<int> associatedGlobalCurrencyIds = currencies.Select(c => c.GlobalCurrencyId).ToList();

            //List of unassociated currencies
            List<int> unassociatedGlobalCurrencyIds =
                _cGlobalCurrencies.getGlobalCurrencyIds()
                                 .Except(associatedGlobalCurrencyIds)
                                 .ToList();
            List<Currency> unassociatedGlobalCurrencies =
                _cGlobalCurrencies.globalcurrencies.Values.Where(gc => gc.sUnicodeSymbol.Trim().Length > 0 && unassociatedGlobalCurrencyIds.Contains(gc.globalcurrencyid))
                                 .Select(gc => gc.Cast<Currency>(true)).ToList();

            currencies = currencies.Union(unassociatedGlobalCurrencies).ToList();

            CurrencyType firsCurrencyType = CurrencyType.Static;
            if (currencies.Count > 0)
            {
                SetCurrencyType(currencies.First());
                firsCurrencyType = (currencies.First().CurrencyType ?? CurrencyType.Static);
            }

            foreach (var currency in currencies)
            {
                currency.CurrencyType = firsCurrencyType;
                currency.CurrencyName = _cGlobalCurrencies.getGlobalCurrencyById(currency.GlobalCurrencyId).label;
                currency.AlphaCode = _cGlobalCurrencies.getGlobalCurrencyById(currency.GlobalCurrencyId).alphacode;
            }

            return currencies;
        }

        public override Currency Get(int currencyId)
        {
            var cCurrency = _currencyData.getCurrencyById(currencyId);
            var currency = cCurrency.Cast<Currency>(_cGlobalCurrencies);
            SetCurrencyType(currency);
            return currency;
        }

        public Currency ByNumericCode(int numericCode)
        {
            var cCurrency = _currencyData.getCurrencyByNumericCode(numericCode.ToString());
            var currency = cCurrency.Cast<Currency>(_cGlobalCurrencies);
            SetCurrencyType(currency);
            return currency;
        }
        public Currency ByAlphaCode(string alphaCode)
        {
            var cCurrency = _currencyData.getCurrencyByAlphaCode(alphaCode);
            var currency = cCurrency.Cast<Currency>(_cGlobalCurrencies);
            SetCurrencyType(currency);
            return currency;
        }

        /// <summary>
        /// Gets a currency by its global currency I.D.
        /// </summary>
        /// <param name="globalCurrencyId">The global currency I.D.</param>
        /// <returns>The<see cref="Currency"/></returns>
        public Currency ByGlobalCurrencyId(int globalCurrencyId)
        {
            var currencyData = _currencyData.getCurrencyByGlobalCurrencyId(globalCurrencyId);
            var currency = currencyData.Cast<Currency>(_cGlobalCurrencies);
            SetCurrencyType(currency);
            return currency;
        }

        /// <summary>
        /// Determines the <see cref="GlobalCurrency"/> from the currencyId
        /// </summary>
        /// <param name="currencyId">The currency I.D.</param>
        /// <returns>The<see cref="GlobalCurrency"/></returns>
        public GlobalCurrency GetGlobalCurrencyFromCurrencyId(int currencyId)
        {
            var currencyData = _currencyData.getCurrencyById(currencyId);

            if (currencyData == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongCurrencyId, ApiResources.ApiErrorWrongCurrencyIdMessage);
            }
            var globalCurrencies = new cGlobalCurrencies();

            cGlobalCurrency globalCurrency = globalCurrencies.getGlobalCurrencyById(currencyData.globalcurrencyid);
      
            if (globalCurrency == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongGlobalCurrencyId, ApiResources.ApiErrorWrongGlobalCurrencyIdMessage);
            }

            return globalCurrency.Cast<GlobalCurrency>();
        }

        public override Currency Add(Currency currency)
        {
            base.Add(currency);

            if (_currencyData.getCurrencyByGlobalCurrencyId(currency.GlobalCurrencyId) != null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorCurrencyAlreadyExistsForGlobal);
            }

            cGlobalCurrencies globalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency globalCurrency = globalCurrencies.getGlobalCurrencyById(currency.GlobalCurrencyId);
            if (globalCurrency == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongGlobalCurrencyId, ApiResources.ApiErrorWrongGlobalCurrencyIdMessage);
            }

            currency.CreatedById = User.EmployeeID;
            currency.CreatedOn = DateTime.Now;
            currency.ModifiedById = User.EmployeeID;
            currency.ModifiedOn = DateTime.Now;

            cCurrency clsCurrency = currency.Cast<cCurrency>();
            int currencyId = _currencyData.saveCurrency(clsCurrency);

            CurrencyType? currencyType = currency.CurrencyType;
            if (currencyType.HasValue)
            {
                _currencyData.ChangeCurrencyType(currencyType.Value.Cast<CurrencyType>(), User.EmployeeID);
            }

            Currency updatedCurrency = Get(currencyId);

            if (currency.Archived != updatedCurrency.Archived)
            {
                _currencyData.changeStatus(updatedCurrency.CurrencyId, currency.Archived);
            }

            return this.Get(currencyId);
        }

        public override Currency Delete(int currencyId)
        {
            var currency = Get(currencyId);
            if (currency == null)
                throw new ApiException(ApiResources.ApiErrorWrongCurrencyId, ApiResources.ApiErrorWrongCurrencyIdMessage);

            int deleteSuccess = _currencyData.deleteCurrency(currency.CurrencyId);

            if (deleteSuccess == 1)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage1);
            }

            if (deleteSuccess == 2)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage2);
            }

            if (deleteSuccess == 3)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage3);
            }

            if (deleteSuccess == 4)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage4);
            }

            if (deleteSuccess == 5)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage5);
            }

            if (deleteSuccess == 6)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage6);
            }

            if (deleteSuccess == 9)
            {
                throw new ApiException(ApiResources.ApiErrorCurrencyDeleteUnsuccessful, ApiResources.ApiErrorCurrencyErrorMessage9);
            }

            return this.Get(currencyId);
        }

        /// <summary>
        /// Archives / UnArchives a currency.
        /// </summary>
        /// <param name="id">The Id of the currency.</param>
        /// <param name="archive">Whether or not to archive the currency.</param>
        /// <returns></returns>
        public override Currency Archive(int id, bool archive)
        {
            var currency = Get(id);
            currency.Archived = archive;
            SaveArchiveStatus(currency);
            return currency;
        }

        /// <summary>
        /// Only updates the archive status of the record
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public override Currency Update(Currency currency)
        {
            base.Update(currency);

            _cCurrency = _currencyData.getCurrencyById(currency.CurrencyId);

            if (_cCurrency == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongCurrencyId, ApiResources.ApiErrorWrongCurrencyIdMessage);
            }

            currency.ModifiedById = User.EmployeeID;
            currency.ModifiedOn = System.DateTime.Now;

            //Saving archive flag only if the flag differs from the existing data
            if (_cCurrency.archived != currency.Archived)
            {
                SaveArchiveStatus(currency);
            }

            //Updating currency type if provided and different from existing value
            var generalOptions = this._generalOptionsFactory[this.User.CurrentSubAccountId].WithCurrency();
            SpendManagementLibrary.CurrencyType currencyType = (SpendManagementLibrary.CurrencyType)generalOptions.Currency.CurrencyType;
            if (currency.CurrencyType.HasValue && currency.CurrencyType.Value.Cast<CurrencyType>() != currencyType)
            {
                _currencyData.ChangeCurrencyType(currency.CurrencyType.Value.Cast<CurrencyType>(), User.EmployeeID);
            }

            cCurrency.RemoveFromCache(User.AccountID, currency.CurrencyId);

            currency = Get(currency.CurrencyId);

            return currency;
        }

        /// <summary>
        /// Determines the currency symbol from the supplied currencyId
        /// </summary>
        /// <param name="currencyId">The currencyId</param>
        /// <returns>The currency symbol</returns>
        public string DetermineCurrencySymbol(int currencyId)
        {                 
            cCurrency currency = this.ActionContext.Currencies.getCurrencyById(currencyId);
            string symbol = currency != null ? this.ActionContext.GlobalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).symbol : "£";

            return symbol;
        }

        /// <summary>
        /// Gets a list of active <see cref="Currency"/> for the account
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/> of active currencies.
        /// </returns>
        public IList<Currency> GetActiveCurrencies()
        {
            IList<Currency> activeCurrencies = new List<Currency>();
            IEnumerable<Currency> currencyList = this.GetAll();

            foreach (var currency in currencyList.Where(currency => !currency.Archived && currency.CurrencyId > 0))
            {
                activeCurrencies.Add(currency);
            }

            return activeCurrencies;
        }

        private void SaveArchiveStatus(Currency currency)
        {
            int returnValue = _currencyData.changeStatus(currency.CurrencyId, currency.Archived);
            if (returnValue != 0)
            {
                var errorMessage = "";

                switch (returnValue)
                {
                    case 1:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedPrimary;
                        break;
                    case 2:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedExpenseClaim;
                        break;
                    case 3:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedContracts;
                        break;
                    case 4:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedSuppliers;
                        break;
                    case 5:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedProducts;
                        break;
                    case 9:
                        errorMessage = ApiResources.ApiErrorCurrencyArchiveFailedBankAccounts;
                        break;
                }

                throw new ApiException(ApiResources.ApiErrorArchiveFailed, errorMessage);
            }
        }

        private void SetCurrencyType(Currency currency)
        {
            if (currency == null)
            {
                return;
            }

            var generalOptions = this._generalOptionsFactory[this.User.CurrentSubAccountId].WithCurrency();
            SpendManagementLibrary.CurrencyType currencyType = (SpendManagementLibrary.CurrencyType)generalOptions.Currency.CurrencyType;
            currency.CurrencyType = currencyType.Cast<CurrencyType>();
        }
    }
}