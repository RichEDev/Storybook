namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Manages operations on <see cref="ItemRole">List</see>.
    /// </summary>
    [RoutePrefix("ItemRoles")]
    [Version(1)]
    public class ItemRolesV1Controller : BaseApiController<ItemRole>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="ItemRole">List</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="ItemRole">List</see> available.
        /// </summary>
        /// <returns>A GetItemRolesResponse containing all the <see cref="ItemRole">List</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.View)]
        public GetItemRolesResponse GetAll()
        {
            return this.GetAll<GetItemRolesResponse>();
        }


        /// <summary>
        /// Gets an <see cref="ItemRole">ItemRole</see> matching the specified id.
        /// </summary>
        /// <param name="id">The id of the <see cref="ItemRole">ItemRole</see></param>
        /// <returns>An ItemRoleResponse containing the matching <see cref="ItemRole">ItemRole</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.View)]
        public ItemRoleResponse Get(int id)
        {
            return this.Get<ItemRoleResponse>(id);
        }

        // POST api/<controller>
        /// <summary>
        /// Adds an <see cref="ItemRole">ItemRole</see> with associated subcategory ids.
        /// </summary>
        /// <param name="request">The <see cref="ItemRole">ItemRole</see>.</param>
        /// <returns>An ItemRoleResponse containing the added <see cref="ItemRole">ItemRole</see> and associated sub categories</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.Add)]
        public ItemRoleResponse Post([FromBody]ItemRole request)
        {
            return this.Post<ItemRoleResponse>(request);
        }

        /// <summary>
        /// Updates <see cref="ItemRole">ItemRole</see> name, description and replaces existing subcategories
        /// </summary>
        /// <param name="id">The Id of the <see cref="ItemRole">ItemRole</see> to edit.</param>
        /// <param name="request">The <see cref="ItemRole">ItemRole</see> to edit.</param>
        /// <returns>An ItemRoleResponse containing the edited <see cref="ItemRole">ItemRole</see> and associated sub categories</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.Edit)]
        public ItemRoleResponse Put([FromUri] int id, [FromBody]ItemRole request)
        {
            request.ItemRoleId = id;
            return this.Put<ItemRoleResponse>(request);
        }


        /// <summary>
        /// Associates Sub Categories with an Item Role
        /// </summary>
        /// <param name="request">A list of <see cref="SubCatItemRole">SubCatItemRoles</see>.</param>
        /// <param name="itemRoleId">The id of the item role to associate the sub categories with.</param>
        /// <returns>The outcome of the action <see cref="NumericResponse">ItemRole</see> where 1 is success and 0 is fail</returns>
        [HttpPut]
        [Route("UpdateItemRoleForSubCats")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.Edit)]
        public NumericResponse UpdateItemRoleForSubCats([FromBody]List<SubCatItemRole> request, [FromUri] int itemRoleId)
        {       
            var response = this.InitialiseResponse<NumericResponse>();
            bool outcome =  ((ItemRoleRepository)this.Repository).UpdateItemRoles(request, itemRoleId);

            response.Item = outcome ? 1 : 0;
            return response;        
        }

       
        /// <summary>
        /// Applies the supplied <see cref="ItemRole">ItemRole</see> to the supplied <see cref="Models.Types.Employees.Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ItemRole">ItemRole</see> to apply.</param>
        /// <param name="eid">The Id of the <see cref="Models.Types.Employees.Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/AssignToEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse AssignToEmployee([FromUri] int id, [FromUri] int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((ItemRoleRepository)this.Repository).UpdateEmployeeItemRole(eid, id, true, this.Repository.User.CurrentSubAccountId);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Assigns the item role with start date and end date to the employee
        /// </summary>
        /// <param name="request"> The <see cref="EmployeeItemRole"/>employee item role</param>
        /// <param name="eid">The id of the employee to whom the item role is to be assigned</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [Route("AssignToEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public EmployeeLinkageResponse AssignToEmployee([FromBody] EmployeeItemRole request, [FromUri] int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((ItemRoleRepository)this.Repository).UpdateEmployeeItemRole(eid, request.ItemRoleId, true, this.Repository.User.CurrentSubAccountId, request.StartDate, request.EndDate);
            response.EmployeeId = eid;
            response.LinkedItemId = request.ItemRoleId;
            return response;
        }

        /// <summary>
        /// Unlinks the supplied <see cref="ItemRole">ItemRole</see> from the supplied <see cref="Models.Types.Employees.Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ItemRole">ItemRole</see> to revoke.</param>
        /// <param name="eid">The Id of the <see cref="Models.Types.Employees.Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/RevokeFromEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse RevokeFromEmployee([FromUri]int id, [FromUri]int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            ((ItemRoleRepository)this.Repository).UpdateEmployeeItemRole(eid, id, false, this.Repository.User.CurrentSubAccountId);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Deletes an <see cref="ItemRole">ItemRole</see> with specified id.
        /// </summary>
        /// <param name="id">The <see cref="ItemRole">ItemRole</see> Id.</param>
        /// <returns>An ItemRoleResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.Delete)]
        public ItemRoleResponse Delete(int id)
        {
            return this.Delete<ItemRoleResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="ItemRole">List</see> matching specified criteria. 
        /// Available querystring parameters : SearchOperator,ItemRoleId,RoleName, ExpenseSubCategory
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetItemRolesResponse containing <see cref="ItemRole">List</see> matching specified criteria</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.ItemRoles, AccessRoleType.View)]
        public GetItemRolesResponse Find([FromUri]FindItemRolesRequest criteria)
        {
            var findItemRolesResponse = this.InitialiseResponse<GetItemRolesResponse>();
            var conditions = new List<Expression<Func<ItemRole, bool>>>();

            if (criteria.ItemRoleId.HasValue)
            {
                conditions.Add(role => role.ItemRoleId == criteria.ItemRoleId);
            }

            if (!string.IsNullOrEmpty(criteria.ItemRoleName))
            {
                conditions.Add(role => role.RoleName.ToLower().Contains(criteria.ItemRoleName.Trim().ToLower()));
            }

            if (criteria.ExpenseSubCategoryId.HasValue)
            {
                conditions.Add(role => role.SubCatItemRoles.Count(subcat => subcat.SubCatId == criteria.ExpenseSubCategoryId) > 0);
            }

            findItemRolesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findItemRolesResponse;
        }
    }

}