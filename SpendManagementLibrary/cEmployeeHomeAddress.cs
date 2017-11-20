using System;
namespace SpendManagementLibrary
{
    using Infragistics.Web.UI.GridControls;

    /// <summary>
    /// Represent the employee Home Addree
    /// </summary>
    public class cEmployeeHomeAddress
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the post code.
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="cEmployeeHomeAddress"/> class.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="addressLine1">
        /// The address line 1.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="postCode">
        /// The post code.
        /// </param>
        public cEmployeeHomeAddress(DateTime? startDate, DateTime? endDate, string addressLine1, string city, string postCode)
        {
            this.AddressLine1 = addressLine1;
            this.City = city;
            this.EndDate = endDate;
            this.StartDate = startDate;
            this.PostCode = postCode;
        }
    }
}
