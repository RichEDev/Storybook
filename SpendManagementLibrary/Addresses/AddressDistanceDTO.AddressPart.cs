namespace SpendManagementLibrary.Addresses
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Address Distance Data Transfer Object used to store <see cref="AddressDistance"/> in cache
    /// IF I AM EVER CHANGED, PLEASE RERUN REDIS CACHE PRELOADER
    /// </summary>
    public partial class AddressDistanceDTO
    {
        /// <summary>
        /// Holds distance values from origin address to distination address and vice versa
        /// </summary>
        [Serializable]
        public class AddressPart
        {
            /// <summary>
            /// Gets or sets the address distance id.
            /// </summary>
            [JsonProperty("distance:id")]
            public int? DistanceId { get; set; }

            /// <summary>
            /// Gets or sets the origin address identifier.
            /// </summary>
            [JsonProperty("from")]
            public int FromId { get; set; }

            /// <summary>
            /// Gets or sets the destination address identifier.
            /// </summary>
            [JsonProperty("to")]
            public int ToId { get; set; }

            /// <summary>
            /// Gets or sets the custom distance from origin and destination
            /// </summary>
            [JsonProperty("custom:distance")]
            public decimal? CustomDistance { get; set; }

            /// <summary>
            /// Gets or sets the fastest distance from origin and destination
            /// </summary>
            [JsonProperty("fastest")]
            public decimal? FastestDistance { get; set; }

            /// <summary>
            /// Gets or sets the shortest distance between origin and destination
            /// </summary>
            [JsonProperty("shortest")]
            public decimal? ShortestDistance { get; set; }
        }
    }


}