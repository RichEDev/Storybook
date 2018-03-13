namespace SpendManagementApi.Controllers.V1
{
    using System;

    using System.Web.Http;
    using System.Web.Http.Description;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// <see cref="CustomEntity"/>
    /// </summary>
    //[ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("CustomEntities")]
    [Version(1)]
    public class CustomEntitiesV1Controller : BaseApiController<CustomEntity>
    {
        /// <summary>
        /// Get the definition of an exising Custom Entity by the system ID
        /// </summary>
        /// <param name="id">The system ID of the Custom Entity</param>
        /// <returns>An instance of <see cref="CustomEntityReponse"/> </returns>
        [HttpGet, Route("{id}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityResponse GetByGuid([FromUri]Guid id)
        {
            var response = this.InitialiseResponse<CustomEntityResponse>();
            response.Item = ((CustomEntityRepository) this.Repository).Get(id);

            return response;
        }
    }
}