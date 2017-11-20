namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;
    using Models.Common;
    using Utilities;

    /// <summary>
    /// The controller dealing with mobile versioning.
    /// </summary>
    [Version(1)]
    [RoutePrefix("Versions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class VersionsV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets the current version number for the specified platform.
        /// </summary>
        /// <param name="platform">The mobile platform</param>
        /// <returns>The <see cref="VersionResult"></see> VersionResult of the application for the specified platform.</returns>
        [HttpGet, Route("{platform}")]
        [AuthAudit(SpendManagementElement.VersionRegistry, AccessRoleType.View)]
        public VersionResult GetCurrentVersion ([FromUri] string platform)
        {
            var result = new VersionResult();

            try
            {
                var versions = new cMobileAPIVersions();
                cMobileAPIVersion reqVersion = versions.GetVersionByTypeKey(platform);

                if (reqVersion != null)
                {
                    result.ApiType = reqVersion.APIType.TypeKey;
                    result.AppStoreURL = reqVersion.AppStoreURL;
                    result.DisableAppUsage = reqVersion.DisableUsage;
                    result.NotifyMessage = reqVersion.NotifyMessage;
                    result.SyncMessage = reqVersion.SyncMessage;
                    result.Title = reqVersion.Title;
                    result.VersionNumber = reqVersion.VersionNumber;
                }
                else
                {
                    result.ReturnCode = MobileReturnCode.InvalidApiVersionType;
                }
            }
            catch (Exception)
            {
                throw new ApiException(ApiResources.ApiErrorGetVersionUnSucessful, ApiResources.ApiErrorGetVersionMessage);
            }

            return result;
        }

        protected override void Init()
        {

        }
    }

}
