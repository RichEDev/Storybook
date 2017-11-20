namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Repositories;

    using Holiday = SpendManagementApi.Models.Types.Holidays.Holiday;

    /// <summary>
    /// Manages operations on <see cref="Holiday">Holiday</see>.
    /// </summary>
    [RoutePrefix("Holidays")]
    [Version(1)]
    public class HolidaysV1Controller : ArchivingApiController<Holiday>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Holiday">Holiday</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return Links();
        }

        /// <summary>
        /// Gets all <see cref="Holiday">Holiday</see> available to the current user.
        /// </summary>
        /// <returns>A <see cref="HolidaysResponse">HolidaysResponse</see> containing all <see cref="Holiday">Holiday</see> for the current user.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HolidaysResponse GetAll()
        {
            return this.GetAll<HolidaysResponse>();
        }

        /// <summary>
        /// Saves a <see cref="Holiday">Holiday</see> for the current user. 
        /// </summary>
        /// <param name="request">The<see cref="HolidayRequest">HolidayRequest</see>details to be saved</param>
        /// <returns>An <see cref="HolidayResponse">HolidayResponse</see> containing the newly saved <see cref="Holiday">Holiday</see></returns>
        [Route("Save")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HolidayResponse SaveHoliday([FromBody] HolidayRequest request)
        {
            var response = InitialiseResponse<HolidayResponse>();
            response.Item = ((HolidayRepository)Repository).Add(request);
            return response; 
        }

        /// <summary>
        /// Updates a <see cref="Holiday">Holiday</see> for the current user. 
        /// </summary>
        /// <param name="request">The<see cref="HolidayRequest">HolidayRequest</see>details to be updated</param>
        /// <returns>An <see cref="HolidayResponse">HolidayResponse</see> containing the updated <see cref="Holiday">Holiday</see></returns>
        [HttpPut, Route("Update")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HolidayResponse UpdateHoliday([FromBody] HolidayRequest request)
        {
            var response = InitialiseResponse<HolidayResponse>();
            response.Item = ((HolidayRepository)Repository).Update(request);
            return response; 
        }

        /// <summary>
        /// Deletes a <see cref="Holiday">Holiday by its Id</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Holiday">Holiday</see> to delete.</param>
        /// <returns>A <see cref="HolidayResponse">HolidayResponse</see> with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public HolidayResponse Delete(int id)
        {
            return this.Delete<HolidayResponse>(id);
        }

    }
}
