﻿namespace SpendManagementApi.Controllers.V1
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="CustomEntityRecord">Custom Entity Records</see>.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("CustomEntityRecords")]
    [Version(1)]
    public class CustomEntityRecordsV1Controller : BaseApiController<CustomEntityRecord>
    {
        /// <summary>
        /// Gets a single <see cref="CustomEntityRecord"/> for a given entity, form and record identifier
        /// </summary>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <param name="formId">The identifier of the custom entity form</param>
        /// <param name="id">The identifier of the record</param>
        /// <returns>A list of <see cref="CustomEntityRecord"/></returns>
        [HttpGet, Route("{entityId:int}/{formId:int}/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityRecordResponse GetById([FromUri] int entityId, [FromUri] int formId, [FromUri] int id)
        {
            var response = this.InitialiseResponse<CustomEntityRecordResponse>();
            response.Item = ((CustomEntityRecordRepository)this.Repository).Get(id, formId, entityId);

            return response;
        }

        /// <summary>
        /// Gets a list of <see cref="CustomEntityRecord"/> for a given entity and view identifier
        /// </summary>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <param name="id">The identifier of the record</param>
        /// <returns>A list of <see cref="CustomEntityRecord"/></returns>
        [HttpGet, Route("ByView/{entityId:int}/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CustomEntityRecordsResponse GetAllByViewId([FromUri] int entityId, [FromUri] int id)
        {
            var response = this.InitialiseResponse<CustomEntityRecordsResponse>();
            response.List = ((CustomEntityRecordRepository)this.Repository).GetAll(entityId, id);

            return response;
        }

        /// <summary>
        /// Gets an individual <see cref="Attachment" /> by its Id and related Entity Id
        /// </summary>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <param name="attachmentId">The identifier of the attachment</param>
        /// <returns>An <see cref="Attachment"/></returns>
        [HttpGet, Route("Attachment/{entityId:int}/{attachmentId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AttachmentResponse Attachment([FromUri] int entityId, [FromUri] int attachmentId)
        {
            var response = this.InitialiseResponse<AttachmentResponse>();
            response.Item = ((CustomEntityRecordRepository)this.Repository).GetAttachment(entityId, attachmentId);

            if (response.Item == null)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Attachment not found."));
            }

            return response;
        }
    }
}