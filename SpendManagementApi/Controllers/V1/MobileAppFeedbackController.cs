namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests.MobileAppFeedback;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Responses.AppFeedback;
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
        /// An instance of <see cref="AppFeedbackRepository">111</see>
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
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public BooleanResponse DoNotPromptEmployeeForAppReviews()
        {      
            var response = this.InitialiseResponse<BooleanResponse>();
            response.Item = _repository.DoNotPromptEmployeeForAppReviews();
            return response;
        }

        /// <summary>
        /// Gets the mobile app's active feedback categories
        /// </summary>
        /// <returns>
        /// The <see cref="MobileAppFeedbackCategoriesResponse">MobileAppFeedbackCategoriesResponse</see>
        /// </returns>
        [HttpGet, Route("GetMobileAppFeedbackCategories")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MobileAppFeedbackCategoriesResponse GetActiveMobileAppFeedbackCategories()
        {      
            var response = this.InitialiseResponse<MobileAppFeedbackCategoriesResponse>();
            response.List = _repository.GetActiveMobileAppFeedbackCategories();
            return response;
        }


        /// <summary>
        /// Saves the mobile app feedback
        /// </summary>
        /// <param name="request">
        /// The <see cref="MobileAppFeedbackRequest">MobileAppFeedbackRequest</see>
        /// </param>
        /// <returns>
        /// <see cref="BooleanResponse">BooleanResponse</see> with the outcome of the save.
        /// </returns>
        [HttpPost, Route("SaveMobileAppFeedback")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public BooleanResponse SaveMobileAppFeedback([FromBody] MobileAppFeedbackRequest request)
        {      
            var response = this.InitialiseResponse<BooleanResponse>();
            response.Item = _repository.SaveMobileAppFeedback(request);
            return response;
        }
    }
}