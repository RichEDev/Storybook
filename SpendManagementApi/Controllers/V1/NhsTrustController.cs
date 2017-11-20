namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// Manages get operations on NHS Trusts.
    /// </summary>
    [RoutePrefix("NHSTrusts")]
    [Version(1)]
    public class NhsTrustV1Controller : ArchivingApiController<NhsTrust>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="NhsTrust">NhsTrust</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="NhsTrust">NhsTrusts</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.View)]
        public GetNhsTrustsResponse GetAll()
        {
            return this.GetAll<GetNhsTrustsResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="NhsTrust">NhsTrust</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A NhsTrustsResponse, containing the <see cref="NhsTrust">NhsTrust</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.View)]
        public NhsTrustResponse Get([FromUri] int id)
        {
            return this.Get<NhsTrustResponse>(id);
        }






/*      Will need this later.

        /// <summary>
        /// Finds all <see cref="NhsTrust">NhsTrusts</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetNhsTrustsResponse containing matching <see cref="NhsTrust">NhsTrusts</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.View)]
        public GetNhsTrustsResponse Find([FromUri] FindNhsTrustRequest criteria)
        {
            var response = InitialiseResponse<GetNhsTrustsResponse>();
            var conditions = new List<Expression<Func<NhsTrust, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException();
            }

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(b => b.Label.ToLower().Contains(criteria.Label.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.TrustVpd))
            {
                conditions.Add(b => b.TrustVpd.ToLower().Contains(criteria.TrustVpd.ToLower()));
            }


            response.List = RunFindQuery(Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="NhsTrust">NhsTrust</see>.
        /// </summary>
        /// <param name="request">The <see cref="NhsTrust">NhsTrust</see> to add. <br/>
        /// When adding a new <see cref="NhsTrust">NhsTrust</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A NhsTrustResponse containing the added <see cref="NhsTrust">NhsTrust</see></returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.Add)]
        public NhsTrustResponse Post([FromBody] NhsTrust request)
        {
            return Post<NhsTrustResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="NhsTrust">NhsTrust</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>A NhsTrustResponse containing the edited <see cref="NhsTrust">NhsTrust</see></returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.Edit)]
        public NhsTrustResponse Put([FromUri] int id, [FromBody] NhsTrust request)
        {
            request.Id = id;
            return Put<NhsTrustResponse>(request);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="NhsTrust">NhsTrust</see>, depeding on what is passed in.
        /// </summary>
        /// <param name="id">The id of the NhsTrust to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="NhsTrust">NhsTrust</see>.</param>
        /// <returns>A NhsTrustResponse containing the freshly archived Item.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.Edit)]
        public NhsTrustResponse Archive(int id, bool archive)
        {
            return Archive<NhsTrustResponse>(id, archive);
        }

        /// <summary>
        /// Deletes a <see cref="NhsTrust">NhsTrust</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="NhsTrust">NhsTrust</see> to be deleted</param>
        /// <returns>A NhsTrustResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ESRTrustDetails, AccessRoleType.Delete)]
        public NhsTrustResponse Delete(int id)
        {
            return Delete<NhsTrustResponse>(id);
        }
*/

        #endregion Api Methods
    }
}
