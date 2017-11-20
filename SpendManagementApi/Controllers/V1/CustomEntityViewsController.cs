namespace SpendManagementApi.Controllers.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="CustomEntityView">Custom Entity Views</see>.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("CustomEntityViews")]
    [Version(1)]
    public class CustomEntityViewsV1Controller : ArchivingApiController<CustomEntityView>
    {
        /// <summary>
        /// Gets all <see cref="CustomEntityView"/> for a particular menu
        /// </summary>
        /// <param name="id">The menu identifier</param>
        /// <returns>A list of <see cref="CustomEntityView"/></returns>
        [HttpGet, Route("ByMenuId/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityViewsResponse GetByMenuId([FromUri] int id)
        {
            var response = this.InitialiseResponse<CustomEntityViewsResponse>();
            response.List = ((CustomEntityViewRepository)this.Repository).GetByMenuId(id, this.MobileRequest);

            return response;
        }
    }
}