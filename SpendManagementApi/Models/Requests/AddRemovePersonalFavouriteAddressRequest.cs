namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The add remove personal favourite address request.
    /// </summary>
    public class AddRemovePersonalFavouriteAddressRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the Identifier for the address. (either addressId or global identifier) 
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the address id. Set to 0 if new favourite, or set to FavouriteId to delete a favourite by its Id
        /// </summary>
        public int FavouriteId { get; set; }

        /// <summary>
        /// Gets or sets the action which needs to take place for the favourite address
        /// </summary>
        public FavouriteAction FavouriteAction { get; set; }
    }
}