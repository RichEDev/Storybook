
namespace SpendManagementApi.Controllers.Expedite.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;


    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Repositories.Expedite;
    using SME = Spend_Management.Expedite;

    using Spend_Management;
    using System.Web.Http.Description;

    /// <summary>
    /// Contains Expedite Clinet's Fund specific actions.
    /// </summary>
    [RoutePrefix("Expedite/Fund")]
    [Version(1)]
    [InternalSelenityMethod, ApiExplorerSettings(/*IgnoreApi = true*/)]
    public class FundManagerV1Controller : BaseApiController<FundManager>
    {
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
        
        #region Fund Methods
        /// <summary>
        /// Gets the data for a fund and returns the fund with its data property populated.
        /// </summary>
        /// <param name="id">The Id of expedite client.</param>
        /// <returns>Available Fund for the expedite client</returns>
        [HttpGet, Route("~/Expedite/Fund/{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public FundManagerResponse Get([FromUri] int id)
        {
            return this.Get<FundManagerResponse>(id);
        }

        /// <summary>
        /// Gets the fund limit for the expedite customer.
        /// </summary>
        /// <param name="id">The Id of expedite client.</param>
        /// <returns>Fund limit for the expedite client</returns>
        [HttpGet, Route("~/Expedite/FundLimit/{id:int}")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public FundManagerResponse GetFundLimit([FromUri] int id)
        {
            var response = this.InitialiseResponse<FundManagerResponse>();
            response.Item = ((FundRepository)this.Repository).GetFund(id);
            return response;
        }

        /// <summary>
        /// Update the fund limit of the expedit client.
        /// </summary>
        /// <param name="fundInfo">Fund information</param>
        /// <returns>New fund limit</returns>
        [HttpPost, Route("~/Expedite/UpdateFundLimit")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public FundManagerResponse UpdateFundLimit([FromBody] FundManager fundInfo)
        {
            var response = this.InitialiseResponse<FundManagerResponse>();
            response.Item = ((FundRepository)this.Repository).UpdateFundLimit(fundInfo.AccountId, fundInfo.FundLimit);
            return response;
        }

        /// <summary>
        /// Adds the supplied <see cref="Models.Types.Expedite.Fund">Fund</see>.    
        /// </summary>
        /// <param name="request">The Fund transaction to add.</param>
        /// <returns>The newly added Transaction.</returns>
        [Route("~/Expedite/Fund")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public FundManagerResponse Post([FromBody] FundManager request)
        {
                
            var response = this.InitialiseResponse<FundManagerResponse>();
            response.Item = ((FundRepository)this.Repository).Add(request);
            return response;
        }

        /// <summary>
        /// Send email to expedite customers whose available fund crosses fund limit.
        /// </summary>
        /// <returns>email sending response</returns>
        [HttpGet, Route("~/Expedite/Funds/NotifyAdministratorsOfFloatBelowLimit")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public EmailSenderResponse NotifyAdministratorsOfFloatBelowLimit()
        {
            var response = this.InitialiseResponse<EmailSenderResponse>();
            try
            {
                response.isSendingSuccessful = new SME.ExpediteEmail().NotifyAdministratorsOfFloatBelowLimit();
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message + "Error details" + ex.InnerException;
            }
            return response;
        }
        #endregion
       
    }
}
