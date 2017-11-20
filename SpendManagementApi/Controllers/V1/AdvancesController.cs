namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;
    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using Models;

    /// <summary>
    /// The advances controller.
    /// </summary>  
    [RoutePrefix("Advances")]
    [Version(1)]
    public class AdvancesV1Controller : BaseApiController<Advance>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Advance">Advance</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [System.Web.Http.HttpOptions]
        [System.Web.Http.Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Gets the available <see cref="Advance">Advances</see> for the currency supplied and current employee.
        /// </summary>
        /// <param name="currencyId">
        /// The currency Id
        /// </param>
        /// <returns>
        /// The <see cref="AdvancesResponse">AdvancesResponse</see>
        /// </returns>
        [HttpGet, Route("GetAdvancesForCurrency")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvancesResponse GetAvailableAdvances([FromUri] int currencyId)
        {
            var response = this.InitialiseResponse<AdvancesResponse>();
            response.List = ((AdvancesRepository)Repository).GetAvailableAdvances(currencyId);
            return response;
        }

        /// <summary>
        /// Gets the available <see cref="Advance">Advances</see> for the currency supplied and current employee.
        /// </summary>
        /// <returns>
        /// The <see cref="AdvancesResponse">AdvancesResponse</see>
        /// </returns>
        [HttpGet, Route("GetAdvancesForUser")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvancesResponse GetAvailableAdvancesForUser()
        {
            var response = this.InitialiseResponse<AdvancesResponse>();
            response.List = ((AdvancesRepository)Repository).GetAvailableAdvancesForUser();
            return response;
        }

        /// <summary>
        /// Gets the available <see cref="MyAdvanceResponse">MyAdvanceResponse</see> containing a list of <see cref="MyAdvance">MyAdvance</see> for the current user.
        /// </summary>
        /// <returns>
        /// The <see cref="AdvancesResponse">AdvancesResponse</see>
        /// </returns>
        [HttpGet, Route("GetUnsettledAdvancesForCurrentUser")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MyAdvanceResponse GetUnsettledAdvancesForCurrentUser()
        {
            var response = this.InitialiseResponse<MyAdvanceResponse>();
            response.List = ((AdvancesRepository)Repository).GetUnsettledAdvancesForCurrentUser();
            return response;
        }
        
        /// <summary>
        /// Gets the available <see cref="Advance">Advances</see> for a claimant, when called as the claimant's approver.
        /// </summary>
        /// <param name="request">GetClaimantsAdvancesRequest</param>
        /// <returns>
        /// The <see cref="AdvancesResponse">AdvancesResponse</see>
        /// </returns>
        [HttpPut, Route("GetAdvancesByEmployee"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvancesResponse GetAdvancesByEmployee(GetClaimantsRequest request)
        {
            var response = this.InitialiseResponse<AdvancesResponse>();
            response.List = ((AdvancesRepository) this.Repository).GetAvailableAdvancesForClaimant(request.ExpenseId, request.EmployeeId);
            return response;
        }

        /// <summary>
        /// Gets the available <see cref="Advance">Advances</see> for the currency supplied and current employee.
        /// </summary>
        /// <returns>
        /// The <see cref="AdvancesResponse">AdvancesResponse</see>
        /// </returns>
        [HttpPost, Route("RequestAdvance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvanceResponse RequestAdvance([FromBody] AdvanceRequest request)
        {
            var response = this.InitialiseResponse<AdvanceResponse>();
            response.Item = ((AdvancesRepository)Repository).RequestAdvance(request.AdvanceName, request.AdvanceReason, request.Amount, request.CurrencyId, request.RequiredByDate);
            return response;
        }

        /// <summary>
        /// Updates the existing <see cref="Advance">Advances</see> for the currency supplied and current employee.
        /// </summary>
        /// <param name="request">Request Payload for update advance</param>
        /// <returns>
        /// The <see cref="AdvanceResponse">AdvanceResponse</see>
        /// </returns>
        [HttpPut, Route("UpdateAdvance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvanceResponse UpdateAdvance([FromBody] AdvanceRequest request)
        {
            var response = this.InitialiseResponse<AdvanceResponse>();
            response.Item = ((AdvancesRepository)this.Repository).UpdateAdvance(request.AdvanceId, request.AdvanceName, request.AdvanceReason, request.Amount, request.CurrencyId, request.RequiredByDate);
            return response;
        }

        /// <summary>
        /// The approve advance.
        /// </summary>
        /// <param name="advanceId">
        /// The advance id.
        /// </param>
        /// <returns>
        /// The <see cref="AdvanceResponse">AdvanceResponse</see>
        /// </returns>
        [HttpPut, Route("ApproveAdvance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvanceResponse ApproveAdvance([FromUri] int advanceId)
        {
            var response = this.InitialiseResponse<AdvanceResponse>();
            response.Item = ((AdvancesRepository)Repository).ApproveAdvance(advanceId);
            return response;
        }

        /// <summary>
        /// The pay advance.
        /// </summary>
        /// <param name="advanceId">
        /// The advance id.
        /// </param>
        /// <returns>
        /// The <see cref="AdvanceResponse">AdvanceResponse</see>
        /// </returns>
        [HttpPut, Route("PayAdvance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AdvanceResponse PayAdvance([FromUri] int advanceId)
        {
            var response = this.InitialiseResponse<AdvanceResponse>();
            response.Item = ((AdvancesRepository)Repository).PayAdvance(advanceId);
            return response;
        }

        /// <summary>
        /// The delete advance endponit.
        /// </summary>
        /// <param name="advanceId">
        /// The advance id.
        /// </param>
        /// <returns>
        /// Returns 'Success' if advance gets deleted successfully otherwise error message.
        /// </returns>
        [HttpDelete, Route("DeleteAdvance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public StringResponse DeleteAdvance([FromUri] int advanceId)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((AdvancesRepository)this.Repository).DeleteAdvance(advanceId);
            return response;
        }
    }
}