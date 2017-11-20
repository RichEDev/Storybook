namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of Countries, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindCountryRequest : FindRequest
    {
        /// <summary>
        /// The Country Id.
        /// </summary>
        public int? CountryId { get; set; }
        
        /// <summary>
        /// The Global Country Id.
        /// </summary>
        public int? GlobalCountryId { get; set; }

        /// <summary>
        /// The name of the country.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Country code, in ISO 3166-1 format.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The 3 Letter country code, as per ISO 3166-1 alpha 3.
        /// </summary>
        public string Alpha3CountryCode { get; set; }

        /// <summary>
        ///The 3 number country code, as per ISO 3166-1. 
        /// </summary>
        public int? Numeric3CountryCode { get; set; }

        /// <summary>
        /// Whether the country is archived.
        /// </summary>
        public bool? Archived { get; set; }
    }
}