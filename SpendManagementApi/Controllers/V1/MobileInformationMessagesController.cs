namespace SpendManagementApi.Controllers.V1
{
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Responses.InformationMessage;
    using SpendManagementApi.Models.Types.InformationMessage;

    /// <summary>
    /// The mobile information messages version 1 controller.
    /// </summary>
    [Version(1)]
    [RoutePrefix("MobileInformationMessages")]
    public class MobileInformationMessagesV1Controller : BaseApiController<MobileInformationMessage>
    {
        /// <summary>
        /// Gets all the active <see cref="MobileInformationMessagesResponse">MobileInformationMessagesResponse</see>
        /// </summary>
        /// <returns>
        /// The <see cref="MobileInformationMessagesResponse">MobileInformationMessagesResponse</see>.
        /// </returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MobileInformationMessagesResponse GetAll()
        {
            return this.GetAll<MobileInformationMessagesResponse>();
        }
    }
}
