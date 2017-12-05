namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// The mobile app feedback V1 controller.
    /// </summary>
    [RoutePrefix("MobileAppFeedback")]
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileAppFeedbackV1Controller : BaseApiController
    {
        /// <summary>
        /// An instance of <see cref="AppFeedbackRepository">AppFeedbackRepository</see>
        /// </summary>
        private AppFeedbackRepository _repository;

        /// <summary>
        /// The init.
        /// </summary>
        protected override void Init()
        {
            this._repository = new AppFeedbackRepository(this.CurrentUser);
        }

        /// <summary>
        /// Returns the endpoints available.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Sets the current employees preference so that they are never prompted to an app review
        /// </summary>
        /// <returns>
        /// <see cref="BooleanResponse">BooleanResponse</see> with the outcome of the action.
        /// </returns>
        [HttpPost, Route("DoNotPromptEmployeeForAppReviews")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public BooleanResponse DoNotPromptEmployeeForAppReviews()
        {      
            var response = this.InitialiseResponse<BooleanResponse>();
            response.Item = _repository.DoNotPromptEmployeeForAppReviews();
            return response;
        }
    }
}