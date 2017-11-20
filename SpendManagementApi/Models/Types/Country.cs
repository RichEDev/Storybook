namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using SpendManagementLibrary;
    using Spend_Management;
    
    /// <summary>
    /// Represents a real world country that users are allowed to make claims in. 
    /// There are a fixed amount of Countries in the world, so SpendManagement
    /// represents these with a Global set. Each account can then choose how many from the global set they would like
    /// including in their custom configuration of Expenses. That is what this Country object is for.
    /// The GlobalCountryId propery can be used with the Globals resource to find more information on countries.
    /// </summary>
    public class Country : ArchivableBaseExternalType, IEquatable<Country>
    {
        /// <summary>
        /// The unique ID of the country.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// The country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// The Id of the Global Country that this represents.<br/>
        /// Use the Globals resource to find GlobalCountries.
        /// </summary>
        [Required]
        public int GlobalCountryId { get; set; }

        /// <summary>
        /// The VAT rates for this country.
        /// </summary>
        public IEnumerable<VatRate> VatRates { get; set; }


        public bool Equals(Country other)
        {
            if (other == null)
            {
                return false;
            }
            return this.GlobalCountryId.Equals(other.GlobalCountryId) && this.Archived.Equals(other.Archived)
                   && this.VatRates.SequenceEqual(other.VatRates);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Country);
        }
    }

    /// <summary>
    /// Conversion from internal to external and vice versa
    /// </summary>
    internal static class CountryExtension
    {
        internal static TResult Cast<TResult>(this ForeignVatRate vatRate)
            where TResult : VatRate, new()
        {
            return new TResult
            {
                ExpenseSubCategoryId = vatRate.SubcatId,
                Vat = vatRate.Vat,
                VatPercent = vatRate.VatPercent
            };
        }

        internal static ForeignVatRate ConvertToInternalType(this VatRate vatRate, int countryId)
        {
            return new ForeignVatRate
            {
                SubcatId = vatRate.ExpenseSubCategoryId,
                Vat = vatRate.Vat,
                VatPercent = vatRate.VatPercent,
                CountryId = countryId
            };
        }

        internal static TResult Cast<TResult>(this cCountry country, cGlobalCountries globalCountries) where TResult : Country, new()
        {
            if (country != null)
                return new TResult
                {
                    Archived = country.Archived,
                    CountryId = country.CountryId,
                    CreatedById = country.CreatedBy ?? 0,
                    CreatedOn = country.CreatedOn,
                    GlobalCountryId = country.GlobalCountryId,
                    VatRates = country.VatRates.Select(v => v.Value.Cast<VatRate>())
                };
            return null;
        }

        internal static TResult Cast<TResult>(this cGlobalCountry globalCountry, bool toCountry) where TResult : Country, new()
        {
            if (globalCountry != null)
                return new TResult
                           {
                                GlobalCountryId = globalCountry.GlobalCountryId
                           };
            return null;
        }
    }
}