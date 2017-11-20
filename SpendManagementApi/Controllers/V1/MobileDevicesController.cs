namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages operations on <see cref="MobileDevice">MobileDevices</see>.
    /// </summary>
    [RoutePrefix("MobileDevices")]
    [Version(1)]
    public class MobileDevicesV1Controller : BaseApiController<MobileDevice>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="MobileDevice">MobileDevices</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="MobileDeviceType">MobileDeviceTypes</see> in the system.
        /// These are immutable.
        /// </summary>
        /// <returns>Returns a list of <see cref="MobileDeviceType">MobileDeviceTypes</see>.</returns>
        [HttpGet, Route("~/MobileDeviceTypes")]
        [AuthAudit(SpendManagementElement.MobileDevices, AccessRoleType.View)]
        public List<MobileDeviceType> MobileDeviceTypes()
        {
            return ((MobileDeviceRepository) this.Repository).GetMobileDeviceTypes();

        }

        /// <summary>
        /// Gets all <see cref="MobileDevice">MobileDevices</see> in the system.
        /// </summary>
        /// <returns>A GetMobileDevicesResponse containing all <see cref="MobileDevice">MobileDevices</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetMobileDevicesResponse GetAll()
        {
            return this.GetAll<GetMobileDevicesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="MobileDevice">MobileDevice</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A MobileDeviceResponse, containing the <see cref="MobileDevice">MobileDevice</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public MobileDeviceResponse Get([FromUri] int id)
        {
            return this.Get<MobileDeviceResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="MobileDevice">MobileDevice</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>
        /// Device Name<br/>Device Type<br/>Employee ID<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetMobileDevicesRolesResponse containing matching <see cref="MobileDevice">MobileDevices</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetMobileDevicesResponse Find([FromUri] FindMobileDevicesRequest criteria)
        {
            var response = this.InitialiseResponse<GetMobileDevicesResponse>();
            var conditions = new List<Expression<Func<MobileDevice, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
            }

            if (!string.IsNullOrWhiteSpace(criteria.DeviceName))
            {
                conditions.Add(b => b.DeviceName.ToLower().Contains(criteria.DeviceName.ToLower()));
            }

            if (criteria.EmployeeId != null)
            {
                conditions.Add(b => b.EmployeeId == criteria.EmployeeId);
            }

            if (criteria.Type != null)
            {
                conditions.Add(b => b.Type == criteria.Type);
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="MobileDevice">MobileDevice</see>.
        /// </summary>
        /// <param name="request">The <see cref="MobileDevice">MobileDevice</see> to add.
        /// When adding a new <see cref="MobileDevice">MobileDevice</see> through the API, the following properties are required: 
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// EmployeeId: The Id of the Employee to link as the budget holder.
        /// </param>
        /// <returns>A MobileDeviceResponse containing the added <see cref="MobileDevice">MobileDevice</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public MobileDeviceResponse Post([FromBody] MobileDevice request)
        {
            return this.Post<MobileDeviceResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="MobileDevice">MobileDevice</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>A MobileDeviceResponse containing the edited <see cref="MobileDevice">MobileDevice</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public MobileDeviceResponse Put([FromUri] int id, [FromBody] MobileDevice request)
        {
            request.Id = id;
            return this.Put<MobileDeviceResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="MobileDevice">MobileDevice</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="MobileDevice">MobileDevice</see> to be deleted.</param>
        /// <returns>A MobileDeviceResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Delete)]
        public MobileDeviceResponse Delete(int id)
        {
            return this.Delete<MobileDeviceResponse>(id);
        }

        #endregion Api Methods
    }
}
