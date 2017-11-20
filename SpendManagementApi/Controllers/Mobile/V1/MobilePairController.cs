namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ServiceResultMessage = SpendManagementLibrary.Mobile.ServiceResultMessage;

    /// <summary>
    /// The controller to handling pairing of a serial key and activation key for a mobile user.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobilePairV1Controller : ApiController
    {
        /// <summary>
        /// Handles the PUT from the WebAPI to match a device serial key to a mobile device pairing key
        /// </summary>
        /// <returns>A ServiceResultMessage detailing success or failure of the pair attempt.</returns>
        [HttpPut]
        [Route("mobile/pair")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResultMessage Put()
        {
            string key = Common.Mobile.HttpRequestMessageExtensions.GetHeader(this.Request, "PairingKeySerialKey");

            var headerArray = key.Split(char.Parse("|"));

            ServiceResultMessage srm = Authenticator.ValidatePairingKey(headerArray[0], null);
            srm.FunctionName = "PairDevice";

            if (srm.ReturnCode == MobileReturnCode.Success)
            {
                var pairing = new PairingKey(headerArray[0]);

                cEmployees clsEmployees = new cEmployees(pairing.AccountID);
                Employee reqEmployee = clsEmployees.GetEmployeeById(pairing.EmployeeID);
                if (reqEmployee.Archived)
                {
                    srm.ReturnCode = MobileReturnCode.EmployeeArchived;
                }
                else
                    if (!reqEmployee.Active)
                    {
                        srm.ReturnCode = MobileReturnCode.EmployeeNotActivated;
                    }
                    else
                    {
                        var clsMobileDevices = new cMobileDevices(pairing.AccountID);
                        srm.ReturnCode = clsMobileDevices.PairMobileDevice(pairing, headerArray[1]);
                    }
            }

            srm.Message = srm.ReturnCode == MobileReturnCode.Success ? "success" : "fail";
            return srm;
        }
    }
}
