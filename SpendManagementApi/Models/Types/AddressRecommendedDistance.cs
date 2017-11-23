namespace SpendManagementApi.Models.Types
{
    using SpendManagementLibrary.Addresses;
    using Interfaces;

    /// <summary>
    /// Represents a recommended (user defined) distance between two <see cref="Address">Addresses</see>.
    /// <strong>Important: The Id pattern here works slightly differently.</strong> Due to the highly flexible nature of 
    /// Addresses and distances in Expenses, multiple journey distances can be recorded for two addresses.
    /// To find other pairs, look up addresses by the Address Ids.
    /// </summary>
    public class AddressRecommendedDistance : BaseExternalType, IApiFrontForDbObject<AddressDistanceLookup, AddressRecommendedDistance>
    {
        /// <summary>
        /// The unique Id of this record in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the first address.
        /// </summary>
        public int AddressAId { get; set; }
        
        /// <summary>
        /// The Id of the target address.
        /// </summary>
        public int AddressBId { get; set; }

        /// <summary>
        /// The recommended (manually entered) distance between the two addresses.
        /// </summary>
        public decimal RecommendedDistance { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>An api Type</returns>
        public AddressRecommendedDistance From(AddressDistanceLookup dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.DestinationIdentifier;

            if (dbType.OutboundIdentifier > 0)
            {
                AddressAId = dbType.OutboundIdentifier;
                AddressBId = dbType.ReturnIdentifier;
                RecommendedDistance = dbType.Outbound;
            }
            else
            {
                AddressAId = dbType.ReturnIdentifier;
                AddressBId = dbType.OutboundIdentifier;
                RecommendedDistance = dbType.Return;
            }

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public AddressDistanceLookup To(IActionContext actionContext)
        {
            var item = new AddressDistanceLookup
            {
                OutboundIdentifier = AddressAId,
                Outbound = RecommendedDistance,
                ReturnIdentifier = AddressBId
            };

            return item;
        }
    }
}