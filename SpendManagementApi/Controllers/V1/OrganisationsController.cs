using SpendManagementApi.Common;

namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="Organisation">Organisations</see>.
    /// </summary>
    [RoutePrefix("Organisations")]
    [Version(1)]
    public class OrganisationsV1Controller : BaseApiController<Organisation>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="Organisation">Organisation</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Organisation">Organisations</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Organisations, AccessRoleType.View)]
        public GetOrganisationsResponse GetAll()
        {
            return this.GetAll<GetOrganisationsResponse>();
        }

        /// <summary>
        /// Adds an <see cref="Organisation">Organisation</see>.
        /// </summary>
        /// <param name="request">The <see cref="Organisation">Organisation</see> to add.
        /// When adding a new <see cref="Organisation">Organisation</see> through the API, the following properties are required: 
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>An OrganisationResponse containing the added <see cref="Organisation">Organisation</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Add)]
        public OrganisationResponse Post([FromBody] Organisation request)
        {
            var response = this.InitialiseResponse<OrganisationResponse>();
            response.Item = ((OrganisationRepository)this.Repository).Add(request, IsMobileRequest());

            return response;

       
        }

        /// <summary>
        /// Finds all <see cref="Organisation">Organisation</see> matching specified criteria.
        /// </summary>
        /// <param name="criteria">Find query <see cref="FindOrganisationRequest">FindOrganisationRequest</see></param>
        /// <returns>A GetOrganisationsResponse containing matching <see cref="Organisation">Organisation</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetOrganisationsResponse Find([FromUri] FindOrganisationRequest criteria)
        {
            var response = this.InitialiseResponse<GetOrganisationsResponse>();
            response.List = ((OrganisationRepository) this.Repository).GetOrganisationByCriteria(criteria);

            return response;
        }


        private bool IsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }
    }
}
