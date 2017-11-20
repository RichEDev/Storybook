namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests.SelfRegistration;
    using SpendManagementApi.Models.Responses.SelfRegistration;

    /// <summary>
    /// Contains self registration specific specific actions.
    /// </summary>
    [RoutePrefix("SelfRegistration")]
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SelfRegistrationV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets ALL of the available end points from the API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        [Route("Options")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// 
        /// You will be provided with a SelfRegistrationResponse object. 
        /// </summary>
        /// <param name="request">Provide a fully populated <see cref="SelfRegistrationInitiatorRequest">SelfRegistrationInitiatorRequest</see> object.</param>
        /// <returns>SelfRegistrationResponse, containing all the data requires to populate relevant fields for a self registration form. Also, details are passed about required fields, and any errors found with the posted information.</returns>
        /// <exception cref="HttpStatusCode.BadRequest">The details you supplied are incorrect.</exception>
        [HttpPost]
        [Route("InitiateSelfRegistration")]
        public SelfRegistrationResponse InitiateSelfRegistration(SelfRegistrationInitiatorRequest request)
        {
            var response = this.InitialiseResponse<SelfRegistrationResponse>();

            // all incoming properties are basic validated
            // prepare and return a response.

            SelfRegistrationDummyDataPopulator.PopulateStandardDummyResponse(response);

            return response;
        }

        /// <summary>
        /// Necessary for BaseApiController, even if it does nothing.
        /// </summary>
        protected override void Init()
        {
        }
    }
}