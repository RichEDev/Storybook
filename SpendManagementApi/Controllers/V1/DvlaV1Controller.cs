namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Spend_Management.shared.code.DVLA;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Responses;
    using SpendManagementLibrary;
    using Spend_Management;

    /// <summary>
    ///  Contains Consent Email Reminder Specific Actions.
    /// </summary>
    [RoutePrefix("Dvla")]
    [Version(1)]
    public class DvlaV1Controller : BaseApiController
    {
        /// <summary>
        /// Send email to claimants on expiry of Consent.
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>email sending response</returns>
        [HttpGet, Route("NotifyOnExpiryOfConsent/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DvlaResponse NotifyOnExpiryOfConsent(int accountId)
        {
            var response = this.InitialiseResponse<DvlaResponse>();
            var subAccounts = new cAccountSubAccounts(accountId);
            var subAccount = subAccounts.getFirstSubAccount();
            var accounts = new cAccounts();
            var account = accounts.GetAccountByID(accountId);
            try
            {
                response.ResponseMessage = new DvlaConsentLookUp().NotifyUserOnExpiryOfConsent(account,subAccount);
            } 
            catch (Exception ex)
            {
                response.ResponseMessage = ex.Message + "Error details" + ex.InnerException;
            }

            return response;
        }

        /// <summary>
        /// Method to be implemented if repository initialization is done.
        /// </summary>
        protected override void Init()
        {
        }
    }
}
