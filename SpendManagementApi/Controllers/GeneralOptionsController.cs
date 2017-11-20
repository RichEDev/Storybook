using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Attributes;
using SpendManagementApi.Models.Responses;
using SpendManagementApi.Repositories;

namespace SpendManagementApi.Controllers
{
    /// <summary>
    /// The controller to handle all <see cref="GeneralOption">GeneralOptions</see> for users.
    /// </summary>
    [RoutePrefix("GeneralOptions")]
    public class GeneralOptionsController : BaseApiController<GeneralOption>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="GeneralOption">GeneralOptions</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

         //<summary>
         //Gets all <see cref="GeneralOption">GeneralOptions</see> in the system.
         //</summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public GeneralOptionsResponse GetAll()
        {
            return GetAll<GeneralOptionsResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="GeneralOption">GeneralOption</see>, by its Id.
        /// </summary>
        /// <param name="subAccountID">The subAccountID of the item to get.</param>
        /// <returns>A GeneralOptionResponse, containing the <see cref="GeneralOption">GeneralOption</see> if found.</returns>
        [HttpGet, Route("{subAccountID:int}")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public GeneralOptionResponse Get([FromUri] int subAccountID)
        {
            return Get<GeneralOptionResponse>(subAccountID);
        }

        /// <summary>
        /// Gets a single <see cref="GeneralOption">GeneralOption</see>, by its key.
        /// </summary>
        /// <param name="key">The key of the item to get.</param>
        /// <returns>A GeneralOptionResponse, containing the <see cref="GeneralOption">GeneralOption</see> if found.</returns>
        [HttpGet, Route("{key}")]
        [AuthAudit(SpendManagementElement.GeneralOptions, AccessRoleType.View)]
        public GeneralOptionResponse Get([FromUri] string key)
        {
            var response = InitialiseResponse<GeneralOptionResponse>();
            response.Item = ((GeneralOptionRepository)Repository).GetByKey(key);
            return response;         
        }
    }
}