using System;

namespace SpendManagementApi.Models.Types.MyDetails
{
    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// Represents an Employee work address
    /// </summary>
    public class WorkAddress : IApiFrontForDbObject<cEmployeeHomeAddress, WorkAddress>
    {
        /// <summary>
        /// Gets or sets the start date for this Address.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this Address.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Postcode.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public WorkAddress From(cEmployeeHomeAddress dbType, IActionContext actionContext)
        {
            this.StartDate = dbType.StartDate;
            this.EndDate = dbType.EndDate;
            this.AddressLine1 = dbType.AddressLine1;
            this.City = dbType.City;
            this.Postcode = dbType.PostCode;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cEmployeeHomeAddress To(IActionContext actionContext)
        {
            //TODO: Not needed
            throw new NotImplementedException();
        }
    }
}