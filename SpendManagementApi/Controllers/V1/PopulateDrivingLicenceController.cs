namespace SpendManagementApi.Controllers.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;
    using Attributes;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using Models.Requests;

    /// <summary>
    /// Populate driving licnce using dvla portal
    /// </summary>
    [RoutePrefix("PopulateDrivingLicence")]
    [Version(1)]
    public class PopulateDrivingLicenceV1Controller : BaseApiController<EmployeesToPopulateDrivingLicence>
    {
        /// <summary>
        /// Gets a list of Ids of Accounts for which DVLA consent is enabled.
        /// </summary>
        /// <returns>GetAccountVatCalculationEnabledResponses</returns>
        [HttpGet,Route("GetAccountsWithAutoDvlaConsentEnabled")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithAutoDvlaConsentEnabled()
        {
            var response = this.InitialiseResponse<GeneralOptionAccountsResponse>();
            response = new PopulateDrivingLicenceRepository().GetAccountsWithDvlaConsentEnabled();
            return response;
        }

        /// <summary>
        /// List of employee to populate driving licence
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>List of employee details</returns>
        [HttpGet, Route("GetEmployeeToPopulateDrivingLicence/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DvlaConsentEmployeesResponse GetEmployeeToPopulateDrivingLicence(int accountId)
        {
            var response = this.InitialiseResponse<DvlaConsentEmployeesResponse>();
            response = new PopulateDrivingLicenceRepository().GetEmployeeToPopulateDrivingLicence(accountId);
            return response;
        }

        /// <summary>
        /// The populate driving licence information.
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <param name="employeeDetials">
        /// The employee detials.
        /// </param>
        /// <returns>
        /// The <see cref="DrivingLicenceResponse"/>.
        /// Driving licence details
        /// </returns>
        [HttpPost, Route("PopulateDrivingLicenceInformation/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DrivingLicenceResponse PopulateDrivingLicenceInformation(int accountId, Spend_Management.shared.code.DVLA.EmployeesToPopulateDrivingLicence employeeDetials)
        {
             return new PopulateDrivingLicenceRepository().PopulateDrivingLicenceInformation(accountId, employeeDetials);
        }

        /// <summary>
        /// Save employee driving licence
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <param name="drivingLicencedetails">
        /// Driving licence details
        /// </param>
        /// <returns>
        /// The <see cref="DvlaDrivingLicenceResponse"/> Driving licence response after inserting the document.
        /// </returns>
        [HttpPost, Route("SaveDvlaDrivingLicenceInformation/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DvlaDrivingLicenceResponse SaveDvlaDrivingLicenceInformation(int accountId, [FromBody] DrivingLicenceRequest drivingLicencedetails)
        {
            return new PopulateDrivingLicenceRepository().SaveDvlaDrivingLicenceInformation(accountId, drivingLicencedetails);
        }

        /// <summary>
        /// Intialises the repository
        /// </summary>
        protected override void Init(){}

        /// <summary>
        /// List of employee who has provided manual consent to dvla populate driving licence
        /// </summary>
        /// <param name="accountId">
        /// The account Id of customer.
        /// </param>
        /// <returns>List of employee details</returns>
        [HttpGet, Route("GetEmployeeWithManualIntervention/{accountId:int}")]
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        public DvlaConsentEmployeesResponse GetEmployeeWithManualIntervention(int accountId)
        {
            var response = this.InitialiseResponse<DvlaConsentEmployeesResponse>();
            response = new PopulateDrivingLicenceRepository().GetEmployeeWithManualIntervention(accountId);
            return response;
        }


        /// <summary>
        /// Deletes the driving licence (both manual and auto lookup)
        /// </summary>
        /// <param name="drivingLicencedetails">
        /// The driving licence reference which needs to be deleted.
        /// </param>
        /// <returns>
        /// The <see cref="NumericResponse"/> record value of the driving licence entity.
        /// </returns>
        [HttpDelete, Route("DeleteDrivingLicenceInformation")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public NumericResponse DeleteDrivingLicenceRecord([FromBody] CustomEntityRequest drivingLicencedetails)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = new PopulateDrivingLicenceRepository().DeleteDrivingLicenceRecord(this.CurrentUser.AccountID, this.CurrentUser.EmployeeID, drivingLicencedetails.ViewId, drivingLicencedetails.EntityId, drivingLicencedetails.RecordId);
            return response;
        }

        /// <summary>
        /// Save employee manual driving licence review 
        /// </summary>
        /// <param name="drivingLicencedetails">
        /// Driving licence review details
        /// </param>
        /// <returns>
        /// The <see cref="DvlaDrivingLicenceResponse"/> Driving licence review response after inserting the document.
        /// </returns>
        [HttpPost, Route("SaveManualDrivingLicenceReview")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public DvlaDrivingLicenceResponse SaveDvlaDrivingLicenceReview([FromBody] DrivingLicenceReviewRequest drivingLicencedetails)
        {
            return new PopulateDrivingLicenceRepository().SaveDrivingLicenceReview(drivingLicencedetails, this.CurrentUser.AccountID, this.CurrentUser.EmployeeID);
        }
    }
}