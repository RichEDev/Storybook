namespace SpendManagementApi.Controllers.V1
{
    using SpendManagementApi.Attributes;
    using System.Web.Http;
    using System.Web.Http.Description;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using Repositories;

    /// <summary>
    ///  System greenlight controller
    /// </summary>
    [RoutePrefix("CustomEntity")]
    [Version(1)]
    public class SystemGreenLightV1Controller : BaseApiController
    {
        #region Green light
        /// <summary>
        /// Copy requested greenlight to target database.
        /// </summary>
        /// <param name="greenLightEntity">Details of entity and target database to copy</param>
        [HttpPost]
        [Route("CopySystem")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        [SystemGreenlightOnly, ApiExplorerSettings(IgnoreApi = true)]
        public SystemGreenLightResponse CopySystemGreenlight([FromBody]CustomEntityToCopy greenLightEntity)
        {
            var response = new SystemGreenLightRepository().CopyCustomEntity(this.CurrentUser,greenLightEntity,this.Request);
            return response;
        }
        #endregion

        /// <summary>
        /// Method to be implemented by derived class to initialise repository
        /// </summary>
        protected override void Init()
        {

        }
    }

}
