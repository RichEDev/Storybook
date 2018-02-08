using Spend_Management.shared.code.DVLA;

namespace Spend_Management.Bootstrap
{
    using DutyOfCareAPI.DutyOfCare;
    using DutyOfCareAPI.DutyOfCare.LicenceCheck;
    using System.Net;
    using SpendManagementLibrary;

    /// <summary>
    /// <see cref="IDutyOfCareApi"/> bootstrapper
    /// </summary>
    public static class BootstrapDvla
    {
        /// <summary>
        /// <see cref="IDutyOfCareApi"/> bootstrapper for Duty of Care APIs.
        /// </summary>
        /// <returns>Returns <see cref="IDutyOfCareApi"/>instance for the duty of care api</returns>
        public static IDutyOfCareApi CreateNew()
        {
            IDutyOfCareApi api;
            cSecureData secureData = new cSecureData();
            string licencePortalUrl;
            var useTestMode = GlobalVariables.LicenceCheckPortalAccessMode == null ? "Live" : GlobalVariables.LicenceCheckPortalAccessMode;
            var username = GlobalVariables.LicenceCheckPortalAccessUserName == null ? string.Empty : GlobalVariables.LicenceCheckPortalAccessUserName;
            var encryptedPassword = GlobalVariables.LicenceCheckPortalAccessPassword == null ? string.Empty : GlobalVariables.LicenceCheckPortalAccessPassword;
            var password = !string.IsNullOrWhiteSpace(encryptedPassword) ? secureData.Decrypt(encryptedPassword) : string.Empty;
            var credentials = new NetworkCredential(username, password);

            switch (useTestMode)
            {
                case "Live":
                    licencePortalUrl = GlobalVariables.LicenceCheckConsentPortalLiveUrl == null ? string.Empty : GlobalVariables.LicenceCheckConsentPortalLiveUrl;
                    api = new LicenceCheckDutyOfCareApi(credentials, licencePortalUrl);
                    break;
                case "MockTest":
                    licencePortalUrl = GlobalVariables.LicenceCheckConsentPortalLiveUrl == null ? string.Empty : GlobalVariables.LicenceCheckConsentPortalDemoUrl;
                    api = new MockLicenceCheckDutyOfCareApi(credentials, licencePortalUrl);
                    break;
                default:
                    licencePortalUrl = GlobalVariables.LicenceCheckConsentPortalDemoUrl == null ? string.Empty : GlobalVariables.LicenceCheckConsentPortalDemoUrl;
                    api = new TestLicenseCheckApi(credentials, licencePortalUrl);
                    break;
            }

            return api;
        }

        public static ILookupLogger CreateLogger(ICurrentUser currentUser)
        {
            return new VehicleLookupLogger(currentUser);
        }
    }
}