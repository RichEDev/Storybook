namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary.FinancialYears;

    /// <summary>
    /// Provides methods for handling <see cref="FinancialYear">FinancialYears</see>.
    /// </summary>
    [RoutePrefix("FinancialYears")]
    [Version(1)]
    public class FinancialYearsV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="FinancialYear">FinancialYears</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Returns list of <see cref="FinancialYear">FinancialYears</see>
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.View)]
        public List<Models.Types.FinancialYear> Get()
        {
            return
                FinancialYears.Years(this.CurrentUser)
                              .Select(year => year.Cast<SpendManagementApi.Models.Types.FinancialYear>()).ToList();
        }

        /// <summary>
        /// Init.
        /// </summary>
        protected override void Init()
        {   
        }
    }
}
