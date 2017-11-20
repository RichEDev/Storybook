namespace SpendManagementApi.Models.Requests
{
    using System;
    using Common;
    using System.ComponentModel.DataAnnotations;
    using Attributes.Validation;

    /// <summary>
    /// Facilitates the finding of an exchange rate by providing search parameters.
    /// </summary>
    public class FindCurrencyExchangeRateRequest : ApiRequest
    {
        /// <summary>
        /// The I.D. of the currency you are converting from. 
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        [CurrencyIdValidation("FromCurrencyId")]
        public int FromCurrencyId { get; set; }

        /// <summary>
        /// The I.D. of the currency you are converting to. 
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        [CurrencyIdValidation("ToCurrencyId")]
        public int ToCurrencyId { get; set; }

        /// <summary>
        /// The date and time of the exchange rate lookup. <br /> 
        /// In the format of yyyy-MM-dd hh:mm:ss
        /// </summary>
        [Required]
        [CurrencyExchangeRateDateValidation("DateTimeOfRate")]     
        public DateTime DateTimeOfRate { get; set; }
    }
}