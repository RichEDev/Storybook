namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using Models.Requests;

    /// <summary>
    /// Manages operations on General Options.
    /// </summary>
    /// <returns>A list of available resource Links</returns>
    [Version(1)]
    [RoutePrefix("GeneralOptions")]
    public class GeneralOptionsV1Controller : BaseApiController<GeneralOption>
    {
        /// <summary>
        /// Gets all of the available end points from the General Options part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="GeneralOption">GeneralOptions</see> for the given subaccount Id.
        /// </summary>
        /// <param name="subAccountId">The sub account identifier.</param>
        /// <returns>A GeneralOptionResponse, containing all <see cref="GeneralOption">GeneralOption</see>s. for the requested sub account.</returns>
        [HttpGet, Route("GetAllBySubAccount")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public GeneralOptionsResponse GetAllBySubAccount([FromUri] int subAccountId)
        {
            var response = InitialiseResponse<GeneralOptionsResponse>();
            response.List = ((GeneralOptionRepository)Repository).GetAllBySubAccount(subAccountId);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="GeneralOption">GeneralOption</see>, by its key and subaccount Id.
        /// </summary>
        /// <param name="key">The key of the item to get.</param>
        /// <param name="subAccountId">The sub account to get the <see cref="GeneralOption">GeneralOption</see> from.</param>
        /// <returns>A GeneralOptionResponse, containing the <see cref="GeneralOption">GeneralOption</see> if found.</returns>
        [HttpGet, Route("GetByKeyAndSubAccount")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public GeneralOptionResponse GetByKeyAndSubAccount([FromUri] string key, [FromUri] int subAccountId)
        {
            var response = InitialiseResponse<GeneralOptionResponse>();
            response.Item = ((GeneralOptionRepository)Repository).GetByKeyAndSubAccount(key.Trim(), subAccountId);
            return response;
        }

        /// <summary>
        /// Updates a the value of a <see cref="GeneralOption">GeneralOption</see>, by its key and subaccount Id.
        /// </summary>
        /// <param name="request">
        /// The <see cref="GeneralOption">GeneralOption</see>
        /// </param>
        /// <returns>
        /// A GeneralOptionResponse, containing the updated <see cref="GeneralOption">GeneralOption</see>.
        /// </returns>
        [HttpPut, Route("")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.Edit)]
        public GeneralOptionResponse UpdateGeneralOption([FromBody] GeneralOption request)
        {
            return this.Put<GeneralOptionResponse>(request);
        }

        /// <summary>
        /// Updates multiple <see cref="GeneralOption">GeneralOptions</see>, by its key and subaccount Id.
        /// </summary>
        /// <param name="request">
        /// The <see cref="UpdateMultipleGeneralOptions">UpdateMultipleGeneralOptions</see>
        /// </param>
        /// <returns>
        /// A GeneralOptionResponse, containing the list of updated <see cref="GeneralOption">GeneralOptions</see>.
        /// </returns>
        [HttpPut, Route("UpdateMultipleGeneralOptions")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.Edit)]
        public GeneralOptionsResponse UpdateMultipleGeneralOption([FromBody] UpdateMultipleGeneralOptions request)
        {
            var response = InitialiseResponse<GeneralOptionsResponse>();
            response.List = ((GeneralOptionRepository)Repository).UpdateMultipleGeneralOptions(request.GeneralOptions);
            return response;
        }

        /// <summary>
        /// Saves the <see cref="GeneralOptionsDisplayFieldSetting"/> that can be enabled/disabled in general options to display specific fields when claiming for an expense
        /// </summary>
        /// <param name="request">
        /// The Field Setting request
        /// </param>
        /// <returns>
        /// Returns a FieldSettingResponse, containing a <see cref="GeneralOptionsDisplayFieldSetting"/>
        /// </returns>
        [HttpPut, Route("SaveDisplayFieldSetting")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.Edit)]
        public GeneralOptionsDisplayFieldSettingResponse SaveDisplayFieldSetting([FromBody] GeneralOptionsDisplayFieldSetting request)
        {
            var response = this.InitialiseResponse<GeneralOptionsDisplayFieldSettingResponse>();
            response.Item = ((GeneralOptionRepository)this.Repository).SaveDisplayFieldSetting(request);
            return response;
        }
    }
}