namespace SpendManagementLibrary.Addresses
{
    /// <summary>
    /// An class to hold the outbound and inbound journey distances
    /// </summary>
    public class AddressDistanceLookup
    {
        #region Public Properties

        /// <summary>
        /// The friendly name for an address, currently - Line1, City, Postcode
        /// </summary>
        public string DestinationFriendlyName { get; set; }

        /// <summary>
        /// The identifier for the destination address (the origin is known from the calling address)
        /// </summary>
        public int DestinationIdentifier { get; set; }

        /// <summary>
        /// The custom distance specified for the outbound journey
        /// </summary>
        public decimal Outbound { get; set; }

        /// <summary>
        /// The postcode anywhere fastest distance for the outbound journey
        /// </summary>
        public decimal? OutboundFastest { get; set; }

        /// <summary>
        /// The AddressDistance record identifier where the DestinationIdentifier is AddressIDB
        /// </summary>
        public int OutboundIdentifier { get; set; }

        /// <summary>
        /// The postcode anywhere shortest distance for the outbound journey
        /// </summary>
        public decimal? OutboundShortest { get; set; }

        /// <summary>
        /// The custom distance specified for the return journey
        /// </summary>
        public decimal Return { get; set; }

        /// <summary>
        /// The postcode anywhere fastest distance for the return journey
        /// </summary>
        public decimal? ReturnFastest { get; set; }

        /// <summary>
        /// The AddressDistance record identifier where the DestinationIdentifier is AddressIDA
        /// </summary>
        public int ReturnIdentifier { get; set; }

        /// <summary>
        /// The postcode anywhere shortest distance for the return journey
        /// </summary>
        public decimal? ReturnShortest { get; set; }

        #endregion

        /// <summary>
        /// The standard constructor that gives all properties default values
        /// </summary>
        public AddressDistanceLookup()
        {
            this.DestinationFriendlyName = "";
            this.DestinationIdentifier = 0;
            this.OutboundIdentifier = 0;
            this.Outbound = 0m;
            this.OutboundFastest = null;
            this.OutboundShortest = null;
            this.ReturnIdentifier = 0;
            this.Return = 0m;
            this.ReturnFastest = null;
            this.ReturnShortest = null;
        }
    }
}