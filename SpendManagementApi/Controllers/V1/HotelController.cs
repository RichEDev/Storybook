namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Attributes;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Repositories;


    /// <summary>
    /// Manages operations on <see cref="Hotel">Hotel</see>.
    /// </summary>
    [RoutePrefix("Hotels")]
    [Version(1)]
    public class HotelV1Controller : BaseApiController<Hotel>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HotelV1Controller"/> class. 
        /// </summary>
        public HotelV1Controller()
        {
        }
 
        /// <summary>
        /// Gets all of the available end points from the <see cref="Hotel">Hotel</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return Links();
        }

        /// <summary>
        /// Gets a list of hotels that that match the hotel name search criteria
        /// </summary>
        /// <param name="hotelName">The hotel name to search on</param>
        /// <returns>A HotelResponse, containing a list of <see cref="Hotel">Hotels</see></returns>
        [HttpGet, Route("GetHotelsByName")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HotelResponseList GetHotelsByName([FromUri] string hotelName)
        {
            var response = InitialiseResponse<HotelResponseList>();
            response.List = ((HotelRepository)Repository).GetHotelsByName(hotelName);
            return response;
        }


        /// <summary>
        /// Saves a <see cref="Hotel">Hotel</see>. 
        /// </summary>
        /// <param name="request">The<see cref="Hotel">Hotel</see>details to be saved</param>
        /// <returns>An HotelResponse containing the newly saved <see cref="Hotel">Hoitel</see></returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HotelResponse Post([FromBody] Hotel request)
        {
            return this.Post<HotelResponse>(request);
        }
    }
}
