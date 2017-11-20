namespace SpendManagementApi.Models.Requests
{
    using System;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The address request. Used for searching for an address.
    /// </summary>
    public class AddressRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the search term, i.e. postcode, street name.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the country Id the address belongs to.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the date of the expense.
        /// </summary>
        public DateTime ExpenseDate { get; set; }

        /// <summary>
        /// Gets or sets the Esr assignment id. Used to filter office addresses for an assignment, if applicable
        /// </summary>
        public int? EsrAssignmentId { get; set; }
    }
}