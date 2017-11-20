namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// The controller for handling the app versions.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileVersionsV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the current version number for the specified platform.
        /// </summary>
        /// <param name="platform">The platform</param>
        /// <returns>The version of the application for the specified platform.</returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/appversion")]
        public VersionResult GetCurrentVersion(string platform)
        {
            VersionResult result = new VersionResult { FunctionName = "GetCurrentVersion", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cMobileAPIVersions versions = new cMobileAPIVersions();
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
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetCurrentVersion():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

// ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }

            return result;
        }
    }
}
