using SpendManagementApi.Utilities;

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;

    internal class CountryRepository : ArchivingBaseRepository<Country>, ISupportsActionContext
    {
        private readonly cCountries _countryData;
        private readonly cGlobalCountries _clsGlobalCountries;
        private readonly cSubcats _clsSubCategories;
        private cCountry _cCountry;
        
        public CountryRepository(ICurrentUser user, IActionContext actionContext = null) : base(user, actionContext, country => country.CountryId, null)
        {
            ActionContext.SubAccountId = User.CurrentSubAccountId;
            _countryData = this.ActionContext.Countries;
            _clsGlobalCountries = this.ActionContext.GlobalCountries;
            _clsSubCategories = this.ActionContext.SubCategories;
        }

        public override IList<Country> GetAll()
        {
            List<cCountry> dbCountries = _countryData.list.Values.ToList();
            Dictionary<int, cGlobalCountry> dbGlobalCountries = _clsGlobalCountries.GetList();

            List<Country> countries = dbCountries.Select(c => c.Cast<Country>(_clsGlobalCountries)).ToList();
            List<int> unassociatedGlobalCountryIds =
                dbGlobalCountries.Select(gc => gc.Value.GlobalCountryId)
                                 .Except(dbCountries.Select(c => c.GlobalCountryId))
                                 .ToList();
            List<Country> unassociatedGlobalCountries =
                dbGlobalCountries.Values.Where(gc => unassociatedGlobalCountryIds.Contains(gc.GlobalCountryId))
                                 .Select(gc => gc.Cast<Country>(true)).ToList();

            countries = countries.Union(unassociatedGlobalCountries).ToList();

            var globalcountries = new cGlobalCountries();

            foreach (var country in countries)
            {
                country.CountryName = globalcountries.getGlobalCountryById(country.GlobalCountryId).Country;
            }

            return countries;
        }

        public override Country Get(int countryId)
        {
            cCountry country = _countryData.getCountryById(countryId);
            return country.Cast<Country>(_clsGlobalCountries);
        }

        public override Country Add(Country country)
        {
            base.Add(country);

            cCountry clsCountry = new cCountry(country.CountryId, country.GlobalCountryId, country.Archived, null, country.CreatedOn, country.CreatedById);

            if (_countryData.getCountryByGlobalCountryId(country.GlobalCountryId) != null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorCountryAlreadyExistsForGlobal);
            }

            if (country.CreatedById != User.EmployeeID)
            {
                throw new ApiException("Invalid Creator Id", "The creator specified does not match that of the current user");
            }

            cGlobalCountry globalCountry = this.ActionContext.GlobalCountries.getGlobalCountryById(country.GlobalCountryId);
            if (globalCountry == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongGlobalCurrencyId, ApiResources.ApiErrorWrongGlobalCurrencyIdMessage);
            }

            if (country.VatRates == null)
            {
                country.VatRates = new List<VatRate>();
            }

            if (country.VatRates.Any(rate => (rate.ExpenseSubCategoryId <= 0)))
            {
                throw new ApiException("Invalid Vat Rate Data",
                                       "Vat Rates provided have invalid expense sub category ids associated");
            }


            if (country.VatRates.Any(rate => _clsSubCategories.GetSubcatById(rate.ExpenseSubCategoryId) == null))
            {
                throw new ApiException("Invalid Expense Sub Category Data",
                                       "One or more of the expense sub categories associated with the vat rates provided is invalid");
            }

            int countryId = _countryData.saveCountry(clsCountry);

            if (!country.Archived.Equals(false))
            {
                _countryData.changeStatus(countryId, country.Archived);
            }

            if (countryId > 0)
            {
                _countryData.InitialiseData();
                country.VatRates.Select(rate=>rate.ConvertToInternalType(countryId)).ToList().ForEach(vatRate => _countryData.saveVatRate(vatRate, 0, User.EmployeeID));
            }

            Country updatedCountry = _countryData.getCountryById(countryId).Cast<Country>(_clsGlobalCountries);
            return updatedCountry;
        }

        public override Country Delete(int countryId)
        {
            Country country = Get(countryId);

            if (country == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongCountryId, ApiResources.ApiErrorWrongCountryIdMessage);
            }

            if (_countryData.deleteCountry(country.CountryId) > 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }
            
            return this.Get(countryId);
        }


        /// <summary>
        /// Archives / UnArchives a Country.
        /// </summary>
        /// <param name="id">The Id of the Country.</param>
        /// <param name="archive">Whether or not to archive the Country.</param>
        /// <returns></returns>
        public override Country Archive(int id, bool archive)
        {
            var country = Get(id);
            country.Archived = archive;
            _countryData.changeStatus(country.CountryId, country.Archived);
            return country;
        }


        /// <summary>
        /// Only updates the archive status of the record and the expense sub category vat rates/ percentages
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public override Country Update(Country country)
        {
            base.Update(country);

            _cCountry = _countryData.getCountryById(country.CountryId);

            if (_cCountry == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongCountryId, ApiResources.ApiErrorWrongCountryIdMessage);
            }

            cGlobalCountries globalCountries = new cGlobalCountries();
            cGlobalCountry globalCountry = globalCountries.getGlobalCountryById(country.GlobalCountryId);
            if (globalCountry == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongGlobalCurrencyId, ApiResources.ApiErrorWrongGlobalCurrencyIdMessage);
            }
            
            //Saving archive flag only if the flag differs from the existing data
            if (_cCountry.Archived != country.Archived)
            {
                int returnValue = _countryData.changeStatus(country.CountryId, country.Archived);
                if (returnValue != 0)
                    return null;
            }

            //Update/ add associated vat rates
            //Delete associations marked with the ForDelete flag
            if (country.VatRates != null && country.VatRates.Any())
            {
                //Records for update
                List<ForeignVatRate> ratesToUpdate = _cCountry.VatRates.Values.Join(country.VatRates.Where(r=>!r.ForDelete), 
                    x => x.SubcatId, 
                    y => y.ExpenseSubCategoryId, 
                    (x,y)=>y.ConvertToInternalType(country.CountryId)).ToList();
                ratesToUpdate.ForEach(rate => UpdateVatRateSubCat(rate));
                
                //Records for delete
                List<ForeignVatRate> ratesToDelete = _cCountry.VatRates.Values.Join(country.VatRates.Where(r => r.ForDelete),
                    x => x.SubcatId,
                    y => y.ExpenseSubCategoryId,
                    (x, y) => y.ConvertToInternalType(country.CountryId)).ToList();
                ratesToDelete.ForEach(rate => UpdateVatRateSubCat(rate, true));

                //Records for add
                List<VatRate> existingVatRates = country.VatRates.Join(_cCountry.VatRates.Values.Select(r=>r.Cast<VatRate>()),
                    x => x.ExpenseSubCategoryId,
                    y => y.ExpenseSubCategoryId,
                    (x, y) => x).ToList();
                IEnumerable<VatRate> ratesToAdd = country.VatRates.Except(existingVatRates);
                ratesToAdd.ToList().ForEach(vatRate => _countryData.saveVatRate(vatRate.ConvertToInternalType(country.CountryId), 0, User.EmployeeID));
            }

            _countryData.InitialiseData();
            country = _countryData.getCountryById(country.CountryId).Cast<Country>(_clsGlobalCountries);
            return country;
        }

        /// <summary>
        /// Gets a list of active <see cref="Country"/> for the account
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/> of active countries.
        /// </returns>
        public IList<Country> GetActiveCountries()
        {
            IList<Country> activeCountries = new List<Country>();
            IEnumerable<Country> countryList = this.GetAll();

            foreach (var country in countryList.Where(country => !country.Archived && country.CountryId > 0))
            {
                activeCountries.Add(country);
            }

            return activeCountries;
        }

        private void UpdateVatRateSubCat(ForeignVatRate vatRate, bool forDelete = false)
        {
            KeyValuePair<int, ForeignVatRate> existingVatRate =
                _cCountry.VatRates.First(
                    rate => rate.Value.CountryId == vatRate.CountryId && rate.Value.SubcatId == vatRate.SubcatId);
            if (forDelete)
            {
                _countryData.deleteVatRate(existingVatRate.Key);
            }
            else
            {
                _countryData.saveVatRate(vatRate, existingVatRate.Key, User.EmployeeID);
            }
        }
    }
}