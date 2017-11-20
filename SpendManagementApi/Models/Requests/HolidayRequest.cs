namespace SpendManagementApi.Models.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Attributes.Validation;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The holiday request.
    /// </summary>
    public class HolidayRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the holiday Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the holiday start date.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the holiday end date.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
    }
}