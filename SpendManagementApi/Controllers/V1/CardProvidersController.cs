namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    /// <summary>
    /// Provides methods for handling <see cref="CardProvider">CardProviders</see>.
    /// </summary>
    [RoutePrefix("CardProviders")]
    [Version(1)]
    public class CardProvidersV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="CardProvider">CardProviders</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Returns list of <see cref="CardProvider">CardProviders</see>
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.CorporateCards, AccessRoleType.View)]
        public List<CardProvider> Get()
        {
            CardProviders providers = new CardProviders();
            return providers.getCardProviders().Values.Select(p => p.Cast<CardProvider>()).ToList();
        }

        /// <summary>
        /// Init.
        /// </summary>
        protected override void Init()
        {   
        }
    }
}
