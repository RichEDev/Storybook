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

    /// <summary>
    /// Manages operations on <see cref="CostCode">CostCodes</see>.
    /// </summary>
    [RoutePrefix("CostCodes")]
    [Version(1)]
    public class CostCodesV1Controller : ArchivingApiController<CostCode>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="CostCode">CostCode</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="CostCode">CostCodes</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetCostCodesResponse GetAll()
        {
            return this.GetAll<GetCostCodesResponse>();
        }

        /// <summary>
        /// Gets all active costs codes
        /// </summary>
        /// <returns>
        /// The <see cref="CostCodeBasicResponse">CostCodeBasicResponse"</see>
        /// </returns>
        [HttpGet, Route("GetAllActive")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CostCodeBasicResponse GetAllActive()
        {
            var response = this.InitialiseResponse<CostCodeBasicResponse>();
            response.List = ((CostCodeRepository)this.Repository).GetAllActive();
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="CostCode">CostCode</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A CostCodesResponse, containing the <see cref="CostCode">CostCode</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.View)]
        public CostCodeResponse Get([FromUri] int id)
        {
            return this.Get<CostCodeResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="CostCode">CostCodes</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetCostCodesResponse containing matching <see cref="CostCode">CostCodes</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.View)]
        public GetCostCodesResponse Find([FromUri] FindCostCodesRequest criteria)
        {
            var response = this.InitialiseResponse<GetCostCodesResponse>();
            var conditions = new List<Expression<Func<CostCode, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException();
            }

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(b => b.Label.ToLower().Contains(criteria.Label.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Description))
            {
                conditions.Add(b => b.Description.ToLower().Contains(criteria.Description.ToLower()));
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="CostCode">CostCode</see>.
        /// </summary>
        /// <param name="request">The <see cref="CostCode">CostCode</see> to add. <br/>
        /// When adding a new <see cref="CostCode">CostCode</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A CostCodeResponse containing the added <see cref="CostCode">CostCode</see></returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.Add)]
        public CostCodeResponse Post([FromBody] CostCode request)
        {
            return this.Post<CostCodeResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="CostCode">CostCode</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>A CostCodeResponse containing the edited <see cref="CostCode">CostCode</see></returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.Edit)]
        public CostCodeResponse Put([FromUri] int id, [FromBody] CostCode request)
        {
            request.Id = id;
            return this.Put<CostCodeResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="CostCode">CostCode</see>, depeding on what is passed in.
        /// </summary>
        /// <param name="id">The id of the CostCode to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="CostCode">CostCode</see>.</param>
        /// <returns>A CostCodeResponse containing the freshly archived Item.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.Edit)]
        public CostCodeResponse Archive(int id, bool archive)
        {
            return this.Archive<CostCodeResponse>(id, archive);
        }

        /// <summary>
        /// Deletes a <see cref="CostCode">CostCode</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="CostCode">CostCode</see> to be deleted</param>
        /// <returns>A CostCodeResponse with the item set to null upon a successful delete.</returns>
        [HttpDelete, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.CostCodes, AccessRoleType.Delete)]
        public CostCodeResponse Delete(int id)
        {
            return this.Delete<CostCodeResponse>(id);
        }

        #endregion Api Methods
    }
}
