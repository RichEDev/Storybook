namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;

    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using Models.Types.Employees;

    using Repositories;

    using SpendManagementApi.Models;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    using Spend_Management;

    using global::Utilities;

    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages operations on <see cref="Vehicle">Vehicles</see>.
    /// </summary>
    [RoutePrefix("Vehicles")]
    [Version(1)]
    public class VehiclesV1Controller : BaseApiController<Vehicle>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Vehicle">Vehicles</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all Pool cars for this account.<br/>
        /// See <see cref="Employee">Employee</see> to get Employee-owned cars.
        /// </summary>
        /// <returns>A GetVehiclesResponse containing all the <see cref="Vehicle">Vehicles</see> that are pool cars.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.PoolCars, AccessRoleType.View)]
        public GetVehiclesResponse GetAll()
        {
            return this.GetAll<GetVehiclesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="Vehicle">Vehicle</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Vehicle">Vehicle</see> to get.</param>
        /// <returns>A VehiclesResponse containing the matching <see cref="Vehicle">Vehicle</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.PoolCars, AccessRoleType.View)]
        public VehicleResponse Get([FromUri] int id)
        {
            var response = this.Get<VehicleResponse>(id);
            this.CheckMultipleAccessRoles(response.Item, AccessRoleType.View);
            return response;
        }

        /// <summary>
        /// Finds all <see cref="Vehicle">Vehicles</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>
        /// Make, Model, Registration, Active, Approved, StartsBefore, StartsAfter, EndsBefore, EndsAfter, OdometerReadingOver, OdometerReadingUnder. <br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetVehiclesResponse containing <see cref="Vehicle">Vehicles</see> matching specified criteria</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.PoolCars, AccessRoleType.View)]
        public GetVehiclesResponse Find([FromUri] FindVehiclesRequest criteria)
        {
            var response = this.InitialiseResponse<GetVehiclesResponse>();
            var conditions = new List<Expression<Func<Vehicle, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Make))
            {
                conditions.Add(c => c.Make.ToLower().Contains(criteria.Make.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Model))
            {
                conditions.Add(c => c.Model.ToLower().Contains(criteria.Model.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Registration))
            {
                conditions.Add(c => c.Registration.ToLower().Contains(criteria.Registration.Trim().ToLower()));
            }

            if (criteria.Active.HasValue)
            {
                conditions.Add(c => c.IsActive == criteria.Active);
            }

            if (criteria.Approved.HasValue)
            {
                conditions.Add(c => c.Approved == criteria.Approved);
            }

            if (criteria.StartsBeforeOrOn != null)
            {
                conditions.Add(c => c.CarUsageStartDate <= criteria.StartsBeforeOrOn);
            }

            if (criteria.StartsAfter != null)
            {
                conditions.Add(c => c.CarUsageStartDate > criteria.StartsAfter);
            }

            if (criteria.EndsBeforeOrOn != null)
            {
                conditions.Add(c => c.CarUsageEndDate <= criteria.EndsBeforeOrOn);
            }

            if (criteria.EndsAfter != null)
            {
                conditions.Add(c => c.CarUsageEndDate > criteria.EndsAfter);
            }

            if (criteria.OdometerReadingOver != null)
            {
                conditions.Add(c => (c.OdometerReadings ?? new OdometerReadings()).EndOdometerReading > criteria.OdometerReadingOver);
            }

            if (criteria.OdometerReadingUnder != null)
            {
                conditions.Add(c => (c.OdometerReadings ?? new OdometerReadings()).EndOdometerReading < criteria.OdometerReadingUnder);
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// This endpoint helps to build the add vehicle form by returning the data required to popualte the form's controls
        /// </summary>
        /// <returns>A <see cref="VehicleDefinitionResponse">VehicleDefinitionResponse</see></returns>
        [HttpGet, Route("GetVehicleDefinition")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public VehicleDefinitionResponse GetVehicleDefinition()
        {
            var response = this.InitialiseResponse<VehicleDefinitionResponse>();
            response.Item = ((VehicleRepository)this.Repository).GetVehicleDefinition();    
            return response;
        }

        /// <summary>
        /// Adds a <see cref="Vehicle">Vehicle</see>.
        /// EmployeeId must be set to 0 for poolcars and to a valid employee id for an employee-owned car. <br/>To add or remove an employee - poolcar association, please use the relevant link and un-link patches<br/>
        /// </summary>
        /// <param name="request">The <see cref="Vehicle">Vehicle</see> to add. <br/>
        /// When adding a new Vehicle through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.<br/>
        /// Make: The Vehicle must have a make / manufacturer.<br/>
        /// Model: The Vehicle must have a model / type.<br/>
        /// Registration: Every Vehicle must have a registration.<br/>
        /// Engine Size: The engine size, in cubic centimetre (cc) must be provided.<br/>
        /// EmployeeId: Must be set to 0 for poolcars and to a non-zero number for an employee-owned car. <br/> 
        /// </param>
        /// <returns>A VehiclesResponse containing the added <see cref="Vehicle">Vehicle</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public VehicleResponse Post([FromBody] Vehicle request)
        {
            return this.Post<VehicleResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="Vehicle">Vehicle</see>.
        /// EmployeeId must be set to 0 for poolcars and to a valid employee id for an employee-owned car. <br/>To add or remove an employee - poolcar association, please use the relevant link and un-link patches<br/>
        /// </summary>
        /// <param name="id">The Id of the <see cref="Vehicle">Vehicle</see> to edit.</param>
        /// <param name="request">The <see cref="Vehicle">Vehicle</see> to edit.</param>
        /// <returns>A VehiclesResponse containing the edited <see cref="Vehicle">Vehicle</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public VehicleResponse Put([FromUri] int id, [FromBody] Vehicle request)
        {
            //Determine whether existing entity is a pool car or company car and then check user's access role
            var existing = this.Get(id);
            this.CheckMultipleAccessRoles(existing.Item, AccessRoleType.Edit);

            request.Id = id;
            return this.Put<VehicleResponse>(request);
        }

        /// <summary>
        /// Links an <see cref="Employee">Employee</see> to a <see cref="Vehicle">Vehicle</see>. That is to say that the user is allowed to use that pool car.
        /// </summary>
        /// <param name="id">The <see cref="Vehicle">Vehicle</see> Id</param>
        /// <param name="eid">The Id of the Employee</param>
        /// <returns>An EmployeeLinkageResponse containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/LinkPoolCarUser/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse LinkPoolCarUser(int id, int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((VehicleRepository)this.Repository).LinkUser(id, eid);
            response.LinkedItemId = id;
            response.EmployeeId = eid;
            return response;
        }

        /// <summary>
        /// Un-Links an <see cref="Employee">Employee</see> from a <see cref="Vehicle">Vehicle</see>. That is to say that the user is not allowed to use that pool car.
        /// </summary>
        /// <param name="id">The <see cref="Vehicle">Vehicle</see> Id.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employees</see>.</param>
        /// <returns>An EmployeeLinkageResponse containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/UnLinkPoolCarUser/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse UnLinkPoolCarUser(int id, int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((VehicleRepository)this.Repository).UnLinkUser(id, eid);
            response.LinkedItemId = id;
            response.EmployeeId = eid;
            return response;
        }

        /// <summary>
        /// Un-Links ALL <see cref="Employee">Employees</see> from a <see cref="Vehicle">Vehicle</see>. That is to say that the pool car will then have no users.
        /// </summary>
        /// <param name="id">The <see cref="Vehicle">Vehicle</see> Id.</param>
        /// <returns>A VehiclesResponse containing the edited <see cref="Vehicle">Vehicle</see>.</returns>
        [HttpPatch, Route("{id:int}/UnLinkAllPoolCarUsers")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public VehicleResponse UnLinkAllUsers(int id)
        {
            var response = this.InitialiseResponse<VehicleResponse>();
            response.Item = ((VehicleRepository)this.Repository).UnLinkAllUsers(id);
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="Vehicle">Vehicle</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Vehicle">Vehicle</see> to be deleted.</param>
        /// <returns>A VehicleResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Delete)]
        public VehicleResponse Delete(int id)
        {
            var response = this.Get(id);
            this.CheckMultipleAccessRoles(response.Item, AccessRoleType.View);
            return this.Delete<VehicleResponse>(id);
        }

        /// <summary>
        /// Gets the list of the user's <see cref="VehicleBasic">Vehicles</see>.
        /// </summary>
        /// <returns>
        /// A GetVehiclesResponse containing all the <see cref="VehicleBasic">Vehicles</see>.
        /// </returns>
        [HttpGet, Route("GetMyVehicles")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public VehicleBasicResponse GetMyVehicles()
        {
            var response = this.InitialiseResponse<VehicleBasicResponse>();
            response.List = ((VehicleRepository)this.Repository).GetMyVehicles();
            return response;
        }

        /// <summary>
        /// Gets the list of <see cref="VehicleBasic">Vehicles by supplied employee id for approver</see>.
        /// </summary>
        /// <param name="request">
        /// The <see cref="GetClaimantsVehiclesRequest">GetClaimantsVehiclesRequest</see>
        /// </param>
        /// <returns>
        /// A GetVehiclesResponse containing all the <see cref="VehicleBasic">Vehicles</see>.
        /// </returns>
        [HttpPut, Route("GetVehiclesByEmployee"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public VehicleBasicResponse GetVehiclesByEmployee(GetClaimantsVehiclesRequest request)
        {
            var response = this.InitialiseResponse<VehicleBasicResponse>();
            response.List = ((VehicleRepository)this.Repository).GetMyVehiclesForEmployee(request.ExpenseId, request.ExpenseDate, request.SubCatId, request.EmployeeId);
            return response;
        }

        /// <summary>
        /// Gets the list of the user's <see cref="VehicleBasic">Vehicles for the add/edit expense screen</see>.
        /// </summary>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <param name="subcatId">
        /// The subcat Id.
        /// </param>
        /// <returns>
        /// A GetVehiclesResponse containing all the <see cref="VehicleBasic">Vehicles</see>.
        /// </returns>
        [HttpGet, Route("GetMyVehiclesForAddEditExpense")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public VehicleBasicResponse GetMyVehiclesForAddEditExpense(int claimId, DateTime expenseDate,int subcatId)
        {
            var response = this.InitialiseResponse<VehicleBasicResponse>();
            response.List = ((VehicleRepository)this.Repository).GetMyVehiclesForAddEditExpense(claimId, expenseDate, subcatId);
            return response;
        }

        /// <summary>
        /// Gets a count of unapproved vehicles for the current user
        /// </summary>   
        /// <returns>
        /// A <see cref="NumericResponse">NumericResponse</see> with a count of unapproved vehicles.
        /// </returns>
        [HttpGet, Route("GetUnapprovedVehiclesCount")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public NumericResponse GetUnapprovedVehicleCount()
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((VehicleRepository)this.Repository).GetUnapprovedVehiclesCount();
            return response;

        }

        /// <summary>
        /// Gets a count of unapproved vehicles for a claimant, when called as the claimant's approver.
        /// </summary>
        /// <param name="request">
        /// The <see cref="GetClaimantsRequest">GetClaimantsRequest</see>
        /// </param>
        /// <returns>
        /// A <see cref="NumericResponse">NumericResponse</see> with a count of unapproved vehicles for the claimant.
        /// </returns>
        [HttpPut, Route("GetUnapprovedVehicleCountForClaimant"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public NumericResponse GetUnapprovedVehicleCountForClaimant(GetClaimantsRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((VehicleRepository)this.Repository).GetUnapprovedVehiclesCountForClaimant(request.EmployeeId, request.ExpenseId);
            return response;
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
                    throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.ApiErrorEpenseItemApprovalPermission);
                }
            }
            else
            {
                var subAccounts = new cAccountSubAccounts(CurrentUser.AccountID);
                cAccountProperties subAccountProperties = subAccounts.getSubAccountById(CurrentUser.CurrentSubAccountId).SubAccountProperties;

                if (this.CurrentUser.EmployeeID == item.EmployeeId && subAccountProperties.AllowUsersToAddCars)
                {
                    return;
                }

                if (!this.CurrentUser.CheckAccessRoleApi(SpendManagementElement.Employees, typeForVehicle, this.MobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api))
                {
                    throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.ApiErrorEpenseItemApprovalPermission);
                }
            }
        }
    }
}
