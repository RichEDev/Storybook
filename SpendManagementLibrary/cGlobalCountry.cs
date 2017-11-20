using System;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cGlobalCountry
    {
        public cGlobalCountry(int globalCountryId, string country, string countryCode, DateTime createdOn, DateTime modifiedOn, string postcodeRegexFormat, string alpha3CountryCode, int numeric3Code, bool postcodeAnwyhereEnabled)
        {
            this.GlobalCountryId = globalCountryId;
            this.Country = country;
            this.CountryCode = countryCode;
            this.CreatedOn = createdOn;
            this.ModifiedOn = modifiedOn;
            this.PostcodeRegex = postcodeRegexFormat;
            this.Alpha3CountryCode = alpha3CountryCode;
            this.Numeric3CountryCode = numeric3Code;
            this.PostcodeAnywhereEnabled = postcodeAnwyhereEnabled;
        }

        public int GlobalCountryId { get; private set; }

        public string Country { get; private set; }

        public string CountryCode { get; private set; }

        /// <summary>
        /// ISO 3166-1 alpha-3 country code (three digit country code)
        /// </summary>
        public string Alpha3CountryCode { get; private set; }

        /// <summary>
        /// ISO 3166-1 numeric-3 country code (three digit number country code)
        /// </summary>
        public int Numeric3CountryCode { get; private set; }

        public DateTime CreatedOn { get; private set; }
        public DateTime ModifiedOn { get; private set; }

        /// <summary>
        /// Gets the regex format for this countries postcode if we have it, if not it returns an empty string
        /// </summary>
        public string PostcodeRegex { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this country can be used with Postcode Anywhere (for address search and distance/directions lookups)
        /// </summary>
        public bool PostcodeAnywhereEnabled { get; private set; }
    }
}
