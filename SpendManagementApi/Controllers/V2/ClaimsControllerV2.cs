namespace SpendManagementApi.Controllers.V2
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Attributes;
    using Models.Common;
    using Models.Responses;
    using Models.Types;
    using Repositories;

    /// <summary>
    /// Manages operations on <see cref="Claim">Claims</see>.
    /// </summary>
    [RoutePrefix("Claims")]
    [Version(2)]
    public class ClaimsV2Controller : BaseApiController<Claim>
    {
        //BaseApiController<Claim>

        #region Api Methods

        /// <summary>
        /// Gets ALL of the available end points from the API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        [Route("~/Options")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Gets the list of claims awaiting approval by the employee, including assigned and unassigned
        /// </summary>
        /// <returns>A list of <see cref="ClaimBasicResponse">ClaimBasicResponse</see></returns>
        [HttpGet]
        [AuthAudit(SpendManagementElement.CheckAndPay, AccessRoleType.View)]
        [Route("AwaitingApproval")]
        public ClaimBasicResponse AwaitingApproval()
        {
            var response = this.InitialiseResponse<ClaimBasicResponse>();
            response.List = ((ClaimRepository)this.Repository).GetClaimsWaitingApprovalAssignedAndUnassigned(
                this.CurrentUser.AccountID,
                this.CurrentUser.EmployeeID);
            return response;
        }

        #endregion
    }
}