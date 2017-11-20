namespace SpendManagementApi.Models.Requests
{
    using System;
    using Common;

    /// <summary>
    /// Facilitates the finding of Cars, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindVehiclesRequest : FindRequest
    {
        /// <summary>
        /// Search for a car by make.
        /// </summary>
        public string Make { get; set; }
        
        /// <summary>
        /// Search for a car by model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Search for a car by Reg number.
        /// </summary>
        public string Registration { get; set; }

        /// <summary>
        /// Search for active cars.
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Search for approved cars.
        /// </summary>
        public bool? Approved { get; set; }

        /// <summary>
        /// Search for cars that begin before this date.
        /// </summary>
        public DateTime? StartsBeforeOrOn { get; set; }

        /// <summary>
        /// Search for cars that begin after this date.
        /// </summary>
        public DateTime? StartsAfter { get; set; }

        /// <summary>
        /// Search for cars that begin before this date.
        /// </summary>
        public DateTime? EndsBeforeOrOn { get; set; }

        /// <summary>
        /// Search for cars that begin after this date.
        /// </summary>
        public DateTime? EndsAfter { get; set; }

        /// <summary>
        /// Search for cars with an Odometer reading over this value.
        /// </summary>
        public int? OdometerReadingOver { get; set; }

        /// <summary>
        /// Search for cars with an Odometer reading under this value.
        /// </summary>
        public int? OdometerReadingUnder { get; set; }
    }
}