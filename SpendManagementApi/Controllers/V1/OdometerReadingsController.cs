namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Manages operations on <see cref="OdometerReading">OdometerReadings</see>.
    /// </summary>
    [RoutePrefix("OdometerReadings")]
    [Version(1)]
    public class OdometerReadingsV1Controller : BaseApiController<OdometerReading>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="OdometerReading">OdometerReadings</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="OdometerReading">OdometerReadings</see> in the system for a given car.
        /// </summary>
        /// <returns>A GetOdometerReadingsResponse containing the matching <see cref="OdometerReading">OdometerReadings</see>.</returns>
        [HttpGet, Route("ForCar/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetOdometerReadingsResponse ForCar(int id)
        {
            var car = new VehicleRepository(this.CurrentUser).Get(id);
            this.CheckMultipleAccessRoles(car, AccessRoleType.View);
            var response = this.InitialiseResponse<GetOdometerReadingsResponse>();
            response.List = ((OdometerReadingRepository)this.Repository).ForCar(id).ToList();
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="OdometerReading">OdometerReading</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="OdometerReading">OdometerReading</see> to get.</param>
        /// <returns>A GetOdometerReadingsResponse object, which will contain a list of zero-to-many List, depending on how the criteria object is configured.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public OdometerReadingResponse Get([FromUri] int id)
        {
            var response = this.Get<OdometerReadingResponse>(id);
            if (response.Item != null)
            {
                var car = new VehicleRepository(this.CurrentUser).Get(response.Item.CarId);
                this.CheckMultipleAccessRoles(car, AccessRoleType.View);
            }
            return response;
        }

        /// <summary>
        /// Adds an <see cref="OdometerReading">OdometerReading</see>.
        /// </summary>
        /// <param name="request">The <see cref="OdometerReading">OdometerReading</see> to add. <br/>
        /// When adding a new <see cref="OdometerReading">OdometerReading</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.<br/>
        /// CarId: Must be set to the Id of an existing car, or the add will throw an error.<br/>
        /// TakenOn: The OdometerReading must have been taken on a valid date.<br/>
        /// Old: Every OdometerReading must have an old reading.<br/>
        /// New: Every OdometerReading must have a new reading.<br/>
        /// </param>
        /// <returns>A GetOdometerReadingsResponse containing the added <see cref="OdometerReading">OdometerReadings</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public OdometerReadingResponse Post([FromBody] OdometerReading request)
        {
            var response = this.Post<OdometerReadingResponse>(request);
            if (response.Item != null)
            {
                var car = new VehicleRepository(this.CurrentUser).Get(response.Item.CarId);
                this.CheckMultipleAccessRoles(car, AccessRoleType.View);
            }
            return response;
        }

        /// <summary>
        /// Edits an <see cref="OdometerReading">OdometerReading</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="OdometerReading">OdometerReading</see> to edit.</param>
        /// <param name="request">The <see cref="OdometerReading">OdometerReading</see> to edit.</param>
        /// <returns>A GetOdometerReadingsResponse containing the edited <see cref="OdometerReading">OdometerReadings</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public OdometerReadingResponse Put([FromUri] int id, [FromBody] OdometerReading request)
        {
            request.OdometerReadingId = id;
            var response = this.Put<OdometerReadingResponse>(request);
            if (response.Item != null)
            {
                var car = new VehicleRepository(this.CurrentUser).Get(response.Item.CarId);
                this.CheckMultipleAccessRoles(car, AccessRoleType.View);
            }
            return response;
        }

        /// <summary>
        /// Deletes an <see cref="OdometerReading">OdometerReading</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="OdometerReading">OdometerReading</see> to be deleted</param>
        /// <returns>An OdometerReadingResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Delete)]
        public OdometerReadingResponse Delete(int id)
        {
            var response = this.Get(id);
            if (response.Item != null)
            {
                var car = new VehicleRepository(this.CurrentUser).Get(response.Item.CarId);
                this.CheckMultipleAccessRoles(car, AccessRoleType.View);
            }
            return this.Delete<OdometerReadingResponse>(id);
        }


        #endregion Api Methods


        private void CheckMultipleAccessRoles(Vehicle item, AccessRoleType typeForVehicle)
        {
            if (item == null)
            {
                return;
            }

            if (item.EmployeeId == null)
            {
                if (!this.CurrentUser.CheckAccessRoleApi(SpendManagementElement.PoolCars, typeForVehicle, this.MobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api))
                {
                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbidden));
                }
            }
            else
            {
                if (!this.CurrentUser.CheckAccessRoleApi(SpendManagementElement.Employees, AccessRoleType.View, this.MobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api))
                {
                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbidden));
                }
            }
        }
    }
}
