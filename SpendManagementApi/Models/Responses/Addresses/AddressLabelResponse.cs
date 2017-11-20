namespace SpendManagementApi.Models.Responses.Addresses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The address label response.
    /// </summary>
    public class AddressLabelResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="AddressLabel">AddressLabel</see>
        /// </summary>
        public List<AddressLabel> List { get; set; }
    }
}