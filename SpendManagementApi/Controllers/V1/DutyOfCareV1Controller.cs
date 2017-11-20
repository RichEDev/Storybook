namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models.Requests;
    using Spend_Management.shared.code;

    /// <summary>
    ///  Contains Duty Of Care Email Reminder Specific Actions.
    /// </summary>
    [RoutePrefix("DutyOfCare")]
    [Version(1)]
    public class DutyOfCareV1Controller : BaseApiController<DocumentExpiry>
    {

        #region Duty Of care endpoints

        /// <summary>
        /// Send email to Line Manager on expiry date of the Duty of care documents.
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>
        /// Success or failure information
        /// </returns>
        [HttpGet, Route("NotifyApproverOnExpiryOfDutyOfCareDocument/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DutyOfCareResponse NotifyApproverOnExpiryOfDutyOfCareDocument(int accountId)
        {
            var response = this.InitialiseResponse<DutyOfCareResponse>();
            try
            {
                response.isSendingSuccessful = new DutyOfCare().NotifyApproverOnExpiryOfDOC(accountId);
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message + "Error details" + ex.InnerException;
            }
            return response;
        }

        /// <summary>
        /// Send email to claimants on expiry date of the Duty of care documents.
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>
        /// Success or failure information
        /// </returns>
        [HttpGet, Route("NotifyClaimantOnExpiryOfDutyOfCareDocument/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DutyOfCareResponse NotifyClaimantOnExpiryOfDutyOfCareDocument(int accountId)
        {
            var response = this.InitialiseResponse<DutyOfCareResponse>();
            try
            {
                response.isSendingSuccessful = new DutyOfCare().NotifyClaimantOnExpiryOfDOC(accountId);
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message + "Error details" + ex.InnerException;
            }
            return response;
        }

        /// <summary>
        /// Send email to Claimant on expiry of Driving licence review.
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>
        /// Success or failure information
        /// </returns>
        [HttpGet, Route("NotifyOnExpiryOfDrivingLicenceReview/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DutyOfCareResponse NotifyOnExpiryOfDrivingLicenceReview(int accountId)
        {
            var response = this.InitialiseResponse<DutyOfCareResponse>();
            try
            {
                response.isSendingSuccessful = new DutyOfCare().NotifyClaimantOnExpiryOfDrivingLicence(accountId);
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message + "Error details" + ex.InnerException;
            }

            return response;
        }

        /// <summary>
        /// Gets a list of Ids of Accounts for which Driving licence check and expiry review is enabled.
        /// </summary>
        /// <returns>General option response</returns>
        [HttpGet, Route("GetAccountsWithDrivingLicenceReviewsEnabled")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithAutoDvlaConsentEnabled()
        {
            var response = DutyOfCareRepository.GetAccountsWithDrivingLicenceAndReviewExpiryEnabled();
            return response;
        }

        /// <summary>
        /// Gets a list of Accounts for which consent expiry notifications are enabled.
        /// </summary>
        /// <returns>General option response</returns>
        [HttpGet, Route("GetAccountsWithConsentExpiryRemindersEnabled")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithConsentExpiryRemindersEnabled()
        {
            var response = DutyOfCareRepository.GetAccountsWithDrivingLicenceAndReviewExpiryEnabled();
            return response;
        }


        /// <summary>
        /// Gets a list of Accounts for which approver Duty of care reminders are enabled.
        /// </summary>
        /// <returns>General option response</returns>
        [HttpGet, Route("GetAccountsWithApproverDutyOfCareRemindersEnabled")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithApproverDutyOfCareRemindersEnabled()
        {
            var response = DutyOfCareRepository.GetAccountsWithApproverDutyOfCareDocumentsExpiryEnabled();
            return response;
        }

        /// <summary>
        /// Gets a list of Accounts for which claimants Duty of care reminders are enabled.
        /// </summary>
        /// <returns>General option response</returns>
        [HttpGet, Route("GetAccountsWithClaimantDutyOfCareRemindersEnabled")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithClaimantDutyOfCareRemindersEnabled()
        {
            var response = DutyOfCareRepository.GetAccountsWithClaimantDutyOfCareDocumentsExpiryEnabled();
            return response;
        }


        /// <summary>
        /// Gets the duty of care results for the expense date.
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <returns>
        /// The <see cref="DutyOfCareCheckResults">DutyOfCareCheckResults</see>
        /// </returns>
        [HttpGet, Route("GetDutyOfCareResults")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public DutyOfCareCheckResults GetDutyOfCareResults(DateTime expenseDate)
        {
            var response = this.InitialiseResponse<DutyOfCareCheckResults>();
            response.List = ((DutyOfCareRepository)Repository).GetDutyOfCareResults(expenseDate);
            return response;
        }

        /// <summary>
        /// Gets the duty of care documents required when a vehicle has been added.
        /// </summary>
        /// <returns>
        /// The <see cref="VehicleDocumentsRequiredForDOCResponse">VehicleDocumentsRequiredForDOCResponse</see>
        /// </returns>
        [HttpGet, Route("VehicleDocumentsRequiredForDOC")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public VehicleDocumentsRequiredForDOCResponse VehicleDocumentsRequiredForDOC()
        {
            var response = this.InitialiseResponse<VehicleDocumentsRequiredForDOCResponse>();
            response.List = ((DutyOfCareRepository)Repository).VehicleDocumentsRequiredForDOC();
            return response;
        }

        /// <summary>
        /// The get SORN vehicles for the current user.
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <returns>
        /// The <see cref="SornVehiclesResponse">SornVehiclesResponse</see>
        /// </returns>
        [HttpGet, Route("GetSornVehicles")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public SornVehiclesResponse GetSornVehicles(DateTime expenseDate)
        {
            var response = this.InitialiseResponse<SornVehiclesResponse>();
            response.List = ((DutyOfCareRepository)Repository).GetSornVehicles(expenseDate);
            return response;
        }

        /// <summary>
        /// Gets the <see cref="DutyOfCareCheckResults">DutyOfCareCheckResults</see> for a claimant/expense, when called as the claimant's approver
        /// </summary>
        /// <param name="request">The <see cref="GetClaimantsVehiclesRequest">GetClaimantsVehiclesRequest</see></param>
        /// <returns>
        /// The <see cref="DutyOfCareCheckResults">DutyOfCareCheckResults</see>
        /// </returns>
        [HttpPut, Route("GetDutyOfCareResultsForEmployee")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public DutyOfCareCheckResults GetDutyOfCareResultsForEmployee(GetClaimantsVehiclesRequest request)
        {
            var response = this.InitialiseResponse<DutyOfCareCheckResults>();
            response.List = ((DutyOfCareRepository) this.Repository).GetDutyOfCareResults(request.ExpenseDate, request.EmployeeId, request.ExpenseId);
            return response;
        }

        /// <summary>
        /// Gets the <see cref="SornVehiclesResponse">SornVehiclesResponse</see> for a claimant/expense, when called as the claimant.
        /// </summary>
        /// <param name="request">The <see cref="GetClaimantsVehiclesRequest">GetClaimantsVehiclesRequest</see></param>
        /// <returns>
        /// The <see cref="SornVehiclesResponse">SornVehiclesResponse</see>
        /// </returns>
        [HttpPut, Route("GetSornVehiclesForEmployee")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public SornVehiclesResponse GetSornVehiclesForEmployee(GetClaimantsVehiclesRequest request)
        {
            var response = this.InitialiseResponse<SornVehiclesResponse>();
            response.List = ((DutyOfCareRepository) this.Repository).GetSornVehicles(request.ExpenseDate, request.EmployeeId, request.ExpenseId);
            return response;
        }
        #endregion
    }
}
