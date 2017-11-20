namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Attributes.Validation;
    using Common;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// The <see cref="MileageCategoryRequest">MileageCategoryRequest</see> Used to Create or Update a MileageCategory
    /// </summary>
    public class MileageCategoryRequest : ApiRequest
    {
        /// The unique Id for this mileage category.
        /// </summary>
        public int MileageCategoryId { get; set; }

        /// <summary>
        /// The label for this mileage category.
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// The comment for this mileage category.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The ThresholdType for this mileage category.
        /// </summary>
        [Required]
        public SpendManagementApi.Common.Enums.ThresholdType ThresholdType { get; set; }

        /// <summary>
        /// Whether new journey totals should be calculated for this mileage category.
        /// </summary>
        public bool CalculateNewJourneyTotal { get; set; }

        /// <summary>
        /// The unit of measure for this mileage category.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public MileageUOM UnitOfMeasure { get; set; }

        /// <summary>
        /// The Id of the currency that applies to this mileage category.
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// The user rates table.
        /// </summary>
        [Required]
        public string NhsMileageCode { get; set; }

        /// <summary>
        /// The user rates from engine size.
        /// </summary>
        public int StartEngineSize { get; set; }

        /// <summary>
        /// The user rates to engine size.
        /// </summary>
        public int EndEngineSize { get; set; }

        /// <summary>
        /// The Id of the financial year for this mileage category.
        /// </summary>
        public int? FinancialYearId { get; set; }
    }
}