using SpendManagementLibrary.Addresses;

namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="Address">Item</see>s.
    /// </summary>
    public class GetAddressesResponse : GetApiResponse<Address>
    {
        /// <summary>
        /// Creates a new GetAddressesResponse.
        /// </summary>
        public GetAddressesResponse()
        {
            List = new List<Address>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Address">Address</see>.
    /// </summary>
    public class AddressResponse : ApiResponse<Address>
    {
    }

    /// <summary>
    /// A response containing the name of an address' account wide label.
    /// </summary>
    public class AccountWideLabelResponse : ApiResponse
    {
        /// <summary>
        /// The Id of the requested account wide label.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the address that this label applies to.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// The name of the account wide label, or the 'label' itself.
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// A response containing a particular <see cref="HomeAddressLinkage">HomeAddressLinkage</see>.
    /// </summary>
    public class HomeAddressLinkageResponse : ApiResponse<HomeAddressLinkage>
    {
    }

    /// <summary>
    /// A response containing a particular <see cref="WorkAddressLinkage">WorkAddressLinkage</see>.
    /// </summary>
    public class WorkAddressLinkageResponse : ApiResponse<WorkAddressLinkage>
    {
    }

}