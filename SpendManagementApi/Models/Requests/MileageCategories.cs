namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// Facilitates the finding of MileageCategories, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindMileageCategoriesRequest : FindRequest
    {
        /// <summary>
        /// Gets or sets the Mileage Category Id
        /// </summary>
        public int? MileageCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Journey Rate
        /// </summary>
        public string VehicleJourneyRate { get; set; }

        /// <summary>
        /// Gets or sets the Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the FinancialYearId
        /// </summary>
        public int? FinancialYearId { get; set; }
    }

    /// <summary>
    /// The <see cref="MileageDateRangeRequest">MileageDateRangeRequest</see>
    /// </summary>
    public class MileageDateRangeRequest : ApiRequest
    {
        /// <summary>
        /// A list of <see cref="DateRanges">DateRanges</see>
        /// </summary>
        public List<DateRange> DateRanges { get; set; } 
    }

    /// <summary>
    /// Thw <see cref="MileageThresholdRequest">MileageThresholdRequest</see>
    /// </summary>
    public class MileageThresholdRequest : ApiRequest
    {
        /// <summary>
        /// A list of <see cref="Thresholds">DateRanges</see>
        /// </summary>
        public List<Threshold> Thresholds { get; set; }
    }

    /// <summary>
    /// The <see cref="ThresholdFuelRateRequest">ThresholdFuelRateRequest</see>
    /// </summary>
    public class ThresholdFuelRateRequest : ApiRequest
    {
        /// <summary>
        /// A <see cref="FuelRate">FuelRate</see>
        /// </summary>
        public FuelRate FuelRate { get; set; }
    }
}