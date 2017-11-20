namespace SpendManagementApi.Models.Types
{
    using SpendManagementLibrary;
    
    /// <summary>
    /// A Global Country is an immutable country that exists on Earth. 
    /// Different Accounts pull in which countries they wish to use, by way of a <see cref="Country">Country</see>.
    /// </summary>
    public class GlobalCountry : BaseExternalType
    {
        #region properties

        /// <summary>
        /// The unique Id for this global country.
        /// </summary>
        public int GlobalCountryid { get; set; }

        /// <summary>
        /// The name of this global country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The country code for this global country.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The ISO 3166-1 alpha-3 country code (three digit country code).
        /// </summary>
        public string Alpha3CountryCode { get; set; }

        /// <summary>
        /// The ISO 3166-1 numeric-3 country code (three digit number country code).
        /// </summary>
        public int Numeric3CountryCode { get; set; }
        
        /// <summary>
        /// The regex format for this countries postcode if we have it, if not it returns an empty string.
        /// </summary>
        public string PostcodeRegex { get; set; }

        /// <summary>
        /// Whether or not this country can be used with Postcode Anywhere (for address search and distance/directions lookups).
        /// </summary>
        public bool PostcodeAnywhereEnabled { get; set; }

        #endregion
    }

    internal static class GlobalCountryExtension
    {
        internal static TResult Cast<TResult>(this cGlobalCountry globalCountry) where TResult : GlobalCountry, new()
        {
            if (globalCountry != null)
                return new TResult
                {
                    Alpha3CountryCode = globalCountry.Alpha3CountryCode,
                    Country = globalCountry.Country,
                    CountryCode = globalCountry.CountryCode,
                    CreatedOn = globalCountry.CreatedOn,
                    GlobalCountryid = globalCountry.GlobalCountryId,
                    ModifiedOn = globalCountry.ModifiedOn,
                    Numeric3CountryCode = globalCountry.Numeric3CountryCode,
                    PostcodeAnywhereEnabled = globalCountry.PostcodeAnywhereEnabled,
                    PostcodeRegex = globalCountry.PostcodeRegex
                };
            return null;
        }
    }

}