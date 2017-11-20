namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Contains a few utility methods for getting  <see cref="UserDefinedField">UserDefinedFields</see>.
    /// </summary>
    [RoutePrefix("UserDefinedFields")]
    [Version(1)]
    public class UserDefinedFieldsV1Controller : BaseApiController<UserDefinedField>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="UserDefinedField">UserDefinedFields</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Gets all the  <see cref="UserDefinedField">UserDefinedFields</see>
        /// </summary>
        /// <returns>A List of  <see cref="UserDefinedField">UserDefinedFields</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.UserDefinedFields, AccessRoleType.View)]
        public GetUserDefinedFieldsResponse GetAll()
        {
            return this.GetAll<GetUserDefinedFieldsResponse>();
        }

        /// <summary>
        /// Gets the <see cref="UserDefinedField">UserDefinedField</see> for the specified id
        /// </summary>
        /// <param name="id">User Defined Field Id</param>
        /// <returns>A UserDefinedFieldResponse object containing the matching <see cref="UserDefinedField">UserDefinedField</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.UserDefinedFields, AccessRoleType.View)]
        public UserDefinedFieldResponse Get([FromUri] int id)
        {
            return this.Get<UserDefinedFieldResponse>(id);
        }

        /// <summary>
        /// Saves a <see cref="UserDefinedFieldRequest">UserDefinedFieldRequest</see>
        /// </summary>
        /// <param name="request">
        /// The <see cref="UserDefinedFieldRequest">UserDefinedFieldRequest</see>
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedFieldResponse">UserDefinedFieldResponse</see>
        /// </returns>
        [Route("SaveUserDefinedField")]
        [HttpPost]
        [AuthAudit(SpendManagementElement.UserDefinedFields, AccessRoleType.Add)]
        public UserDefinedFieldResponse SaveUserDefinedField([FromBody] UserDefinedFieldRequest request)
        {
            var response = this.InitialiseResponse<UserDefinedFieldResponse>();
            response.Item = ((UserDefinedFieldsRepository)this.Repository).SaveUserDefinedField(request);
            return response;
        }

        /// <summary>
        /// Deleted the specified <see cref="UserDefinedField">UserDefinedField</see> by its Id
        /// </summary>
        /// <param name="id">The Id of the <see cref="UserDefinedField">UserDefinedField</see> to delete.</param>
        /// <returns>A UserDefinedFieldResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.UserDefinedFields, AccessRoleType.Delete)]
        public UserDefinedFieldResponse Delete(int id)
        {
            return this.Delete<UserDefinedFieldResponse>(id);
        }
    }

}
