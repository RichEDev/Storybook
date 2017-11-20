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
    [RoutePrefix("CustomEntityForms")]
    [Version(1)]
    public class CustomEntityFormsV1Controller : BaseApiController<CustomEntityForm>
    {
        /// <summary>
        /// Gets all <see cref="CustomEntityView"/> for a particular menu
        /// </summary>
        /// <param name="id">The identifier of the form</param>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <returns>A list of <see cref="CustomEntityView"/></returns>
        [HttpGet, Route("{id:int}/Get/{entityId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityFormsResponse GetById([FromUri] int id, [FromUri] int entityId)
        {
            var response = this.InitialiseResponse<CustomEntityFormsResponse>();
            response.Item = ((CustomEntityFormRepository)this.Repository).Get(id, entityId, this.CurrentUser);

            return response;
        }
    }
}