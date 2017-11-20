namespace SpendManagementApi.Models.Requests
{
    using System;
    using Common;

    /// <summary>
    /// Facilitates the finding of OdometerReadings, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindOdometerReadingsRequest : FindRequest
    {
        /// <summary>
        /// Search for OdometerReadings that were taken before this date.
        /// </summary>
        public DateTime? Before { get; set; }

        /// <summary>
        /// Search for OdometerReadings that were taken after this date.
        /// </summary>
        public DateTime? After { get; set; }

        /// <summary>
        /// Search for OdometerReadings with an Odometer reading over this value.
        /// </summary>
        public int? OdometerReadingOver { get; set; }

        /// <summary>
        /// Search for OdometerReadings with an Odometer reading under this value.
        /// </summary>
        public int? OdometerReadingUnder { get; set; }
    }
}