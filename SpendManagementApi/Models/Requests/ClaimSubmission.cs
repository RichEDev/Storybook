namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// Defines a request that describes claim submission details.
    /// </summary>
    public class ClaimSubmissionRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the claim id.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the claim name.
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// Gets or sets the claim description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cash claim or not, True if a cash claim.
        /// </summary>
        public bool Cash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether credit claim or not, True if a credit claim.
        /// </summary>
        public bool Credit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether purchase claim or not, True if a purchase claim.
        /// </summary>
        public bool Purchase { get; set; }

        /// <summary>
        /// Gets or sets the claim approver id.
        /// </summary>
        public int? Approver { get; set; }

        /// <summary>
        /// Gets or sets the odometer readings.
        /// </summary>
        public List<List<object>> OdometerReadings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether business mileage.
        /// </summary>
        public bool BusinessMileage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore approver on holiday.
        /// </summary>
        public bool IgnoreApproverOnHoliday { get; set; }

        /// <summary>
        /// Gets or sets the viewFilter.
        /// </summary>
        public byte ViewFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether continue although authoriser is on holiday.
        /// </summary>
        public bool ContinueAlthoughAuthoriserIsOnHoliday { get; set; }
    }
}