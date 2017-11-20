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
    /// Manages operations on <see cref="CustomEntityAttribute">Custom Entity Attributes</see>.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("CustomEntityAttributes")]
    [Version(1)]
    public class CustomEntityAttributesV1Controller : BaseApiController<CustomEntityAttribute>
    {
        /// <summary>
        /// Get custom entity attributes by a view id.
        /// </summary>
        /// <param name="entityId">
        /// The entity Id.
        /// </param>
        /// <param name="viewId">
        /// The view id.
        /// </param>
        /// <returns>
        /// The <see cref="CustomEntityAttributesResponse"/>.
        /// </returns>
        [HttpGet, Route("ByView/{entityId:int}/{viewId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityAttributesResponse GetAttributesByViewId([FromUri] int entityId, [FromUri] int viewId)
        {
            var response = this.InitialiseResponse<CustomEntityAttributesResponse>();
            response.List = ((CustomEntityAttributeRepository)this.Repository).GetAll(entityId, viewId, this.MobileRequest);

            return response;
        }
    }
}