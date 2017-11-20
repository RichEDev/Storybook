namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="Tooltip">Tooltips</see>.
    /// </summary>
    [RoutePrefix("Tooltips")]
    [Version(1)]
    public class TooltipsV1Controller : BaseApiController<Tooltip>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Tooltip">Tooltip</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Tooltip">Tooltips</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.Tooltips, AccessRoleType.View)]
        public TooltipResponse GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a specific <see cref="Tooltip">Tooltip</see> in the system.
        /// </summary>
        /// <param name="id">A <see cref="Guid">Guid</see> value representing the identifier of the <see cref="Tooltip">Tooltip</see> you want to retrieve.</param>
        [HttpGet, Route("{id:Guid}")]
        [AuthAudit(SpendManagementElement.Tooltips, AccessRoleType.View)]
        public TooltipResponse Get([FromUri, Required] Guid id)
        {
            var response = this.InitialiseResponse<TooltipResponse>();
            response.Item = ((TooltipRepository)this.Repository).Get(id);
            return response;
        }
    }
}
