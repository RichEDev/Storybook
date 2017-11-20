namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a <see cref="Hotel">Hotel</see>
    /// </summary>
    public class HotelResponse : ApiResponse<Hotel>
    {
      
    }

    /// <summary>
    /// A response containing a list of <see cref="Hotel">Hotels</see>
    /// </summary>
    public class HotelResponseList : ApiResponse
    {
        /// <summary>
        /// A list of <see cref="Hotel">Hotels</see>
        /// </summary>
        public List<Hotel> List { get; set; }
    }
}